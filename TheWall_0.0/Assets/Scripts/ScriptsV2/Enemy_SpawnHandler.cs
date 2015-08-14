using UnityEngine;
using System.Collections.Generic;

public class Enemy_SpawnHandler : MonoBehaviour {
	public int posX;
	public int posY;

	public ResourceGrid resourceGrid;

	public GameObject enemyFab;

	public int targetPosX;
	public int targetPosY;

	// This stores this unit's path
	public List<Node>currentPath = null;

	void Start () {


	}
	public void CreateEnemyPath(){
		// Since the path to the Capital doesn't change from when this Spawn Point appears,
		// we can GET A PATH at the start and then just feed it to each spawned enemy.
		// Feed it this gameObject as the unitToPath
		resourceGrid.unitOnPath = gameObject;
		resourceGrid.GenerateWalkPath (targetPosX, targetPosY, false);
		currentPath = currentPath;
	}
	
	public void Spawn(){
		CreateEnemyPath ();
		GameObject newEnemy = Instantiate (enemyFab, transform.position, Quaternion.identity) as GameObject;
		// Initialize the Enemy unit's variables for pathfinding
		newEnemy.GetComponent<Enemy_MoveHandler> ().posX = posX;
		newEnemy.GetComponent<Enemy_MoveHandler> ().posY = posY;
		newEnemy.GetComponent<Enemy_MoveHandler>().resourceGrid = resourceGrid;
		// Once the new Enemy is spawned it will need a new path
		// give it the path this Spawner has already calculated
		if (currentPath != null){
			newEnemy.GetComponent<Enemy_MoveHandler>().currentPath = currentPath;
		}

//		// First we tell the resourceGrid that this new enemy is the unit to path
//		resourceGrid.unitOnPath = newEnemy;
//		// Then we tell it to create the path
//		resourceGrid.GenerateWalkPath (targetPosX, targetPosY, false);
	}
}
