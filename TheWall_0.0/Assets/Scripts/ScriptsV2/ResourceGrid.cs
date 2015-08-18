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

//	public TileType[] tileTypes;
	public TileData[,] tiles;

//	TileType.Types tTypes;
//	int[,] tiles;
	public GameObject[,] spawnedTiles;

	// rows and colums determined by level
	public int mapSizeX;
	public int mapSizeY;
	

	public int level;

	public Transform tileHolder; // transform to hold Instantiated tiles in Hierarchy

	public GameObject discoverTileFab; // fab for grey discover tile with discover tile script

	public GameObject unitOnPath;

//	public GameObject playerUnit;

	public Player_SpawnHandler playerSpawnHandler;
	public GameObject playerCapital; 
	public GameObject playerCapitalFab;// to spawn at the start of a new level
	public int capitalSpawnX, capitalSpawnY;

	public Building_UIHandler buildingUIHandler;

	//prefab tiles for instantiate
	public GameObject rockTile, buildableTile;
	// building tile prefabs
	public GameObject building, sExtractFab, mExtractFab, lExtractFab;

	// PATHFINDING VARS:
	Node[,] graph;

//	List<Node>currentPath;

	void Awake(){
		// Initialize the position of the capital
		capitalSpawnX = playerSpawnHandler.capitalPosX;
		capitalSpawnY = playerSpawnHandler.capitalPosY;
	
//		if (level == 0) {
//			mapSizeX = 10;
//			mapSizeY = 10;
//		}
	
	}
	void Start () {

		tiles = new TileData[mapSizeX, mapSizeY];
		spawnedTiles = new GameObject[mapSizeX, mapSizeY];
		InitGrid ();
		InitVisualGrid ();
		InitPathFindingGraph ();
	}

//	void Update(){
//		if (Input.GetMouseButtonDown (1)) {
//			Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			CreateBuildableTile(Mathf.RoundToInt(m.x),Mathf.RoundToInt( m.y));
//
//		}
//	}
	
	void InitGrid(){
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				tiles[x,y]= new TileData(TileData.Types.empty, 0, 1, null); // default to empty tile
			}
		}

		// SPAWN PLAYER CAPITAL HERE:
		tiles [capitalSpawnX, capitalSpawnY] = new TileData(TileData.Types.capital, 0, 10000, playerCapitalFab, 200, 5,0,0);

		//First build tiles
//		tiles[3,2]=new TileData(TileData.Types.buildable, 0, 10000, buildableTile);
//		tiles[4,4]=new TileData(TileData.Types.buildable, 0, 10000, buildableTile);
//		tiles[6,4]=new TileData(TileData.Types.buildable, 0, 10000, buildableTile);
//		tiles[3,8]=new TileData(TileData.Types.buildable, 0, 10000, buildableTile);
//
//		tiles[7,2]=new TileData(TileData.Types.buildable, 0, 10000, buildableTile);
//		tiles[4,6]=new TileData(TileData.Types.buildable, 0, 10000, buildableTile);
//		tiles[6,6]=new TileData(TileData.Types.buildable, 0, 10000, buildableTile);
//		tiles[7,8]=new TileData(TileData.Types.buildable, 0, 10000, buildableTile);
//
//		//first visible tiles
		tiles[5,1]=new TileData(TileData.Types.rock, 20, 10000, rockTile);
//		tiles[5,4]=new TileData(TileData.Types.rock, 20, 10000, rockTile);
		tiles[5,3]=new TileData(TileData.Types.rock, 20, 10000, rockTile);
		tiles[7,2]=new TileData(TileData.Types.rock, 20, 10000, rockTile);
		tiles[8,2]=new TileData(TileData.Types.rock, 20, 10000, rockTile);
		tiles[8,4]=new TileData(TileData.Types.rock, 20, 10000, rockTile);
		tiles[8,5]=new TileData(TileData.Types.rock, 20, 10000, rockTile);
//
//		tiles[3,3]=3;
//		tiles[3,2]=3;
//		tiles[3,4]=3;
//		tiles[4,4]=3;
//		tiles[4,5]=3;
//		tiles[4,6]=3;
	}

//	int SimpleTileSelection(int lastValue){
//		int randomT = Random.Range (0, 5);
//		int randomRepeat = Random.Range (0, 2);
//		if (randomT == lastValue) {
//			if (randomRepeat >= 1) {
//				return lastValue;
//			} else {
//				return SimpleTileSelection (lastValue);
//			}
//		} else {
//			return randomT;
//		}
//	}

	void InitVisualGrid(){
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
//				The ONLY TILES VISIBLE at the start of the game are Buildable tiles
				if (tiles [x, y].tileType==TileData.Types.capital){
					SpawnDiscoverTile(tiles [x, y].tileAsGObj, new Vector3(x, y, 0.0f),tiles [x, y].tileType); 
				}else if (tiles [x, y].tileType==TileData.Types.rock){
					SpawnDiscoverTile(tiles [x, y].tileAsGObj, new Vector3(x, y, 0.0f),tiles [x, y].tileType); 
				}
				
			}
		}
	}

	void CreateBuildableTile(int x, int y){
		// Player clicks on a tile, if that tile is not a buildable tile, call swap tile to swap and "discover"
		if (tiles [x, y].tileType != TileData.Types.buildable) {
			SwapTileType(x, y, TileData.Types.buildable);
		}
	}

	public void DamageTile(int x, int y, float damage){
		tiles [x, y].hp = tiles [x, y].hp - damage;
		if (tiles [x, y].hp <= 0) {
			SwapTileType(x, y, TileData.Types.empty);	// to KILL TILE I just swap it ;)
		}
	}

	public void SwapTileType(int x, int y, TileData.Types newType){
		// MAKE SURE THIS IS NOT A SPAWNED TILE ALREADY!!! 
		// So we don't change the grid tile data where we don't want to!
		if (spawnedTiles [x, y] == null) {
			// swap the old type to new type
			// and give it the right GameObject to instantiate
			switch (newType) {
			case TileData.Types.sextractor:
				tiles [x, y] = new TileData (newType, 0, 10000, sExtractFab, 30, 5, 0, 0);
				break;
			case TileData.Types.mextractor:
				tiles [x, y] = new TileData (newType, 0, 10000, mExtractFab, 30, 5, 0, 0);
				break;
			case TileData.Types.lextractor:
				tiles [x, y] = new TileData (newType, 0, 10000, lExtractFab, 30, 5, 0, 0);
				break;
			case TileData.Types.building:
				tiles [x, y] = new TileData (newType, 0, 10000, building);
				break;
			default:
				print ("No tile changed.");
				break;
			}
			// Discover the tile to display it
			DiscoverTile (x, y, true);
		} else { // if we are swappin an already spawned tile we are MOST LIKELY turning it into an empty tile
			// For example, depleted rock tiles turn into EMPTY tiles - empty tiles DONT require spawning
			Destroy(spawnedTiles[x,y].gameObject);
			tiles[x,y] = new TileData(newType, 0,1,null);
		}
	}

	public void DiscoverTile(int x, int y, bool trueIfSwapping){
		if (spawnedTiles [x, y] == null) { // if it's null it means it hasn't been spawned
			//Dont Spawn a tile if the type is Empty
			// the space will still be walkable because it will still be mapped on the Node Graph
			
			if (tiles [x, y].tileType != TileData.Types.empty) {
				SpawnDiscoverTile (tiles [x, y].tileAsGObj, new Vector3 (x, y, 0.0f), tiles [x, y].tileType);
				// set it so it knows it has been spawned
				tiles [x, y].hasBeenSpawned = true;
			}
		} else { // it HAS been spawned
			if (trueIfSwapping){
				// since we know this tile has already been spawned we need to destroy the old one,
				// before adding the new one
				Destroy (spawnedTiles [x, y].gameObject);
				SpawnDiscoverTile (tiles [x, y].tileAsGObj, new Vector3 (x, y, 0.0f), tiles [x, y].tileType);
			}
		}
	}



   void SpawnDiscoverTile(GameObject tileObj,Vector3 position, TileData.Types type){
		GameObject discoverTile = Instantiate (discoverTileFab, position, Quaternion.identity) as GameObject;
		DiscoverTile dTile = discoverTile.GetComponent<DiscoverTile> ();
		if (dTile != null) {
			dTile.TileToDiscover(newTile: tileObj, mapPosX: (int) position.x , mapPosY: (int)position.y, tileHolder: tileHolder, grid: this,  tileType: type, playerCapital: playerCapital);
		}

	}



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
		float moveCost = (float)tiles[targetX,targetY].movementCost;

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
		return tiles[x,y].isWalkable;

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
