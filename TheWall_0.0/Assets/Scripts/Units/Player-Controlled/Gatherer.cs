using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gatherer : Unit {
	// Gatherers are placed on resource tiles by the Player in order to extract an ammount of resources of that type

	// Ammount this unit can gather per turn, this can be upgraded later
	public int gatherAmmount = 2; 

	// Time it takes to gather
	public float gatherTime = 30f;

	// Picked Up, returns true if player is moving this Gatherer
	bool beingMoved = true;

	// true when gathering
	public bool gathering;

	// My transform and stored position
	Transform myTransform;

	// Access to town resources
	TownResources townResources;

	// Access to Map Manager to verify tile with my location
	Map_Manager mapManager;

	// Tile this unit is on
	Tile currentTile;

	// Current tile Index to reference the list
	int currentTileIndex;

	// current tile as GameObject
	public GameObject currTileObj;

	// layer Mask for linecast
	public LayerMask mask = 9;

	void Start () {
		GetName (false);
		myTransform = transform;
		townResources = GameObject.FindGameObjectWithTag ("Town_Central").GetComponent<TownResources> ();
		mapManager = GameObject.FindGameObjectWithTag ("Map_Manager").GetComponent<Map_Manager> ();
		gathering = true;
	}
	
	void Update () {
		if (beingMoved) {
			FollowMouse ();
		} else {
			if (!gathering){
				StartCoroutine (GatherTime (gatherTime, currentTile.resourceType));
			}
		}
	}

	void FollowMouse(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		myTransform.position = new Vector3 (Mathf.Round (m.x), Mathf.Round (m.y), 0);
		if (Input.GetMouseButtonDown (0)) {
			beingMoved = false;
			TileCheck(); // check what type of tile this gatherer was place on
		}
	}

	// once positioned the gatherer checks what type of tile it is in to begin extracting
//	void TileCheck(){
//		Vector2 rayPos = new Vector2 (myTransform.position.x - 0.2f, myTransform.position.y);
//		Debug.DrawLine (new Vector3(rayPos.x, rayPos.y, 0), new Vector3(rayPos.x, myTransform.position.y, 0), Color.red);
//
//		RaycastHit2D hit = Physics2D.Linecast (rayPos, new Vector2(rayPos.x, myTransform.position.y - 0.2f), mask.value);
//		if (hit.collider != null) {
//			print ("Gatherer on top of " + hit.collider.gameObject.name);
//			if (hit.collider.CompareTag("Tile")){
//				Tile currentTile = hit.collider.gameObject.GetComponent<Tile>();
//				StartCoroutine (GatherTime (gatherTime, currentTile.resourceType));
//				print("Starting to gather!");
//			}
//		}
//	}

	void TileCheck(){
		// check my position against the position in the Tile Data List
		for (int x = 0; x < mapManager.tileDataList.Count; x++) {
			if (myTransform.position == mapManager.tileDataList[x].gridPosition){
				currentTile = mapManager.tileDataList[x];
				currentTileIndex = x;
				gathering = false;
				print("Starting to gather!");
			}
		}
	}


	IEnumerator GatherTime(float time, string tileType){
		print ("Counting down gather time.");
		gathering = true;
		yield return new WaitForSeconds(time);
		Gather (tileType, currentTileIndex);

	}

	void Gather(string tileType, int tileIndex){
		if (currTileObj != null) { // make sure we are standing on a tile / tile has not been destroyed
			townResources.AddResource (tileType, gatherAmmount);
			int resourceQ = mapManager.tileDataList [tileIndex].maxResourceQuantity;
			mapManager.tileDataList [tileIndex].maxResourceQuantity = resourceQ - gatherAmmount;
			mapManager.CheckResourceQuantity (currentTile, tileIndex, currTileObj);
			print ("Gathering " + gatherAmmount + " " + tileType);
			gathering = false;
		}
	

	}

	void OnTriggerStay2D(Collider2D coll){
		if(coll.gameObject.CompareTag("Tile")){
			currTileObj= coll.gameObject;
		}
	}
}
