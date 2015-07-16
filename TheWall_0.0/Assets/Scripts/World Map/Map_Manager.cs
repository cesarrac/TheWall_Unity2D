using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Map_Manager : MonoBehaviour {

	// list of possible positions on the grid
	public List<Vector3> gridPositions = new List<Vector3>();

	// we can reference a Tile class that has:
	// a variable for its type (stone, wood, empty, etc.),
	// a corresponding GameObject prefab of that type for when it needs to be spawned (e.g. when the Player need to SEE it)

	//list of tiles to reference each tile's data
	public List<Tile> tileDataList = new List<Tile> ();

	//prefab Resource tiles for instantiate
	public GameObject emptyTile, woodTile, stoneTile, grainTile, metalTile;

	//a string to ID the type of tile
	string typeID;

	// map divided in rows and colums
	public int colums = 20;
	public int rows = 20;
	// with max tiles calculated at Start
	int maxTiles;

	//store my tranform and positions
	Transform myTransform;
	Vector3 myStoredPosition;
	Vector3 myCurrentPosition;

	// prefab Town tiles to instantiate
	public GameObject initialTownTile;
	public GameObject centerTTile, topLTTile, topCTTile, topRTTile, centerLTTile, centerRTTile, bottomLTTile, bottomCTTile, bottomRTTile;
	// a list to keep track of tiles and be able to ADD or REMOVE
	List<GameObject>townTiles = new List<GameObject>();

	// a list of the tiles spawned during Scouting, to Remove them when scouting is done
	[HideInInspector]
	public List<GameObject>scoutedTiles = new List<GameObject>();

	// a Transform for holding the Tiles in the Hierarchy
	public Transform tileHolder;

	void Start () {
		maxTiles = colums * rows;
		myTransform = transform;
		myStoredPosition = new Vector3 (myTransform.position.x, myTransform.position.y, 0);

		InitTileDataList ();


	}
	

	void Update () {
		//TODO: change this so it DOESNT spawn town tiles everytime I move
		myCurrentPosition = new Vector3 (myTransform.position.x, myTransform.position.y, 0);
		if (myCurrentPosition != myStoredPosition) {
			SpawnTilesByExpanding (myCurrentPosition);
		}
	}

	// here we initialize the possible positions for the tiles
	public List<Vector3> InitGridPositionsList(){

		for (int x = 0; x < colums; x++) {
			for (int y = 0; y < rows; y++){
				gridPositions.Add(new Vector3(x, y, 0));
			}
		}
		return gridPositions;
	}

	// create Tile Data for entire World Map
	void InitTileDataList(){
		for (int x = 0; x < colums; x++) {
			for (int y = 0; y < rows; y++){
				// to populate this list we need to create tiles using the tile class
					// Tiles need a type: (Empty, Wood, Stone, Metal, Grain)
					// Tiles need a resource ammount: (None, Small, Medium, Large)
				// for now EVERYTHING IS RANDOM
				int tileTypePick = Random.Range(0, 4);
				tileDataList.Add(new Tile(tileTypePick, 10, new Vector3(x, y, 0), TileType(tileTypePick)));
			}
		}
		SpawnInitialTiles(myStoredPosition);
	}

	// we need a function that checks where the camera is centered (using this position for now)
	// in order to instantiate the proper GameObjects that should be visible to the player
	// if the Player/Camera is centered on x:11, y:11 they should be able to see tiles:
	// x: 10 - 12
	// y: 10 - 12

	// Add the first viewable tiles and the first Town tile
	void SpawnInitialTiles (Vector3 position){

		int posX = (int)position.x;
		int posY = (int)position.y;
		for (int x = posX - 1; x <= posX + 1; x++) {
			for (int y = posY -1; y <= posY +1; y++){
				Vector3 tilePos = new Vector3(x, y, 0);
				foreach (Tile tile in tileDataList){
					if (tile.gridPosition == tilePos){
						GameObject spawnedTile = Instantiate(tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
						spawnedTile.transform.parent = tileHolder;
						break;
					}
				}
			}
		}
		// create initial TOWN TILE
		GameObject initTownTile = Instantiate (initialTownTile, position, Quaternion.identity) as GameObject;
		// and add it to the list of town tiles
		townTiles.Add (initTownTile);
		myStoredPosition = position;
	}

	// This is called right now by Mouse Controls everytime the Player clicks on a tile
	void SpawnTilesByExpanding(Vector3 centerPosition){
		// check which direction we moved
		int posX = (int)centerPosition.x;
		int posY = (int)centerPosition.y;
		GameObject expandedTownTile;

		if (centerPosition.x > myStoredPosition.x) { // right
			// spawn town tile first
			expandedTownTile = Instantiate (initialTownTile, centerPosition, Quaternion.identity) as GameObject;
			// loop to Instantiate all the resource tiles around the new town tile 
			for (int x = posX; x <= posX + 1; x++) { // right
				for (int y = posY -1; y <= posY +1; y++) {
					Vector3 tilePos = new Vector3 (x, y, 0);
					foreach (Tile tile in tileDataList) {
						if (tile.gridPosition == tilePos) {
							GameObject spawnedTile = Instantiate (tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
							spawnedTile.transform.parent = tileHolder;
							break;
						}
					}
				}
			}		// repeat the same process for each side... 
		} else if (centerPosition.x < myStoredPosition.x) {// left
			expandedTownTile = Instantiate (initialTownTile, centerPosition, Quaternion.identity) as GameObject;

			for (int x = posX; x >= posX - 1; x--) {
				for (int y = posY -1; y <= posY +1; y++) {
					Vector3 tilePos = new Vector3 (x, y, 0);
					foreach (Tile tile in tileDataList) {
						if (tile.gridPosition == tilePos) {
							GameObject spawnedTile = Instantiate (tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
							spawnedTile.transform.parent = tileHolder;
							break;
						}
					}
				}
			}
		} else if (centerPosition.y < myStoredPosition.y) {// down
			expandedTownTile = Instantiate (initialTownTile, centerPosition, Quaternion.identity) as GameObject;

			for (int x = posX; x >= posX - 1; x--) {
				for (int y = posY -1; y <= posY; y++) {
					Vector3 tilePos = new Vector3 (x, y, 0);
					foreach (Tile tile in tileDataList) {
						if (tile.gridPosition == tilePos) {
							GameObject spawnedTile = Instantiate (tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
							spawnedTile.transform.parent = tileHolder;
							break;
						}
					}
				}
			}
		} else if (centerPosition.y > myStoredPosition.y) {// up
			expandedTownTile = Instantiate (initialTownTile, centerPosition, Quaternion.identity) as GameObject;

			for (int x = posX; x >= posX - 1; x--) {
				for (int y = posY; y <= posY + 1; y++) {
					Vector3 tilePos = new Vector3 (x, y, 0);
					foreach (Tile tile in tileDataList) {
						if (tile.gridPosition == tilePos) {
							GameObject spawnedTile = Instantiate (tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
							spawnedTile.transform.parent = tileHolder;

							break;
						}
					}
				}
			}
		}
		myStoredPosition = centerPosition; // STORE THE NEW POSITION (right now this is always the new created town tile)

	}

	public void SpawnTilesForScout(Vector3 centerPosition){
		// check which direction we moved
		int posX = (int)centerPosition.x;
		int posY = (int)centerPosition.y;
		GameObject expandedTownTile;
		if (centerPosition.y < myStoredPosition.y) {// down
			expandedTownTile = Instantiate (initialTownTile, centerPosition, Quaternion.identity) as GameObject;
			
			for (int x = posX; x >= posX - 1; x--) {
				for (int y = posY -1; y <= posY; y++) {
					Vector3 tilePos = new Vector3 (x, y, 0);
					foreach (Tile tile in tileDataList) {
						if (tile.gridPosition == tilePos) {
							GameObject spawnedTile = Instantiate (tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
							// add it to the list of scouted game objects to later destroy
							scoutedTiles.Add(spawnedTile);
							spawnedTile.transform.parent = tileHolder;

							break;
						}
					}
				}
			}
		} else if (centerPosition.y > myStoredPosition.y) {// up
			expandedTownTile = Instantiate (initialTownTile, centerPosition, Quaternion.identity) as GameObject;
			
			for (int x = posX; x >= posX - 1; x--) {
				for (int y = posY; y <= posY + 1; y++) {
					Vector3 tilePos = new Vector3 (x, y, 0);
					foreach (Tile tile in tileDataList) {
						if (tile.gridPosition == tilePos) {
							GameObject spawnedTile = Instantiate (tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
							// add it to the list of scouted game objects to later destroy
							scoutedTiles.Add(spawnedTile);
							spawnedTile.transform.parent = tileHolder;

							break;
						}
					}
				}
			}
		}

	}

	public void ClearScoutedTiles(){
		if (scoutedTiles.Count > 0) {
			foreach (GameObject obj in scoutedTiles) {
				int i= 0;
				Destroy(obj);
//				scoutedTiles.RemoveAt(i);
				i++;
			}
		}
//		scoutedTiles.Clear ();
	}

//	void SpawnTownTiles (Vector3 position){
//		if (position.x > myStoredPosition.x) { // right
//		
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

	public void CheckResourceQuantity(Tile tile, int index, GameObject tileObj){
		if (tile.maxResourceQuantity <= 0) {
			tileDataList.RemoveAt(index);
			Destroy(tileObj);
		}
	}
}
