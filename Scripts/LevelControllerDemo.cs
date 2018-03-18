using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControllerDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void triggerEnemies(float originX, float originY){
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("enemy");
		foreach (GameObject enemy in enemies)
			enemy.GetComponent<EnemyAIv2>().Trigger (originX, originY);
	}
}
