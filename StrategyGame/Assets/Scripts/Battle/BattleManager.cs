using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle_ST_Game;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Battle_ST_Game {

	public class BattleManager : Photon.MonoBehaviour {

		public Transform playerPrefab;

		//instance
		public static BattleManager instance { get; private set;}

		public GAME_STATE gameState{ get; set;}
		
		//unit list
		public List<Battle_ST_Game.Unit> _bottomUnits;
		public List<Battle_ST_Game.Unit> _topUnits;

		private GameObject _topFormation, _bottomFormation;

		private const string BATTLE_PREFAB_PATH = "Prefabs/Battle/GameBattle";
		private const string UNIT_PREFAB_PATH = "Prefabs/Units/Unit";

		private const string BATTLE_TOP_AREA_PATH = "BattleArea/Top_Formation";
		private const string BATTLE_BOTTOM_AREA_PATH = "BattleArea/Bottom_Formation";

		
		
		//UI
		[SerializeField]
		private GameObject winPanel;
		
		[SerializeField]
		private Text winText;

		private Vector3[] _bottomUnitPositions = new Vector3[5] { 
			new Vector3(-8f,0f,0f),       //!<1 
			new Vector3(-4f,0f,0f),      //!<2 
			new Vector3(0f,0f,0f),        //!<3 
			new Vector3(4f,0f,0f),      //!<4 
			new Vector3(8f,0f,0f),        //!<5 
		};

		private Vector3[] _topUnitPositions = new Vector3[5] { 
			new Vector3(-8f,0f,0f),       //!<1 
			new Vector3(-4f,0f,0f),      //!<2 
			new Vector3(0f,0f,0f),        //!<3 
			new Vector3(4f,0f,0f),      //!<4 
			new Vector3(8f,0f,0f),        //!<5 
		};

		/** Game state */
		public enum GAME_STATE 
		{
			PLAYING ,	//!<playing
			PREPARING ,	//!<preparing
			PAUSING, //!<pausing
			STOPPED, //!<Game stopped
		}
		
		void Awake () {

			instance = this;

//			PhotonNetwork.automaticallySyncScene = true;
			this.winPanel.SetActive(false);

			instance.gameState = GAME_STATE.PREPARING;

			//top formation
			_topFormation = gameObject.transform.FindChild(BATTLE_TOP_AREA_PATH).gameObject;

			//Bottom formation
			_bottomFormation = gameObject.transform.FindChild(BATTLE_BOTTOM_AREA_PATH).gameObject;

			// in case we started this demo with the wrong scene being active, simply load the menu scene
			if (!PhotonNetwork.connected)
			{
				SceneManager.LoadScene(BattleMenu.SceneNameMenu);
				return;
			}

		}

//		public void OnGUI()
//		{
//			if (GUILayout.Button("Return to main menu"))
//			{
//				PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
//			}
//		}
		
		void Start () {
			makeDemoData();
		}
		
		public void Initalize() {
			
		}

		[PunRPC]
		public void startBattle() {
			
			//make game start
			if(this.checkAllPlayerGetReady() && instance.gameState != GAME_STATE.PLAYING){
				StartCoroutine(this.setUpListPlayerEnemy());
				instance.gameState = GAME_STATE.PLAYING;
			} 
		}

		private void makeDemoData() {

			if ( PhotonNetwork.isMasterClient ) {
				_bottomUnits.Clear();
				//bottom units
				for(int i = 0; i< 5; i++) {
					Battle_ST_Game.Unit.UnitStatus stt = new Unit.UnitStatus();
					stt.unitNo = Random.Range(1, 7); 
					stt.name = "Unit_Bottom_"+stt.unitNo;
					stt.maxHp = Random.Range(200, 400);
					stt.hp = stt.maxHp;
					stt.mana = 10;
					stt.attack = Random.Range(15, 100);
					stt.def = Random.Range(5, 25);
					stt.leader = false;
					
					stt.weaponKind = getWeaponKindFrom(stt.unitNo);
					stt.rangeAttack = getRangeAttackFrom(stt.weaponKind);
					stt.memberKind = Unit.MEMBER_KIND.BOTTOM_SIDE;
					
//					makeBottomUnit(stt, i);
					var dict = ObjectUtil.unitStatusToDic(stt);
					object[] obj = new object[]{dict, i};
//					makeBottomUnit(dict, i);
					photonView.RPC("makeBottomUnit", PhotonTargets.All, obj);
				}



				Hashtable readyProperty = new Hashtable() {{"ready", true}};
				PhotonNetwork.player.SetCustomProperties(readyProperty);
			} else {
				//top units
				_topUnits.Clear();
				for(int i = 0; i< 5; i++) {

					Battle_ST_Game.Unit.UnitStatus stt = new Unit.UnitStatus();
					stt.unitNo = Random.Range(1, 7); 
					stt.name = "Unit_Top_"+stt.unitNo;
					stt.maxHp = Random.Range(200, 400);
					stt.hp = stt.maxHp;
					stt.mana = 10;
					stt.attack = Random.Range(10, 150);
					stt.def = Random.Range(5, 25);
					stt.leader = false;
					
					stt.weaponKind = getWeaponKindFrom(stt.unitNo);
					stt.rangeAttack = getRangeAttackFrom(stt.weaponKind);
					stt.memberKind = Unit.MEMBER_KIND.TOP_SIDE;
					
					var dict = ObjectUtil.unitStatusToDic(stt);
					object[] obj = new object[]{dict, i};
//					makeTopUnit(dict, i);
					photonView.RPC("makeTopUnit", PhotonTargets.OthersBuffered, obj);
				}
				Hashtable readyProperty = new Hashtable() {{"ready", true}};
				PhotonNetwork.player.SetCustomProperties(readyProperty);

				photonView.RPC("startBattle", PhotonTargets.All, null);
			}
		}

		[PunRPC]
		public void makeBottomUnit(Dictionary<string,string> dict, int placeIndex) {
//			if(_bottomUnits.Count >= 5) return ;
			object[] obj = ObjectUtil.dicToUnitObjectArray(dict);
			if (obj == null) return;

			// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
			GameObject go = PhotonNetwork.Instantiate(UNIT_PREFAB_PATH, transform.position, Quaternion.identity, 0, obj);

			go.name = string.Format("bottom_unit_{0}", placeIndex);
			go.transform.parent = _bottomFormation.transform;
			go.transform.localPosition = _bottomUnitPositions[placeIndex];
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = new Vector3(1, 1, 1);

			Unit unit = go.gameObject.GetComponent<Unit>();
			_bottomUnits.Add(unit);
		}

		[PunRPC]
		public void makeTopUnit(Dictionary<string,string> dict, int placeIndex) {

//			if(_topUnits.Count >= 5) return ;
			object[] obj = ObjectUtil.dicToUnitObjectArray(dict);
			if (obj == null) return;

			// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
			GameObject go = PhotonNetwork.Instantiate(UNIT_PREFAB_PATH, transform.position, Quaternion.identity, 0, obj);

			go.name = string.Format("top_unit_{0}", placeIndex);
			go.transform.parent = _topFormation.transform;
			go.transform.localPosition = _topUnitPositions[placeIndex];
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = new Vector3(1, 1, 1);
			
			//Add unit
			Unit unit = go.gameObject.GetComponent<Unit>();
			_topUnits.Add(unit);

		}

		/**
		 * Check if all bottom are dead 
		 */ 
		public bool checkAllDeadBottomUnit() {
			bool result = true;
			foreach(Unit u in instance._bottomUnits) {
				if (!u.dead) result = false;
			}

			return result;
		}

		/**
		 * Check if all top are dead 
		 */
		public bool checkAllDeadTopUnit() {
			bool result = true;
			foreach(Unit u in instance._topUnits) {
				if (!u.dead) result = false;
			}
			
			return result;
		}

		public void endTheGame(bool winnerBot) {
			//bot is winner
			if ((winnerBot && PhotonNetwork.isMasterClient) || (!winnerBot && !PhotonNetwork.isMasterClient)) {
				winText.text = "You win!!!";
			} else {//top is winner
				winText.text = "You lose!!!";
			}

			this.winPanel.SetActive(true);
		}

		private bool checkAllPlayerGetReady() {
			if (PhotonNetwork.playerList.Length < 2) return false;
			bool result = true;
			foreach(PhotonPlayer player in PhotonNetwork.playerList) {

				if (player.customProperties["ready"] == null) break;

				if(!(bool)player.customProperties["ready"]) {
					result = false;
				}
			}

			return result;
		}
		
		void Update () {
			Debug.Log(BattleManager.instance.gameState.ToString());
			Debug.Log("bot member: "+_bottomUnits.Count);
			Debug.Log("top member: "+_topUnits.Count);
		}

		public void backToMain() {
			PhotonNetwork.LeaveRoom();
		}

		/**
		 * Get WEAPON_KIND from id
		 */ 
		public static Unit.WEAPON_KIND getWeaponKindFrom(int id) {
			Unit.WEAPON_KIND result;
			switch(id) {
			case 1:
				result = Unit.WEAPON_KIND.AXE;
				break;
			case 2:
				result = Unit.WEAPON_KIND.HAMMER;
				break;
			case 3:
				result = Unit.WEAPON_KIND.SWORD;
				break;
			case 4:
				result = Unit.WEAPON_KIND.RIFLE;
				break;
			case 5:
				result = Unit.WEAPON_KIND.MACHINE_GUN;
				break;
			default:
				result = Unit.WEAPON_KIND.AXE;
				break;
			}
			
			return result;
		}

		/**
		 * Get range attack from weapo kind
		 */ 
		public static float getRangeAttackFrom(Unit.WEAPON_KIND kind) {
			float result = 5;
			switch(kind) {
			case Unit.WEAPON_KIND.AXE:
				result = 5;
				break;
			case Unit.WEAPON_KIND.HAMMER:
				result = 5;
				break;
			case Unit.WEAPON_KIND.SWORD:
				result = 6;
				break;
			case Unit.WEAPON_KIND.RIFLE:
				result = 8.5f;
				break;
			case Unit.WEAPON_KIND.MACHINE_GUN:
				result = 9;
				break;
			default:
				break;
			}

			return result;
		}

		void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.isWriting) {
				//We own this player: send the others our data
				//			stream.SendNext((int)controllerScript._characterState);
				stream.SendNext(transform.position);
				stream.SendNext(transform.rotation); 
//				stream.SendNext(_topUnits);
//				stream.SendNext(_bottomUnits);
			} else {
				//Network player, receive data
				//			controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
//				_topUnits = (List<Battle_ST_Game.Unit>)stream.ReceiveNext();
//				_bottomUnits = (List<Battle_ST_Game.Unit>)stream.ReceiveNext();
			}
		}

		public void OnMasterClientSwitched(PhotonPlayer player)
		{
			Debug.Log("OnMasterClientSwitched: " + player);
			
			string message;
			InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message
			
			if (chatComponent != null)
			{
				// to check if this client is the new master...
				if (player.isLocal)
				{
					message = "You are Master Client now.";
				}
				else
				{
					message = player.name + " is Master Client now.";
				}
				
				
				chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
			}
		}

		private IEnumerator setUpListPlayerEnemy() {
			if ( PhotonNetwork.isMasterClient) {
				yield return _topUnits = convertObjectToUnit(getAllEnemies());
				
			} else {
				yield return _bottomUnits = convertObjectToUnit(getAllEnemies());
			}

			yield return null;
		}

		private List<GameObject> getAllEnemies() {
			List<GameObject> list = new List<GameObject>();
			if ( PhotonNetwork.isMasterClient) {
				GameObject[] goList = GameObject.FindGameObjectsWithTag(Unit.TOP_TAG);
				list = goList.ToList();
			} else {
				GameObject[] goList = GameObject.FindGameObjectsWithTag(Unit.BOTTOM_TAG);
				list = goList.ToList();
			}

			return list;
		}

		private List<Unit> convertObjectToUnit(List<GameObject> list) {
			List<Unit> li = new List<Unit>();
			foreach(GameObject go in list) {
				li.Add(go.GetComponent<Unit>());
			}
			return li;

		}
		
		public void OnLeftRoom()
		{
			Debug.Log("OnLeftRoom (local)");
			
			// back to main menu
			SceneManager.LoadScene(BattleMenu.SceneNameMenu);
		}
		
		public void OnDisconnectedFromPhoton()
		{
			Debug.Log("OnDisconnectedFromPhoton");
			
			// back to main menu
			SceneManager.LoadScene(BattleMenu.SceneNameMenu);
		}
		
		public void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
		}
		
		public void OnPhotonPlayerConnected(PhotonPlayer player)
		{
			Debug.Log("OnPhotonPlayerConnected: " + player);
		}
		
		public void OnPhotonPlayerDisconnected(PhotonPlayer player)
		{
			Debug.Log("OnPlayerDisconneced: " + player);
			PhotonNetwork.LeaveRoom();
		}
		
		public void OnFailedToConnectToPhoton()
		{
			Debug.Log("OnFailedToConnectToPhoton");
			
			// back to main menu
			SceneManager.LoadScene(BattleMenu.SceneNameMenu);
		}

	}
	
}
