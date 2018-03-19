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


	private CharacterControllerScript playerScript;
	private EnemyController controller;
	private Rigidbody2D rb;
	private Fsm fsm;
	private Animator anim;
	private GameObject bulletEmitter;

	public float bulletForceDiv;
	public float maxSpeed = 4f;
	public float patrolDistance = 10f;
	public float idleTime = 2f;
	public float maxPursueTime = 1f;
	public behaviourList behList;
	public GameObject bullet;

	private float leftPosPatrol;
	private float rightPosPatrol;
	private float targetX;
	private float targetY;
	private float stairsX;
	private float shootAnimDuration; 

	private bool isAllowedToShoot = true;
	private bool animationDelayBool = true;
	private bool isTriggerRefreshed = true;
	private bool isFacingRight;
	private bool isPlayerDetected;
	private bool isIdle = false;
	private bool isToChangeFloor = false;
	private bool isChangingFloor = false;
	private bool isPursuing = false;

	// Use this for initialization
	void Start () {

		anim = GetComponent<Animator>();
		playerScript = GameObject.FindWithTag ("Player").GetComponent<CharacterControllerScript>();
		controller = gameObject.GetComponent<EnemyController> ();
		rb = gameObject.GetComponent<Rigidbody2D> ();
		fsm = new Fsm ();
		if (behList.ToString () == "Patrol")
			fsm.SetState (Patrol);
		if (behList.ToString () == "Idle")
			fsm.SetState (Idle);
		
		for (int i = 0; i < 4; i++) {
			AnimationClip clip = anim.runtimeAnimatorController.animationClips [i];
			if (clip.name == "EnemyShoot")
				shootAnimDuration = clip.length;
		}

		bulletEmitter = this.gameObject.transform.GetChild(1).gameObject;
		
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
		Debug.Log ("Patrol");

		if (isPlayerDetected)
			fsm.SetState (Attack);

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
		Debug.Log ("MoveTo");

		if (isPlayerDetected)
			fsm.SetState (Attack);
		
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

	private void Attack()
	{
		Debug.Log ("Attack");
		if (playerScript.IsDead () || !isPlayerDetected) {
			fsm.SetState (Patrol);
		}

		if (!playerScript.IsDead () && !isPlayerDetected) {
			fsm.SetState (Pursue);
		}

		rb.velocity = new Vector2(0, 0);
		Shoot();
	}

	private void Pursue()
	{
		Debug.Log ("Pursue");
		if (!isPursuing)
			StartCoroutine (Pursuing());

		if (isPlayerDetected) {
			isPursuing = false;
			fsm.SetState (Attack);
		}

		Move ();
	}

	IEnumerator Pursuing()
	{
		
		isPursuing = true;
		yield return new WaitForSecondsRealtime(maxPursueTime);
		isPursuing = false;

		fsm.SetState (Patrol);
	}


	public void Shoot()
	{

		if (!isAllowedToShoot)
			return;

		if (isTriggerRefreshed)
		{
			anim.SetTrigger("Shoot");
			isTriggerRefreshed = false;
			StartCoroutine(ShootAnimationDelay());
		}

		if (!animationDelayBool)
			return;
		
		Vector2 direction;
		if (!isFacingRight) {
			direction = Vector2.left;
		} else {
			direction = Vector2.right;
		}
			

		//The Bullet instantiation happens here.
		GameObject Temporary_Bullet_Handler;
		Temporary_Bullet_Handler = Instantiate(bullet, bulletEmitter.transform.position, bulletEmitter.transform.rotation) as GameObject;

		//Sometimes bullets may appear rotated incorrectly due to the way its pivot was set from the original modeling package.
		//This is EASILY corrected here, you might have to rotate it from a different axis and or angle based on your particular mesh.
		Temporary_Bullet_Handler.transform.Rotate(Vector2.right);

		//Retrieve the Rigidbody component from the instantiated Bullet and control it.
		Rigidbody2D Temporary_RigidBody;
		Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody2D>();

		//Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
		Temporary_RigidBody.AddForce(direction / bulletForceDiv);

		//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
		Destroy(Temporary_Bullet_Handler, 10.0f);

		StartCoroutine(TimeBetweenShots());
	}

	IEnumerator ShootAnimationDelay()
	{
		animationDelayBool = false;
		yield return new WaitForSecondsRealtime(shootAnimDuration / 2);
		animationDelayBool = true;

	}

	IEnumerator TimeBetweenShots()
	{
		isAllowedToShoot = false;
		yield return new WaitForSecondsRealtime(shootAnimDuration + 0.5f);
		isAllowedToShoot = true;
		isTriggerRefreshed = true;

	}
}
