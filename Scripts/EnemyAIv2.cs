using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIv2 : MonoBehaviour {

	private EnemyController controller;
	private Rigidbody2D rb;

	public float maxSpeed = 4f;
	public float patrolDistance = 10f;

	private float leftPosPatrol;
	private float rightPosPatrol;

	private bool isGoingToLPP = true;
	private bool isFacingRight;
	private bool isPlayerDetected;

	// Use this for initialization
	void Start () {
		controller = gameObject.GetComponent<EnemyController> ();
		rb = gameObject.GetComponent<Rigidbody2D> ();

		leftPosPatrol = transform.position.x - patrolDistance / 2;
		rightPosPatrol = transform.position.x + patrolDistance / 2;
	}
	
	// Update is called once per frame
	void Update () {
		
		isFacingRight = controller.IsFacingRight ();

		Patrol();
	}

	private void Patrol()
	{
		float x = transform.position.x;

		Debug.Log ($"X: {Mathf.Round (x)}; RPP: {Mathf.Round (rightPosPatrol)}");
		Debug.Log ($"X: {Mathf.Round (x)}; LPP: {Mathf.Round (leftPosPatrol)}");

		if ((Mathf.Round (x) == Mathf.Round (rightPosPatrol)) && !isGoingToLPP) {
			isGoingToLPP = true;
			controller.Flip ();
		}

		if ((Mathf.Round (x) == Mathf.Round (leftPosPatrol)) && isGoingToLPP) {
			isGoingToLPP = false;
			controller.Flip ();
		}

		if (isGoingToLPP && x > leftPosPatrol)
			Move ();

		if (!isGoingToLPP && x < leftPosPatrol)
			Move ();
	}

	private void Move()
	{
		if (isFacingRight)
		{
			rb.velocity = new Vector2(maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
		}
		else
		{
			rb.velocity = new Vector2(-maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
		}
	}

	public void PlayerDetected(bool _detected)
	{
		isPlayerDetected = _detected;
	}
}
