using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

	private float shootAnimDuration; 

	private Animator anim;
	private Rigidbody2D rb;
	private CharacterControllerScript playerScript;
	private EnemyController controller;
	private GameObject bulletEmitter;
	public GameObject bullet;

	private float leftPosPatrol;
	private float rightPosPatrol;
	private float playerPursueTimer = 0;

	public float bulletForceDiv;
	public float maxSpeed = 4f; 
	public float maxPursueTime = 4f;
	public float patrolDistance = 5f;
	public float idleTime = 2;

	private bool isAllowedToShoot = true;
	private bool animationDelayBool = true;
	private bool isGoingToPlayer = false;
	private bool isIdle = false;
	private bool isPlayerDetected = false;
	private bool isTriggerRefreshed = true;

	// Use this for initialization
	void Start () {

		anim = GetComponent<Animator>();

		for (int i = 0; i < 4; i++) {
			AnimationClip clip = anim.runtimeAnimatorController.animationClips [i];
			if (clip.name == "EnemyShoot")
				shootAnimDuration = clip.length;
		}

		bulletEmitter = this.gameObject.transform.GetChild(1).gameObject;
		leftPosPatrol = this.transform.position.x - patrolDistance;
		rightPosPatrol = this.transform.position.x + patrolDistance;

		anim = GetComponent<Animator>();
		controller = GetComponent<EnemyController>();
		playerScript = GameObject.Find ("player").GetComponent<CharacterControllerScript>();
		rb = GetComponent<Rigidbody2D>();

	}
	
	// Update is called once per frame
	void Update () {

		if (controller.IsDead ())
			return;
		
		if (playerPursueTimer > -1)
        	playerPursueTimer -= Time.deltaTime;
		

        if (isGoingToPlayer)
        {
            GoToPlayer();
            return;
        }


        if (isIdle && !isPlayerDetected)
            return;

        if (!isPlayerDetected && playerPursueTimer <= 0)
            Patrol();
        if (!isPlayerDetected && playerPursueTimer > 0)
            Pursue();
        if (isPlayerDetected)
            Attack();
    }

	private void Reset()
	{
		StopAllCoroutines ();

		isAllowedToShoot = true;
		animationDelayBool = true;
		isIdle = false;
		isTriggerRefreshed = true;
	}

    private void GoToPlayer()
    {
        if (isPlayerDetected || !playerScript.IsVisible())
            isGoingToPlayer = false;

        float playerPosX = playerScript.gameObject.transform.position.x;
        float posX = this.gameObject.transform.position.x;

		if ((playerPosX > posX && !controller.IsFacingRight()) || (playerPosX < posX && controller.IsFacingRight()))
			controller.Flip();

        //Debug.Log (isGoingToPlayer);

        Move();
    }

    private void Patrol()
    {

        //Debug.Log (isInFrontOfWall);

        float posX = transform.position.x;

        //Debug.Log ($"pos X: {posX}; lpp: {leftPosPatrol}; rpp: {rightPosPatrol}; isfacing: {isFacingRight}; isinfron: {isInFrontOfWall}");

		if (posX < rightPosPatrol && controller.IsFacingRight())
        {
            Move();
        }

		if (posX < leftPosPatrol && posX < rightPosPatrol && !controller.IsFacingRight())
		{
            Idle();
        }

		if (posX > rightPosPatrol && posX > leftPosPatrol && controller.IsFacingRight())
		{
            Idle();
        }

		if (posX > leftPosPatrol && !controller.IsFacingRight())
        {
            Move();
        }

		if ((Mathf.Round(posX) == Mathf.Round(leftPosPatrol) && !controller.IsFacingRight()) || controller.GetImpassable())
		{
			Debug.Log (controller.GetImpassable());
            Idle();
        }

		if ((Mathf.Round(posX) == Mathf.Round(rightPosPatrol) && controller.IsFacingRight()) || controller.GetImpassable())
		{
            Idle();
        }

    }


    private void Attack()
    {
        rb.velocity = new Vector2(0, 0);
        Shoot();
    }

    private void Pursue()
    {
        //	Debug.Log (playerPursueTimer);
        isAllowedToShoot = true;
        animationDelayBool = true;
        isTriggerRefreshed = true;
        Move();

    }

    private void Idle()
	{
		Debug.Log($"Current: {this.transform.position.x}, lpp: {leftPosPatrol}, rpp: {rightPosPatrol}");
        StartCoroutine(Idling());
    }

    IEnumerator Idling()
	{
		controller.SetImpassable(false);
        isIdle = true;
        yield return new WaitForSecondsRealtime(idleTime);
        isIdle = false;

		controller.Flip();
		controller.SetImpassable(false);
    }

    private void Move()
    {


		if (controller.IsFacingRight())
        {
            rb.velocity = new Vector2(maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
        }
    }

    public void PlayerDetected(bool isDetected)
    {
		if (isDetected) 
		{

			playerPursueTimer = maxPursueTime;
		}
        isPlayerDetected = isDetected;
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
        Temporary_RigidBody.AddForce(Vector2.left / bulletForceDiv);

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

    public void DamagedByPlayer()
    {
        isGoingToPlayer = true;

		Reset ();
    }
}
