using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private Animator anim;
	private Rigidbody2D rb;

	public float maxTime = 3f;
	public float maxSpeed = 6f; 

	private bool isFacingRight = true;
	private float time = 0;

	// Use this for initialization
	void Start () {

		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

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

	private void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
