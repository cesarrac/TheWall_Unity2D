using UnityEngine;
using System.Collections;

public class Mouse_Controls : MonoBehaviour {
	// This script will handle the Mouse Controls including:
		// Friendly Unit SELECTION **
		// Target SELECTION that overrides the selected unit's current target **
		// Menu interaction
		// Placing Units on the battle map

	GameObject selectedUnit;
	GameObject selectedTarget;
	public GameObject selectionBox;
	GameObject mySBox;

	GameMaster gmScript;

	Transform myTransform;

	Horde selectedHorde;

	Town_Central townCentral; // using this access to control Gatherers and get damages of player tap hits

	// Access to the Camera script for Scout ability
	Camera_Follow camFollowScript;

	// Bool to stop expansion and town shooting when placing with mouse
	public bool mouseIsBusy; // this turns true when mouse is holding a Gatherer or building for placement

	// Access to Map Manager to expand town tiles
	Map_Manager mapScript;

	// Tile select box
//	public GameObject tileSelectionBox;

	// need to store the resource tile to destroy when Expanding
	public GameObject resourceTileToDestroy;

	void Start () {
		gmScript = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster> ();
		if (Application.loadedLevel == 0) {
			townCentral = GameObject.FindGameObjectWithTag("Town_Central").GetComponent<Town_Central>();
			camFollowScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera_Follow>();
			mapScript = GetComponent<Map_Manager>();
		}
	
		myTransform = transform;
		// FOR NOW im going to spawn a selection box here to be able to see what town tile i have selected
		GameObject tileSelectBox = Instantiate (selectionBox, myTransform.position, Quaternion.identity) as GameObject;
		tileSelectBox.transform.parent = myTransform;
	}
	
	void Update () {
		Select ();
		if (selectedUnit != null) {
			Captain cpn = selectedUnit.GetComponent<Captain> ();
			SelectTargetWithMouse(cpn);
		}

	
	}


	void Select (){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//		Vector3 camPos = Camera.main.transform.position;
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(m.x, m.y), -Vector2.up);  
//		RaycastHit2D hit = Physics2D.Linecast (new Vector2 (m.x, m.y), -Vector2.up);
		if (hit.collider != null) {
			if (hit.collider.CompareTag ("Captain")) {
				if (Input.GetMouseButtonDown (0)) {
					print ("You clicked on Captain.");
					selectedUnit = hit.collider.gameObject; // this stores the selected unit as a GameObject

					// Instantiate a Selection Box at that Unit's position
					if (mySBox == null) {
						mySBox = Instantiate (selectionBox, selectedUnit.transform.position, Quaternion.identity) as GameObject;
						mySBox.transform.parent = selectedUnit.transform; 
						// sets the Selection box parent to be the Unit so it can follow it around if Moved

					} else {
						Destroy (mySBox); // Destroy the old one, bring in the new selection
						mySBox = Instantiate (selectionBox, selectedUnit.transform.position, Quaternion.identity) as GameObject;
						mySBox.transform.parent = selectedUnit.transform; 

					}
				}
			
			} else if (hit.collider.CompareTag ("Tile") || hit.collider.CompareTag ("Destroyed Town")  ) {
				// if you click on a town tile, watch where the mouse position is when player lets it go to move there
					// mouse must not be busy to be able to expand, meaning not currently shooting or placing units
//				mouseIsBusy = false;
				// as soon as I touch the tile I need to store it in case it will need to be destroyed
//				resourceTileToDestroy = hit.collider.gameObject;
//				GameObject destroyThisTile = resourceTileToDestroy;

				if (Input.GetMouseButtonUp(0) && !mouseIsBusy) {
				
					print ("You clicked on a tile");
					Vector2 mouseRounded = new Vector2 (Mathf.Round (m.x), Mathf.Round (m.y));
					Vector2 myPosRounded = new Vector2 (Mathf.Round (myTransform.position.x), Mathf.Round (myTransform.position.y));
				
					if (mouseRounded.x == myPosRounded.x - 1 && mouseRounded.y == myPosRounded.y) { // left
						//ADD & clear the resource tile under new town tile
						mapScript.SpawnTilesByExpanding(mouseRounded);
					} else if (mouseRounded.x == myPosRounded.x + 1 && mouseRounded.y == myPosRounded.y) { // right
						//ADD & clear the resource tile under new town tile
						mapScript.SpawnTilesByExpanding(mouseRounded);
					} else if (mouseRounded.y == myPosRounded.y + 1 && mouseRounded.x == myPosRounded.x) { // up
						//ADD & clear the resource tile under new town tile
						mapScript.SpawnTilesByExpanding(mouseRounded);
					} else if (mouseRounded.y == myPosRounded.y - 1 && mouseRounded.x == myPosRounded.x) { // down
						//ADD & clear the resource tile under new town tile
						mapScript.SpawnTilesByExpanding(mouseRounded);
					}

					// vvvvvv THIS CODE BELOW ACTUALLY MOVES THE GAMEOBJECT WITH THIS SCRIPT WHEN EXPANDING vvvv

					//Vector2 myPosRounded = new Vector2 (Mathf.Round (myTransform.position.x), Mathf.Round (myTransform.position.y));
//					if (mouseRounded.x < myPosRounded.x && mouseRounded.y < myPosRounded.y + 1 && mouseRounded.y > myPosRounded.y - 1) { // left
//						myTransform.position = new Vector3 (myTransform.position.x - 1, myTransform.position.y, 0);
//					} else if (mouseRounded.x > myPosRounded.x && mouseRounded.y < myPosRounded.y + 1 && mouseRounded.y > myPosRounded.y - 1) { // right
//						myTransform.position = new Vector3 (myTransform.position.x + 1, myTransform.position.y, 0);
//					} else if (mouseRounded.y > myPosRounded.y && mouseRounded.x < myPosRounded.x + 1 && mouseRounded.x > myPosRounded.x - 1) { // up
//						myTransform.position = new Vector3 (myTransform.position.x, myTransform.position.y + 1, 0);
//					} else if (mouseRounded.y < myPosRounded.y && mouseRounded.x < myPosRounded.x + 1 && mouseRounded.x > myPosRounded.x - 1) { // up
//						myTransform.position = new Vector3 (myTransform.position.x, myTransform.position.y - 1, 0); // down
//					}

				}
			} 
//			else if (hit.collider.CompareTag ("Badge")) {
//				mouseIsBusy = true;
//				if (Input.GetMouseButtonUp (0)) {
//					mouseIsBusy = false;
//					print ("Clicked on " + hit.collider.name);
//					selectedHorde = hit.collider.gameObject.GetComponent<Horde> ();
//					if (selectedHorde.nextToTownTile){
//						selectedHorde.TakeDamage (townCentral.shortRangeDamage);
//						print (selectedHorde.gameObject.name + " takes " + townCentral.shortRangeDamage + " damage!");
//					}
//
////
////					//check how many tiles away this Horde is
////					// both positions rounded
//////					Vector2 myPosRounded = new Vector2 (Mathf.Round (myTransform.position.x), Mathf.Round (myTransform.position.y));
//////					Vector2 hordePosRounded = new Vector2 (Mathf.Round (selectedHorde.gameObject.transform.position.x), Mathf.Round (selectedHorde.gameObject.transform.position.y));
//////					if (hordePosRounded.x > myPosRounded.x + 2 || hordePosRounded.y > myPosRounded.y + 2){
//////						// long range damage
//////						selectedHorde.TakeDamage(townCentral.longRangeDamage);
//////						print (selectedHorde.gameObject.name + " takes " + townCentral.longRangeDamage + " damage!");
//////					} else if (hordePosRounded.x > myPosRounded.x || hordePosRounded.y > myPosRounded.y){
//////						// short range
//////						selectedHorde.TakeDamage(townCentral.shortRangeDamage);
//////						print (selectedHorde.gameObject.name + " takes " + townCentral.shortRangeDamage + " damage!");
//////					}
////
////					// tell the GM to load battleview
////					// THIS WOULD LOAD BATTLEVIEW selectedHorde.GoToBattle ();
//				}
//			} 
//				else if (hit.collider.CompareTag ("Gatherer")) {
//				mouseIsBusy = true;
//				if (Input.GetMouseButtonDown (1)) {
//					Destroy (hit.collider.gameObject);
//					townCentral.availableGatherers++;
//					mouseIsBusy = false;
//				}
//			}
		} else { // hit.collider is null so mouse is definitely NOT busy
			mouseIsBusy = false;
		} 
		// for dragging the unit with mouse
		if (selectedUnit != null) { // once you click on a unit this will be true
			if (Input.GetMouseButton(0) && !gmScript.battleStarted){
				Vector3 followMousePosition = new Vector3 (m.x, m.y, 0);
				selectedUnit.transform.position = followMousePosition;
			} else{
				selectedUnit = null;
			}
		}

		//Controls for Scout function (ability to look around the map) //TODO: Add finite ammount of scouting time
		if (Input.GetMouseButton (1)) {
			camFollowScript.scouting = true;
			camFollowScript.ScoutCam (m);
		} else {
			camFollowScript.scouting = false;
		}
	} 


	void SelectTargetWithMouse(Captain selectCaptain){
		// this will check if the Mouse is over a Monster, if it IS  when user RIGHTCLICKS it returns target as GameObject 
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), -Vector2.up);  
		if (hit.collider != null) {
			if (hit.collider.CompareTag ("Enemy")) {
				if (Input.GetMouseButtonDown (1)) {
					selectedTarget = hit.collider.gameObject;
					selectCaptain.PlayerTargetAssign(selectedTarget);
					print ("You clicked a Monster");
					// Once target is selected we need to DESTROY the selection box
					Destroy(mySBox);
				}
			}
		} 

	}




}
