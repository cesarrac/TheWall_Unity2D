using UnityEngine;
using System.Collections;

public class Player_SpawnHandler : MonoBehaviour {

	public ResourceGrid resourceGrid;

	public int capitalPosX, capitalPosY;

	Vector3 spawnPos; // the position to instantiate a unit

	// Prefabs for all types of Units that can be spawned
	public GameObject basicUnit;

	public int posX, posY;

	public ObjectPool objPool;

	void Start () {
		resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();
		capitalPosX = resourceGrid.capitalSpawnX;
		capitalPosY = resourceGrid.capitalSpawnY;
		objPool = GameObject.FindGameObjectWithTag ("Pool").GetComponent<ObjectPool> ();

		// spawning units one below me
		spawnPos = new Vector3 (transform.position.x, transform.position.y - 1f, 0.0f); 
		posX = (int)transform.position.x;
		posY = (int)transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Spawn(){
		GameObject unitSpawn = objPool.GetObjectForType ("Basic Unit", true);
		unitSpawn.transform.position = spawnPos;
		if (unitSpawn != null) {
			unitSpawn.GetComponent<SelectedUnit_MoveHandler> ().resourceGrid = resourceGrid;
			unitSpawn.GetComponentInChildren<Player_AttackHandler> ().objPool = objPool;
		}
	}
}
