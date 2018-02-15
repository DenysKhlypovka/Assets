using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		Debug.Log ("trigger entered");
		if (coll.tag == "enemy")
			coll.gameObject.GetComponent<EnemyController> ().Kill();
		if (coll.tag == "enemy" || coll.tag == "ground")
			Destroy(this.gameObject);
	}
}
