using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour {

	CharacterControllerScript playerScript;
	EnemyController carrierScript;

	public float raycastDistance = 6;

	// Use this for initialization
	void Start () {
		carrierScript = this.transform.parent.gameObject.GetComponent<EnemyController> ();
		playerScript = GameObject.Find ("player").GetComponent<CharacterControllerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		
		int layerMask = 512;
		float distance;

		Vector2 forward = transform.TransformDirection (Vector2.right) * 3; 


		for (int i = -1; i < 2; i++) {
			forward.y = i;
			Debug.DrawRay (transform.position, forward, Color.green);
			RaycastHit2D hit = Physics2D.Raycast (transform.position, forward, raycastDistance, layerMask);
			if (hit && playerScript.IsVisible()) {
				carrierScript.PlayerDetected(true);
				//distance = hit.distance;
				//Debug.Log (i + ") " + distance + " " + hit.collider.gameObject.name);
			}
			else
				carrierScript.PlayerDetected(false);
		}
	}
}
