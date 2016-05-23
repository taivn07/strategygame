using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float Speed = 7.5f;
	public GameObject Target = null;
	public float damage = 10f;

	void Start() {
		
	}


	void Update() {
		if (Target != null) {
			float step = Speed * Time.deltaTime;
			this.transform.position = Vector3.MoveTowards(this.transform.position, 
			                                              Target.transform.position + new Vector3(0f, 0.5f, 0f), step);
		} else {
			Destroy(this.gameObject);
		}
	}
	
	public void OnTriggerEnter(Collider other) {

	}
}