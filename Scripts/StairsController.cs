using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsController : MonoBehaviour {
	
	public const float DURATION = 1; 

	public GameObject SecondStairs;

	private float targetX;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	//	Debug.Log ("Sters: " + Mathf.Round(transform.position.y));
	}

	public float GetSecondStairsY()
	{
		return SecondStairs.transform.position.y;
	}


	void OnTriggerStay2D(Collider2D other)
	{
		CharacterControllerScript player = other.GetComponent<CharacterControllerScript> ();
		EnemyController enemyController = other.GetComponent<EnemyController> ();
		EnemyAIv2 enemyAI = other.GetComponent<EnemyAIv2> ();
		if (player != null)
			player.ChangeFloor (this);
		else if (enemyAI != null)
			enemyAI.ChangeFloor (this);

	}

	public void ChangeFloor(GameObject target)
	{
		Vector3 translationFitStairs = new Vector3 ();

		float targetPosX = target.gameObject.transform.position.x;
		float targetPosY = target.gameObject.transform.position.y;

		float posDelta = targetPosX - this.gameObject.transform.position.x;
		float posDeltaY = -targetPosY + SecondStairs.transform.position.y - 0.1f;
		//Debug.Log ($"posY: {targetPosY}; stairsY: {SecondStairs.transform.position.y}; delta: {posDeltaY}");
		translationFitStairs.x = -posDelta;  

		target.transform.Translate(translationFitStairs);
		targetX = target.transform.position.x;
		if (target.GetComponent<CharacterControllerScript>() != null)
			target.GetComponent<CharacterControllerScript> ().DisableInput ();

		StartCoroutine(ChangingFloor(target, posDeltaY, DURATION));
	}

	IEnumerator ChangingFloor(GameObject target, float stairsY, float DURATION)
	{
		Vector3 translationHide = new Vector3(0, 0, 30f);
		target.transform.Translate (translationHide);

		yield return new WaitForSecondsRealtime(DURATION); 
		Vector3 translationShow = new Vector3(targetX - target.transform.position.x, stairsY, -30f);
		target.transform.Translate (translationShow);
		if (target.GetComponent<EnemyAIv2> () != null)
			target.GetComponent<EnemyAIv2> ().SetChangingFloor (false);
	}
}
