using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Battle_ST_Game;

namespace Battle_ST_Game {

	public class BattleManager : MonoBehaviour {
		
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

		private Vector3[] _bottomUnitPositions = new Vector3[5] { 
			new Vector3(-9f,0f,0f),       //!<1 
			new Vector3(-5f,0f,0f),      //!<2 
			new Vector3(0f,0f,0f),        //!<3 
			new Vector3(5f,0f,0f),      //!<4 
			new Vector3(9f,0f,0f),        //!<5 
		};

		private Vector3[] _topUnitPositions = new Vector3[5] { 
			new Vector3(-9f,0f,0f),       //!<1 
			new Vector3(-5f,0f,0f),      //!<2 
			new Vector3(0f,0f,0f),        //!<3 
			new Vector3(5f,0f,0f),      //!<4 
			new Vector3(9f,0f,0f),        //!<5 
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

			instance.gameState = GAME_STATE.PREPARING;

			//top formation
			_topFormation = gameObject.transform.FindChild(BATTLE_TOP_AREA_PATH).gameObject;

			//Bottom formation
			_bottomFormation = gameObject.transform.FindChild(BATTLE_BOTTOM_AREA_PATH).gameObject;

		}
		
		void Start () {
			makeDemoData();

			//make game start
			instance.gameState = GAME_STATE.PLAYING;

			_bottomUnits[0].SendMessage("goTo", BattleManager.instance._topUnits[0].gameObject);
		}
		
		public void Initalize() {
			
		}

		public void startBattle() {

		}

		private void makeDemoData() {
			_bottomUnits.Clear();
			_topUnits.Clear();

			//bottom units
			for(int i = 0; i< 5; i++) {
				Battle_ST_Game.Unit.UnitStatus stt = new Unit.UnitStatus();
				stt.unitNo = Random.Range(1, 7); 
				stt.maxHp = 400;
				stt.hp = stt.maxHp;
				stt.mana = 10;
				stt.attack = Random.Range(10, 150);
				stt.def = Random.Range(5, 55);
				stt.leader = false;

				stt.weaponKind = getWeaponKindFrom(stt.unitNo);
				stt.rangeAttack = getRangeAttackFrom(stt.weaponKind);
				stt.memberKind = Unit.MEMBER_KIND.BOTTOM_SIDE;

				makeBottomUnit(stt, i);
			}

			//top units
			for(int i = 0; i< 5; i++) {
//				Battle_ST_Game.Unit unit = new Unit();
				Battle_ST_Game.Unit.UnitStatus stt = new Unit.UnitStatus();
				stt.unitNo = Random.Range(1, 7); 
				stt.maxHp = 400;
				stt.hp = stt.maxHp;
				stt.mana = 10;
				stt.attack = Random.Range(10, 150);
				stt.def = Random.Range(5, 55);
				stt.leader = false;

				stt.weaponKind = getWeaponKindFrom(stt.unitNo);
				stt.rangeAttack = getRangeAttackFrom(stt.weaponKind);
				stt.memberKind = Unit.MEMBER_KIND.TOP_SIDE;

				makeTopUnit(stt, i);
			}
		}
		
		private void makeBottomUnit(Unit.UnitStatus status, int placeIndex) {
			if (status == null) return;
			
			GameObject go = Instantiate(Resources.Load(UNIT_PREFAB_PATH)) as GameObject;
			go.name = string.Format("bottom_unit_{0}", placeIndex);
			go.transform.parent = _bottomFormation.transform;
			go.transform.localPosition = _bottomUnitPositions[placeIndex];
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = new Vector3(1, 1, 1);
			
			//Add unit
			Unit unit = go.gameObject.GetComponent<Unit>();
			unit.Initalize(status, placeIndex + 1);
			_bottomUnits.Add(unit);
		}

		private void makeTopUnit(Unit.UnitStatus status, int placeIndex) {
			if (status == null) return;
			
			GameObject go = Instantiate(Resources.Load(UNIT_PREFAB_PATH)) as GameObject;
			go.name = string.Format("top_unit_{0}", placeIndex);
			go.transform.parent = _topFormation.transform;
			go.transform.localPosition = _topUnitPositions[placeIndex];
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = new Vector3(1, 1, 1);
			
			//Add unit
			Unit unit = go.gameObject.GetComponent<Unit>();
			unit.Initalize(status, placeIndex + 1);
			_topUnits.Add(unit);
		}
		
		
		void Update () {
			
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
			float result = 1;
			switch(kind) {
			case Unit.WEAPON_KIND.AXE:
				result = 1;
				break;
			case Unit.WEAPON_KIND.HAMMER:
				result = 2;
				break;
			case Unit.WEAPON_KIND.SWORD:
				result = 3;
				break;
			case Unit.WEAPON_KIND.RIFLE:
				result = 5.5f;
				break;
			case Unit.WEAPON_KIND.MACHINE_GUN:
				result = 7;
				break;
			default:
				break;
			}

			return result;
		}

	}
}
