using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_SpawnHandler : MonoBehaviour {
	public int posX;
	public int posY;

	public ResourceGrid resourceGrid;

	public GameObject leaderEnemyFab, followerEnemyFab;
	public string enemyName;

	public int targetPosX;
	public int targetPosY;

	// This stores this unit's path
	public List<Node>currentPath = new List<Node> ();

	public int howManyEnemies;
	int enemiesCount;

	public float timeBetweenSpawns = 1.5f; // default 1.5 seconds between each spawn
	bool canSpawn;

	GameObject[] spawnedEnemies;

	public ObjectPool objPool;

	void Awake(){
		// Init my start position
		posX = (int)transform.position.x;
		posY = (int)transform.position.y;
		// init my name
		enemyName = leaderEnemyFab.name;
	}

	void Start () {
		spawnedEnemies = new GameObject[howManyEnemies];

		if (resourceGrid != null) {
			targetPosX = resourceGrid.capitalSpawnX;
			targetPosY = resourceGrid.capitalSpawnY;
			resourceGrid.GenerateWalkPath(targetPosX, targetPosY, false, posX, posY);
		}
	
		// start with the first spawn as soon as this object is instantiated
		StartCoroutine (WaitToSpawn ());
	}

	void Update(){
		if (canSpawn) {
			StartCoroutine(WaitToSpawn());
		}
	}

	IEnumerator WaitToSpawn(){
//		CreateEnemyPath ();
		canSpawn = false;
		yield return new WaitForSeconds (timeBetweenSpawns);
		SpawnFromPool ();
	}

	public void SpawnFromPool(){
		GameObject newEnemy = objPool.GetObjectForType (enemyName, true); // only gets it if pooled
		if (newEnemy != null) {
			newEnemy.transform.position = transform.position;// move unit to my spawn point
			// Give it pathfinding ability. The unit will calculate its own path when spawned
			newEnemy.GetComponent<Enemy_MoveHandler> ().resourceGrid = resourceGrid;
			if (enemiesCount < spawnedEnemies.Length)
			// add new enemy to array of spawned enemies
				spawnedEnemies [enemiesCount] = newEnemy;
				// Check if we need to spawn more enemies
			enemiesCount++;
			if (enemiesCount < howManyEnemies) {
				canSpawn = true;
			} else {
				canSpawn = false;
			
			}
		}
	}

	public void Spawn(){
//		GameObject newEnemy = Instantiate (leaderEnemyFab, transform.position, Quaternion.identity) as GameObject;
//		// this is the first unit spawned, the rest are followers
//		newEnemy.GetComponent<Enemy_MoveHandler> ().posX = posX;
//		newEnemy.GetComponent<Enemy_MoveHandler> ().posY = posY;
//		newEnemy.GetComponent<Enemy_MoveHandler> ().resourceGrid = resourceGrid;
//		newEnemy.GetComponent<Enemy_MoveHandler> ().mySpawnHandler = this;
//		// The unit will calculate its own path when spawned
//		
//		newEnemy.GetComponent<Enemy_AttackHandler> ().mySpawnHandler = this;
//		newEnemy.GetComponent<Enemy_AttackHandler> ().moveHandler = newEnemy.GetComponent<Enemy_MoveHandler>();
//		newEnemy.GetComponent<Enemy_AttackHandler> ().resourceGrid = resourceGrid;
//		// add new enemy to array of spawned enemies
//		spawnedEnemies [enemiesCount] = newEnemy;
//
////		if (enemiesCount == 0) {
////			GameObject newEnemy = Instantiate (leaderEnemyFab, transform.position, Quaternion.identity) as GameObject;
////			// this is the first unit spawned, the rest are followers
////			newEnemy.GetComponent<Enemy_MoveHandler> ().posX = posX;
////			newEnemy.GetComponent<Enemy_MoveHandler> ().posY = posY;
////			newEnemy.GetComponent<Enemy_MoveHandler> ().resourceGrid = resourceGrid;
////			newEnemy.GetComponent<Enemy_MoveHandler> ().mySpawnHandler = this;
////			// Once the new Enemy is spawned it will need a new path
////			// give it the path this Spawner has already calculated
////			newEnemy.GetComponent<Enemy_MoveHandler> ().currentPath = currentPath;
////
////			newEnemy.GetComponent<Enemy_AttackHandler> ().mySpawnHandler = this;
////			newEnemy.GetComponent<Enemy_AttackHandler> ().moveHandler = newEnemy.GetComponent<Enemy_MoveHandler>();
////			newEnemy.GetComponent<Enemy_AttackHandler> ().resourceGrid = resourceGrid;
////			// add new enemy to array of spawned enemies
////			spawnedEnemies [enemiesCount] = newEnemy;
////		} else {
////			GameObject newEnemy = Instantiate (followerEnemyFab, transform.position, Quaternion.identity) as GameObject;
////			newEnemy.GetComponent<Enemy_Follower>().target = spawnedEnemies[0].gameObject.transform; // the target for the followers
////			newEnemy.GetComponent<Enemy_AttackHandler> ().mySpawnHandler = this;
////			newEnemy.GetComponent<Enemy_AttackHandler> ().resourceGrid = resourceGrid;
//			if (enemiesCount < spawnedEnemies.Length)
////			// add new enemy to array of spawned enemies
////				spawnedEnemies [enemiesCount] = newEnemy;
////		}
//
//		// Check if we need to spawn more enemies
//		enemiesCount++;
//		if (enemiesCount < howManyEnemies) {
//			canSpawn = true;
//		} else {
//			canSpawn = false;
//
//		}
	}
	
	public void CreateEnemyPath(){
		// Since the path to the Capital doesn't change from when this Spawn Point appears,
		// we can GET A PATH at the start and then just feed it to each spawned enemy.
		// Feed it this gameObject as the unitToPath
		resourceGrid.unitOnPath = gameObject;
		resourceGrid.GenerateWalkPath (targetPosX, targetPosY, false);
	}

}
