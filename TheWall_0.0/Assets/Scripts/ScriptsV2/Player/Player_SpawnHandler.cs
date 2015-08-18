using UnityEngine;
using System.Collections;

public class Player_SpawnHandler : MonoBehaviour {

	public ResourceGrid resourceGrid;

	public int capitalPosX, capitalPosY;

	Vector3 spawnPos; // the position to instantiate a unit

	// Prefabs for all types of Units that can be spawned
	public GameObject basicUnit;

	public int posX, posY;

	void Start () {
		// spawning units one below me
		spawnPos = new Vector3 (transform.position.x, transform.position.y - 1, 0.0f); 
		posX = (int)transform.position.x;
		posY = (int)transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Spawn(){
		GameObject unitSpawn = Instantiate (basicUnit, spawnPos, Quaternion.identity) as GameObject;
		unitSpawn.GetComponent<SelectedUnit_MoveHandler> ().resourceGrid = resourceGrid;
	}
}
