using UnityEngine;
using System.Collections;
using Battle_ST_Game;

public class Bullet : Photon.MonoBehaviour  {
	public float Speed = 7.5f;
	public Unit Target = null;
	public Tower TargetTower = null;
	public float damage = 10f;

	public bool dead = false;

	private SpriteRenderer spriteRen;
	private CircleCollider2D circleColl;

	void Awake() {
		
		this.spriteRen = GetComponent<SpriteRenderer>();
		this.circleColl = GetComponent<CircleCollider2D>();
	}

	void Start() {
		object[] data = this.gameObject.GetPhotonView ().instantiationData;
		if (data != null) {
			this.InitStatusWith(data);
		} 
	}

	public void InitStatusWith(object[] obj) {
		
		if(obj == null) return;
		string targetName = (string)obj[0];
		GameObject target = GameObject.Find(targetName) as GameObject;
		if (target != null) this.Target = target.GetComponent<Unit>();
		this.tag = (string)obj[1];
		this.damage = float.Parse(obj[2].ToString());
		string towerName = (string)obj[3];
		GameObject tower = GameObject.Find(towerName) as GameObject;
		if (tower != null) this.TargetTower = tower.GetComponent<Tower>();
	}

	public void updateStatus() {
		this.setDead(this.dead);
	}

	public void setDead(bool isDead) {
		this.spriteRen.enabled = !isDead;
		this.circleColl.enabled = !isDead;
	}


	void Update() {

		if (BattleManager.instance.gameState != BattleManager.GAME_STATE.PLAYING){
			this.dead = true;
		} 

		if (!PhotonNetwork.isMasterClient) {
			this.setDead(this.dead);
			return;
		}

		if (Target == null && TargetTower == null) {
//			this.dead = true;
//			this.setDead(true);
		}

		if (Target != null && !Target.dead) {
			this.dead = false;
			float step = Speed * Time.deltaTime;
			this.transform.position = Vector3.MoveTowards(this.transform.position, 
			                                              Target.gameObject.transform.position + new Vector3(0f, 0.5f, 0f), step);
		} else if (TargetTower != null && !TargetTower.isDead) {
			this.dead = false;
			float step = Speed * Time.deltaTime;
			this.transform.position = Vector3.MoveTowards(this.transform.position, 
			                                              TargetTower.gameObject.transform.position + new Vector3(0f, 0.5f, 0f), step);
		} else {
//			this.dead = true;
			this.setDead(true);
		}
	}
	
	void OnTriggerEnter2D (Collider2D other) {

		if (!PhotonNetwork.isMasterClient) return;
		if (TargetTower != null) {
			if (other.tag == TargetTower.tag) this.setDead(true);
		}

	}
}