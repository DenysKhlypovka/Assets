using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsController : MonoBehaviour {

	public GameObject SecondStairs;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerStay2D(Collider2D other)
	{
		if (other.GetComponent<CharacterControllerScript> () == null)
			return;

		CharacterControllerScript playerScript = GameObject.FindWithTag ("Player").GetComponent<CharacterControllerScript>();


		playerScript.ChangeFloor (this.gameObject.transform.position.x, SecondStairs.transform.position.y);

	}
}
