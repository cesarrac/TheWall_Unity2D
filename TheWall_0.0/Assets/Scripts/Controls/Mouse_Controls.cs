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

	void Start () {
		gmScript = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster> ();
		myTransform = transform;
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
//		RaycastHit2D hit = Physics2D.Raycast(new Vector2(m.x, m.y), -Vector2.up);  
		RaycastHit2D hit = Physics2D.Linecast (new Vector2 (m.x, m.y), -Vector2.up);
		if (hit.collider != null) {
			if (hit.collider.CompareTag ("Captain")) {
				if (Input.GetMouseButtonDown(0)){
					print ("You clicked on Captain.");
					selectedUnit = hit.collider.gameObject; // this stores the selected unit as a GameObject

					// Instantiate a Selection Box at that Unit's position
					if (mySBox == null){
						mySBox = Instantiate(selectionBox, selectedUnit.transform.position, Quaternion.identity) as GameObject;
						mySBox.transform.parent = selectedUnit.transform; 
						// sets the Selection box parent to be the Unit so it can follow it around if Moved

					}else{
						Destroy(mySBox); // Destroy the old one, bring in the new selection
						mySBox = Instantiate(selectionBox, selectedUnit.transform.position, Quaternion.identity) as GameObject;
						mySBox.transform.parent = selectedUnit.transform; 

					}
				}
			
			} 
				else if (hit.collider.CompareTag("Tile")){
				// if you click on a town tile, watch where the mouse position is when player lets it go to move there
				if (Input.GetMouseButtonUp(0)){
					print ("You clicked on a tile");
					if (m.x < myTransform.position.x && m.y < myTransform.position.y + 1 && m.y > myTransform.position.y -1){ // left
						myTransform.position = new Vector3(myTransform.position.x -1, myTransform.position.y, 0);
					} else if(m.x > myTransform.position.x && m.y < myTransform.position.y + 1 && m.y > myTransform.position.y -1){ // right
						myTransform.position = new Vector3(myTransform.position.x +1, myTransform.position.y, 0);
					} else if (m.y > myTransform.position.y && m.x < myTransform.position.x + 1 && m.x > myTransform.position.x -1){ // up
						myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y + 1, 0);
					} else if (m.y < myTransform.position.y && m.x < myTransform.position.x + 1 && m.x > myTransform.position.x -1){ // up
						myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y - 1, 0); // down
					}
				}
			} 
				else if (hit.collider.CompareTag("Badge")){
				if (Input.GetMouseButtonUp(0)){
					// go to battle view
					print ("Clicked on " + hit.collider.name);
					selectedHorde = hit.collider.gameObject.GetComponent<Horde>();
					// a function here tells the GM to load battleview, with a parameter asking for this Horde unit

					selectedHorde.GoToBattle();
				}
			}
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
