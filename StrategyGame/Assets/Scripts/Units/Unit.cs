using UnityEngine;
using System.Collections;


namespace Battle_ST_Game {

	public class Unit : MonoBehaviour {

		public UnitStatus status;	//!<Unit status

		public bool dead { get; protected set;}	//!<unit are dead

		protected int placeNo;//!<１～６
		protected Vector3 defaultPosition;	//!<Used to return at the start of the location

		public class UnitStatus
		{
			public int unitNo;	//!<Unit id
			public string name; //!<Unit name

			public int hp;	    //!<Hp of unit
			public int maxHp;   //!<Max hp of unit
			public int mana;	//!<Unit mana
			public int requestMana;	//!<Unit mana for use skill

			public int rangeAttack; //!<Attack Range
			public int attack;	//!<Base attack damage
			public int def;	//!<Base def 
			public int heal;	//!<Base heal

			public WEAPON_KIND weaponKind; //!<weapon kind
			public MEMBER_KIND memberKind; //!<member kind: player or opponent
			public bool leader; //!<leader
		}

		/** Member kind enum */
		public enum MEMBER_KIND 
		{
			Player ,	//!<近接攻撃
			Opponent ,	//!<遠距離攻撃
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
		 * Find nearest Enemy to attack
		 * 
		 * @param status player status
		 * 
		 * @return enemy status
		 */ 
		protected UnitStatus findNearestEnemy(UnitStatus status) {

			return null;
		}

		/**
		 * Attack Enemy 
		 * 
		 * @param playerStatus: player status
		 * @param enemyStatus: enemy status
		 */ 
		protected void attackEnemy(UnitStatus playerStatus, UnitStatus enemyStatus) {

		}

	}



}
