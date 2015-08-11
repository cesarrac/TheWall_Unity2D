using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gatherer : Unit {
	// Gatherers are placed on resource tiles by the Player in order to extract an ammount of resources of that type

		//GATHERING:
	// Ammount this unit can gather per turn, this can be upgraded later
	public int gatherAmmount = 2; 

	// Time it takes to gather by type of resource
	public float grainGatherTime, woodGatherTime, stoneGatherTime, metalGatherTime, genGatherTime;
	float currGatherTime;

	// Picked Up, returns true if player is moving this Gatherer
	bool beingMoved = true;

	// true when gathering
	public bool gathering;

		// SALVAGING:
	public bool salvaging;
	public float salvageTime;

	// My transform and stored position
	Transform myTransform;

	// Access to town resources
	GameObject town;
	TownResources townResources;
	Town_Central townCentral;

	// Access to Map Manager to verify tile with my location
	Map_Manager mapManager;

	// Tile this unit is on
	Tile currentTile;

	// Current tile Index to reference the list
	public int currentTileIndex;

	// current tile as GameObject
	public GameObject currTileObj;

	Mouse_Controls mouse; // to tell it to stop selecting


	void Start () {
		// check if name is null to see if this is a new gatherer or if it's already on the list
		if (name == null) {
			name = GetName (false);
		}

		myTransform = transform;
		town = GameObject.FindGameObjectWithTag ("Town_Central");
		townResources = town.GetComponent<TownResources> ();
		townCentral = town.GetComponent<Town_Central> ();
		GameObject map = GameObject.FindGameObjectWithTag ("Map_Manager");
		mapManager = map.GetComponent<Map_Manager> ();
		mouse = map.GetComponent <Mouse_Controls> ();

		gathering = true;
	}
	
	void Update () {
		if (beingMoved) {
			FollowMouse ();
			if (mouse != null){
				mouse.mouseIsBusy = true;
			}
		} else {
			if (!gathering && currTileObj != null){	// WHEN IT'S TIME TO GATHER
				StartCoroutine (GatherTime (currGatherTime, false, currentTile.resourceType));
			}
			if (salvaging){		// WHEN IT'S TIME TO SALVAGE (ON A DEPLETED TILE)
				StartCoroutine (GatherTime (salvageTime, true));
			}

		}
	}

	void FollowMouse(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		myTransform.position = new Vector3 (Mathf.Round (m.x), Mathf.Round (m.y), -2f);
//		myTransform.position = new Vector3 (m.x, m.y, -2f);

		if (Input.GetMouseButtonUp (0) && !salvaging) {
			TileCheck (); // check what type of tile this gatherer was place on
			beingMoved = false;
			if (mouse != null){
				mouse.mouseIsBusy= false;
			}
		} else if (salvaging) {
			beingMoved = false;
			if (mouse != null){
				mouse.mouseIsBusy = false;
			}
		}
	}

	// to better control how and when we destroy this gatherer
	void OnMouseOver(){
		if (Input.GetMouseButtonDown (1)) {
			GathererDestroy();
		}

	}

	// to check what tile we are in we check this Gatherer's position against the list of tile positions
	//TODO: Once a render detector is created to only render objects that the camera can see, this check
	// can be performed against those tiles on the list that can be found within the camera's viewing space
	void TileCheck(){
		// check my position against the position in the Tile Data List
		for (int x = 0; x < mapManager.tileDataList.Count-1; x++) {
			if (myTransform.position == mapManager.tileDataList[x].gridPosition){
				currentTile = mapManager.tileDataList[x];
				currentTileIndex = x;
				TimeToGather(currentTile.resourceType);
				print("Starting to gather!");
				break;
			}
		}
	}

	// this finds out how long it would take this gatherer to gather this resource and starts the coroutine 
	// by making gathering false
	void TimeToGather(string tileType){
		switch (tileType) {
		case "wood":
			currGatherTime = woodGatherTime + genGatherTime;
			gathering = false;
			break;
		case "grain":
			currGatherTime = grainGatherTime + genGatherTime;
			gathering = false;
			break;
		case  "metal":
			currGatherTime = metalGatherTime + genGatherTime;
			gathering = false;
			break;
		case  "stone":
			currGatherTime = stoneGatherTime + genGatherTime;
			gathering = false;
			break;
		case "empty":
			gathering = true; // stop gathering
			break;
		default:
			gathering = true; // stop gathering
			break;
		}
	}

	IEnumerator GatherTime(float time, bool trueIfSalvaging, string tileType = "none"){
		print ("Counting down gather time.");
		gathering = true;
		salvaging = false;
		yield return new WaitForSeconds(time);
		if (trueIfSalvaging) {
			Salvage();
		} else {
			Gather (tileType, currentTileIndex);
		}
	}

	void Gather(string tileType, int tileIndex){
		if (currTileObj != null) { // make sure we are standing on a tile / tile has not been destroyed
			// first check if the tile has resources left
			if (mapManager.CheckResourceQuantity (currentTile)) {
				// it does, add resources to the town
				townResources.AddResource (tileType, gatherAmmount);
				// substract from tile
				int resourceQ = mapManager.tileDataList [tileIndex].maxResourceQuantity;
				mapManager.tileDataList [tileIndex].maxResourceQuantity = resourceQ - gatherAmmount; 
				print ("Gathering " + gatherAmmount + " " + tileType);
				// start the coroutine again
				gathering = false;
			} else {
				// there are no more resources left in this tile, destroy and substract from spawnedGatherers
				mapManager.SpawnDepletedTile(myTransform.position, tileIndex, currTileObj);
				GathererDestroy ();
			}


		} else {
			GathererDestroy ();
		}
	}

	void GathererDestroy(){
		Destroy (this.gameObject);
		townCentral.spawnedGatherers--;
		townCentral.availableGatherers++;
	}

	void OnTriggerStay2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Tile")) {
			currTileObj = coll.gameObject;
		} 
		if (coll.gameObject.CompareTag ("Food Source")) {
			currTileObj = coll.gameObject;
		} 
		else {
			currTileObj = null;
		}
	}
	void OnTriggerEnter2D(Collider2D coll){

		if (coll.gameObject.CompareTag ("Depleted")) {
			salvaging = true;
		}
	}
	void OnTriggerExit2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Depleted")) {
			salvaging = false;
		}
	}

	// SALVAGE allows the gatherer to search a field (a Depleted tile) and get random resources
	void Salvage(){
		Debug.Log ("Look ma! I'm salvaging!");
		int resourceSelect = Random.Range (0, 20);
		if (resourceSelect <= 4) {
			//get resource
			townResources.AddResource ("grain", 1);
		} else if (resourceSelect <= 8) {
			townResources.AddResource ("grain", 2);
		} else if (resourceSelect <= 12) {
			townResources.AddResource ("grain", 3);
		} else if (resourceSelect <= 16) {
			townResources.AddResource ("grain", 4);
		} else if (resourceSelect <= 20) {
			townResources.AddResource ("grain", 5);
		}
		salvaging = true;
	}
}
