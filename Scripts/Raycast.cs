using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour {

	CharacterControllerScript playerScript;
	EnemyController carrierScript;

	public float playerRaycastDistance = 6;
	public float wallRaycastDistance = 3;
	public float groundRaycastDistance = 3;

	// Use this for initialization
	void Start () {
		carrierScript = this.transform.parent.gameObject.GetComponent<EnemyController> ();
		playerScript = GameObject.Find ("player").GetComponent<CharacterControllerScript>();
	}
	
	// Update is called once per frame
	void Update () {

		int layerMaskPlayer = 512;
		int layerMaskWall = 1024;
		float distance;

		Vector2 forward = transform.TransformDirection (Vector2.right) * wallRaycastDistance; 
		RaycastHit2D hitWall = Physics2D.Raycast (transform.position, forward, wallRaycastDistance, layerMaskWall);


		Vector2 down = transform.TransformDirection (new Vector2(1, -1)) * groundRaycastDistance;
		RaycastHit2D hitGround = Physics2D.Raycast (transform.position, down, groundRaycastDistance, layerMaskWall);

		Debug.DrawRay (transform.position, down, Color.blue);


		if (hitWall || !hitGround) {

			carrierScript.SetImpassable (true);
		}
		else
			carrierScript.SetImpassable(false);

		for (int i = -1; i < 2; i++) {
			forward.y = i;
			Debug.DrawRay (transform.position, forward, Color.green);
			RaycastHit2D hit = Physics2D.Raycast (transform.position, forward, playerRaycastDistance, layerMaskPlayer);
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

