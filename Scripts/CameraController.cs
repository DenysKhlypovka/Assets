using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private GameObject player;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("player");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = this.gameObject.transform.position;

		offset = pos - player.transform.position;

		pos = player.transform.position + offset;
		Debug.Log (pos.x);
	}
}
