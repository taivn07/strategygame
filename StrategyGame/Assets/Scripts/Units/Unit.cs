using UnityEngine;
using System.Collections;


namespace Battle_ST_Game {

	public class Unit : MonoBehaviour {

		public UnitStatus status;	//!<Unit status

		protected Animator _animator; //!<Unit animator

		protected SpriteRenderer _spriteRenderer; //!<Unit sprite renderer

		public bool dead { get; protected set;}	//!<unit are dead

		protected int placeNo;//!<１～６
		protected Vector3 defaultPosition;	//!<Used to return at the start of the location

		protected CircleCollider2D _sightOfUnit; //!<Sight of unit

		private const string UNIT_SPRITE_PATH = "Icons/f002";


		public class UnitStatus {
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

		void Awake() {


			//get status
//			if (status == null) return;


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

			Sprite sp = Resources.Load(UNIT_SPRITE_PATH) as Sprite;

			if (sp) _spriteRenderer.sprite = sp;
			
			//setup sight
			if (!_sightOfUnit) _sightOfUnit = this.gameObject.GetComponentInChildren<CircleCollider2D>();
			//			_sightOfUnit.radius = this.status.rangeAttack;
			_sightOfUnit.radius = 5;
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
