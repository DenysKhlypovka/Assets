using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampController : MonoBehaviour {

	private bool IsTurnedOn = false;
	private SpriteRenderer spriteRend;
	public Sprite lampOn;
	public Sprite lampOff;

	// Use this for initialization
	void Start () {
		spriteRend = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay2D(Collider2D other)
	{
		
		if (other.GetComponent<CharacterControllerScript> () != null) {
			CharacterControllerScript playerScript = other.GetComponent<CharacterControllerScript> ();
			playerScript.TurnLampOnOff (this);
		}
			
	}

	public void TurnOnOff(){
		if (IsTurnedOn) {
			spriteRend.sprite = lampOff;
			IsTurnedOn = false;
		} else {
			spriteRend.sprite = lampOn;
			IsTurnedOn = true;
		}
	}
}
