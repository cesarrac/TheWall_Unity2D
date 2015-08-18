using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapV2 : MonoBehaviour {
	public TileData[,] tiles;
	
	// rows and colums determined by level
	public int mapSizeX;
	public int mapSizeY;

	public int level;
	
	public Transform tileHolder; // transform to hold Instantiated tiles in Hierarchy
	
	public GameObject discoverTileFab; // fab for grey discover tile with discover tile script
	
	public GameObject unitOnPath;
	
	public Player_SpawnHandler playerSpawnHandler;
	public int capitalSpawnX, capitalSpawnY;
	
	public Building_UIHandler buildingUIHandler;
	//prefab Resource tiles for instantiate
	public GameObject emptyTile, woodTile, stoneTile, grainTile, metalTile, buildableTile;
	
	// PATHFINDING VARS:
	Node[,] graph;

	void Awake(){
		// Initialize the position of the capital
		capitalSpawnX = playerSpawnHandler.capitalPosX;
		capitalSpawnY = playerSpawnHandler.capitalPosY;
		
		if (level == 0) {
			mapSizeX = 10;
			mapSizeY = 10;
		}
	}

	void Start () {
		
		tiles = new TileData[mapSizeX, mapSizeY];
//		spawnedTiles = new GameObject[mapSizeX, mapSizeY];
//		InitGrid ();
//		InitVisualGrid ();
//		InitPathFindingGraph ();
	}

//	void InitGrid(){
//		for (int x = 0; x < mapSizeX; x++) {
//			for (int y = 0; y < mapSizeY; y++) {
//				tiles[x,y] = new TileData(TileData.Types.empty, 0, 1, grainTile);
//			}
//		}
//		// capital tile is a buildable tile
////		tiles [capitalSpawnX, capitalSpawnY] = 5;
////		
////		//First build tiles
//		tiles[3,2]=new TileData(TileData.Types.buildable, 0, 100000, buildableTile);
//		tiles[4,4]=new TileData(TileData.Types.buildable, 0, 100000, buildableTile);
//		tiles[6,4]=new TileData(TileData.Types.buildable, 0, 100000, buildableTile);
//		tiles[3,8]=new TileData(TileData.Types.buildable, 0, 100000, buildableTile);
//		
//		tiles[7,2]=new TileData(TileData.Types.buildable, 0, 100000, buildableTile);
//		tiles[4,6]=new TileData(TileData.Types.buildable, 0, 100000, buildableTile);
//		tiles[6,6]=new TileData(TileData.Types.buildable, 0, 100000, buildableTile);
//		tiles[7,8]=new TileData(TileData.Types.buildable, 0, 100000, buildableTile);
//	}

//	TileData.Types SimpleTileSelection(TileData.Types lastValue){
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
//	void InitVisualGrid(){
//		for (int x = 0; x < mapSizeX; x++) {
//			for (int y = 0; y < mapSizeY; y++) {
//				//				The ONLY TILES VISIBLE at the start of the game are Buildable tiles
//				if (tiles[x,y].tileType == TileData.Types.buildable){
//					SpawnDiscoverTile(tiles[x,y].tileAsGObj, new Vector3(x, y, 0.0f),tiles[x,y].tileType); 
//				}
//				
//			}
//		}
//	}
//	void SpawnDiscoverTile(GameObject tileObj,Vector3 position, TileType.Types type){
//		GameObject discoverTile = Instantiate (discoverTileFab, position, Quaternion.identity) as GameObject;
//		DiscoverTile dTile = discoverTile.GetComponent<DiscoverTile> ();
//		if (dTile != null) {
//			dTile.TileToDiscover(newTile: tileObj, mapPosX: (int) position.x , mapPosY: (int)position.y, tileHolder: tileHolder, grid: this,  tileType: type);
//		}
//		
//	}

}
