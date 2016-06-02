using UnityEngine;
using System.Collections;

public class Tower : Photon.MonoBehaviour {

	public float health = 500;

	public bool isDead = false;

	private bool isBottom = false;

	string enemyBulletTag = "";

	void Start () {
		if (this.gameObject.tag == "Tower_Bottom") {
			enemyBulletTag = "Unit_Top_Bullet";
			isBottom = true;
		} else {
			enemyBulletTag = "Unit_Bottom_Bullet";
			isBottom = false;
		}

		this.GetComponent<HealthBar>().curHealth = this.GetComponent<HealthBar>().maxHealth = (int)health;
	}

	public void updateStatus() {
		this.GetComponent<HealthBar>().curHealth = (int)health;
		if (health <= 0) {
			health = 0;
			isDead = true;
			setDead();
			//			Battle_ST_Game.BattleManager.instance.endTheGame(isBottom);
		}
	}

	public void setDead() {
		this.GetComponent<SpriteRenderer>().enabled = !this.isDead;
		this.GetComponent<BoxCollider2D>().enabled = !this.isDead;
		this.GetComponent<HealthBar>().enabled = !this.isDead;
		bool botWin = false;
		if (!isBottom) {
			botWin = true;
		}
		object[] obj = new object[]{botWin};
		photonView.RPC("stopGame", PhotonTargets.AllBuffered, obj);
	}
	
	[PunRPC]
	public void stopGame(bool botWin) {
		Battle_ST_Game.BattleManager.instance.gameState = Battle_ST_Game.BattleManager.GAME_STATE.STOPPED;
		Battle_ST_Game.BattleManager.instance.endTheGame(botWin);
	}

	void OnTriggerEnter2D (Collider2D other) {

		if(other.gameObject.tag == enemyBulletTag) {
			health -= other.gameObject.GetComponent<Bullet>().damage;
			this.GetComponent<HealthBar>().curHealth = (int)health;
		}

		if (health <= 0) {
			health = 0;
			isDead = true;
//			Battle_ST_Game.BattleManager.instance.endTheGame(isBottom);
		}
		
		this.updateStatus();
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			//We own this player: send the others our data
			//			stream.SendNext((int)controllerScript._characterState);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation); 
			stream.SendNext(this.health);
			stream.SendNext(this.isDead);
		} else {
			//Network player, receive data
			//			controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			this.health = (float)stream.ReceiveNext();
			this.isDead = (bool)stream.ReceiveNext();
			this.updateStatus();
		}
	}

	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
	
	void Update() {
		
		if (!PhotonNetwork.isMasterClient) {
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
			this.updateStatus();
		}
	}

}
