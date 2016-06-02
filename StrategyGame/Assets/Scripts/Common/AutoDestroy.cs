using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

	public float timeForDestroy = 3f;

	// Use this for initialization
	void Start () {
//		if (!PhotonNetwork.isMasterClient) {
//			return;
//		}
		StartCoroutine(setDestroy());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator setDestroy() {
		yield return new WaitForSeconds(timeForDestroy);
		this.GetComponent<Bullet>().dead = true;
		this.GetComponent<Bullet>().setDead(true);
		yield return null;
	}
}
