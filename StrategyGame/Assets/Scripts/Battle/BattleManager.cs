using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Battle_ST_Game;

public class BattleManager : MonoBehaviour {

	//instance
	public static BattleManager instance { get; private set;}

	//unit list
	public List<Battle_ST_Game.Unit> _bottomUnits;
	public List<Battle_ST_Game.Unit> _topUnits;

	public Vector3[] _bottomUnitPositions;
	public Vector3[] _topUnitPositions;

	private const string UNIT_PREFAB_PATH = "Prefabs/Units/Unit";

	void Awake () {
		instance = this;
	}

	void Start () {

		Battle_ST_Game.Unit.UnitStatus stt = new Unit.UnitStatus();
		stt.unitNo = 2;
		stt.rangeAttack = 5;

		makeBottomUnit(stt, 1);
	}

	public void Initalize() {

	}

	private void makeBottomUnit(Unit.UnitStatus status, int index) {
		if (status == null) return;
		
		GameObject go = Instantiate(Resources.Load(UNIT_PREFAB_PATH)) as GameObject;
		go.name = string.Format("bottom_unit_{0}", index);
//		card.transform.parent = _charaImage.transform;
//		card.transform.localPosition = PlayerCharaPos[placeIndex];
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = new Vector3(1, 1, 1);

		//Add unit
		Unit unit = go.gameObject.GetComponent<Unit>();
		unit.Initalize(status, index + 1);
		_bottomUnits.Add(unit);
	}


	void Update () {
	
	}
}
