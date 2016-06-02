using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Battle_ST_Game;

namespace Battle_ST_Game {

	public class Unit : Photon.MonoBehaviour {

		public bool isMine = false;

		public UnitStatus status;	//!<Unit status

		//some information to show
		public int unit_no = 0;
		public string unit_name = "";

		public float speed = 1;

		protected Animator _animator; //!<Unit animator

		protected SpriteRenderer _spriteRenderer; //!<Unit sprite renderer

		public bool dead { get;  set;}	//!<unit are dead

		protected int placeNo;//!<１～６
		protected Vector3 defaultPosition;	//!<Used to return at the start of the location

		protected CircleCollider2D _sightOfUnit; //!<Sight of unit

		private const string UNIT_SPRITE_PATH = "Icons/f00";

		public const string BOTTOM_TAG = "Unit_Bottom";
		public const string TOP_TAG = "Unit_Top";
		private const string BOTTOM_TOWER_TAG = "Tower_Bottom";
		private const string TOP_TOWER_TAG = "Tower_Top";

		private string enemyTag = "";
		private string enemyTowerTag = "";
		public Unit nowTarget = null;

		public GameObject enemyTower;

		private bool attackable = false;

		public float attackRate = 1f;
		private float nextFire;

		private Vector3 screenPoint;
		private Vector3 offset;

		private SpriteRenderer thisSpriteRen;
		private BoxCollider2D boxColl;
		private HealthBar healthBar;

		public class UnitStatus {
			public int unitNo;	//!<Unit id
			public string name; //!<Unit name

			public int hp;	    //!<Hp of unit
			public int maxHp;   //!<Max hp of unit
			public int mana;	//!<Unit mana
			public int requestMana;	//!<Unit mana for use skill

			public float rangeAttack; //!<Attack Range
			public int attack;	//!<Base attack damage
			public int def;	//!<Base def 
			public int heal;	//!<Base heal

			public WEAPON_KIND weaponKind; //!<weapon kind
			public MEMBER_KIND memberKind; //!<member kind: player or opponent
			public bool leader; //!<leader
		}

		private void Awake() {

			object[] data = this.gameObject.GetPhotonView ().instantiationData;
			if (data != null) {
				this.InitStatusWith(data);
			} 
		}

		private void Start() {

			if (this.status == null) {
				Debug.Log("Unit is null!!!");
				Destroy(this.gameObject);
				return;
			}

			//get component
			this.thisSpriteRen = GetComponent<SpriteRenderer>();
			this.boxColl = GetComponent<BoxCollider2D>();
			this.healthBar = GetComponent<HealthBar>();
		}

		private void Update() {

			if (BattleManager.instance.gameState != (int)BattleManager.GAME_STATE.PLAYING){
				return;
			} 

			if(!PhotonNetwork.isMasterClient) {
				return;
			}

			if (this.status == null) {
				return;
			}

			if (this.dead || this.status.hp <= 0) {
//				this.gameObject.SetActive(false);
				this.setDead();
				return;
			}
			
			//check id all enemy are dead, go to destroy tower
			if ((BattleManager.instance.checkAllDeadBottomUnit() && this.status.memberKind == MEMBER_KIND.TOP_SIDE) ||
			    (BattleManager.instance.checkAllDeadTopUnit() && this.status.memberKind == MEMBER_KIND.BOTTOM_SIDE)) {
				if (enemyTower != null) {
					goToTower(enemyTower);


				} 
				return;
			}
			
			nowTarget = findNearestEnemy();
			
			//Attack tartget
			if (nowTarget != null) { 
				goToEnemy(nowTarget);

				//Attack
				attackEnemy(this, nowTarget);
			} else {
				//Keep move and find them
				
			}
			
		}

		/** Member kind enum */
		public enum MEMBER_KIND 
		{
			BOTTOM_SIDE = 0,	//!<Bottom
			TOP_SIDE = 1,	//!<Top
		}

		/** Weapon kind enum */
		public enum WEAPON_KIND 
		{
			AXE = 1 ,	//!<Axe
			SWORD = 2 ,	//!<Sword
			HAMMER = 3, //!<Hammer
			RIFLE = 4, //!<Rifle
			MACHINE_GUN = 5, //!<Machine Gun
		}

		/**
		 * init unit 
		 */ 
		public void Initalize(UnitStatus status, int posIndex) {

			this.status = status;
			this.dead = false;
			this.unit_no = status.unitNo;
			this.unit_name = status.name;
//			this.isMine = false;
			this.setUpComponents();

		}

		/**
		 * init unit 
		 */ 
		public void InitStatusWith(object[] obj) {

			if(obj == null) return;

			this.status = ObjectUtil.getUnitStatusFrom(obj);

//			this.status = status;
			this.dead = false;
			this.unit_no = status.unitNo;
			this.unit_name = status.name;
			//			this.isMine = false;
			this.setUpComponents();
			
		}

		public void updateStatus() {
			if (this.dead) {
				this.setDead();
			}
			this.GetComponent<SpriteRenderer>().enabled = !this.dead;
			this.GetComponent<HealthBar>().curHealth = this.status.hp;
		}

		/**
		 * setup other components for unit
		 */ 
		public void setUpComponents() {
			if (!_animator) _animator = this.GetComponent<Animator>();
			if (!_spriteRenderer) _spriteRenderer = this.GetComponent<SpriteRenderer>();

			Sprite sp = SpriteUtil.loadSpriteFromSource(UNIT_SPRITE_PATH+""+this.status.unitNo);

			if (sp) _spriteRenderer.sprite = sp;

			if (this.status.memberKind == MEMBER_KIND.BOTTOM_SIDE) {
				this.gameObject.tag = BOTTOM_TAG;
//				StartCoroutine(MoveObject(BattleManager.instance._topUnits[0].gameObject, 3f));
				enemyTag = TOP_TAG;
				enemyTowerTag = TOP_TOWER_TAG;
			} else {
				this.gameObject.tag = TOP_TAG;
				enemyTag = BOTTOM_TAG;
				enemyTowerTag = BOTTOM_TOWER_TAG;
			}

			//add health bar
//			this.gameObject.AddComponent("HealthBar");
			HealthBar hBar = (HealthBar) this.gameObject.AddComponent(typeof(HealthBar));
			hBar.curHealth = this.status.hp;
			hBar.maxHealth = this.status.maxHp;

			//see enemy tower
			enemyTower = GameObject.FindGameObjectWithTag(enemyTowerTag) as GameObject;

		}

		protected void setDead() {
			this.dead = true;
			this.thisSpriteRen.enabled = false;
			this.boxColl.enabled = false;
			this.healthBar.enabled = false;

		}


		/**
		 * Find nearest Enemy to attack
		 * 
		 * @param status player status
		 * 
		 * @return enemy status
		 */ 
		protected Unit findNearestEnemy() {

			Unit target = null;
			float[] arr;

			List<Battle_ST_Game.Unit> units = new List<Unit>();
			if (this.gameObject.tag == BOTTOM_TAG) {
				units = BattleManager.instance._topUnits;

				float tempDis = 1000f;

				for(int i = 0; i < units.Count; i++) {
					if (tempDis >= getRangeFromAtoB(this, units[i]) && !units[i].dead) {
						tempDis = getRangeFromAtoB(this, units[i]);
						target = units[i];
					}
				}
			} else {
				units = BattleManager.instance._bottomUnits;
				float tempDis = 1000f;
				for(int i = 0; i < units.Count; i++) {
					if (tempDis >= getRangeFromAtoB(this, units[i]) && !units[i].dead) {
						tempDis = getRangeFromAtoB(this, units[i]);
						target = units[i];
					}
				}
			}
			return target;
		}

		/**
		 * Get range array from unit to top units
		 * @param unit
		 * @return float[]
		 */
		protected float[] getRangeArrayToTopUnit(Unit unit) {
			float[] result = new float[BattleManager.instance._topUnits.Count];
//			List<float> = new List<float>();

			for (int i = 0; i < BattleManager.instance._topUnits.Count; i++) {
				float dist = Vector3.Distance(unit.transform.position, BattleManager.instance._topUnits[i].gameObject.transform.position);
				result[i] = dist;
			}

			return result;
		}

		/**
		 * Get range array from unit to bottom units
		 * @param unit
		 * @return float[]
		 */
		protected float[] getRangeArrayToBottomUnit(Unit unit) {
			float[] result = new float[BattleManager.instance._bottomUnits.Count];
			for (int i = 0; i < BattleManager.instance._bottomUnits.Count; i++) {
				float dist = Vector3.Distance(unit.transform.position, BattleManager.instance._bottomUnits[i].gameObject.transform.position);
				result[i] = dist;
			}
			
			return result;
		}

		/**
		 * Attack Enemy 
		 * 
		 * @param playerStatus: player status
		 * @param enemyStatus: enemy status
		 */ 
		protected void attackEnemy(Unit player, Unit enemy) {

			if (getRangeFromAtoB(player, enemy) > player.status.rangeAttack) return;

			//range attack
			if (this.status.weaponKind == WEAPON_KIND.MACHINE_GUN || this.status.weaponKind == WEAPON_KIND.RIFLE) {
				shoot(this.status, enemy.status);
			} else { //normal attack

//				this.attackByHand(enemy);
				shoot(this.status, enemy.status);

			}
		}

		/**
		 * Attack Enemy 
		 * 
		 * @param playerStatus: player status
		 * @param enemyStatus: enemy status
		 */ 
		protected void attackEnemy(Unit player, GameObject tower) {
			
			//range attack
			if (this.status.weaponKind == WEAPON_KIND.MACHINE_GUN || this.status.weaponKind == WEAPON_KIND.RIFLE) {
				shoot(this.status, tower);
			} else { //normal attack
				shoot(this.status, tower);
//				this.attackByHand(tower);
				
			}
		}

		/**
		 * shoot enemy
		 * @param playerStatus this
		 * @param enemyStatus enemy
		 */ 
		protected void shoot(UnitStatus playerStatus, UnitStatus enemyStatus) {
			if (Time.time > nextFire && BattleManager.instance.gameState == (int)BattleManager.GAME_STATE.PLAYING) {
				nextFire = Time.time + attackRate;
				var dict = new Dictionary<string,string>();
				dict.Add("target", this.nowTarget.gameObject.name);
				dict.Add("tag", this.gameObject.tag +"_Bullet");
				dict.Add("damage", this.status.attack.ToString());
				dict.Add("target_tower", "");
				object[] obj = new object[4];
				obj[0] = dict["target"];
				obj[1] = dict["tag"];
				obj[2] = dict["damage"];
				obj[3] = dict["target_tower"];
//				GameObject loadedResource = Resources.Load<GameObject>("Prefabs/Bullet");
//				GameObject newInstance = GameObject.Instantiate<GameObject>(loadedResource);
				GameObject go = PhotonNetwork.Instantiate("Prefabs/Bullet/Bullet", transform.position, Quaternion.identity, 0, obj);

			}
		}

		/**
		 * shoot enemy
		 * @param playerStatus this
		 * @param enemyStatus enemy
		 */ 
		protected void shoot(UnitStatus playerStatus, GameObject tower) {
			if (Time.time > nextFire && BattleManager.instance.gameState == (int)BattleManager.GAME_STATE.PLAYING) {
				nextFire = Time.time + attackRate;
				var dict = new Dictionary<string,string>();
				dict.Add("target", "");
				dict.Add("tag", this.gameObject.tag +"_Bullet");
				dict.Add("damage", this.status.attack.ToString());
				dict.Add("target_tower", tower.name);
				object[] obj = new object[4];
				obj[0] = dict["target"];
				obj[1] = dict["tag"];
				obj[2] = dict["damage"];
				obj[3] = dict["target_tower"];
				GameObject go = PhotonNetwork.Instantiate("Prefabs/Bullet/Bullet", transform.position, Quaternion.identity, 0, obj);
			}
		}

		/**
		 * attack enemy by hand 
		 * @param playerStatus this
		 * @param enemyStatus enemy
		 */ 
		protected void attackByHand(Unit enemy) {
			if (Time.time > nextFire) {
				nextFire = Time.time + attackRate;

				//animation

				//attack
				enemy.status.hp -= enemy.getDamage(this.status);
			}
		}

		/**
		 * attack enemy by hand 
		 * @param playerStatus this
		 * @param enemyStatus enemy
		 */ 
		protected void attackByHand(GameObject tower) {
			if (Time.time > nextFire) {
				nextFire = Time.time + attackRate;
				
				//animation
				
				//attack
//				enemy.status.hp -= enemy.getDamage(this.status);
			}
		}

		/**
		 * get damage from enemy
		 */ 
		protected int getDamage(UnitStatus enemyStatus) {

			if (this.status.def >= enemyStatus.attack) return 0;
			int damage = enemyStatus.attack - this.status.def;
			return damage;
		}

		private void goTo(GameObject target) {
			StartCoroutine(MoveObject(target, 2f));
		}

		private IEnumerator MoveObject(GameObject target, float speed) {
			var direction = target.transform.position - transform.position;
			GetComponent<Rigidbody>().velocity = direction * speed;
			yield return null;
		}

		protected void goToEnemy(Unit enemy) {
			
			if (enemy == null || this.checkRangeToEnemy(enemy)) return;
			transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, this.speed/15);
//			transform.Translate(enemy.gameObject.transform.position * Time.deltaTime * this.speed, Space.World);
		}

		protected void goToTower(GameObject tower) {

			if (tower == null) return;

			if (tower != null && this.checkRangeToTower(tower)) {
				attackEnemy(this, enemyTower);
				return;
			} else if (!this.checkRangeToTower(tower)) {
				transform.position = Vector3.MoveTowards(transform.position, tower.transform.position, this.speed/15);
			}

		}


		protected void goAhead() {

			if (nowTarget != null) return;

			if (this.status.memberKind == MEMBER_KIND.BOTTOM_SIDE) {
				transform.Translate(Vector3.up * Time.deltaTime * 1f, Space.World);
			} else {
				transform.Translate(Vector3.down * Time.deltaTime * 1f, Space.World);
			}
		}


		
		
		void OnMouseDown()
		{
//			if (PhotonNetwork.isMasterClient && this.gameObject.tag == BOTTOM_TAG) return;
			screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
//			
			offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			
		}
		
		void OnMouseDrag()
		{
//			if (!isMine) return;
			Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
			transform.position = curPosition;
			
		}



		/**
		 * OnTriggerEnter2D function
		 */ 
		void OnTriggerEnter2D (Collider2D other) {

			if (!PhotonNetwork.isMasterClient) return;
			
			if(other.gameObject.tag == enemyTag+"_Bullet") {
				this.status.hp = (int)(this.status.hp - other.GetComponent<Bullet>().damage + this.status.def);
				if (this.status.hp <= 0) {
					this.status.hp = 0;
					this.setDead();
				}
				this.GetComponent<HealthBar>().curHealth = this.status.hp;
				other.GetComponent<Bullet>().dead = true;
//				Destroy(other.gameObject);
			}
			
		}

		void OnTriggerStay2D (Collider2D other) {

			
		}

		void OnTriggerExit2D (Collider2D other) {
			
		}

		protected bool checkRangeToEnemy(Unit enemy) {

			return getRangeFromAtoB(this, enemy) <= this.status.rangeAttack;
		}

		protected bool checkRangeToTower(GameObject tower) {
			float a = getRangeFromAtoB(this, tower);
			float b = this.status.rangeAttack;
			return a <= b;
		}

		private float getRangeFromAtoB(Unit unit1, Unit unit2) {
			Vector3 vec1 = unit1.gameObject.transform.position;
			Vector3 vec2 = unit2.gameObject.transform.position;

			return Vector3.Distance(vec1, vec2);

		}

		private float getRangeFromAtoB(Unit unit1, GameObject go) {
			Vector3 vec1 = unit1.gameObject.transform.position;
			Vector3 vec2 = go.gameObject.transform.position;
			
			return Vector3.Distance(vec1, vec2);
			
		}

	}



}
