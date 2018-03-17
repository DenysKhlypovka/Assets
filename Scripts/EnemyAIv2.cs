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

public class EnemyAIv2 : MonoBehaviour {

	private EnemyController controller;
	private Rigidbody2D rb;
	private Fsm fsm;

	public float maxSpeed = 4f;
	public float patrolDistance = 10f;
	public float idleTime = 2f;

	private float leftPosPatrol;
	private float rightPosPatrol;

	private bool isFacingRight;
	private bool isPlayerDetected;
	private bool isIdle = false;
	private bool isToChangeFloor = true;
	private bool isChangingFloor = false;

	// Use this for initialization
	void Start () {
		controller = gameObject.GetComponent<EnemyController> ();
		rb = gameObject.GetComponent<Rigidbody2D> ();
		fsm = new Fsm ();
		fsm.SetState (Patrol);

		leftPosPatrol = transform.position.x - patrolDistance / 2;
		rightPosPatrol = transform.position.x + patrolDistance / 2;
	}
	
	// Update is called once per frame
	void Update () {
		
		isFacingRight = controller.IsFacingRight ();

		fsm.Act ();
	}

	private void Patrol()
	{
		float x = transform.position.x;
		Debug.Log (x);


		if (Mathf.Round (x) == Mathf.Round (rightPosPatrol) && isFacingRight) {
			fsm.SetState (Idle);
			Debug.Log ($"X: {Mathf.Round (x)}; RPP: {Mathf.Round (rightPosPatrol)}");
		}

		if (Mathf.Round (x) == Mathf.Round (leftPosPatrol) && !isFacingRight) {
			fsm.SetState (Idle);
			Debug.Log ($"X: {Mathf.Round (x)}; RPP: {Mathf.Round (leftPosPatrol)}");
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

	public void ChangeFloor(StairsController stairsScript)
	{
		if (isToChangeFloor) {
			SetChangingFloor (true);
			stairsScript.ChangeFloor (gameObject);
			isToChangeFloor = false;
		}
	}

	public void SetChangingFloor(bool _isChanging)
	{
		isChangingFloor = _isChanging;
	}

	private void Idle(){
		if (!isIdle)
			StartCoroutine (Idling());
	}

	IEnumerator Idling()
	{
		controller.SetImpassable(false);
		isIdle = true;
		yield return new WaitForSecondsRealtime(idleTime);
		isIdle = false;

		controller.Flip();
		isToChangeFloor = true;
		fsm.SetState (Patrol);

		controller.SetImpassable(false);
	}

	public void PlayerDetected(bool _detected)
	{
		isPlayerDetected = _detected;
	}
}
