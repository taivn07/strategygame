using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleMenu : MonoBehaviour {

	public InputField inputName, inputRoom;
	public Text textMember, textConnectionStatus, textRoomsStatus;

	private string roomName = "myRoom";
	
	private bool connectFailed = false;
	
	public static readonly string SceneNameMenu = "BattleMainMenu";
	
	public static readonly string SceneNameGame = "BattleGame";
	
	private string errorDialog;
	private double timeToClearDialog;
	
	public string ErrorDialog
	{
		get { return this.errorDialog; }
		private set {
			this.errorDialog = value;
			if (!string.IsNullOrEmpty(value)) {
				this.timeToClearDialog = Time.time + 4.0f;
			}
		}
	}
	
	public void Awake() {
		// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		PhotonNetwork.automaticallySyncScene = true;
		
		// the following line checks if this client was just created (and not yet online). if so, we connect
		if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated) {
			// Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
			PhotonNetwork.ConnectUsingSettings("0.9");
		}
		
		// generate a name for this player, if none is assigned yet
		if (String.IsNullOrEmpty(PhotonNetwork.playerName)) {
			PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
			inputName.text = PhotonNetwork.playerName;
		}
//		if (String.IsNullOrEmpty(this.roomName)) {
			inputRoom.text = this.roomName;
//		}
		
		// if you wanted more debug out, turn this on:
		// PhotonNetwork.logLevel = NetworkLogLevel.Full;
	}
	
	public void Update() {
		
		if (!PhotonNetwork.connected) {
			if (PhotonNetwork.connecting) {
				textConnectionStatus.text = "Connecting to: " + PhotonNetwork.ServerAddress;
			} else {
				textConnectionStatus.text = "Not connected. Check console output. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress;
			}
			
			if (this.connectFailed) {
				textConnectionStatus.text = "Connection failed. Check setup and use Setup Wizard to fix configuration.";
				//try again
				this.connectFailed = false;
				PhotonNetwork.ConnectUsingSettings("0.9");
			}
			
			return;
		}

		//player name
		if(inputName.text != PhotonNetwork.playerName) {
			PhotonNetwork.playerName = inputName.text;
			PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
		}

		// Join room by title
		if(inputRoom.text != this.roomName) {
			this.roomName = inputRoom.text;
		}

		if (!string.IsNullOrEmpty(ErrorDialog)) {
			
			if (this.timeToClearDialog < Time.time) {
				this.timeToClearDialog = 0;
				ErrorDialog = "";
			}
		}
		
		textMember.text = PhotonNetwork.countOfPlayers + " users are online in " + PhotonNetwork.countOfRooms + " rooms.";

		if (PhotonNetwork.GetRoomList().Length == 0) {
			textRoomsStatus.text = "Currently no games are available.";
		} else {
			textRoomsStatus.text = PhotonNetwork.GetRoomList().Length + " rooms available:";
//			foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
//			{
//				GUILayout.BeginHorizontal();
//				GUILayout.Label(roomInfo.name + " " + roomInfo.playerCount + "/" + roomInfo.maxPlayers);
//				if (GUILayout.Button("Join", GUILayout.Width(150)))
//				{
//					PhotonNetwork.JoinRoom(roomInfo.name);
//				}
//				
//				GUILayout.EndHorizontal();
//			}
//			
//			GUILayout.EndScrollView();
		}
	}
	
	// We have two options here: we either joined(by title, list or random) or created a room.
	public void OnJoinedRoom() {
		Debug.Log("OnJoinedRoom");
	}
	
	public void OnPhotonCreateRoomFailed() {
		ErrorDialog = "Error: Can't create room (room name maybe already used).";
		Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
	}
	
	public void OnPhotonJoinRoomFailed(object[] cause) {
		ErrorDialog = "Error: Can't join room (full or unknown room name). " + cause[1];
		Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
	}
	
	public void OnPhotonRandomJoinFailed() {
		ErrorDialog = "Error: Can't join random room (none found).";
		Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
	}
	
	public void OnCreatedRoom() {
		Debug.Log("OnCreatedRoom");
		PhotonNetwork.LoadLevel(SceneNameGame);
	}
	
	public void OnDisconnectedFromPhoton() {
		Debug.Log("Disconnected from Photon.");
	}
	
	public void OnFailedToConnectToPhoton(object parameters) {
		this.connectFailed = true;
		Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
	}

	public void createRoom() {
		PhotonNetwork.CreateRoom(this.roomName, new RoomOptions() {maxPlayers = 2}, null);
	}

	public void joinRoom() {
		PhotonNetwork.JoinRoom(this.roomName);
	}

	public void joinRandomRoom () {
		PhotonNetwork.JoinRandomRoom();
	}
}
