using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

	public float timeForDestroy = 4f;

	// Use this for initialization
	void Start () {
		Destroy(this.gameObject, timeForDestroy);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
