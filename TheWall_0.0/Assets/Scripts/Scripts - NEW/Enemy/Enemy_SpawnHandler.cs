using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_SpawnHandler : MonoBehaviour {
	public int posX;
	public int posY;

	public ResourceGrid resourceGrid;

	public GameObject enemyFab;
	public string enemyName;

	public int targetPosX;
	public int targetPosY;

	// This stores this unit's path
	public List<Node>currentPath = new List<Node> ();

	public int howManyEnemies;
	int enemiesCount;

	public float timeBetweenSpawns = 1.5f; // default 1.5 seconds between each spawn
	bool canSpawn;

	public GameObject[] spawnedEnemies;

	public ObjectPool objPool;

	public int indexForPath;

	public SpawnPoint_Handler spwnPointHandler;

	public Vector3[] posVariations;
	Vector3 neighborEnemyPosition = Vector3.zero;

	float newOffsetX = 0, newOffsetY = 0;

	public float unitHP; // to reset when spawning from pool

	private IEnumerator _coRoutine;

	void Awake(){
		// init my name
		enemyName = enemyFab.name;
	}

	void Start () {
		spawnedEnemies = new GameObject[howManyEnemies];
		// Init my start position
		posX = (int)transform.position.x;
		posY = (int)transform.position.y;

		// start with the first spawn as soon as this object is instantiated
		_coRoutine = WaitToSpawn ();
		StartCoroutine (_coRoutine);
	}

	void Update(){
		if (canSpawn) {
			StartCoroutine(_coRoutine);
		}
	}

	IEnumerator WaitToSpawn(){
//		CreateEnemyPath ();
		canSpawn = false;
		yield return new WaitForSeconds (timeBetweenSpawns);
		int randomVariation = Random.Range (0, posVariations.Length - 1);
		SpawnFromPool (posVariations[randomVariation]);
	}

	public void SpawnFromPool(Vector3 randomVar){
		GameObject newEnemy = objPool.GetObjectForType (enemyName, true); // only gets it if pooled
		if (newEnemy != null) {
			// reset the HP of this unit in case its being reused from pool
			newEnemy.GetComponentInChildren<Enemy_AttackHandler>().stats.curHP = unitHP;

			// give it an offset variation
			Vector3 offsetPos = new Vector3(transform.position.x - randomVar.x, transform.position.y - randomVar.y, 0);
			newEnemy.transform.position = offsetPos;
			neighborEnemyPosition = newEnemy.transform.position;
		

			// Give it pathfinding ability. Using the spwnPtIndex it can find which path it needs to take out of the 3
			newEnemy.GetComponent<Enemy_MoveHandler> ().resourceGrid = resourceGrid;
			newEnemy.GetComponent<Enemy_MoveHandler> ().spwnPtIndex = indexForPath;
			newEnemy.GetComponent<Enemy_MoveHandler> ().spwnPtHandler = spwnPointHandler; 
			newEnemy.GetComponentInChildren<Enemy_AttackHandler>().objPool = objPool;

			if (enemiesCount < spawnedEnemies.Length){
			// add new enemy to array of spawned enemies
				spawnedEnemies [enemiesCount] = newEnemy;
				if (enemiesCount > 0){
					// I need to feed each enemy spawn AFTER THE FIRST the variable it needs for pathfindind (start, stopping, blocked attack)
					// They specifically need is the MoveHandler of the enemy in line in front of them
					newEnemy.GetComponent<Enemy_MoveHandler>().myBuddy = spawnedEnemies[enemiesCount - 1].GetComponent<Enemy_MoveHandler>();
				}

			}
				// Check if we need to spawn more enemies
			enemiesCount++;
			if (enemiesCount < howManyEnemies) {
				canSpawn = true;
			} else {
				// done spawning enemies from this handler
				canSpawn = false;

			}
		}
	}

	
//	public void CreateEnemyPath(){
//		// Since the path to the Capital doesn't change from when this Spawn Point appears,
//		// we can GET A PATH at the start and then just feed it to each spawned enemy.
//		// Feed it this gameObject as the unitToPath
//		resourceGrid.unitOnPath = gameObject;
//		resourceGrid.GenerateWalkPath (targetPosX, targetPosY, false);
//	}

}
