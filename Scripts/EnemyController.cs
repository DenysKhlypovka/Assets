using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private float deathAnimDuration; 
	private float shootAnimDuration; 

	private Animator anim;
	private Rigidbody2D rb;
	private GameObject ray;
	private CapsuleCollider2D colliderBody;
	private CircleCollider2D colliderHead;

	public float idleTime = 2;
	public float patrolDistance = 3f;
	public float maxSpeed = 4f; 
	public int maxHealth = 2;
	public float bulletForceDiv;
	public float maxPursueTime = 4f;

	private float leftPosPatrol;
	private float rightPosPatrol;

	private float playerPursueTimer = 0;
	private int currentHealth;
	private bool isDead = false;
	private bool isFacingRight = true;
	private bool isIdle = false;
	private bool isPlayerDetected = false;
	private bool isAllowedToShoot = true;
	private bool animationDelayBool = true;
	private bool isTriggerRefreshed = true;


	private GameObject bulletEmitter;
	public GameObject bullet;


	// Use this for initialization
	void Start () 
	{

		leftPosPatrol = this.transform.position.x - patrolDistance;
		rightPosPatrol = this.transform.position.x + patrolDistance;

		currentHealth = maxHealth;
		ray = this.gameObject.transform.GetChild(0).gameObject;
		bulletEmitter = this.gameObject.transform.GetChild(1).gameObject;
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		colliderBody = GetComponent<CapsuleCollider2D>();
		colliderHead = GetComponent<CircleCollider2D>();


		for (int i = 0; i < 4; i++) {
			AnimationClip clip = anim.runtimeAnimatorController.animationClips [i];
			if (clip.name == "EnemyDie")
				deathAnimDuration = clip.length;
			if (clip.name == "EnemyShoot")
				shootAnimDuration = clip.length;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (isDead)
			return;

		playerPursueTimer -= Time.deltaTime;
		
		anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

		if (isIdle && !isPlayerDetected)
			return;
		
		if (!isPlayerDetected && playerPursueTimer <= 0)
			Patrol ();
		if (!isPlayerDetected && playerPursueTimer > 0)
			Pursue ();
		if (isPlayerDetected)
			Attack ();
		

	}

	private void Patrol()
	{
		float posX = transform.position.x;

		if (posX < rightPosPatrol && isFacingRight) {
			Move ();
		}

		if (posX > leftPosPatrol && !isFacingRight) {
			Move ();
		}

		if (Mathf.Round(posX) == Mathf.Round(leftPosPatrol) && !isFacingRight) {
			Idle ();
		}

		if (Mathf.Round(posX) == Mathf.Round(rightPosPatrol) && isFacingRight)
		{
			Idle ();
		}


	}

	private void Attack()
	{
		rb.velocity = new Vector2 (0, 0);
		Shoot ();
	}

	private void Pursue()
	{
		isAllowedToShoot = true;
		animationDelayBool = true;
		isTriggerRefreshed = true;
		Move ();
	}

	void Idle()
	{
		StartCoroutine (Idling());
	}

	IEnumerator Idling()
	{
		isIdle = true;
		yield return new WaitForSecondsRealtime(idleTime); 
		isIdle = false;
		Flip ();
	}

	private void Move()
	{


		if (isFacingRight) {
			rb.velocity = new Vector2 (maxSpeed, GetComponent<Rigidbody2D> ().velocity.y);
		} else {
			rb.velocity = new Vector2 (-maxSpeed, GetComponent<Rigidbody2D> ().velocity.y);
		}
	}

	public void PlayerDetected(bool isDetected)
	{
		if (isDetected)
			playerPursueTimer = maxPursueTime;
		isPlayerDetected = isDetected;
	}

	public void Shoot(){

		if (!isAllowedToShoot)
			return;

		if (isTriggerRefreshed) {
			anim.SetTrigger ("Shoot");
			isTriggerRefreshed = false;
			StartCoroutine (ShootAnimationDelay());
		}

		if (!animationDelayBool)
			return;
		
		//The Bullet instantiation happens here.
		GameObject Temporary_Bullet_Handler;
		Temporary_Bullet_Handler = Instantiate(bullet,bulletEmitter.transform.position,bulletEmitter.transform.rotation) as GameObject;

		//Sometimes bullets may appear rotated incorrectly due to the way its pivot was set from the original modeling package.
		//This is EASILY corrected here, you might have to rotate it from a different axis and or angle based on your particular mesh.
		Temporary_Bullet_Handler.transform.Rotate(Vector2.right);

		//Retrieve the Rigidbody component from the instantiated Bullet and control it.
		Rigidbody2D Temporary_RigidBody;
		Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody2D>();

		//Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
		Temporary_RigidBody.AddForce(Vector2.left / bulletForceDiv);

		//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
		Destroy(Temporary_Bullet_Handler, 10.0f);

		StartCoroutine (TimeBetweenShots());
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


	public void ApplyDamage(string area){

		if (area == "unknown")
			return;

		if (area == "head")
			currentHealth -= 2;
		if (area == "body")
			currentHealth --;

		CheckIfDead(area);
	}

	private void CheckIfDead(string latestHitArea)
	{
		Debug.Log (latestHitArea);
		if (currentHealth <= 0)
			Kill ();
	}

	public void Kill()
	{
		if (isDead)
			return;
		rb.velocity = new Vector2(0, -20);
		isDead = true;
		Destroy (ray);
		Destroy (colliderBody);
		Destroy (colliderHead);
		Destroy (rb);
		anim.SetTrigger ("Killed");
		StartCoroutine (Dying());
	}

	IEnumerator Dying()
	{
		yield return new WaitForSecondsRealtime(deathAnimDuration - 0.2f); 
		Destroy (gameObject);

	}

	public void Flip()
	{	
		if (isDead)
			return;
		isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
