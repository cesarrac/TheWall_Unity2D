using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//using System;
//Random = UnityEngine.Random;

public class ResourceGrid : MonoBehaviour{
	/// <summary>
	/// Handles creating the grid of resources according to the positions created by Map.
	/// Map call a method here that takes (colums, rows, level, playerspawnPos).
	/// According to level it creates Tiles of different types.
	/// </summary>

	// List of Resource Tiles
//	public List<Tile> resourceTiles = new List<Tile>();

	public TileType[] tileTypes;
	TileType.Types tTypes;
	int[,] tiles;
	public GameObject[,] spawnedTiles;

	// rows and colums determined by level
	public int mapSizeX;
	public int mapSizeY;
	

	public int level;

	public Transform tileHolder; // transform to hold Instantiated tiles in Hierarchy

	public GameObject discoverTileFab; // fab for grey discover tile with discover tile script

	public GameObject unitOnPath;

	public GameObject playerUnit;

	public int playerSpawnX, playerSpawnY;

	// PATHFINDING VARS:
	Node[,] graph;

//	List<Node>currentPath;

	void Awake(){
	
		if (level == 0) {
			mapSizeX = 10;
			mapSizeY = 10;
		}
	
	}
	void Start () {
		// For now I'll initialize the PLAYER VARS here.
		// Later I can initialize these vars from whatever spawn's this player unit
		if (playerUnit != null) {
			playerUnit.GetComponent<SelectedUnit_MoveHandler>().posX = (int)playerUnit.transform.position.x;
			playerUnit.GetComponent<SelectedUnit_MoveHandler>().posY = (int)playerUnit.transform.position.y;
			playerUnit.GetComponent<SelectedUnit_MoveHandler>().resourceGrid = this;
		}

//		InitVisualGrid (mapSizeX, mapSizeY, level, playerSpawnPos);
		tiles = new int[mapSizeX, mapSizeY];
		spawnedTiles = new GameObject[mapSizeX, mapSizeY];
		InitGrid ();
		InitVisualGrid ();
		InitPathFindingGraph ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitGrid(){
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				if (x > 0 && y > 0){
					tiles[x,y]= SimpleTileSelection(tiles[x-1,y-1]); // default to empty tile

				}else{
					if (x != playerSpawnX && y != playerSpawnY)
						tiles[x,y] = 5;
				}
			}
		}
//		//First build tiles
//		tiles[6,5]=4;
//		tiles[6,4]=4;
//		tiles[6,3]=4;
//		tiles[7,3]=4;
//		tiles[8,3]=4;
//		tiles[8,4]=4;
//		tiles[8,5]=4;
//
//		//first visible tiles
//		tiles[5,1]=3;
//		tiles[5,4]=2;
//		tiles[5,3]=0;
//		tiles[7,2]=0;
//		tiles[8,2]=0;
//		tiles[8,4]=2;
//		tiles[8,5]=2;
//
//		tiles[3,3]=3;
//		tiles[3,2]=3;
//		tiles[3,4]=3;
//		tiles[4,4]=3;
//		tiles[4,5]=3;
//		tiles[4,6]=3;
	}

	int SimpleTileSelection(int lastValue){
		int randomT = Random.Range (0, 5);
		int randomRepeat = Random.Range (0, 2);
		if (randomT == lastValue) {
			if (randomRepeat >= 1) {
				return lastValue;
			} else {
				return SimpleTileSelection (lastValue);
			}
		} else {
			return randomT;
		}
	}

	void InitVisualGrid(){
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
//				if (tiles[x,y]==0){
//					SpawnDiscoverTile(tileTypes[0].tileAsGObj, new Vector3(x, y, 0.0f), tileTypes[0].tileType); 
//				}else if (tiles[x,y]==1){
//					SpawnDiscoverTile(tileTypes[1].tileAsGObj, new Vector3(x, y, 0.0f), tileTypes[1].tileType); 
//				}else if (tiles[x,y]==2){
//					SpawnDiscoverTile(tileTypes[2].tileAsGObj, new Vector3(x, y, 0.0f), tileTypes[2].tileType); 
//				}else if (tiles[x,y]==3){
//					SpawnDiscoverTile(tileTypes[3].tileAsGObj, new Vector3(x, y, 0.0f),tileTypes[3].tileType); 
//				}else if (tiles[x,y]==4){
//					SpawnDiscoverTile(tileTypes[4].tileAsGObj, new Vector3(x, y, 0.0f),tileTypes[4].tileType); 
//				}
				if (tiles[x,y]==4){
					SpawnDiscoverTile(tileTypes[4].tileAsGObj, new Vector3(x, y, 0.0f),tileTypes[4].tileType); 
				}
				
			}
		}
	}


	public void DiscoverTile(int x, int y){
		if (spawnedTiles [x, y] == null) { // if it's null it means it hasn't been spawned
			//Dont Spawn a tile if the type is Empty
			// the space will still be walkable because it will still be mapped on the Node Graph
			
			if (tileTypes [tiles [x, y]].tileType != TileType.Types.empty) {
				SpawnDiscoverTile (tileTypes [tiles [x, y]].tileAsGObj, new Vector3 (x, y, 0.0f), tileTypes[tiles[x,y]].tileType);
				// set it so it knows it has been spawned
				tileTypes[tiles [x, y]].hasBeenSpawned = true;
			}
		}

		


	}

//	public void DiscoverTile(Vector3 position){
//
//		for (int x = 0; x < resourceTiles.Count; x++) {
//			if (resourceTiles[x].gridPosition == position){
//				// if tile hasn't already been spawned
//				if (!resourceTiles[x].hasBeenSpawned){
//					// instantiate the half discovered tile (this tile will slowly appear)
//					Debug.Log("New tile: " + resourceTiles[x].resourceType);
//					// this half-discovered tile has a script in it that just before destoying itself spawns the proper tile obj
//					SpawnDiscoverTile(resourceTiles[x].tileGameObject, position, resourceTiles[x].myType);
//					// mark this tile as spawned
//					resourceTiles[x].hasBeenSpawned = true;
//				
//				}
//			}
//		}
//	}
   void SpawnDiscoverTile(GameObject tileObj,Vector3 position, TileType.Types type){
		GameObject discoverTile = Instantiate (discoverTileFab, position, Quaternion.identity) as GameObject;
		DiscoverTile dTile = discoverTile.GetComponent<DiscoverTile> ();
		if (dTile != null) {
			dTile.TileToDiscover(newTile: tileObj, mapPosX: (int) position.x , mapPosY: (int)position.y, tileHolder: tileHolder, grid: this, selectedUnit: playerUnit, tileType: type);
		}
	}

//	void SpawnDiscoverTile(GameObject tileObj,Vector3 position, Tile.tileType type){
//		GameObject discoverTile = Instantiate (discoverTileFab, position, Quaternion.identity) as GameObject;
//		DiscoverTile dTile = discoverTile.GetComponent<DiscoverTile> ();
//		if (dTile != null) {
//			dTile.TileToDiscover(newTile: tileObj, mapPosX: (int) position.x , mapPosY: (int)position.y, tileHolder: tileHolder, grid: this, selectedUnit: selectedUnit, tileType: type);
//		}
//	}

	// ** PATHFINDING GRAPH



	void InitPathFindingGraph(){
		// Init array
		graph = new Node[mapSizeX, mapSizeY];
		// Init Nodes as new Nodes
		for (int x =0; x < mapSizeX; x++) {
			for (int y =0; y < mapSizeY; y++) {
				graph[x,y] = new Node();
				
				graph[x,y].x = x;
				graph[x,y].y = y;
				
				graph[x,y].nodeID = x;
			}
		}
		// Populate the graph by calculating the neigbors of each node
		for (int x =0; x < mapSizeX; x++) {
			for (int y =0; y < mapSizeY; y++){

				// grab the neighbors on this node
				if (x > 0){
					// to our left
					graph[x,y].neighbors.Add(graph[x-1, y]);
					if (y > 0){
						graph[x,y].neighbors.Add(graph[x-1, y - 1]); // left down
					}
					if (y < mapSizeY - 1){
						graph[x,y].neighbors.Add(graph[x-1, y +1]); // left up
					}
						
				}
				if (x < mapSizeX - 1){
					// to our right
					graph[x,y].neighbors.Add(graph[x+1, y]);
					if (y > 0){
						graph[x,y].neighbors.Add(graph[x+1, y - 1]); // left down
					}
					if (y < mapSizeY - 1){
						graph[x,y].neighbors.Add(graph[x+1, y +1]); // left up
					}

				}
				if (y > 0){
					// below
					graph[x,y].neighbors.Add(graph[x, y - 1]);
				}
				if (y < mapSizeY - 1){
					// above
					graph[x,y].neighbors.Add(graph[x, y + 1]);
				}
			}
		}
	}

	public Vector3 TileCoordToWorldCoord(int x, int y){
		return new Vector3 (x, y, 0.0f);
	}

	float CheckIfTileisWalkable(int sourceX, int sourceY, int targetX, int targetY){
		float moveCost = (float)tileTypes[tiles[targetX,targetY]].movementCost;

//		if (UnitCanEnterTile (targetX, targetY) == false) {
//			return Mathf.Infinity;
//		}

		if (sourceX != targetX && sourceY != targetY) {
			// MOVING Diagonal! change cost so it's more expensive
			moveCost += 0.001f;
		}

		return moveCost;
		// If there is no difference in cost between straight and diagonal, it moves weird
	}

	public bool UnitCanEnterTile(int x, int y){
		return tileTypes[tiles[x,y]].isWalkable;

//		if (spawnedTiles [x, y] != null) {
//			bool iswalk = spawnedTiles [x, y].GetComponent<TileClick_Handler> ().isWalkable;
//			return iswalk;
//		} else {
//			if (tileTypes[tiles[x,y]].tileType == TileType.Types.empty){
//				return true;
//			}else{
//				return false; // we return false because if null then it hasnt been spawned
//			}
//		}
	}

	public void GenerateWalkPath(int x, int y, bool trueIfPlayerUnit){
		int unitPosX;
		int unitPosY;
		// Clear out selected unit's old path
		if (trueIfPlayerUnit) {
			unitOnPath.GetComponent<SelectedUnit_MoveHandler> ().currentPath = null;
			unitPosX = unitOnPath.GetComponent<SelectedUnit_MoveHandler> ().posX;
			unitPosY = unitOnPath.GetComponent<SelectedUnit_MoveHandler> ().posY;
		} else {
			unitOnPath.GetComponent<Enemy_SpawnHandler> ().currentPath = null;
			unitPosX = unitOnPath.GetComponent<Enemy_SpawnHandler> ().posX;
			unitPosY = unitOnPath.GetComponent<Enemy_SpawnHandler> ().posY;
		}

//		if (UnitCanEnterTile(x,y)== false){ // STOPS from pathfinding to an unwalkable tile
//			return;
//		}


		// Every Node that hasn't been checked yet
		List<Node> unvisited = new List<Node> ();
		
		Dictionary<Node, float> dist = new Dictionary<Node, float> ();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node> ();

		Node source = graph [
		                     unitPosX, 
		                     unitPosY
		                     ];
		Node target = graph [
		                     x, 
		                     y
		                     ];	

		foreach (Node v in graph) {
			if(v != source){
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}else{
				dist [source] = 0; // distance
				prev [source] = null;
			}
			unvisited.Add(v); 
		}

		while (unvisited.Count > 0) {
			Node u = null;
			// I need to sort the list of unvisited every time a v is added
			// this loop gets me the unvisited node with shortest distance
			foreach(Node possibleU in unvisited){
				if (u== null || dist[possibleU] < dist[u]){
					u = possibleU;
				}
			}


			if (u == target){
				break;			// Here we found Target, EXIT while loop
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbors){
//				float alt = dist[u] + u.DistanceTo(v); // distance to move
//				float alt = dist[u] + CheckIfTileisWalkable(v.x, v.y); // distance to move
				float alt = dist[u] + CheckIfTileisWalkable(u.x, u.y, v.x, v.y); 
				if(alt < dist[v]){
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// Check if there is no route to our target
		if (prev [target] == null) {
			// no route from target to source!
			return;
		}

		// Here we have a route from source to target
		List<Node> currentPath = new List<Node> ();

		Node curr = target;

		while (curr != null) {
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// This route is right now from our target to our source, so we need to invert it to move the Unit
		currentPath.Reverse ();

		// Give the unit it's NEW PATH!
		if (trueIfPlayerUnit) {
			unitOnPath.GetComponent<SelectedUnit_MoveHandler> ().currentPath = currentPath;
		} else {
			unitOnPath.GetComponent<Enemy_SpawnHandler> ().currentPath = currentPath;
		}



	}// end Movetarget


}
