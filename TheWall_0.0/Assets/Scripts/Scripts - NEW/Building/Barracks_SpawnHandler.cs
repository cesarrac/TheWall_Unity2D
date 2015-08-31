using UnityEngine;
using System.Collections;

public class Barracks_SpawnHandler : MonoBehaviour {

	public ResourceGrid resourceGrid;

	
	Vector3 spawnPos; // the position to instantiate a unit

	
	public int posX, posY;
	
	public ObjectPool objPool;

	bool canSpawn = false;
	private int maxNumberofUnits = 4;
	public GameObject[] unitsSpawned;
	public int unitCount = 0, spwnCount = 0;
	public float spawnRate;

	// name of unit to spawn
	public GameObject unitFab;
	float unitHP;
	bool firstSpawn = false; // to get its stats for further spawns
	float offset = 0.8f;

	public Vector3[] spawnPositions;

	Transform parentTransform;

	GameObject attackTarget;

	public bool starvedMode; // MANIPULATED BY THE RESOURCE MANAGER

	Vector3[] unitPositions;

	void Start () {
		resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();
	
		objPool = GameObject.FindGameObjectWithTag ("Pool").GetComponent<ObjectPool> ();

		parentTransform = transform.parent;
		
		// spawning units one below me
//		spawnPos = new Vector3 (transform.position.x-1f, transform.position.y - 1f, 0.0f); 
		posX = (int)transform.position.x;
		posY = (int)transform.position.y;

		//init array
		unitsSpawned = new GameObject[maxNumberofUnits];

		unitPositions = new Vector3[maxNumberofUnits];



		// Start already Spawning
		for (int i = 0; i < maxNumberofUnits; i++) {
			Spawn (); 				// spawns four units
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (canSpawn && !starvedMode) {
			StartCoroutine(WaitToSpawn());
		}
	}
	IEnumerator WaitToSpawn(){
		canSpawn = false;
		yield return new WaitForSeconds (spawnRate);
		CheckSpawnedForNull();

	}
	
	void Spawn(){
		Debug.Log ("Spawning units!");
		GameObject unitSpawn = Instantiate (unitFab, spawnPos, Quaternion.identity)as GameObject;
		Vector3 storedPos = spawnPositions [spwnCount];
		offset = offset + 0.8f;
		if (unitSpawn != null) {
			unitSpawn.transform.position = transform.position;
			unitSpawn.transform.parent = parentTransform;

			Vector3 newPos =  storedPos - unitSpawn.transform.localPosition;
			unitSpawn.transform.localPosition = newPos;
			unitSpawn.GetComponent<SelectedUnit_MoveHandler> ().resourceGrid = resourceGrid;
			unitSpawn.GetComponentInChildren<Player_AttackHandler> ().objPool = objPool;
			unitSpawn.GetComponentInChildren<Player_AttackHandler> ().myBarracks = this;

			//STORE THE POSITION 
			unitPositions [spwnCount] = newPos;

//			if (!firstSpawn){
//				firstSpawn = true;
//				// get the HP of the first unit to store it
//				unitHP = unitSpawn.GetComponentInChildren<Player_AttackHandler> ().hp;
//			}
//			// init the HP of this unit since it is supposed to be a NEW unit
//			unitSpawn.GetComponentInChildren<Player_AttackHandler> ().hp = unitHP;

			unitsSpawned [spwnCount] = unitSpawn;

			spwnCount ++;
			if (spwnCount == maxNumberofUnits){
				unitCount = spwnCount;
				canSpawn = true;
			}

		} else {
			Debug.Log("Couldn't access object from pool!");
		}
	}

	void CheckSpawnedForNull(){
//		Debug.Log ("Checking if any Player units are dead");
		for (int x = 0; x < unitsSpawned.Length; x++) {
//			if (!unitsSpawned[x].activeSelf){
//				unitCount--;
//			}
			// THE PROBLEM HERE IS THAT ITS COUNTING THEM OVER AND OVER AGAIN AS NULL SO UNIT COUNT GOES DOWN WAY BEFORE
			// FOR EXAMPLE THERE'S TWO DUDES LEFT BUT IT DETECTS 3 NULL, ADDING TO THE LAST 1 IT COUNTED AS NULL, IT SPAWNS.
			// IT SHOULD ONLY SPAWN IF IT DETECTS THE 4 NULL ONCE!!
			if (unitsSpawned[x] == null){
				unitCount--;
			}
		}

		if (unitCount == 0) {

			spwnCount = 0;
			for (int i = 0; i < maxNumberofUnits; i++) {
				Spawn ();
			}
		} else {
			unitCount = spwnCount;
			canSpawn = true;
		}
	}

	void MoveUnits(Vector3 pos){
		foreach (GameObject unit in unitsSpawned) {
			if (unit != null && attackTarget != null){
				SelectedUnit_MoveHandler moveHandler = unit.GetComponent<SelectedUnit_MoveHandler>();
				moveHandler.attackTarget = attackTarget;
				moveHandler.moving = true;
				moveHandler.movingToAttack = true;
				//			Transform child = enemyUnit.transform.FindChild("sprite").transform;
				moveHandler.mX = Mathf.RoundToInt(pos.x);
				moveHandler.mY = Mathf.RoundToInt(pos.y);
			}
		}
	}
	void StopUnits(){
		foreach (GameObject unit in unitsSpawned) {
			if (unit != null){
				SelectedUnit_MoveHandler moveHandler = unit.GetComponent<SelectedUnit_MoveHandler>();
				moveHandler.attackTarget = null;
				moveHandler.moving = false;
				moveHandler.movingToAttack = false;
			}
		}
	}

	public void MoveUnitsBack(){
		for (int x = 0; x < unitsSpawned.Length; x++) {
			if (unitsSpawned[x] != null){
				SelectedUnit_MoveHandler moveHandler = unitsSpawned[x].GetComponent<SelectedUnit_MoveHandler>();

				moveHandler.moving = true;
				moveHandler.movingToAttack = false;
				moveHandler.mX = Mathf.RoundToInt(unitPositions[x].x);
				moveHandler.mY = Mathf.RoundToInt(unitPositions[x].y);
			}
		}
//		StopUnits ();
	}

//	void OnTriggerEnter2D(Collider2D coll){
//		if (coll.gameObject.CompareTag ("Enemy")) {
//			attackTarget = coll.gameObject;
//			MoveUnits (coll.transform.position);
//		} 
//	}
//	void OnTriggerExit2D(Collider2D coll){
//		if (coll.gameObject.CompareTag ("Enemy")) {
//			attackTarget = null;
//			MoveUnitsBack();
//			Debug.Log("Target exit. Setting attack target to null!");
//
//		}
//	}
}
