using UnityEngine;
using System.Collections.Generic;

public class Enemy_MoveHandler : MonoBehaviour {
	public ResourceGrid resourceGrid;
	
	public int posX, targetPosX;
	public int posY, targetPosY;
	
	// This stores this unit's path
	public List<Node>currentPath = null;
	
	private Vector3 velocity = Vector3.zero;

	public bool isAttacking = false; // this turns true when this object enters a player unit's collider OR a blocking tile

	public Enemy_AttackHandler enemyAttkHandler;

	public float movementSpeed = 1f;

	bool moving;

	void Start () {
		posX = (int)transform.position.x;
		posY = (int)transform.position.y;
		enemyAttkHandler = GetComponent<Enemy_AttackHandler> ();

		if (resourceGrid != null) {
			currentPath = new List<Node>();
			for (int x = 0; x < resourceGrid.pathToCapital.Count; x++){
				currentPath.Add(resourceGrid.pathToCapital[x]);
			}
//			currentPath = resourceGrid.pathToCapital;
			moving = true;
//			CreateEnemyPath();
		}
	}

	public void CreateEnemyPath(){
		resourceGrid.unitOnPath = gameObject;
		resourceGrid.GenerateWalkPath (resourceGrid.capitalSpawnX, resourceGrid.capitalSpawnY, false);
		moving = true;
	}

	public void StartMoving(){
		moving = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (currentPath != null) {
			int currNode = 0;
			while (currNode < currentPath.Count -1) {
				Vector3 start = resourceGrid.TileCoordToWorldCoord (currentPath [currNode].x, currentPath [currNode].y);
				Vector3 end = resourceGrid.TileCoordToWorldCoord (currentPath [currNode + 1].x, currentPath [currNode + 1].y);
				;
				Debug.DrawLine (start, end, Color.blue);
				currNode++;
			}
		
		} 
		if (resourceGrid != null && moving) {
			// Have we moved close enough to the target tile that we can move to next tile in current path?
			if (Vector3.Distance (transform.position, resourceGrid.TileCoordToWorldCoord (posX, posY)) < 0.1f && !isAttacking) {
				MoveToNextTile ();
			}
			//			transform.position = Vector3.SmoothDamp (transform.position, resourceGrid.TileCoordToWorldCoord (posX, posY), ref velocity, movementSpeed * Time.deltaTime);

			if (!isAttacking){
				transform.position = Vector3.Lerp (transform.position, 
				                                   resourceGrid.TileCoordToWorldCoord (posX, posY), 
				                                   movementSpeed * Time.deltaTime);
//				transform.position = resourceGrid.TileCoordToWorldCoord (posX, posY);
			}
		}
	}
	public void MoveToNextTile(){
		if (currentPath == null) {
			return;
		}
		// Remove the old first node from the path
		currentPath.RemoveAt (0);
		
		// Check if the next tile is a UNWAKABLE tile, if it is clear path
		if (resourceGrid.UnitCanEnterTile (currentPath [1].x, currentPath [1].y) == false) {
			Debug.Log("Path is blocked! at x" + currentPath[1].x + ", y" + currentPath[1].y);
			if (CheckForTileAttack(currentPath [1].x, currentPath [1].y)){
				// here I would tell the Attack script to start its attack on the tile
				enemyAttkHandler.targetTilePosX = currentPath [1].x;
				enemyAttkHandler.targetTilePosY = currentPath [1].y;
				enemyAttkHandler.resourceGrid = resourceGrid;
				enemyAttkHandler.canAttack = true;
				isAttacking = true;
				currentPath = null;
				return;
			}else if (!isAttacking){
				// ask the spawner for another destination to see if we can get unstuck
				currentPath = null;
				return;
			}else{
				currentPath = null;
				return;
			}

		}	
		// Move to the next Node position in path
		posX = currentPath [1].x;
		posY = currentPath [1].y;

		// We are on the tile that is our DESTINATION, 
		// CLEAR PATH
		if (currentPath.Count == 1) {
			currentPath = null;
		}
	}

	bool CheckForTileAttack(int x, int y){
		if (resourceGrid.tiles [x, y].tileType != TileData.Types.empty && resourceGrid.tiles [x, y].tileType != TileData.Types.rock) {
			return true;
		} else {
			return false;
		}
	}
}
