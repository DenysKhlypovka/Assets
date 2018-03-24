using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	private bool isShotByEnemy;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetShotByEnemy(bool _isByEnemy)
	{
		isShotByEnemy = true;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "enemy" && isShotByEnemy) {
			return;
		}
		if (coll.tag == "Player") {
			coll.gameObject.GetComponent<CharacterControllerScript> ().ApplyDamage ();
			Destroy(this.gameObject);
		}

		if (coll.tag == "enemy") {
			Debug.LogWarning ("zdarowa");
			string areaOfApplication = "unknown";

			if (coll.GetType () == typeof(CapsuleCollider2D))
				areaOfApplication = "body";
			if (coll.GetType () == typeof(CircleCollider2D))
				areaOfApplication = "head";

			coll.gameObject.GetComponent<EnemyController> ().ApplyDamage (areaOfApplication);
			Destroy(this.gameObject);
		}
		if (coll.gameObject.layer == 10)
			Destroy(this.gameObject);
	}
}
