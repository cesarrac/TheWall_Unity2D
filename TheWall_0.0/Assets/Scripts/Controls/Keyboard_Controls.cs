using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Keyboard_Controls : MonoBehaviour {
	// In Map view, this is attached to the Map Manager object
	Transform myTransform;

	// Access the map manager to get the list of Town tiles and their positions
	Map_Manager mapScript;

	//access to Town Building to perform check what to show only when you move
	TownBuilding townBuilding;

	// lists
	public List<Vector2> townTilePositions = new List<Vector2>();

	int storedTownCount;
	int currentTownCount;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		mapScript = GetComponent<Map_Manager> ();
		townBuilding = GetComponent<TownBuilding> ();

		storedTownCount = mapScript.townTiles.Count;
		foreach (GameObject obj in mapScript.townTiles) {
			townTilePositions.Add(new Vector2(obj.transform.position.x, obj.transform.position.y));
		}
	}
	
	// Update is called once per frame
	void Update () {
		currentTownCount = mapScript.townTiles.Count;
		if (currentTownCount > storedTownCount || currentTownCount < storedTownCount) {
			RefreshList();
		}

		if (Input.GetKey (KeyCode.LeftShift)) {
			MoveTileToTileFast ();
		} else {
			MoveTileToTile ();
		}

	}



	void RefreshList(){
		townTilePositions.Clear ();
		foreach (GameObject obj in mapScript.townTiles) {
			if (obj != null){
				townTilePositions.Add(new Vector2(obj.transform.position.x, obj.transform.position.y));
			}
		}
		storedTownCount = mapScript.townTiles.Count;
	}

	void MoveTileToTile(){

		if (Input.GetKeyDown (KeyCode.W)) {
			Vector2 up = new Vector2(myTransform.position.x, myTransform.position.y + 1);
			if (CheckIfValidMove(up)){
				myTransform.position = new Vector3 (up.x, up.y, 0);
			}
		}else if (Input.GetKeyDown (KeyCode.S)) {
			Vector2 down = new Vector2(myTransform.position.x, myTransform.position.y - 1);
			if (CheckIfValidMove(down)){
				myTransform.position = new Vector3 (down.x, down.y, 0);

			}
		}else if (Input.GetKeyDown (KeyCode.A)) {
			Vector2 left = new Vector2(myTransform.position.x - 1, myTransform.position.y);
			if (CheckIfValidMove(left)){
				myTransform.position = new Vector3 (left.x, left.y, 0);

			}
		}else if (Input.GetKeyDown (KeyCode.D)) {
			Vector2 right = new Vector2(myTransform.position.x + 1, myTransform.position.y);
			if (CheckIfValidMove(right)){
				myTransform.position = new Vector3 (right.x, right.y, 0);

			}
		}
	}

	void MoveTileToTileFast(){

		if (Input.GetKey (KeyCode.W)) {
			Vector2 up = new Vector2(myTransform.position.x, myTransform.position.y + 1);
			if (CheckIfValidMove(up)){
				myTransform.position = new Vector3 (up.x, up.y, 0);
			}
		}else if (Input.GetKey (KeyCode.S)) {
			Vector2 down = new Vector2(myTransform.position.x, myTransform.position.y - 1);
			if (CheckIfValidMove(down)){
				myTransform.position = new Vector3 (down.x, down.y, 0);
				
			}
		}else if (Input.GetKey (KeyCode.A)) {
			Vector2 left = new Vector2(myTransform.position.x - 1, myTransform.position.y);
			if (CheckIfValidMove(left)){
				myTransform.position = new Vector3 (left.x, left.y, 0);
				
			}
		}else if (Input.GetKey (KeyCode.D)) {
			Vector2 right = new Vector2(myTransform.position.x + 1, myTransform.position.y);
			if (CheckIfValidMove(right)){
				myTransform.position = new Vector3 (right.x, right.y, 0);
				
			}
		}
	}

	bool CheckIfValidMove(Vector2 nextPos){
		foreach (Vector2 pos in townTilePositions) {
			if (pos == nextPos){
				return true;
				break;
			}
		}
		return false;
	}
}
