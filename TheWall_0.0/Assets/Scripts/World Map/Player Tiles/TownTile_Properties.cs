using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TownTile_Properties : MonoBehaviour {

	// this holds all the interactable properties of a town tile game object

	// index to hold its ID in Map Manager's towntileDataList
	public int listIndex;

	// bool to tell if this tile has a building on it
	public bool tileHasBuilding;
	public bool tileHasAdvancedBuilding;

	// float for tile's Hit Points
	public float tileHitPoints = 10f;

	// stored transform
	Transform myTransform;

	// public Depleted tile to replace when this tile is destroyed
	public GameObject depletedTile, destroyedTTile;

	// bool to know if this tile is being attacked (this will impede the player from moving to this tile)
	public bool beingAttacked;

	//string to manipulate gameobject tag
	string myTag;
	public string storedTag;

	//access to Map Manager to tell it to take this out of the list when it's destroyed
	Map_Manager mapScript;

	//storing the town list will help update this tile's index when one is destroyed and removed from list
	public int townListCount, startingListCount;

	// storing the old building when Player adds advanced building on this tile
	public GameObject deactivatedBuilding;

	void Start () {
		myTransform = transform;

		storedTag = gameObject.tag;

		mapScript = GameObject.FindGameObjectWithTag ("Map_Manager").GetComponent<Map_Manager> ();
		townListCount = mapScript.townTiles.Count;
		startingListCount = townListCount;
	}
	

	void Update () {
		TownListCheck ();


		if (beingAttacked) {
			gameObject.tag = "Tile Under Attack";
		} else {
			gameObject.tag = storedTag;
		}
	}

	public void TakeDamage(float damage){
		tileHitPoints = tileHitPoints - damage;
		if (tileHitPoints <= 0) {
			KillTile();
		}
	}

	void TownListCheck(){
		// if the list goes down by one or up by one check if my index is above or below the new count
		// has the list changed / has a tile been eliminated?
		townListCount = mapScript.townTiles.Count;
		if (townListCount < startingListCount) {
			// which tile was taken away?
			int missingTileIndex = mapScript.townTileIndex;
			// is my index a higher or lower number?
			if (listIndex > missingTileIndex) {
				listIndex = listIndex - 1; // take my index down one
				startingListCount = townListCount;
			}// the rest of the town index stay the same
			
		} else if (townListCount > startingListCount) {
			//store count again
			startingListCount = townListCount;
		}
	}
	void KillTile(){
	
		if (mapScript != null) {
			// remove it from the list of town tiles
			mapScript.ClearTownTile (listIndex, myTransform.position);
			// create a position for the depleted tile at the same Z as a resource tile (for mouse linecast to work)
			Vector3 depPos = new Vector3 (myTransform.position.x, myTransform.position.y, -2f);
			// spawn Destroyed tile (this will fade away by itself, leaving only the depleted tile)
			GameObject destroyedTile = Instantiate(destroyedTTile, depPos, Quaternion.identity) as GameObject;
			// spawn depleted tile
			GameObject depTile = Instantiate (depletedTile, depPos, Quaternion.identity)as GameObject;
			// parent it to the town holder
			depTile.transform.parent = myTransform.parent;
			destroyedTile.transform.parent = myTransform.parent;
			if (depTile != null) {
				//destroy this town tile
				Destroy (this.gameObject);
			}
		}
	}

	// to Destroy this tile im going to detect Mouse over and wait for a right click
	void OnMouseOver(){
		if (Input.GetMouseButtonDown (1)) {
			KillTile();
		}
	}
}
