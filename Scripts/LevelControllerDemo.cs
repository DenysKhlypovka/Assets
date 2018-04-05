using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControllerDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R))
			Application.LoadLevel(Application.loadedLevel);
	}

	public void TriggerEnemies(float originX, float originY){
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("enemy");

		foreach (GameObject enemy in enemies)
			if (enemy.GetComponent<EnemyAIv2>() != null)
				enemy.GetComponent<EnemyAIv2>().Trigger (originX, originY);
	}
}
