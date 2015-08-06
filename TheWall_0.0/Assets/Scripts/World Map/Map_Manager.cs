using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Map_Manager : MonoBehaviour {

	// list of possible positions on the grid
	[HideInInspector]
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
	public GameObject initialTownTile, newTownTile;
//	public GameObject centerTTile, topLTTile, topCTTile, topRTTile, centerLTTile, centerRTTile, bottomLTTile, bottomCTTile, bottomRTTile;
	// a list to keep track of tiles and be able to ADD or REMOVE
	public List<GameObject>townTiles = new List<GameObject>();

	// a list of the tiles spawned during Scouting, to Remove them when scouting is done
	[HideInInspector]
	public List<GameObject>scoutedTiles = new List<GameObject>();

	// Transforms for holding the Tiles in the Hierarchy
	public Transform tileHolder;
	public Transform townHolder;

	//Access to the Town Resources
	public TownResources townResourcesScript;

	// currently selected town tile 
	public GameObject selectedTownTile;

	// Prefab to spawn when resource is depleted by a Gatherer
	public GameObject depletedTile;

	// Index of town tile destroyed in town list
	public int townTileIndex;

	// Index for resource tile to not Instantiate over tiles already created
	int resourceTileIndex;

	// an array of Transforms for resource tiles that have been instantiated
	public Transform[] spawnedTiles;

	// going to set a crude timer to check if any of my town tiles are being attacked
	// in order to SPAWN A INDICATOR RED ARROW
	int countTTiles;
	public GameObject redCombatArrow, spawnedArrow;

	void Start () {
		// start timer to check for attacks in town tiles
		countTTiles = townTiles.Count;

		maxTiles = colums * rows;

		myTransform = transform;
		myStoredPosition = new Vector3 (myTransform.position.x, myTransform.position.y, -2f);

		InitTileDataList ();


	}
	

	void Update () {
//		// check attack
//		if (townTiles.Count != countTTiles) {
//			CheckForAnAttack();
//			countTTiles = townTiles.Count;
//		}
//		myCurrentPosition = new Vector3 (myTransform.position.x, myTransform.position.y, 0);
		//TODO: Add it so you have to use some other control to expand instead of just left click
		// can only call spawn tiles by expanding IF town has at least 1 XP point
//		if (myCurrentPosition != myStoredPosition && townResourcesScript.xp > 0) {
//			SpawnTilesByExpanding (myCurrentPosition);
//		}

	}

	// The town tile itself can call this saying it's being attacked
	public void CheckForAnAttack(GameObject tTile){
		Debug.Log ("Checking for an attack!!!");
		Vector3 arrowPos = new Vector3(myTransform.position.x, myTransform.position.y, 0);
		if (spawnedArrow == null){
		  	spawnedArrow = Instantiate(redCombatArrow, arrowPos, Quaternion.identity) as GameObject;
			spawnedArrow.transform.parent = gameObject.transform;
			// give it the town tile that is under attack
			CombatIndicator arrow = spawnedArrow.GetComponent<CombatIndicator>();
			arrow.tileUnderAttack = tTile.transform;
		}



//		foreach (GameObject ttile in townTiles) {
//			bool beingAttacked = ttile.GetComponent<TownTile_Properties>().beingAttacked;
//			Vector3 arrowPos = new Vector3((Mathf.Round(myTransform.position.x)) + 4f,(Mathf.Round(myTransform.position.y)) + 4f, 0);
//			if (beingAttacked){
//				// spawn an arrow in that direction
//				if (spawnedArrow == null){
//					spawnedArrow = Instantiate(redCombatArrow, arrowPos, Quaternion.identity) as GameObject;
//				}
//			}
//		}
	}

	// here we initialize the possible positions for the tiles
	public List<Vector3> InitGridPositionsList(){

		for (int x = 0; x < colums; x++) {
			for (int y = 0; y < rows; y++){
				gridPositions.Add(new Vector3(x, y, -2f)); // default Z to -2 so resource tiles get detected first by linecasts
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
				int tileTypePick = Random.Range(0, 5);
				tileDataList.Add(new Tile(tileTypePick, 10, new Vector3(x, y,  -2f), TileType(tileTypePick)));
			}
		}
		SpawnInitialTiles(myStoredPosition);
	}

	// SpawnInitialTiles checks where the camera is centered (using this position for now)
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
				Vector3 tilePos = new Vector3(x, y,  -2f);
				foreach (Tile tile in tileDataList){
				 	if(tile.gridPosition == tilePos){
						if (tilePos == position){ // dont spawn resources under init town tile
							// make this resource on the list 0
							tile.maxResourceQuantity = 0;
						}else{
							GameObject spawnedTile = Instantiate(tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
							// attach to holder
							spawnedTile.transform.parent = tileHolder;
							break;
						}
					}

				}
			}
		}
		// create initial TOWN TILE
		GameObject initTownTile = Instantiate (initialTownTile, position, Quaternion.identity) as GameObject;
		//attach to town holder
		initTownTile.transform.parent = townHolder;
		// and add it to the list of town tiles
		townTiles.Add (initTownTile);

		myStoredPosition = position;

		spawnedTiles = tileHolder.GetComponentsInChildren<Transform> (); 
	}

	// This is called right now by Mouse Controls everytime the Player clicks on a tile
	public void SpawnTilesByExpanding(Vector3 centerPosition){
		// check which direction we moved
		int posX = (int)centerPosition.x;
		int posY = (int)centerPosition.y;
		GameObject expandedTownTile;

		AddXPGainRate ();

//		// clear resource tile under this new town tile
//		ClearResourceTilesUnderTown (centerPosition, resourceTile);

		// get the Transforms of the tiles that already exist
		spawnedTiles = tileHolder.GetComponentsInChildren<Transform> (); 
		// SPAWNED TILE CHECK LOGIC: 
		//if we are expanding to the RIGHT or the LEFT we need to get what # in spawntiles[]
		// is the Transformm that is below myStoredPosition.x on the grid
		//if expanding UP or DOWN we need the Transform to the right of myStoredPosition.x on the grid
		float xp = Mathf.Round(townResourcesScript.xp);
	
		if ( xp  >= 1) { // must check if we have any XP left to expand with
		
			if (centerPosition.x > myTransform.position.x) { // right
				// spawn town tile first
				expandedTownTile = Instantiate (newTownTile, centerPosition, Quaternion.identity) as GameObject;
				expandedTownTile.transform.parent = townHolder;
				//spend the resources XP point
				townResourcesScript.xp = townResourcesScript.xp - 1;
				// and add it to the list of town tiles
				townTiles.Add (expandedTownTile);
				// MOVE me to that new Tile
				myTransform.position = expandedTownTile.transform.position;
				// inside each town tile a script can store its index in the townTiles list (for easy id)
				TownTile_Properties tTileProperties = expandedTownTile.GetComponent<TownTile_Properties> ();
				// since we just added this tile to the list, its index will be count
				tTileProperties.listIndex = townTiles.Count - 1;
				// loop to Instantiate all the resource tiles around the new town tile 
				for (int x = posX; x <= posX + 1; x++) { // right
					for (int y = posY -1; y <= posY +1; y++) {
						Vector3 tilePos = new Vector3 (x, y, -2f);
						// make sure there isn't already a tile there
						if (CheckIfTileExists(tilePos)){
							// skips these
						}else{
							foreach (Tile tile in tileDataList) {
								if (tile.gridPosition == tilePos) {
									GameObject spawnedTile = Instantiate (tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
									spawnedTile.transform.parent = tileHolder;
									break;
								}
							}
						}
					}
				}		// repeat the same process for each side... 
			} else if (centerPosition.x < myTransform.position.x) {// left
				expandedTownTile = Instantiate (newTownTile, centerPosition, Quaternion.identity) as GameObject;
				expandedTownTile.transform.parent = townHolder;
				//spend the resources XP point
				townResourcesScript.xp = townResourcesScript.xp - 1;
				// and add it to the list of town tiles
				townTiles.Add (expandedTownTile);
				// MOVE me to that new Tile
				myTransform.position = expandedTownTile.transform.position;
				// inside each town tile a script can store its index in the townTiles list (for easy id)
				TownTile_Properties tTileProperties = expandedTownTile.GetComponent<TownTile_Properties> ();
				// since we just added this tile to the list, its index will be count
				tTileProperties.listIndex = townTiles.Count - 1;

				for (int x = posX; x >= posX - 1; x--) {
					for (int y = posY -1; y <= posY +1; y++) {
						Vector3 tilePos = new Vector3 (x, y, -2f);
						// make sure there isn't already a tile there
						if (CheckIfTileExists(tilePos)){							
							// skips these
						}else{
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
			} else if (centerPosition.y < myTransform.position.y) {// down
				expandedTownTile = Instantiate (newTownTile, centerPosition, Quaternion.identity) as GameObject;
				expandedTownTile.transform.parent = townHolder;
				//spend the resources XP point
				townResourcesScript.xp = townResourcesScript.xp - 1;
				// and add it to the list of town tiles
				townTiles.Add (expandedTownTile);
				// MOVE me to that new Tile
				myTransform.position = expandedTownTile.transform.position;
				// inside each town tile a script can store its index in the townTiles list (for easy id)
				TownTile_Properties tTileProperties = expandedTownTile.GetComponent<TownTile_Properties> ();
				// since we just added this tile to the list, its index will be count
				tTileProperties.listIndex = townTiles.Count - 1;

				for (int x = posX + 1; x >= posX - 1; x--) {
					for (int y = posY -1; y <= posY; y++) {
						Vector3 tilePos = new Vector3 (x, y, -2f);
						// make sure there isn't already a tile there
						if (CheckIfTileExists(tilePos)){							
							// skips these
						}else{
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
			} else if (centerPosition.y > myTransform.position.y) {// up
				expandedTownTile = Instantiate (newTownTile, centerPosition, Quaternion.identity) as GameObject;
				expandedTownTile.transform.parent = townHolder;
				//spend the resources XP point
				townResourcesScript.xp = townResourcesScript.xp - 1;
				// and add it to the list of town tiles
				townTiles.Add (expandedTownTile);
				// MOVE me to that new Tile
				myTransform.position = expandedTownTile.transform.position;
				// inside each town tile a script can store its index in the townTiles list (for easy id)
				TownTile_Properties tTileProperties = expandedTownTile.GetComponent<TownTile_Properties> ();
				// since we just added this tile to the list, its index will be count
				tTileProperties.listIndex = townTiles.Count - 1;


				for (int x = posX + 1; x >= posX - 1; x--) {
					for (int y = posY; y <= posY + 1; y++) {
						Vector3 tilePos = new Vector3 (x, y, -2f);
						// make sure there isn't already a tile there
						if (CheckIfTileExists(tilePos)){
							// skips these
						}else{
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
			}
		} else {
			print ("Not enough XP to expand!");
		}
		myStoredPosition = centerPosition; // STORE THE NEW POSITION (right now this is always the new created town tile)

	}

//	void FindTileOnGrid(Vector3 centerPos, Vector3 storedPos, Transform[] spawnedT){
//		Vector3 yBelow = new Vector3 (centerPos.x, centerPos.y - 1f, -2f);
//		Vector3 xRight = new Vector3 (centerPos.x + 1f, centerPos.y, -2f);
//
//		if (centerPos.x > storedPos.x || centerPos.x < storedPos.x) { // moving left or right
//			for (int x = 0; x < spawnedT.Length; x++) {
//				// find the y below storedPos y
//				if (spawnedT[x].position == yBelow){
//					resourceTileIndex = x;
//				}
//			}
//		} else if (centerPos.y > storedPos.y || centerPos.y < storedPos.y){ // up or down
//			for (int x = 0; x < spawnedT.Length; x++) {
//				// find the x to the right of storedPos x
//				if (spawnedT[x].position == xRight){
//					resourceTileIndex = x;
//				}
//			}
//		}
//		print ("Index is " + resourceTileIndex);
//
//	}

	bool CheckIfTileExists(Vector3 newTilePos){
		foreach (Transform trans in spawnedTiles) {
			if (trans.position == newTilePos){
				return true;
			}
		}
		return false;
	}

	public void SpawnTilesForScout(Vector3 centerPosition){
		// check which direction we moved
		int posX = (int)centerPosition.x;
		int posY = (int)centerPosition.y;
	
		if (centerPosition.y < myTransform.position.y) {// down
			for (int x = posX; x >= posX - 1; x--) {
				for (int y = posY -1; y <= posY; y++) {
					Vector3 tilePos = new Vector3 (x, y, 0);
					foreach (Tile tile in tileDataList) {
						if (tile.gridPosition == tilePos) {
							GameObject spawnedTile = Instantiate (tile.tileGameObject, tilePos, Quaternion.identity) as GameObject;
							scoutedTiles.Add(spawnedTile);// add it to the list of scouted game objects to later destroy
							spawnedTile.transform.parent = tileHolder;// parent it
							break;
						}
					}
				}
			}
		} else if (centerPosition.y > myTransform.position.y) {// up
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

	// Mouse control call this when player clicks on tile to expand, it feeds this the resource tile to destroy and take out of list
	public void ClearResourceTilesUnderTown(Vector3 position, GameObject tile){
		// check position of new town tile against tile data list
		foreach (Tile t in tileDataList){
			if (t.gridPosition == position){
				t.maxResourceQuantity = 0; // instead of clearing it off the list, just make the quantity 0
				break;
			}
		}
		Destroy (tile);
	}

	// Called by a dying town tile to take it out of town list
	public void ClearTownTile(int townIndex, Vector3 position){
		int index = 0;
		// if there are any problems finding this index, we can use the position
		GameObject currTown = (townIndex <= townTiles.Count - 1) ? townTiles [townIndex] : null;
		if (currTown != null) {
			townTiles.RemoveAt (townIndex);
			townTileIndex = townIndex;
		} else {
			foreach (GameObject obj in townTiles){
				if (obj.transform.position == position){
					townTiles.RemoveAt(index);
				}else{
					index++;
				}
			}
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

	//This checks each resource tile being gathered to see if it is depleted and needs to be destroyed
	// Accesing the postion of the tile was not working so I'm just going to get the Pos from the gatherer that calls this
	public bool CheckResourceQuantity(Tile tile){
		if (tile.maxResourceQuantity <= 0) {
			// first we need to store the position of this tile
//			Vector3 depletedTilePos = new Vector3(tileDataList[index].gridPosition.x, tileDataList[index].gridPosition.y, tileDataList[index].gridPosition.z);
			//then remove

			return false;
		} else {
			return true;
		}
	}

	public void SpawnDepletedTile(Vector3 position, int index, GameObject tileObj){
		//then remove from list
		tileDataList.RemoveAt(index);
		Destroy(tileObj); // & Destroy it
		// then it its former position, spawn an empty tile tagged tile
		GameObject replacementTile = Instantiate (depletedTile, position, Quaternion.identity) as GameObject;
		replacementTile.transform.parent = tileHolder;
	}

	//call this each time player expands
	public void AddXPGainRate(){
		// player starts with a rate of 1 XP / turn
		// for each five town tiles they have they get an extra 1XP / turn
		if (townTiles.Count % 5 == 0) {
			// xp rate goes up by one
			townResourcesScript.xpGainRate++;
			print("XP gain rate goes up by " + townResourcesScript.xpGainRate);
			int result = townTiles.Count % 5;
			print ("result: " + result);
		}
	}



	
	// This checks what Town tile we currently have selected and stores it to check if it has a building on it
	// called when we move
//	public void CheckTownTileForBuilding(Vector3 currPos){
//		bool hasBuilding = 
//	
//		foreach (GameObject obj in townTiles) {
//			if (obj.transform.position == currPos){ // find the town tile that matches
//
//				selectedTownTile = obj;
//				return thisHasBuilding;
//				break;
//			}
//		}
//		return thisHasBuilding;
//	}

	// Quicker way to get the current Town Tile, using its stored index
	public GameObject GetTownTileByIndex(int index){
		GameObject currTile = townTiles [index];
		// if theres a problem with the index use the position
		return currTile;
	}

	// Slower way to get current Town Tile, searching through entire list
	public GameObject GetTownTileSearch(){
		GameObject tile = new GameObject ();
		foreach (GameObject obj in townTiles) {
			if (obj.transform.position == myTransform.position){

				tile = obj;
				return tile;
				break;
			}
		}
		return null;
	}
}
