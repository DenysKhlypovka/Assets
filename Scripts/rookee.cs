using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rookee : MonoBehaviour {


	private const int RELOAD_DURATION = 2; 

	private Camera cam;
	private Transform my;
	private Rigidbody2D body;
	private CharacterControllerScript playerScript;

	private GameObject bulletEmitter;
	public GameObject bullet;

	//private int delta = 90;
	private int viewAngle = 90;
	private int multip = 1;
	private bool isShootingEnabled = true;

	public float bulletForceDiv;

	// Use this for initialization
	void Start () {

		cam = Camera.main;
		my = GetComponent <Transform> ();
		body = GetComponent <Rigidbody2D> ();

		playerScript = GameObject.Find ("player").GetComponent<CharacterControllerScript>();

		bulletEmitter = this.gameObject.transform.GetChild(0).gameObject;
	}
	 
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.E))
			Debug.Log ("zdarowa from the closet");


		// Distance from camera to object.  We need this to get the proper calculation.
		float camDis = cam.transform.position.y - my.position.y;

		// Get the mouse position in world space. Using camDis for the Z axis.
		Vector3 mouse = cam.ScreenToWorldPoint  (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, camDis));

		float AngleRad = Mathf.Atan2 (mouse.y - my.position.y, mouse.x - my.position.x);
		float angle = (180 / Mathf.PI) * AngleRad;


		if (playerScript.IsFacingRight ()) {
			if (angle > viewAngle)
				angle = viewAngle;
			if (angle < -viewAngle)
				angle = -viewAngle;
			multip = 1;
		} else {
			if (angle > -viewAngle && angle < 0) {
				angle = viewAngle;
			} else if (angle < viewAngle && angle > 0) {
				angle = -viewAngle;
			} else {
				if (angle > 180)
					angle -= 180;
				else
					angle += 180;
			}
			multip = -1;
		}

		if (Input.GetKeyDown (KeyCode.R) && !playerScript.IsMagazineFull()) {
			StartCoroutine (Reloading ());
			return;
		}

	//	Debug.Log (isShootingEnabled);
		if (Input.GetMouseButtonDown (0) && isShootingEnabled) {

			if (!playerScript.ShootBullet ()) {
				StartCoroutine(Reloading());
				return;
			}

			float newAngleRad = (angle * Mathf.PI) / 180;

			Vector2 bulletDirection = new Vector2 (Mathf.Cos(newAngleRad) / bulletForceDiv, Mathf.Sin(newAngleRad) / bulletForceDiv);

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
			Temporary_RigidBody.AddForce(bulletDirection * multip);

			//Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
			Destroy(Temporary_Bullet_Handler, 10.0f);
		}

	//	body.MoveRotation (angle);
		body.rotation  = angle;

	//	Debug.Log ("Velocity: " + Input.GetAxisRaw("Horizontal"));
		//Debug.Log ($"Angle: {angle} Body Rotation: {body.rotation}");
	}

	IEnumerator Reloading()
	{
		isShootingEnabled = false;
		playerScript.Reload ();
		yield return new WaitForSecondsRealtime(RELOAD_DURATION); 
		isShootingEnabled = true;
	}

}
