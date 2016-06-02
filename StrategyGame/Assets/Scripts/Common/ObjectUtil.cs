using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectUtil : MonoBehaviour {

	public static Dictionary<TKey, TValue> objectToDic<TKey, TValue>(object obj)
	{
		var stringDictionary = obj as Dictionary<TKey, TValue>;
		
		if (stringDictionary!= null)
		{
			return stringDictionary;
		}
		var baseDictionary = obj as IDictionary;
		
		if (baseDictionary != null)
		{
			var dictionary = new Dictionary<TKey, TValue>();
			foreach (DictionaryEntry keyValue in baseDictionary)
			{
				if (!(keyValue.Value is TValue))
				{
					// value is not TKey. perhaps throw an exception
					return null;
				}
				if (!(keyValue.Key is TKey))
				{
					// value is not TValue. perhaps throw an exception
					return null;
				}
				
				dictionary.Add((TKey)keyValue.Key, (TValue)keyValue.Value);
			}
			return dictionary;
		}
		// object is not a dictionary. perhaps throw an exception
		return null;
		
	}

	public static Dictionary<string,string> unitStatusToDic(Battle_ST_Game.Unit.UnitStatus status) {

		if (status == null) return null;

		var dict = new Dictionary<string,string>();
		dict.Add("unitNo", status.unitNo.ToString());
		dict.Add("name", status.name);
		dict.Add("hp", status.hp.ToString());
		dict.Add("maxHp", status.maxHp.ToString());
		dict.Add("mana", status.mana.ToString());
		dict.Add("requestMana", status.requestMana.ToString());
		dict.Add("rangeAttack", status.rangeAttack.ToString());
		dict.Add("attack", status.attack.ToString());
		dict.Add("def", status.def.ToString());
		dict.Add("heal", status.heal.ToString());

		dict.Add("weaponKind", ((int)status.weaponKind).ToString());

		if (status.memberKind == Battle_ST_Game.Unit.MEMBER_KIND.BOTTOM_SIDE) {
			dict.Add("memberKind", "bottom");
		} else {
			dict.Add("memberKind", "top");
		}

		if(status.leader) {
			dict.Add("leader", "1");
		} else {
			dict.Add("leader", "0");
		}

		return dict;

	}

	public static Battle_ST_Game.Unit.UnitStatus dicToUnitStatus(Dictionary<string,string> dict) {
		
		if (dict == null) return null;
		
		var unit = new Battle_ST_Game.Unit.UnitStatus();

		if (dict.ContainsKey("unitNo")) {
			unit.unitNo = int.Parse(dict["unitNo"]);
		}

		if (dict.ContainsKey("name")) {
			unit.name = dict["name"];
		}

		if (dict.ContainsKey("hp")) {
			unit.hp = int.Parse(dict["hp"]);
		}

		if (dict.ContainsKey("maxHp")) {
			unit.maxHp = int.Parse(dict["maxHp"]);
		}

		if (dict.ContainsKey("mana")) {
			unit.mana = int.Parse(dict["mana"]);
		}

		if (dict.ContainsKey("requestMana")) {
			unit.requestMana = int.Parse(dict["requestMana"]);
		}
		
		if (dict.ContainsKey("rangeAttack")) {
			unit.rangeAttack = float.Parse(dict["rangeAttack"]);
		}
		
		if (dict.ContainsKey("attack")) {
			unit.attack = int.Parse(dict["attack"]);
		}
		
		if (dict.ContainsKey("def")) {
			unit.def = int.Parse(dict["def"]);
		}
		
		if (dict.ContainsKey("heal")) {
			unit.heal = int.Parse(dict["heal"]);
		}

		if (dict.ContainsKey("weaponKind")) {
			int kind =int.Parse(dict["weaponKind"]);

			if (kind == 2) {
				unit.weaponKind = Battle_ST_Game.Unit.WEAPON_KIND.SWORD;
			} else if (kind == 3) {
				unit.weaponKind = Battle_ST_Game.Unit.WEAPON_KIND.HAMMER;
			} else if (kind == 4) {
				unit.weaponKind = Battle_ST_Game.Unit.WEAPON_KIND.RIFLE;
			} else if (kind == 5) {
				unit.weaponKind = Battle_ST_Game.Unit.WEAPON_KIND.MACHINE_GUN;
			} else {
				unit.weaponKind = Battle_ST_Game.Unit.WEAPON_KIND.AXE;
			}
		}

		if (dict.ContainsKey("memberKind")) {
			if (dict["memberKind"] == "bottom") {
				unit.memberKind = Battle_ST_Game.Unit.MEMBER_KIND.BOTTOM_SIDE;
			} else {
				unit.memberKind = Battle_ST_Game.Unit.MEMBER_KIND.TOP_SIDE;
			}
		}

		if (dict.ContainsKey("leader")) {
			if (dict["weaponKind"] == "1") {
				unit.leader = true;
			} else {
				unit.leader = false;
			}
		}
		return unit;
		
	}

	public static object[] dicToUnitObjectArray(Dictionary<string,string> dict) {
		
		if (dict == null) return null;
		
		object[] obj = new object[13];
		
		if (dict.ContainsKey("unitNo")) {
			obj[0] = int.Parse(dict["unitNo"]);
		}
		
		if (dict.ContainsKey("name")) {
			obj[1] = dict["name"];
		}
		
		if (dict.ContainsKey("hp")) {
			obj[2] = int.Parse(dict["hp"]);
		}
		
		if (dict.ContainsKey("maxHp")) {
			obj[3] = int.Parse(dict["maxHp"]);
		}
		
		if (dict.ContainsKey("mana")) {
			obj[4] = int.Parse(dict["mana"]);
		}
		
		if (dict.ContainsKey("requestMana")) {
			obj[5] = int.Parse(dict["requestMana"]);
		}
		
		if (dict.ContainsKey("rangeAttack")) {
			obj[6] = float.Parse(dict["rangeAttack"]);
		}
		
		if (dict.ContainsKey("attack")) {
			obj[7] = int.Parse(dict["attack"]);
		}
		
		if (dict.ContainsKey("def")) {
			obj[8] = int.Parse(dict["def"]);
		}
		
		if (dict.ContainsKey("heal")) {
			obj[9] = int.Parse(dict["heal"]);
		}
		
		if (dict.ContainsKey("weaponKind")) {
			int kind = int.Parse(dict["weaponKind"]);
			
			if (kind == 2) {
				obj[10] = Battle_ST_Game.Unit.WEAPON_KIND.SWORD;
			} else if (kind == 3) {
				obj[10] = Battle_ST_Game.Unit.WEAPON_KIND.HAMMER;
			} else if (kind == 4) {
				obj[10] = Battle_ST_Game.Unit.WEAPON_KIND.RIFLE;
			} else if (kind == 5) {
				obj[10] = Battle_ST_Game.Unit.WEAPON_KIND.MACHINE_GUN;
			} else {
				obj[10] = Battle_ST_Game.Unit.WEAPON_KIND.AXE;
			}
		}
		
		if (dict.ContainsKey("memberKind")) {
			if (dict["memberKind"] == "bottom") {
				obj[11] = Battle_ST_Game.Unit.MEMBER_KIND.BOTTOM_SIDE;
			} else {
				obj[11] = Battle_ST_Game.Unit.MEMBER_KIND.TOP_SIDE;
			}
		}
		
		if (dict.ContainsKey("leader")) {
			if (dict["weaponKind"] == "1") {
				obj[12] = true;
			} else {
				obj[12] = false;
			}
		}
		return obj;
		
	}

	public static object[] getObjectArrayFrom(Battle_ST_Game.Unit.UnitStatus status) {

		if (status == null) return new object[0];
		object[] obj = new object[13];
		obj[0] = status.unitNo;
		obj[1] = status.name;
		obj[2] = status.hp;
		obj[3] = status.maxHp;
		obj[4] = status.mana;
		obj[5] = status.requestMana;
		obj[6] = status.rangeAttack;
		obj[7] = status.attack;
		obj[8] = status.def;
		obj[9] = status.heal;
		obj[10] = (int)status.weaponKind;
		obj[11] = (int)status.memberKind;
		obj[12] = status.leader;
		return obj;

	}

	public static Battle_ST_Game.Unit.UnitStatus getUnitStatusFrom(object[] obj) {
		if (obj == null) return null;
		
		Battle_ST_Game.Unit.UnitStatus status = new Battle_ST_Game.Unit.UnitStatus();
		status.unitNo = (int)obj[0];
		status.name = (string)obj[1];
		status.hp = (int)obj[2];
		status.maxHp = (int)obj[3];
		status.mana = (int)obj[4];
		status.requestMana = (int)obj[5];
		status.rangeAttack = (float)obj[6];
		status.attack = (int)obj[7];
		status.def = (int)obj[8];
		status.heal = (int)obj[9];

		int wk = (int)obj[10];
		if (wk == 1) {
			status.weaponKind = Battle_ST_Game.Unit.WEAPON_KIND.AXE;
		} else if (wk == 2) {
			status.weaponKind = Battle_ST_Game.Unit.WEAPON_KIND.HAMMER;
		} else if (wk == 3) {
			status.weaponKind = Battle_ST_Game.Unit.WEAPON_KIND.SWORD;
		} else if (wk == 4) {
			status.weaponKind = Battle_ST_Game.Unit.WEAPON_KIND.RIFLE;
		} else if (wk == 5) {
			status.weaponKind = Battle_ST_Game.Unit.WEAPON_KIND.MACHINE_GUN;
		}

		if((int)obj[11] == 0) {
			status.memberKind = Battle_ST_Game.Unit.MEMBER_KIND.BOTTOM_SIDE;
		} else {
			status.memberKind = Battle_ST_Game.Unit.MEMBER_KIND.TOP_SIDE;
		}

		status.leader = (bool)obj[12];
		return status;
		
	}

}
