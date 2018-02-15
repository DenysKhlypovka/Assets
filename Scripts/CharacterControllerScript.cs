using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterControllerScript : MonoBehaviour
{
	private const int DURATION = 1; 
	private static bool IsInputEnabled = true;
	private Vector3 translation = new Vector3(0, 0, 2f);
	private Vector3 translationUnhide = new Vector3(0, 0, -2f);

	public Transform groundCheck;
	public LayerMask whatIsGround;

	private CapsuleCollider2D collider;
	private Rigidbody2D rb;
	private Animator anim;
	private GameObject hands;

//	private bool isGrounded = false;
	private bool isFacingRight = true;
	private bool visible = true;

	private float groundRadius = 0.2f;
	public float maxSpeed = 6f; 

//	public int vForce = 250;


	private void Start()
	{
		hands = this.gameObject.transform.GetChild(1).gameObject;
		collider = GetComponent<CapsuleCollider2D> ();
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		anim.SetBool("Visible", true);

	}


	private void Update()
	{
		if (!IsInputEnabled)
			return;
	//	isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround); 
	//	anim.SetBool ("Ground", isGrounded);
	//	anim.SetFloat ("vSpeed", rb.velocity.y);

		float move = Input.GetAxisRaw("Horizontal");

		if(move > 0 && !isFacingRight)		//FLIP ANIM
			Flip();
		else if (move < 0 && isFacingRight)
			Flip();

		anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
		rb.velocity = new Vector2(move * maxSpeed, rb.velocity.y);


	//	if (visible && isGrounded && Input.GetKeyDown (KeyCode.Space)) //JUMP
	//		{
	//		anim.SetBool("Ground", false);
	//		rb.AddForce(new Vector2(0, vForce));				
	//	}

		if (!visible && Input.GetAxisRaw("Horizontal") != 0) 
			Unhide ();
	}

	public bool IsFacingRight()
	{
		return isFacingRight;
	}

	public void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void Unhide()
	{

		SwitchIgnoreCollisions (false);
		visible = true;
		anim.SetBool("Visible", visible);		
		hands.SetActive (true);		
		this.transform.Translate(translationUnhide);
		//	anim.SetBool("Table", true);		
		//	anim.SetBool("Closet", true);	
	}

	public void Hide(bool cover)
	{
		if (!(Input.GetKeyDown (KeyCode.E) && IsInputEnabled))
			return;

		string coverType = "Table";
		if (!cover)
			coverType = "Closet";

		SwitchIgnoreCollisions (true);
		visible = false;
		anim.SetBool("Visible", visible);	
		this.transform.Translate(translation);

		hands.SetActive (false);


		StartCoroutine(HideDelay());

	//	anim.SetBool(coverType, true);		

	}

	public bool IsVisible()
	{
		return visible;
	}

	IEnumerator HideDelay()
	{
		IsInputEnabled = false;
		rb.velocity = new Vector2(0, -20);
		yield return new WaitForSecondsRealtime(DURATION); 
		IsInputEnabled = true;
	}

	void SwitchIgnoreCollisions(bool ignore)
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
		foreach (GameObject enemy in enemies) {
			CapsuleCollider2D colliderEnemy = enemy.GetComponent<CapsuleCollider2D> ();
			Physics2D.IgnoreCollision (collider, colliderEnemy, ignore);
		}
	}
}