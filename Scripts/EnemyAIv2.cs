using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ActionFSM();
public class Fsm {

	ActionFSM action;

	public void SetState(ActionFSM _action)
	{
		action = _action;
	}

	public void Act()
	{
		action ();
	}
}

public enum behaviourList
{
	Patrol,
	Idle
};

public class EnemyAIv2 : MonoBehaviour {



	private EnemyController controller;
	private Rigidbody2D rb;
	private Fsm fsm;

	public float maxSpeed = 4f;
	public float patrolDistance = 10f;
	public float idleTime = 2f;
	public behaviourList behList;

	private float leftPosPatrol;
	private float rightPosPatrol;
	private float targetX;
	private float targetY;
	private float stairsX;

	private bool isFacingRight;
	private bool isPlayerDetected;
	private bool isIdle = false;
	private bool isToChangeFloor = false;
	private bool isChangingFloor = false;

	// Use this for initialization
	void Start () {
		controller = gameObject.GetComponent<EnemyController> ();
		rb = gameObject.GetComponent<Rigidbody2D> ();
		fsm = new Fsm ();
		if (behList.ToString () == "Patrol")
			fsm.SetState (Patrol);
		if (behList.ToString () == "Idle")
			fsm.SetState (Idle);
		
		leftPosPatrol = transform.position.x - patrolDistance / 2;
		rightPosPatrol = transform.position.x + patrolDistance / 2;
	}
	
	// Update is called once per frame
	void Update () {

	//	Debug.Log ("Eneme: " + Mathf.Round(transform.position.y));

		if (isChangingFloor)
			return;

		isFacingRight = controller.IsFacingRight ();

		fsm.Act ();
	}

	private void Patrol()
	{
		float x = transform.position.x;
		//Debug.Log (x);


		if (Mathf.Round (x) == Mathf.Round (rightPosPatrol) && isFacingRight) {
			fsm.SetState (Idle);
		//	Debug.Log ($"X: {Mathf.Round (x)}; RPP: {Mathf.Round (rightPosPatrol)}");
		}

		if (Mathf.Round (x) == Mathf.Round (leftPosPatrol) && !isFacingRight) {
			fsm.SetState (Idle);
		//	Debug.Log ($"X: {Mathf.Round (x)}; RPP: {Mathf.Round (leftPosPatrol)}");
		}

		if (x > leftPosPatrol && !isChangingFloor)
			Move ();

		if (x < leftPosPatrol && !isChangingFloor)
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

	private void MoveTo()
	{
		
		if ((Mathf.Round (transform.position.x) == Mathf.Round (targetX)) &&
			(Mathf.Round (transform.position.y) == Mathf.Round (targetY))) {
			fsm.SetState (Idle);

			return;
		}

		if (Mathf.Round (targetY) != Mathf.Round (transform.position.y)) {

			if (Mathf.Round (transform.position.x) == Mathf.Round (stairsX))
				isToChangeFloor = true;
			

		} else {
			if (targetX > transform.position.x && !controller.IsFacingRight()) {
				controller.Flip();

			} 
			if (targetX < transform.position.x && controller.IsFacingRight()) {
				controller.Flip();
			}
		}



		Move ();
	}

	public void Trigger(float _targetX, float _targetY)
	{
		targetX = _targetX;
		targetY = _targetY;

		StopAllCoroutines ();
		isIdle = false;

		if (Mathf.Round (targetY) != Mathf.Round (transform.position.y)) {

			GameObject[] stairs = GameObject.FindGameObjectsWithTag ("Stairs");

			foreach (GameObject stair in stairs) {
				if (Mathf.Round (stair.transform.position.y) == Mathf.Round (transform.position.y) &&
				    Mathf.Round (stair.GetComponent<StairsController> ().GetSecondStairsY ()) == Mathf.Round (targetY)) {
					stairsX = stair.transform.position.x;
				}
			}


			if (stairsX > transform.position.x && !controller.IsFacingRight()) {
				controller.Flip();

			} 
			if (stairsX < transform.position.x && controller.IsFacingRight()) {
				controller.Flip();
			}
		}
		fsm.SetState (MoveTo);
	}

	public void ChangeFloor(StairsController stairsScript)
	{
		if (isToChangeFloor) {
			isToChangeFloor = false;
			SetChangingFloor (true);
			stairsScript.ChangeFloor (gameObject);

		}
	}

	public void SetChangingFloor(bool _isChanging)
	{
		isChangingFloor = _isChanging;
	}

	private void Idle(){
		
		if (!isIdle) {
			rb.velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
			StartCoroutine (Idling ());
		}
	}

	IEnumerator Idling()
	{
		
		controller.SetImpassable(false);
		isIdle = true;
		yield return new WaitForSecondsRealtime(idleTime);
		isIdle = false;

		if (behList.ToString () == "Patrol") {
		
			controller.Flip ();
			isToChangeFloor = true;

			fsm.SetState (Patrol);

			controller.SetImpassable (false);
		}
	}

	public void PlayerDetected(bool _detected)
	{
		isPlayerDetected = _detected;
	}
}
