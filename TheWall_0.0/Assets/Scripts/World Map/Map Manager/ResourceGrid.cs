using UnityEngine;
using System.Collections;
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
	//prefab Resource tiles for instantiate
	public GameObject emptyTile, woodTile, stoneTile, grainTile, metalTile;

	// rows and colums determined by level
	public int mapSizeX;
	public int mapSizeY;

	public Vector3 playerSpawnPos;

	public int level;

	public Transform tileHolder; // transform to hold Instantiated tiles in Hierarchy

	public GameObject discoverTileFab; // fab for grey discover tile with discover tile script

	public PlayerControls selectedUnit;

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
		// Initialize selected Unit's vars
		if (selectedUnit != null) {
			selectedUnit.posX = (int)selectedUnit.transform.position.x;
			selectedUnit.posY = (int)selectedUnit.transform.position.y;
			selectedUnit.resourceGrid = this;
		}

//		InitVisualGrid (mapSizeX, mapSizeY, level, playerSpawnPos);
		tiles = new int[mapSizeX, mapSizeY];
		spawnedTiles = new GameObject[mapSizeX, mapSizeY];
		InitGrid ();
//		InitVisualGrid ();
		InitPathFindingGraph ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitGrid(){
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				tiles[x,y]= 0;
			}
		}
		tiles[6,5]=4;
		tiles[6,4]=4;
		tiles[6,3]=4;
		tiles[7,3]=4;
		tiles[8,3]=4;
		tiles[8,4]=4;
		tiles[8,5]=4;

		tiles[3,3]=3;
		tiles[3,2]=3;
		tiles[3,4]=3;
		tiles[4,4]=3;
		tiles[4,5]=3;
		tiles[4,6]=3;
	}

	void InitVisualGrid(){
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				if (tiles[x,y]==0){
					SpawnDiscoverTile(tileTypes[0].tileAsGObj, new Vector3(x, y, 0.0f), tileTypes[0].tileType); 
				}else if (tiles[x,y]==1){
					SpawnDiscoverTile(tileTypes[1].tileAsGObj, new Vector3(x, y, 0.0f), tileTypes[1].tileType); 
				}else if (tiles[x,y]==2){
					SpawnDiscoverTile(tileTypes[2].tileAsGObj, new Vector3(x, y, 0.0f), tileTypes[2].tileType); 
				}else if (tiles[x,y]==3){
					SpawnDiscoverTile(tileTypes[3].tileAsGObj, new Vector3(x, y, 0.0f),tileTypes[3].tileType); 
				}else if (tiles[x,y]==4){
					SpawnDiscoverTile(tileTypes[4].tileAsGObj, new Vector3(x, y, 0.0f),tileTypes[4].tileType); 
				}
				
			}
		}
	}


//	void InitVisualGrid(int colums, int rows, int level, Vector3 playerspawnPos){
//		for (int x = 0; x < colums; x++) {
//			for (int y = 0; y < rows; y++){
//				// to populate this list we need to create tiles using the tile class
//				// Tiles need a type: (Empty, Wood, Stone, Metal, Grain)
//
//				Vector3 tilePos = new Vector3(x, y, 0);
//				if (tilePos != playerspawnPos){			// Dont create a resource tile on the player spawn
//					int tileTypePick = SelectTiles(level);
//					resourceTiles.Add(new Tile(tileTypePick, 10, new Vector3(x, y,  0.0f), tileGameObj: TileType(tileTypePick)));
//				}
//			}
//		}
//	}

//	int SelectTiles(int level){
//		int levelCalc = (level+1) * 10;
//		int tileTypePick = UnityEngine.Random.Range(0, levelCalc);
//		if (tileTypePick < levelCalc - (levelCalc /2)) {
//			return 2; // grain
//		} else if (tileTypePick < levelCalc - 4) {
//			return 1; // wood
//		} else if (tileTypePick < levelCalc - 3) {
//			return 4; // stone
//		} else if (tileTypePick < levelCalc - 1) {
//			return 5; //empty
//		} else {
//			return 3; // metal
//		}
//	}


	GameObject TileType(int type){
		GameObject tile;
		switch (type) {
		case 1:
			tile = woodTile;
			break;
		case 2:
			tile = grainTile;
			break;
		case 3:
			tile = metalTile;
			break;
		case 4:
			tile = stoneTile;
			break;
		case 5:
			tile = emptyTile;
			break;
		default:
			tile = emptyTile;
			break;
		}
		return tile;
	}

	public void DiscoverTile(int x, int y){
		if (spawnedTiles [x, y] != null) {
			TileClick_Handler tC = spawnedTiles [x, y].GetComponent<TileClick_Handler> ();
			if (tiles [x, y] != 4) { // if not an empty tile
				tC.isWalkable = true;
			}
		} else {
			SpawnDiscoverTile (tileTypes [tiles [x, y]].tileAsGObj, new Vector3 (x, y, 0.0f), tileTypes[tiles[x,y]].tileType);

		}


		// Here we make this tile a walkable tile

		// We know that when a Player clicks on a Tile there HAS to BE a GAMEOBJECT instantiated
		// So when we Spawn a tile give its Clickable tile script its grid PosX and PosY
		// and store it in an Array of int[,] in this script, called spawnedTiles[]
		// When Pathfinding asks if the next tile is walkable it Gets Component from spawnedTiles[x,y]
		// and checks if it is walkable

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
			dTile.TileToDiscover(newTile: tileObj, mapPosX: (int) position.x , mapPosY: (int)position.y, tileHolder: tileHolder, grid: this, selectedUnit: selectedUnit, tileType: type);
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
//				if (x ==0 && y ==0){
//					graph[x,y].neighbors.Add(graph[x, y + 1]);
//					graph[x,y].neighbors.Add(graph[x+1, y + 1]);
//					graph[x,y].neighbors.Add(graph[x+1, y]);
//
//				}
					
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
//		return tileTypes[tiles[x,y]].isWalkable;
		// Instead of returning from tileTypes, lets return from actual spawnedTiles data
		if (spawnedTiles [x, y] != null) {
			bool iswalk = spawnedTiles [x, y].GetComponent<TileClick_Handler> ().isWalkable;
			return iswalk;
		} else {
			return false; // we return false because if null then it hasnt been spawned
		}
	}

	public void GenerateWalkPath(int x, int y){
		// Clear out selected unit's old path
		selectedUnit.currentPath = null;

//		if (UnitCanEnterTile(x,y)== false){ // STOPS from pathfinding to an unwalkable tile
//			return;
//		}


		// Every Node that hasn't been checked yet
		List<Node> unvisited = new List<Node> ();
		
		Dictionary<Node, float> dist = new Dictionary<Node, float> ();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node> ();

		Node source = graph [
		                     selectedUnit.GetComponent<PlayerControls> ().posX, 
		                     selectedUnit.GetComponent<PlayerControls> ().posY
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


		selectedUnit.currentPath = currentPath;


	}// end Movetarget


}
