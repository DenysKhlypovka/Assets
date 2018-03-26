using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfController : MonoBehaviour {

	private SpriteRenderer spriteRend;
	public Sprite shelfOpened;

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
			playerScript.OpenShelf (this);
		}

	}

	public void OpenShelf(){
		spriteRend.sprite = shelfOpened;

	}
}
