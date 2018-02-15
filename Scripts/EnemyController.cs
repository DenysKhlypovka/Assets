using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private float deathAnimDuration; 

	private Animator anim;
	private Rigidbody2D rb;
	private GameObject ray;
	private CapsuleCollider2D coll;

	public float maxTime = 3f;
	public float maxSpeed = 6f; 

	private bool isDead = false;
	private bool isFacingRight = true;
	private float time = 0;

	// Use this for initialization
	void Start () 
	{
		
		ray = this.gameObject.transform.GetChild(0).gameObject;
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<CapsuleCollider2D>();


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

	public void Kill()
	{
		if (isDead)
			return;
		isDead = true;
		Destroy (ray);
		Destroy (coll);
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
