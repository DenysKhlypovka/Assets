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

	public ActionFSM GetState()
	{
		return action;
	}

	public void Act()
	{
		action ();
	}

}

public enum behaviourList
{
	Patrol,
	Idle,
	WatchTV,
	Toilet
};

public class EnemyAIv2 : MonoBehaviour {


	private CharacterControllerScript playerScript;
	private EnemyController controller;
	private Rigidbody2D rb;
	private Fsm fsm;
	private Animator anim;
	private GameObject bulletEmitter;
	private LevelControllerDemo levelController;

	public float triggerDistance = 5f;
	public float bulletForceDiv;
	public float maxSpeed = 4f;
	public float patrolDistance = 10f;
	public float idleTime = 2f;
	public float maxPursueTime = 1f;
	public float flipDelay = 1f;
	public float toilettingTime = 2f;
	public float smokingCooldown = 10f;
	public behaviourList behList;
	public GameObject bullet;

	private float leftPosPatrol;
	private float rightPosPatrol;
	private float targetX;
	private float targetY;
	private float stairsX;
	private float shootAnimDuration; 

	private string animBool = "unknown";

	private bool isAllowedToShoot = true;
	private bool animationDelayBool = true;
	private bool isTriggerRefreshed = true;
	private bool isFacingRight;
	private bool isPlayerDetected;
	private bool isIdle = false;
	private bool isToChangeFloor = false;
	private bool isChangingFloor = false;
	private bool isPursuing = false;
	private bool isFlipEnabled = true;
	private bool isInSmokingCooldown = false;

	private bool passiveState = false;

	// Use this for initialization
	void Start () {

		levelController = GameObject.FindWithTag ("LevelControllerDemo").GetComponent<LevelControllerDemo>();
		anim = GetComponent<Animator>();
		playerScript = GameObject.FindWithTag ("Player").GetComponent<CharacterControllerScript>();
		controller = gameObject.GetComponent<EnemyController> ();
		rb = gameObject.GetComponent<Rigidbody2D> ();

		fsm = new Fsm ();
		switch (behList.ToString ()) {
		case "Patrol":
			fsm.SetState (Patrol);
			break;
		case "Idle":
			fsm.SetState (Idle);
			break;
		case "WatchTV":
			fsm.SetState (Idle);
			animBool = "WatchingTV";
			passiveState = true;
			break;
		case "Toilet":
			fsm.SetState (Idle);
			animBool = "Toilet";
			passiveState = true;
			StartCoroutine (Toiletting());
			break;
		}
		
		for (int i = 0; i < anim.runtimeAnimatorController.animationClips.Length; i++) {
			AnimationClip clip = anim.runtimeAnimatorController.animationClips [i];
			if (clip.name == "enemy-shot") {
				shootAnimDuration = clip.length;
			}
			Debug.Log (clip.name);
		}

		bulletEmitter = this.gameObject.transform.GetChild(1).gameObject;
		
		leftPosPatrol = transform.position.x - patrolDistance / 2;
		rightPosPatrol = transform.position.x + patrolDistance / 2;
	}

	IEnumerator Toiletting()
	{
		
		yield return new WaitForSecondsRealtime(idleTime);
		resetState ();
	}

	IEnumerator Smoking()
	{
		isInSmokingCooldown = true;
		yield return new WaitForSecondsRealtime(smokingCooldown);
		isInSmokingCooldown = false;
		Smoke ();
	}

	private void Smoke()
	{
		anim.SetTrigger ("WatchingTVSmoking");
	}

	void resetState(){

		passiveState = false;
		anim.SetBool (animBool, false);
		isIdle = false;
		StopCoroutine (Idling());
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

		if (isPlayerDetected) {
			fsm.SetState (Attack);
			return;
		}
		if (controller.GetImpassable() || 
			(Mathf.Floor (x) == Mathf.Floor (rightPosPatrol) && isFacingRight) ||
			(Mathf.Floor (x) == Mathf.Floor (leftPosPatrol) && !isFacingRight)) {


		//	Debug.Log ($"Impassable: {controller.GetImpassable()}; X: {Mathf.Floor (x)}; RPP: {Mathf.Floor (rightPosPatrol)}; LPP: {Mathf.Floor (leftPosPatrol)}");

			fsm.SetState (Idle);
			return;

		}

		if (x > leftPosPatrol && !isChangingFloor)
			Move ();

		if (x < leftPosPatrol && !isChangingFloor)
			Move ();
	}

	private void Move()
	{
		if (passiveState)
			resetState ();
		
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

		if (isPlayerDetected) {
			fsm.SetState (Attack);
			return;
		}
		
		if ((Mathf.Floor (transform.position.x) == Mathf.Floor (targetX)) &&
			(Mathf.Floor (transform.position.y) == Mathf.Floor (targetY))) {

			fsm.SetState (Idle);

			return;
		}

		if (Mathf.Floor (targetY) != Mathf.Floor (transform.position.y)) {


			if (Mathf.Floor (transform.position.x) == Mathf.Floor (stairsX)) {
				isToChangeFloor = true;
			}
			

		} else {
		//	Debug.Log ($"target X: {targetX}, X: {transform.position.x}, IFR: {controller.IsFacingRight()}");
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
		if (isPlayerDetected || isPursuing) {
			return;
		}

		targetX = _targetX;
		targetY = _targetY;

		if (Mathf.Abs (targetX - transform.position.x) > triggerDistance ||
		    Mathf.Abs (targetY - transform.position.y) > triggerDistance) {

			return;
		}

		StopAllCoroutines ();
		isIdle = false;


		if (animBool != "unknown")
			anim.SetBool(animBool, false);

		if (Mathf.Floor (targetY) != Mathf.Floor (transform.position.y)) {

			GameObject[] stairs = GameObject.FindGameObjectsWithTag ("Stairs");

			foreach (GameObject stair in stairs) {
				if (Mathf.Floor (stair.transform.position.y) == Mathf.Floor (transform.position.y) &&
					Mathf.Floor (stair.GetComponent<StairsController> ().GetSecondStairsY ()) == Mathf.Floor (targetY)) {
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

		Debug.Log ("Idle");

		if (isPlayerDetected) {
			isIdle = false;
			fsm.SetState (Attack);

			if (passiveState)
				anim.SetBool(animBool, false);
			return;
		}
	

		if (passiveState) {
			if (animBool == "WatchingTV" && !isInSmokingCooldown)
				StartCoroutine (Smoking());
			anim.SetBool (animBool, true);
			return;
		}

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

		if (behList.ToString () == "Patrol" || animBool == "Patrol") {
		
			Debug.Log ("+=");

			controller.Flip ();
//			isToChangeFloor = true;
//			patrolling with changing the floor

			fsm.SetState (Patrol);

			controller.SetImpassable (false);
		}
	}

	public void PlayerIsBehind()
	{
		if (isFlipEnabled) {
			controller.Flip ();
			return;
		}
		StartCoroutine (FlipDelay());
	}

	IEnumerator FlipDelay()
	{
		isFlipEnabled = false;
		yield return new WaitForSecondsRealtime(flipDelay);
		isFlipEnabled = true;
	}

	public void PlayerDetected(bool _detected)
	{
		isPlayerDetected = _detected;
	}

	private void Attack()
	{

		if (passiveState)
			resetState ();

	//	Debug.Log ("Attack");
		if (!isPlayerDetected) {
			if (playerScript.IsDead ()) {
				fsm.SetState (Patrol);
			}
			else 
				fsm.SetState (Pursue);

			StopAllCoroutines ();

			animationDelayBool = false;
			isTriggerRefreshed = true;
			isAllowedToShoot = true;

			return;
		}

		rb.velocity = new Vector2(0, 0);
		Shoot();
	}

	private void Pursue()
	{
	//	Debug.Log ("Pursue");
		if (!isPursuing)
			StartCoroutine (Pursuing());

		if (Mathf.Floor (playerScript.transform.position.y) != Mathf.Floor (transform.position.y))
			Trigger (playerScript.transform.position.x, playerScript.transform.position.y);

		if (isPlayerDetected) {
			isPursuing = false;
			StopCoroutine (Pursuing());
			fsm.SetState (Attack);
			return;
		}

		Move ();
	}

	IEnumerator Pursuing()
	{
		
		isPursuing = true;
		yield return new WaitForSecondsRealtime(maxPursueTime);
		isPursuing = false;

		if (!isPlayerDetected)
			fsm.SetState (Patrol);
	}


	public void Shoot()
	{

		if (!isAllowedToShoot) {
			return;

		}

		if (isTriggerRefreshed)
		{
			anim.SetTrigger("Shoot");
			isTriggerRefreshed = false;
			StartCoroutine(ShotAnimationDelay());
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
		Temporary_Bullet_Handler.GetComponent<BulletController>().SetShotByEnemy(true);

		//Retrieve the Rigidbody component from the instantiated Bullet and control it.
		Rigidbody2D Temporary_RigidBody;
		Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody2D>();

		//Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
		Temporary_RigidBody.AddForce(direction / bulletForceDiv);


		levelController.TriggerEnemies (playerScript.transform.position.x, playerScript.transform.position.y);
		//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
		Destroy(Temporary_Bullet_Handler, 10.0f);

		StartCoroutine(TimeBetweenShots());
	}

	IEnumerator ShotAnimationDelay()
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
