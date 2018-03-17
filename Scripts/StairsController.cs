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
		CharacterControllerScript player = other.GetComponent<CharacterControllerScript> ();
		EnemyController enemyController = other.GetComponent<EnemyController> ();
		EnemyController enemyAI = other.GetComponent<EnemyAIv2> ();
		if (player == null)
			player.ChangeFloor (this);
		else if (enemyAI == null)
			enemyAI.ChangeFloor (this);
	//	playerScript.ChangeFloor (this.gameObject.transform.position.x, SecondStairs.transform.position.y);

	}
	//public void ChangeFloor(float stairsPosX, float secondStairsY){
	public void ChangeFloor(GameObject target, int DURATION)
	{
		Vector3 translationFitStairs = new Vector3 ();

		float targetPosX = target.gameObject.transform.position.x;
		float targetPosY = target.gameObject.transform.position.y;

		float posDelta = targetPosX - this.gameObject.transform.position.x;
		float posDeltaY = -targetPosY + SecondStairs.transform.position.y - 0.25f;
		Debug.Log ($"posY: {targetPosY}; stairsY: {SecondStairs.transform.position.y}; delta: {posDeltaY}");
		translationFitStairs.x = -posDelta;  

		target.transform.Translate(translationFitStairs);

		target.GetComponent<CharacterControllerScript> ().DisableInput ();

		StartCoroutine(ChangingFloor(target, posDeltaY, DURATION));
	}

	IEnumerator ChangingFloor(GameObject target, float stairsY, int DURATION)
	{
		Vector3 translationHide = new Vector3(0, 0, 30f);
		Vector3 translationShow = new Vector3(0, stairsY, -30f);

		target.transform.Translate (translationHide);
		Debug.Log ("start " + transform.position.z);
		yield return new WaitForSecondsRealtime(DURATION); 
		target.transform.Translate (translationShow);
		Debug.Log ("end");
	}
}
