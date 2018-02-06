using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterControllerScript : MonoBehaviour
{
	public Rigidbody2D rb;
	public Transform groundCheck;
	public LayerMask whatIsGround;

	private Animator anim;

	private bool isGrounded = false;
	private bool isFacingRight = true;

	private float groundRadius = 0.2f;
	public float maxSpeed = 6f; 

	public int vForce = 250;


	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}


	private void Update()
	{
		//isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround); 
		//anim.SetBool ("Ground", isGrounded);
		anim.SetFloat ("vSpeed", rb.velocity.y);

		float move = Input.GetAxisRaw("Horizontal");

		if(move > 0 && !isFacingRight)
			Flip();
		else if (move < 0 && isFacingRight)
			Flip();

		//if (!isGrounded)
	//		return;
		anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

		rb.velocity = new Vector2(move * maxSpeed, rb.velocity.y);

	//	if (isGrounded && Input.GetKeyDown (KeyCode.Space)) 
	//	{
	//		anim.SetBool("Ground", false);
	//		rb.AddForce(new Vector2(0, vForce));				
	//	}
			
	}


	private void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}