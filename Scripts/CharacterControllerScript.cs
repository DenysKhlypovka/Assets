using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterControllerScript : MonoBehaviour
{
	private const int DURATION = 1; 

	private float deathAnimDuration; 

	private static bool IsInputEnabled = true;
	private Vector3 translation = new Vector3(0, 0, 2f);
	private Vector3 translationUnhide = new Vector3(0, 0, -2f);

	private CapsuleCollider2D collider;
	private Rigidbody2D rb;
	private Animator anim;
	private GameObject hands;

//	private bool isGrounded = false;
	private bool isFacingRight = true;
	private bool isDead = false;
	private bool visible = true;

//	private float groundRadius = 0.2f;
	public float maxSpeed = 5f; 
	public float yVelocity = 1f;
	public int maxHealth = 5;
	public int maxBullets = 8;

	private int currentHealth;
	private int currentBullets;

//	public int vForce = 250;


	private void Start()
	{

		currentHealth = maxHealth;
		currentBullets = maxBullets;

		hands = this.gameObject.transform.GetChild(0).gameObject;
		collider = GetComponent<CapsuleCollider2D> ();
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		Physics2D.IgnoreCollision (collider, hands.GetComponent<CapsuleCollider2D>(), true);

		anim.SetBool("Visible", true);


		for (int i = 0; i < anim.runtimeAnimatorController.animationClips.Length; i++) {
			AnimationClip clip = anim.runtimeAnimatorController.animationClips [i];
			if (clip.name == "GC-death")
				deathAnimDuration = clip.length;
		}
		//Debug.Log (deathAnimDuration);

	}


	private void Update()
	{
		//Debug.Log ("Char: " + Mathf.Round(transform.position.y));
		if (!IsInputEnabled || isDead)
			return;

	//	isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround); 
	//	anim.SetBool ("Ground", isGrounded);
	//	anim.SetFloat ("vSpeed", rb.velocity.y);

		if (Input.GetKeyDown (KeyCode.K))
			Kill ();

		float move = Input.GetAxisRaw("Horizontal");

		if ((move > 0 && !isFacingRight) || (move < 0 && isFacingRight))
			anim.SetBool("Backwards", true);
		else  
			anim.SetBool("Backwards", false);

		anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
		rb.velocity = new Vector2(move * maxSpeed, -yVelocity);


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
	public void ChangeFloor(StairsController stairsScript){

		
		if (Input.GetKeyDown (KeyCode.E) && IsInputEnabled)
			stairsScript.ChangeFloor(this.gameObject);


	}

	public void TurnLampOnOff(LampController lamp){
		if (Input.GetKeyDown (KeyCode.E) && IsInputEnabled)
			lamp.TurnOnOff ();
	}


	public void OpenShelf(ShelfController shelf){
		if (Input.GetKeyDown (KeyCode.E) && IsInputEnabled)
			shelf.OpenShelf ();
	}

	public void DisableInput()
	{
		StartCoroutine(InputDisable());
	}


	public void Hide(bool cover, float hidePosX)
	{

		if (!(Input.GetKeyDown (KeyCode.E) && IsInputEnabled))
			return;

		float playerPosX = this.gameObject.transform.position.x;

		float posDelta = playerPosX - hidePosX;
		translation.x = -posDelta - .9f; //fit in the closet 

	//	string coverType = "Table";
	//	if (!cover)
	//		coverType = "Closet";

		SwitchIgnoreCollisions (true);
		visible = false;
		anim.SetBool("Visible", visible);	
		this.transform.Translate(translation);

		translation.x = 0; //Z TRANSLATE back to 0

		hands.SetActive (false);


		DisableInput ();

	//	anim.SetBool(coverType, true);		

	}

	public bool IsVisible()
	{
		return visible;
	}

	IEnumerator InputDisable()
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
			CapsuleCollider2D colliderEnemyBody = enemy.GetComponent<CapsuleCollider2D> ();
			Physics2D.IgnoreCollision (collider, colliderEnemyBody, ignore);

			CircleCollider2D colliderEnemyHead = enemy.GetComponent<CircleCollider2D> ();
			Physics2D.IgnoreCollision (collider, colliderEnemyHead, ignore);
		}
	}

	public bool ShootBullet(){
		currentBullets--;
		if (currentBullets <= 0) {
			return false;
		}
		return true;
	}

	public bool IsMagazineFull(){
	
		if (currentBullets == maxBullets)
			return true;
		return false;
	}

	public void SetBulletQty(int _maxBullets, int _currentBullets){
		this.maxBullets = _maxBullets;
		this.currentBullets = _currentBullets;
	}

	public void Reload(){
		currentBullets = maxBullets;
	}


	public void ApplyDamage(){

		currentHealth --;

		CheckIfDead();
	}

	private void CheckIfDead()
	{
		if (currentHealth <= 0)
			Kill ();
	}


	public void Kill()
	{
		if (isDead)
			return;
		isDead = true;
		anim.SetTrigger ("Killed");
		Destroy (hands);
		Destroy (collider);
		Destroy (rb);
		StartCoroutine (Dying());
	}

	IEnumerator Dying()
	{
		rb.velocity = new Vector2(0, -20);
		yield return new WaitForSecondsRealtime(deathAnimDuration);
		Destroy (gameObject);

	}
	public bool IsDead()
	{
		return isDead;
	}
}