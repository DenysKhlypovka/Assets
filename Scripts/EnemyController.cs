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
	private CharacterControllerScript playerScript;
	private EnemyAI aiScript;


	public int maxHealth = 3;
	public int damageToHead = 3;
	public int damageToBody = 1;


	private int currentHealth;
	private bool isDead = false;
	private bool isFacingRight = true;
	private bool isImpassable = false;


	// Use this for initialization
	void Start () 
	{
		playerScript = GameObject.Find ("player").GetComponent<CharacterControllerScript>();

		currentHealth = maxHealth;
		ray = this.gameObject.transform.GetChild(0).gameObject;
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		colliderBody = GetComponent<CapsuleCollider2D>();
		colliderHead = GetComponent<CircleCollider2D>();
		aiScript = GetComponent<EnemyAI>();

		for (int i = 0; i < 4; i++) {
			AnimationClip clip = anim.runtimeAnimatorController.animationClips [i];
			if (clip.name == "EnemyDie")
				deathAnimDuration = clip.length;
		}
	}
	
	// Update is called once per frame
	void Update () {

	//	Debug.Log ($"isdetected: {isPlayerDetected}; isidle: {isIdle}; ppt: {playerPursueTimer}");
	//	Debug.Log (transform.position.y);
		if (isDead)
			return;
		
		anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

	}

	public bool IsFacingRight()
	{
		return isFacingRight;
	}

	public bool IsDead()
	{
		return isDead;
	}

	public void SetImpassable(bool _isImpassable)
	{
		isImpassable = _isImpassable;
	}

	public bool GetImpassable()
	{
		return isImpassable;
	}

	public void ApplyDamage(string area){

		if (area == "unknown")
			return;

		if (area == "head")
			currentHealth -= damageToHead;
		if (area == "body")
			currentHealth -= damageToBody;

		CheckIfDead(area);
		aiScript.DamagedByPlayer ();
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
	//	Destroy (gameObject);

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
