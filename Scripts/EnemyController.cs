using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private float deathAnimDuration; 

	private Animator anim;
	private Rigidbody2D rb;
	private GameObject ray;
	private CapsuleCollider2D colliderBody;
	private CircleCollider2D colliderHead;

	public float maxTime = 3f;
	public float maxSpeed = 6f; 
	public int maxHealth = 2;
	public float bulletForceDiv;

	private int currentHealth;
	private bool isDead = false;
	private bool isFacingRight = true;
	private float time = 0;


	private GameObject bulletEmitter;
	public GameObject bullet;


	// Use this for initialization
	void Start () 
	{
		currentHealth = maxHealth;
		ray = this.gameObject.transform.GetChild(0).gameObject;
		bulletEmitter = this.gameObject.transform.GetChild(0).gameObject;
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		colliderBody = GetComponent<CapsuleCollider2D>();
		colliderHead = GetComponent<CircleCollider2D>();


		for (int i = 0; i < 4; i++) {
			AnimationClip clip = anim.runtimeAnimatorController.animationClips [i];
			if (clip.name == "EnemyDie")
				deathAnimDuration = clip.length;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (isDead)
			return;



		time += Time.deltaTime;

		if (time > maxTime) {
			Flip ();
			time = 0;
		}
		if (isFacingRight) {
			rb.velocity = new Vector2 (maxSpeed, GetComponent<Rigidbody2D> ().velocity.y);
		} else {
			rb.velocity = new Vector2 (-maxSpeed, GetComponent<Rigidbody2D> ().velocity.y);

		}

		anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));


		
	}

	public void Shoot(){

		anim.SetTrigger ("Shoot");

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
		Temporary_RigidBody.AddForce(transform.right / bulletForceDiv);

		//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
		Destroy(Temporary_Bullet_Handler, 10.0f);
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
		if (currentHealth <= 0)
			Kill ();
	}

	public void Kill()
	{
		if (isDead)
			return;
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
		rb.velocity = new Vector2(0, -20);
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
