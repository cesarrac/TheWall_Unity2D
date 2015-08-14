using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerControls : MonoBehaviour {
	/// <summary>
	/// This contains the controls for player movement and for "discovering" tiles.
	/// For Discovery:
	/// It will call a method in ResourceGrid that will verify if my mouse position has just clicked on a "tile".
	/// For Movement:
	/// 
	/// </summary>

	public ResourceGrid resourceGrid;

	Transform myTransform;

	public int posX;
	public int posY;

	// This stores this unit's pathe
	public List<Node>currentPath = null;

	private Vector3 velocity = Vector3.zero;

	void Start () {
		myTransform = transform;
	}
	

	void Update () {
		if (Input.GetMouseButtonDown (1)) {
			ClickToDiscover ();
		}

		if (currentPath != null) {
			int currNode= 0;
			while (currNode < currentPath.Count -1){
				Vector3 start = resourceGrid.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
				Vector3 end = resourceGrid.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1 ].y);;
				Debug.DrawLine(start, end, Color.red);
				currNode++;
			}
		}

		// Have we moved close enough to the target tile that we can move to next tile in current path?
		if (Vector3.Distance (transform.position, resourceGrid.TileCoordToWorldCoord (posX, posY)) < 0.1f) {
			MoveToNextTile();
		}
		// This MOVEMENT will animate towards the next position
//		transform.position = Vector3.Lerp (transform.position, resourceGrid.TileCoordToWorldCoord (posX, posY), 5f * Time.deltaTime);
		transform.position = Vector3.SmoothDamp (transform.position, resourceGrid.TileCoordToWorldCoord (posX, posY), ref velocity,10f * Time.deltaTime);
	}

	void ClickToDiscover(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 mouseRounded = new Vector3 (Mathf.Round (m.x), Mathf.Round (m.y), 0.0f);


		resourceGrid.DiscoverTile ((int)mouseRounded.x,(int) mouseRounded.y);

	}

	public void MoveToNextTile(){
		if (currentPath == null) {
			return;
		}
		// Remove the old first node from the path
		currentPath.RemoveAt (0);

		// Check if the next tile is a UNWAKABLE tile, if it is clear path
		if (resourceGrid.UnitCanEnterTile (currentPath [0].x, currentPath [0].y) == false) {
			currentPath = null;
			return;
		}	
		// Move to the next Node position in path
		posX = currentPath [0].x;
		posY = currentPath [0].y;
		// This MOVEMENT will just TELEPORT the unit to the next position in case we didn't make it
//		transform.position = resourceGrid.TileCoordToWorldCoord (currentPath [0].x, currentPath [0].y);
//		transform.position = resourceGrid.TileCoordToWorldCoord (posX, posY);

	


		// We are on the tile that is our DESTINATION, 
		// CLEAR PATH
		if (currentPath.Count == 1) {
			currentPath = null;
		}
	}

//	public void Move(Vector3 newPos){
////		myTransform.position = Vector2.MoveTowards (myTransform.position, newPos, 2 * Time.deltaTime);
//		myTransform.position = newPos;
//	}
}
