using UnityEngine;
using System.Collections;

public class ListArrayUtil : MonoBehaviour {

	/***
	 * Find min of number array
	 * 
	 * @param float array
	 * 
	 * @return index of min number
	 */
	public static int getMinIndexFrom(float[] array) {
		if (array.Length <= 0) return -1;
		int result = -1;
		float temp = 1000f;
		for(int i = 0; i < array.Length; i++) {
			if (array[i] < temp) {
				result = i;
				temp = array[i];
			}
		}

		return result;
	}

	/***
	 * Find min of number array
	 * 
	 * @param int array
	 * 
	 * @return index of min number
	 */
	public static int getMinIndexFrom(int[] array) {
		if (array.Length <= 0) return -1;
		int result = -1;
		int temp = 1000;
		for(int i = 0; i < array.Length; i++) {
			if (array[i] < temp) {
				result = i;
				temp = array[i];
			}
		}
		
		return result;
	}

}
