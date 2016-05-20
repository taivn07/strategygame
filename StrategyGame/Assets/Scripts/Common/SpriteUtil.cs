using UnityEngine;
using System.Collections;

namespace Battle_ST_Game {

	public class SpriteUtil : MonoBehaviour {

		private const string UNIT_NOSPRITE_PATH = "Icons/f000";

		/**
		 * Get sprite from resource folder
		 * 
		 * @param path: path of sprite
		 * 
		 * @return Sprite
		 */ 
		public static Sprite loadSpriteFromSource(string path) {

			Sprite s = Resources.Load<Sprite>(path) as Sprite;

			if (!s) {
				Debug.Log("Cant find this sprite!!!");
				return Resources.Load<Sprite>(UNIT_NOSPRITE_PATH) as Sprite;
			}

			return s;

		}
		
	}

}
