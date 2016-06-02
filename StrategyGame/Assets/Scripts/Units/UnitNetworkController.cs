using UnityEngine;
using System.Collections;

public class UnitNetworkController : Photon.MonoBehaviour {

	Battle_ST_Game.Unit controllerScript;

	public bool isMine = false;

	void Awake()
	{
		controllerScript = this.GetComponent<Battle_ST_Game.Unit>();
		
		isMine = photonView.isMine;
//		gameObject.name = gameObject.name + photonView.viewID;
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			//We own this player: send the others our data
//			stream.SendNext((int)controllerScript._characterState);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation); 
			stream.SendNext(controllerScript.status.hp);
			stream.SendNext(controllerScript.dead);
		} else {
			//Network player, receive data
//			controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			controllerScript.status.hp = (int)stream.ReceiveNext();
			controllerScript.dead = (bool)stream.ReceiveNext();
			controllerScript.updateStatus();
		}
	}
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
	
	void Update() {

		if (!PhotonNetwork.isMasterClient) {
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
			controllerScript.updateStatus();
		}
	}
}
