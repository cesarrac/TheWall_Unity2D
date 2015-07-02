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

	void Start () {
		gmScript = GetComponent<GameMaster> ();
	}
	
	void Update () {
		SelectUnit ();
		if (selectedUnit != null) {
			Captain cpn = selectedUnit.GetComponent<Captain> ();
			SelectTargetWithMouse(cpn);
		}
	}


	void SelectUnit (){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(m.x, m.y), -Vector2.up);  
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
				if (Input.GetMouseButton(0) && !gmScript.battleStarted){
					selectedUnit = hit.collider.gameObject;
					Vector3 followMousePosition = new Vector3 (m.x, m.y, 0);
					selectedUnit.transform.position = followMousePosition;
				}
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
