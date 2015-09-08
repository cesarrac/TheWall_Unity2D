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
	public Vector2[] rocksOnLevel; // used to initialize the positions of rocks on the grid
	public int totalVertWaterLines, totalHorizWaterLines;
	public Vector2[] vertWaterLines, horizWaterLines; // used to initialize WATER tiles

	public Transform tileHolder; // transform to hold Instantiated tiles in Hierarchy

	public GameObject discoverTileFab; // fab for grey discover tile with discover tile script

	public GameObject unitOnPath;
	public List<Node>pathForEnemy;

//	public Player_SpawnHandler playerSpawnHandler;
	public GameObject playerCapital; 
	public GameObject playerCapitalFab;// to spawn at the start of a new level
	public int capitalSpawnX, capitalSpawnY;

	public Building_UIHandler buildingUIHandler;
		
	public ObjectPool objPool;

	// BUILDING COSTS:
	public int[] extractorCost, machineGunCost, seaWitchCost, harpoonHCost, cannonCost, sFarmCost; // the array's [0] value is ORE Cost, [1] value is FOOD Cost
	public int[] storageCost, sDesaltCost, sniperCost, nutriCost;

	public Player_ResourceManager playerResources;


	// PATHFINDING VARS:
	Node[,] graph;

	void Awake(){
		// Initialize the position of the capital
//		capitalSpawnX = playerCapital.capitalPosX;
//		capitalSpawnY = playerCapital.capitalPosY;
	
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

	void Update(){
		if (Input.GetMouseButtonDown (1)) {
			Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int mX = Mathf.RoundToInt(m.x);
			int mY = Mathf.RoundToInt(m.y);
			Debug.Log("Coords: x: " + mX + " y: " + mY);
			if (mX <= mapSizeX && mY <= mapSizeY)
				Debug.Log("Tile type: " + tiles[mX, mY].tileType);
		}
	}
	
	void InitGrid(){
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				tiles[x,y]= new TileData(TileData.Types.empty, 0, 1); // default to empty tile
			}
		}

		// SPAWN PLAYER CAPITAL HERE:
		tiles [capitalSpawnX, capitalSpawnY] = new TileData("Capital", TileData.Types.capital, 0, 10000, 200, 5,0,0,0,0);

		InitRockTiles ();
		InitWaterTiles ();
	}

	void InitRockTiles(){
		for (int i = 0; i < rocksOnLevel.Length; i ++) {
			tiles[(int)rocksOnLevel[i].x,(int) rocksOnLevel[i].y] = new TileData(TileData.Types.rock, 3000, 3);
		}
	}
	void InitWaterTiles(){
		// one straight line up Vertical
		for (int i = 0; i < totalVertWaterLines; i++) {
			for (int x = (int)vertWaterLines[i].x; x == (int)vertWaterLines[0].x; x++) {
				for (int y =0; y < vertWaterLines[i].y; y++) {
					tiles [x, y] =  new TileData(TileData.Types.water, 0, 10000);
				}
			}
		}
	
		if (totalHorizWaterLines > 0) {
			for (int i = 0; i < totalHorizWaterLines; i++) {
				for (int y = (int)horizWaterLines[i].y; y == (int)horizWaterLines[i].y; y++){
					for (int x = 0; x < horizWaterLines[i].x; x++) {
						tiles [x, y] =  new TileData(TileData.Types.water, 0, 10000);
					}
				}
			}
		}

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
					SpawnDiscoverTile(tiles [x, y].tileName, new Vector3(x, y, 0.0f),tiles [x, y].tileType); 
				}
//				else if (tiles [x, y].tileType==TileData.Types.rock){
//					SpawnDiscoverTile(tiles [x, y].tileName, new Vector3(x, y, 0.0f),tiles [x, y].tileType); 
//				}
				
			}
		}
	}

	/// <summary>
	/// Damages the tile.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="damage">Damage.</param>
	public void DamageTile(int x, int y, float damage)
	{
		// make sure there IS a tile there
		if (spawnedTiles [x, y] != null) {

			tiles [x, y].hp = tiles [x, y].hp - damage;
			if (tiles [x, y].hp <= 0) {
				SwapTileType (x, y, TileData.Types.empty);	// to KILL TILE I just swap it ;)
			}

		} else {
			Debug.Log("GRID: Could NOT find tile to damage!");
		}
	}

	/// <summary>
	/// Gets the type of the tile.
	/// </summary>
	/// <returns>The tile type.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public TileData.Types GetTileType(int x, int y)
	{
		return tiles[x,y].tileType;
	}

	/// <summary>
	/// Gets the tile game object from spawned tiles array.
	/// </summary>
	/// <returns>The tile game object.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public GameObject GetTileGameObj(int x, int y){
		if (spawnedTiles [x, y] != null)
			return spawnedTiles [x, y];
		else
			return null;
	}


	/// <summary>
	/// Swaps the type of the tile.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="newType">New type.</param>
	public void SwapTileType(int x, int y, TileData.Types newType){

		// MAKE SURE THIS IS NOT A SPAWNED TILE ALREADY!!! 
		// So we don't change the grid tile data where we don't want to!
		if (spawnedTiles [x, y] == null) {
			// swap the old type to new type
			switch (newType) {
			case TileData.Types.extractor:
				tiles [x, y] = new TileData ("Extractor",newType, 0, 10000, 5, 5, 0, 0, extractorCost[1], extractorCost[0]);
				break;
			case TileData.Types.machine_gun:
				tiles [x, y] = new TileData ("Machine Gun", newType, 0, 10000, 30, 5, 5, 0, machineGunCost[1], machineGunCost[0]);
				break;
			case TileData.Types.cannons:
				tiles [x, y] = new TileData ("Cannons", newType, 0, 10000, 30, 5, 3, 0, seaWitchCost[1], seaWitchCost[0]);
				break;
			case TileData.Types.harpoonHall:
				tiles [x, y] = new TileData ("Harpooner's Hall", newType, 0, 10000, 50, 6, 0, 0, harpoonHCost[1], harpoonHCost[0]);
				break;
			case TileData.Types.farm_s:
				tiles [x, y] = new TileData ("Seaweed Farm", newType, 0, 10000, 25, 1, 0, 0, sFarmCost[1], sFarmCost[0]);
				break;
			case TileData.Types.storage:
				tiles [x, y] = new TileData ("Storage", newType, 0, 10000, 35, 2, 0, 0, storageCost[1], storageCost[0]);
				break;
			case TileData.Types.desalt_s:
				tiles [x, y] = new TileData ("Desalination Pump", newType, 0, 10000, 15, 1, 0, 0, sDesaltCost[1], sDesaltCost[0]);
				break;
			case TileData.Types.sniper:
				tiles [x, y] = new TileData ("Sniper Gun", newType, 0, 10000, 0, 0, 0, 0, sniperCost[1], sniperCost[0]);
				break;
			case TileData.Types.seaWitch:
				tiles [x, y] = new TileData ("Sea-Witch Crag", newType, 0, 10000, 0, 0, 0, 0, seaWitchCost[1], seaWitchCost[0]);
				break;
			case TileData.Types.nutrient:
				tiles [x, y] = new TileData ("Nutrient Generator", newType, 0, 10000, 0, 0, 0, 0, nutriCost[1], nutriCost[0]);
				break;
			case TileData.Types.building:
				tiles [x, y] = new TileData (newType, 0, 10000);
				break;
			default:
				print ("No tile changed.");
				break;
			}
			// Discover the tile to display it
			DiscoverTile (x, y, true);
			// if tile is a Building with a FOOD COST, apply it to resources
			if (tiles[x,y].foodCost > 0)
				playerResources.totalFoodCost = playerResources.totalFoodCost + tiles[x,y].foodCost;

		} else { 
			// if we are swappin an already spawned tile we are MOST LIKELY turning it into an empty tile
			// BUT if this was any building that has a food cost that must be reflected in Player resources 
			//	by subtracting from the total food cost
			if (tiles[x,y].foodCost > 0){
				playerResources.totalFoodCost = playerResources.totalFoodCost - tiles[x,y].foodCost;
			}

			// ALSO if it's a Farm we need to subtract its FOOD production and its WATER consumed
			if (playerResources.foodProducedPerDay > 0){
				if (tiles[x,y].tileType == TileData.Types.farm_s || tiles[x,y].tileType == TileData.Types.nutrient){
					FoodProduction_Manager foodM = spawnedTiles [x, y].GetComponent<FoodProduction_Manager>();
					playerResources.CalculateFoodProduction(foodM.foodProduced, foodM.productionRate, foodM.waterConsumed, true);
				}
			}

			// AND if it's a STORAGE we need to subtract all the ORE and WATER from the resources
			if (tiles[x,y].tileType == TileData.Types.storage){
				Storage storage = spawnedTiles[x,y].GetComponent<Storage>();
//				if (storage.oreStored > 0 || storage.waterStored > 0){
//					playerResources.ChangeResource("Ore", - storage.oreStored);
//					playerResources.ChangeResource("Water", -storage.waterStored);
//				}
				// remove the storage building from the list
				playerResources.RemoveStorageBuilding(storage);
			}

			// If it's an EXTRACTOR also need to subtract from Ore Produced
			if (tiles[x,y].tileType == TileData.Types.extractor){
				Extractor extra = spawnedTiles [x, y].GetComponent<Extractor>();
				playerResources.CalculateOreProduction(extra.extractAmmnt, extra.extractRate, true);
			}

			// Same thing for a WATER PUMP
			if (tiles[x,y].tileType == TileData.Types.desalt_s || tiles[x,y].tileType == TileData.Types.desalt_m 
			    || tiles[x,y].tileType == TileData.Types.desalt_l){
				DeSalt_Plant pump = spawnedTiles [x, y].GetComponent<DeSalt_Plant>();
				playerResources.CalculateWaterProduction(pump.waterPumped, pump.pumpRate, true);
			}

			// RETURN 30% OF THE ORE COST TO THE RESOURCES
			float calc = (float)tiles[x,y].oreCost * 0.3f;
			playerResources.ore = playerResources.ore + (int)calc;

			Destroy(spawnedTiles[x,y].gameObject);
			tiles[x,y] = new TileData(newType, 0,1);
		}
	}

	public void DiscoverTile(int x, int y, bool trueIfSwapping){
		if (spawnedTiles [x, y] == null) { // if it's null it means it hasn't been spawned
			//Dont Spawn a tile if the type is Empty
			// the space will still be walkable because it will still be mapped on the Node Graph
			
			if (tiles [x, y].tileType != TileData.Types.empty) {
				SpawnDiscoverTile (tiles [x, y].tileName, new Vector3 (x, y, 0.0f), tiles [x, y].tileType);
				// set it so it knows it has been spawned
				tiles [x, y].hasBeenSpawned = true;
			}
		} else { // it HAS been spawned
			if (trueIfSwapping){
				// since we know this tile has already been spawned we need to destroy the old one,
				// before adding the new one
				Destroy (spawnedTiles [x, y].gameObject);
				SpawnDiscoverTile (tiles [x, y].tileName, new Vector3 (x, y, 0.0f), tiles [x, y].tileType);
			}
		}
	}



   void SpawnDiscoverTile(string tileName,Vector3 position, TileData.Types type){
		// spawn the half tile from the pool
		GameObject discoverTile = objPool.GetObjectForType ("Half_Tile", false);
		if (discoverTile != null) {
			discoverTile.transform.position = position;
			DiscoverTile dTile = discoverTile.GetComponent<DiscoverTile> ();
			dTile.objPool = objPool;
			dTile.TileToDiscover(newTileName: tileName, mapPosX: (int) position.x , mapPosY: (int)position.y, tileHolder: tileHolder, grid: this,  tileType: type, playerCapital: playerCapital);
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
	//TODO: Properly check for the tile map coords against world coords, right now this only works b/c map is set to 0,0 in world space
	public Vector2 TileCoordToWorldCoord(int x, int y){
		return new Vector2 ((float)x, (float)y);
	}

	/// <summary>
	/// Checks the tile move cost,
	/// adds extra cost to DIAGONAL movement.
	/// </summary>
	/// <returns>The tile move cost.</returns>
	/// <param name="sourceX">Source x.</param>
	/// <param name="sourceY">Source y.</param>
	/// <param name="targetX">Target x.</param>
	/// <param name="targetY">Target y.</param>
	float CheckTileMoveCost(int sourceX, int sourceY, int targetX, int targetY){
		float moveCost = (float)tiles[targetX,targetY].movementCost;
		if (sourceX != targetX && sourceY != targetY) {
			// MOVING Diagonal! change cost so it's more expensive
			moveCost += 0.001f;
		}
		return moveCost;
		// If there is no difference in cost between straight and diagonal, it moves weird
	}

	public bool UnitCanEnterTile(int x, int y){
		return tiles[x,y].isWalkable;
	}

	/// <summary>
	/// Generates the walk path for both player-controlled units and enemy AI.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="trueIfPlayerUnit">If set to <c>true</c> 
	/// Only true if player unit so method can be shared while X and Y coordinates
	/// are set properly.</param>
	/// <param name="enemyX">Enemy x.
	/// Use when enemy, else default is 0</param>
	/// <param name="enemyY">Enemy y.
	/// Same as x</param>
	public void GenerateWalkPath(int x, int y, bool trueIfPlayerUnit, int unitX = 0, int unitY = 0){
		int unitPosX;
		int unitPosY;
		// Clear out selected unit's old path
		if (trueIfPlayerUnit) {
			unitOnPath.GetComponent<SelectedUnit_MoveHandler> ().currentPath = null;
			unitPosX = unitX;
			unitPosY = unitY;
		} else {
			pathForEnemy = null;
			unitPosX = unitX;
			unitPosY = unitY;
//			unitOnPath.GetComponent<Enemy_MoveHandler> ().currentPath = null;
//			unitPosX = unitOnPath.GetComponent<Enemy_MoveHandler> ().posX;
//			unitPosY = unitOnPath.GetComponent<Enemy_MoveHandler> ().posY;
		}

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
				float alt = dist[u] + CheckTileMoveCost(u.x, u.y, v.x, v.y); 
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
			pathForEnemy = currentPath;
//			unitOnPath.GetComponent<Enemy_MoveHandler> ().currentPath = currentPath;
		}

	}// end Movetarget


}
