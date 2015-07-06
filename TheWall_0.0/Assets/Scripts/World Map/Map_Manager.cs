﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Map_Manager : MonoBehaviour {

	public List<Vector3> gridPositions = new List<Vector3>();

	// we can reference a Tile class that has:
	// a variable for its type (stone, wood, empty, etc.),
	// a corresponding GameObject prefab of that type for when it needs to be spawned (e.g. when the Player need to SEE it)
	public List<Tile> tileDataList = new List<Tile> ();

	public GameObject emptyTile, woodTile, stoneTile, grainTile, metalTile;
	string typeID;
	// map divided in rows and colums
	public int colums = 20;
	public int rows = 20;
	int maxTiles;

	Transform myTransform;
	Vector3 myStoredPosition;
	Vector3 myCurrentPosition;

	// town tiles as Game Objects
	public GameObject initialTownTile;
	public GameObject centerTTile, topLTTile, topCTTile, topRTTile, centerLTTile, centerRTTile, bottomLTTile, bottomCTTile, bottomRTTile;
	List<GameObject>townTiles = new List<GameObject>();


	void Start () {
		maxTiles = colums * rows;
		myTransform = transform;
		myStoredPosition = new Vector3 (myTransform.position.x, myTransform.position.y, 0);

		InitTileDataList ();


	}
	

	void Update () {
		myCurrentPosition = new Vector3 (myTransform.position.x, myTransform.position.y, 0);
		if (myCurrentPosition != myStoredPosition) {
			SpawnTilesByExpanding (myCurrentPosition);
		}
	}

	// here we initialize the possible positions for the tiles
	void InitGridPositionsList(){
		for (int x = 0; x < colums; x++) {
			for (int y = 0; y < rows; y++){
				gridPositions.Add(new Vector3(x, y, 0));
			}
		}
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


	void SpawnInitialTiles (Vector3 position){

		int posX = (int)position.x;
		int posY = (int)position.y;
		for (int x = posX - 1; x <= posX + 1; x++) {
			for (int y = posY -1; y <= posY +1; y++){
				Vector3 tilePos = new Vector3(x, y, 0);
				foreach (Tile tile in tileDataList){
					if (tile.gridPosition == tilePos){
						GameObject spawnedTile = Instantiate(tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
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

	void SpawnTilesByExpanding(Vector3 centerPosition){
		// check which direction we moved
		int posX = (int)centerPosition.x;
		int posY = (int)centerPosition.y;
		GameObject expandedTownTile;

		if (centerPosition.x > myStoredPosition.x) { // right
			// spawn town tile first
			expandedTownTile = Instantiate (initialTownTile, centerPosition, Quaternion.identity) as GameObject;
			for (int x = posX; x <= posX + 1; x++) {
				for (int y = posY -1; y <= posY +1; y++) {
					Vector3 tilePos = new Vector3 (x, y, 0);
					foreach (Tile tile in tileDataList) {
						if (tile.gridPosition == tilePos) {
							GameObject spawnedTile = Instantiate (tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
							break;
						}
					}
				}
			}
		} else if (centerPosition.x < myStoredPosition.x) {// left
			expandedTownTile = Instantiate (initialTownTile, centerPosition, Quaternion.identity) as GameObject;

			for (int x = posX; x >= posX - 1; x--) {
				for (int y = posY -1; y <= posY +1; y++) {
					Vector3 tilePos = new Vector3 (x, y, 0);
					foreach (Tile tile in tileDataList) {
						if (tile.gridPosition == tilePos) {
							GameObject spawnedTile = Instantiate (tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
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
							break;
						}
					}
				}
			}
		}
		myStoredPosition = centerPosition;

	}

	void SpawnTownTiles (Vector3 position){
		if (position.x > myStoredPosition.x) { // right
		
		}
	}

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

}
