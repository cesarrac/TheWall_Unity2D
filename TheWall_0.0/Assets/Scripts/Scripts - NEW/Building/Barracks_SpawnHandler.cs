using UnityEngine;
using System.Collections;

public class Barracks_SpawnHandler : MonoBehaviour {

	public ResourceGrid resourceGrid;
	
	public int capitalPosX, capitalPosY;
	
	Vector3 spawnPos; // the position to instantiate a unit

	
	public int posX, posY;
	
	public ObjectPool objPool;

	bool canSpawn;
	private int maxNumberofUnits = 4;
	public GameObject[] unitsSpawned;
	private int unitCount = 0;
	public float spawnRate;

	// name of unit to spawn
	public string nameOfUnitFab;

	void Start () {
		resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();
		capitalPosX = resourceGrid.capitalSpawnX;
		capitalPosY = resourceGrid.capitalSpawnY;
		objPool = GameObject.FindGameObjectWithTag ("Pool").GetComponent<ObjectPool> ();
		
		// spawning units one below me
		spawnPos = new Vector3 (transform.position.x, transform.position.y - 1f, 0.0f); 
		posX = (int)transform.position.x;
		posY = (int)transform.position.y;

		//init array
		unitsSpawned = new GameObject[maxNumberofUnits];

		// Start already Spawning
		Spawn ();
	}
	
	// Update is called once per frame
	void Update () {
		if (canSpawn) {
			StartCoroutine(WaitToSpawn());
		}
	}
	IEnumerator WaitToSpawn(){
		canSpawn = false;
		yield return new WaitForSeconds (spawnRate);
		if (unitCount < maxNumberofUnits) {
			Spawn ();
		} else {
			CheckSpawnedForNull();
		}

	}
	
	void Spawn(){
		GameObject unitSpawn = objPool.GetObjectForType (nameOfUnitFab, true);
		Vector3 offsetPos = new Vector3 (spawnPos.x + (float)unitCount, spawnPos.y, spawnPos.z);
		if (unitSpawn != null) {
			unitSpawn.transform.position = offsetPos;
			unitSpawn.GetComponent<SelectedUnit_MoveHandler> ().resourceGrid = resourceGrid;
			unitSpawn.GetComponentInChildren<Player_AttackHandler> ().objPool = objPool;

			unitsSpawned[unitCount] = unitSpawn;
			unitCount++;

			canSpawn = true;
		}
	}

	void CheckSpawnedForNull(){
		foreach (GameObject unit in unitsSpawned) {
			if (unit == null)
				unitCount--;
		}
	}
}
