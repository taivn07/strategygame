using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Battle_ST_Game;

namespace Battle_ST_Game {

	public class Unit : MonoBehaviour {

		public UnitStatus status;	//!<Unit status

		public float speed = 1;

		protected Animator _animator; //!<Unit animator

		protected SpriteRenderer _spriteRenderer; //!<Unit sprite renderer

		public bool dead { get; protected set;}	//!<unit are dead

		protected int placeNo;//!<１～６
		protected Vector3 defaultPosition;	//!<Used to return at the start of the location

		protected CircleCollider2D _sightOfUnit; //!<Sight of unit

		private const string UNIT_SPRITE_PATH = "Icons/f00";

		private const string BOTTOM_TAG = "Unit_Bottom";
		private const string TOP_TAG = "Unit_Top";

		private string enemyTag = "";
		private Unit nowTarget = null;

		private bool attackable = false;

		private float lastTimeShooted = 0;
		public int ShootIntervalInSeconds = 0;

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

		void Awake() {

		}

		/** Member kind enum */
		public enum MEMBER_KIND 
		{
			BOTTOM_SIDE ,	//!<Bottom
			TOP_SIDE ,	//!<Top
		}

		/** Weapon kind enum */
		public enum WEAPON_KIND 
		{
			AXE ,	//!<Axe
			SWORD ,	//!<Sword
			HAMMER, //!<Hammer
			RIFLE, //!<Rifle
			MACHINE_GUN, //!<Machine Gun
		}

		/**
		 * init unit 
		 */ 
		public void Initalize(UnitStatus status, int posIndex) {

			this.status = status;
			this.dead = false;

			this.setUpComponents();

		}

		/**
		 * setup other components for unit
		 */ 
		public void setUpComponents() {
			if (!_animator) _animator = this.GetComponent<Animator>();
			if (!_spriteRenderer) _spriteRenderer = this.GetComponent<SpriteRenderer>();

			Sprite sp = SpriteUtil.loadSpriteFromSource(UNIT_SPRITE_PATH+""+this.status.unitNo);

			if (sp) _spriteRenderer.sprite = sp;
			
//			//setup sight
//			if (!_sightOfUnit) _sightOfUnit = this.gameObject.GetComponentInChildren<CircleCollider2D>();
//			_sightOfUnit.radius = this.status.rangeAttack;

			if (this.status.memberKind == MEMBER_KIND.BOTTOM_SIDE) {
				this.gameObject.tag = BOTTOM_TAG;
//				StartCoroutine(MoveObject(BattleManager.instance._topUnits[0].gameObject, 3f));
				enemyTag = TOP_TAG;
			} else {
				this.gameObject.tag = TOP_TAG;
				enemyTag = BOTTOM_TAG;
			}

			//add health bar
//			this.gameObject.AddComponent("HealthBar");
			HealthBar hBar = (HealthBar) this.gameObject.AddComponent(typeof(HealthBar));
			hBar.curHealth = this.status.hp;
			hBar.maxHealth = this.status.maxHp;


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

			List<Battle_ST_Game.Unit> units = new List<Unit>();
			if (this.gameObject.tag == BOTTOM_TAG) {
				units = BattleManager.instance._topUnits;
			} else {
				units = BattleManager.instance._bottomUnits;
			}

			foreach(Unit enemy in units) {
				var distance = Vector3.Distance(enemy.transform.position, this.transform.position);
				if (distance - this.GetComponent<SpriteRenderer>().bounds.size.x/2 < this.status.rangeAttack && !enemy.dead) {
					target = enemy;
					break;
				}
			}

			return target;
		}

		/**
		 * Attack Enemy 
		 * 
		 * @param playerStatus: player status
		 * @param enemyStatus: enemy status
		 */ 
		protected void attackEnemy(UnitStatus playerStatus, UnitStatus enemyStatus) {

			//range attack
			if (this.status.weaponKind == WEAPON_KIND.MACHINE_GUN || this.status.weaponKind == WEAPON_KIND.RIFLE) {
				if  ((lastTimeShooted + ShootIntervalInSeconds) < Time.time) {

					//animation here


					GameObject loadedResource = Resources.Load<GameObject>("Prefabs/Bullet");
					GameObject newInstance = GameObject.Instantiate<GameObject>(loadedResource);
					newInstance.transform.position = this.transform.position;
					Bullet bullet = newInstance.GetComponent<Bullet>();
					bullet.Target = this.nowTarget.gameObject;
					bullet.tag = this.gameObject.tag +"_Bullet";
					bullet.damage = this.status.attack;
					lastTimeShooted = Time.time;
				}
			} else { //normal attack
				//animation here

			}


		}

		private void goTo(GameObject target) {
			StartCoroutine(MoveObject(target, 2f));
		}

		private IEnumerator MoveObject(GameObject target, float speed) {
			var direction = target.transform.position - transform.position;
			GetComponent<Rigidbody>().velocity = direction * speed;
			yield return null;
		}

		void Update() {
			if (BattleManager.instance.gameState != BattleManager.GAME_STATE.PLAYING || this.dead) return;

			nowTarget = findNearestEnemy();

			//Attack tartget
			if (nowTarget != null) { 
				//Attack
				attackEnemy(this.status, nowTarget.status);
			} else {
				//Keep move and find them

			}
		}


		/**
		 * OnTriggerEnter2D function
		 */ 
		void OnTriggerEnter2D (Collider2D other) {
			
			if(other.gameObject.tag == enemyTag+"_Bullet") {
				this.status.hp = (int)(this.status.hp - other.GetComponent<Bullet>().damage + this.status.def);
				Debug.Log(other.GetComponent<Bullet>().damage);
				if (this.status.hp <= 0) {
					this.status.hp = 0;
					this.dead = true;
					this.gameObject.SetActive(false);
				}
				this.GetComponent<HealthBar>().curHealth = this.status.hp;
				Destroy(other.gameObject);
			}
			
		}

		void OnTriggerStay2D (Collider2D other) {

			
		}

		void OnTriggerExit2D (Collider2D other) {
			
		}

	}



}
