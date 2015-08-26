using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupUnit_MoveHandler : MonoBehaviour {
	/// <summary>
	/// Moves a group of 4 units together in formation. If they attack they will move on their own,
	/// and when they are done with their enemy, this script will move them back to formation.
	/// </summary>

	public ResourceGrid resourceGrid;
	
	public int posX;
	public int posY;
	
	// This stores this unit's pathe
	public List<Node>currentPath = null;
	
	//	private Vector2 velocity = Vector2.zero;
	private Vector3 velocity = Vector3.zero;
	
	
	bool isSelected;
	
	public float movementSpeed;
	public bool moving = false, movingToAttack = false;
	public int mX, mY;

	public GameObject[] units;
	
	// ANIMATION VARS:
	Animator anim;
	
	void Start(){
		// When this unit is spawned it sets its own coordinates
		posX =(int)transform.position.x;
		posY = (int)transform.position.y;
		
		anim = GetComponent<Animator> ();
		anim.SetTrigger ("idle");
	}
	
	void OnMouseOver(){
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log(gameObject.name + " Unit selected!");
			// Player has clicked on this Unit, so tell the Grid that
			// THIS is the unit to Path
			if (resourceGrid != null){
				resourceGrid.unitOnPath = gameObject;
			}
			isSelected = true;
		}
	}
	
	void Update () {
		
		// I need to change this later to the script that handles all Player clicking/controls,
		// instead of here - a script that should only control a selected Unit's movement.
		
		
		if (isSelected && Input.GetMouseButtonDown (1)) {
			// If this unit is selected and Player left clicks
			// Store the mouse position in world coords
			Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mX = Mathf.RoundToInt(m.x);
			mY = Mathf.RoundToInt(m.y);
			// Making sure that we are not trying to path outside the BOUNDARIES of the GRID
			if (mX > resourceGrid.mapSizeX - 1){
				mX = resourceGrid.mapSizeX - 1;
			}
			if (mY > resourceGrid.mapSizeY -1){
				mY = resourceGrid.mapSizeY - 1;
			}
			if (mX < 0){
				mX = 0;
			}
			if (mY < 0){
				mY = 0;
			}
			
			// give resource grid this gameobject as unit to path
			resourceGrid.unitOnPath = gameObject;
			// and generate a path using mouse coords ROUNDED as int
			resourceGrid.GenerateWalkPath(mX, mY, true, posX, posY);
			// THIS WILL NOT WORK if ACTUAL tiles BOTTOM LEFT is NOT 0.0!!
			moving = true;
			isSelected = false;
		}
		
		if (currentPath != null) {
			int currNode = 0;
			while (currNode < currentPath.Count -1) {
				Vector3 start = resourceGrid.TileCoordToWorldCoord (currentPath [currNode].x, currentPath [currNode].y);
				Vector3 end = resourceGrid.TileCoordToWorldCoord (currentPath [currNode + 1].x, currentPath [currNode + 1].y);
				;
				Debug.DrawLine (start, end, Color.red);
				currNode++;
			}
		} 
		if (moving) {
			// Have we moved close enough to the target tile that we can move to next tile in current path?
			if (Vector2.Distance (transform.position, resourceGrid.TileCoordToWorldCoord (posX, posY)) < 1f) {
				MoveToNextTile ();
			} 
			if (Vector2.Distance (transform.position, resourceGrid.TileCoordToWorldCoord (mX, mY)) < 0.1f) {
				movingToAttack = false;
				moving = false;
				anim.SetTrigger ("idle");
				anim.ResetTrigger ("movingRight");
				anim.ResetTrigger ("movingLeft");
				anim.ResetTrigger ("attackRight");
				anim.ResetTrigger ("attackLeft");
			} else {
				if (mX > transform.position.x) {
					if (movingToAttack) {
						anim.SetTrigger ("attackRight");
						anim.ResetTrigger("movingRight");
						anim.ResetTrigger("idle");
					} else {
						anim.SetTrigger ("movingRight");
						anim.ResetTrigger("idle");
						anim.ResetTrigger("movingLeft");
						anim.ResetTrigger("attackRight");
					}
				}
				if (mX < transform.position.x) {
					if (movingToAttack) {
						anim.SetTrigger ("attackLeft");
						anim.ResetTrigger("movingLeft");
						anim.ResetTrigger("idle");
					} else {
						anim.SetTrigger ("movingLeft");
						anim.ResetTrigger("idle");
						anim.ResetTrigger("movingRight");
						anim.ResetTrigger("attackLeft");
					}
				}
				// DO ANIMATION CALCULATION HERE:
				//				anim.SetTrigger ("moving");
				//				anim.ResetTrigger("idle");
			}
			
			if (movingToAttack) {
				transform.position = Vector2.MoveTowards (transform.position,
				                                          new Vector2 (mX, mY),
				                                          movementSpeed * Time.deltaTime);
			} else {
				// This MOVEMENT will animate towards the next position
				transform.position = Vector2.MoveTowards (transform.position,
				                                          resourceGrid.TileCoordToWorldCoord (posX, posY),
				                                          movementSpeed * Time.deltaTime);
			}
		} 
	}
	
	
	//	void ClickToDiscover(){
	//		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
	//		Vector3 mouseRounded = new Vector3 (Mathf.Round (m.x), Mathf.Round (m.y), 0.0f);
	//		
	//		
	//		resourceGrid.DiscoverTile ((int)mouseRounded.x,(int) mouseRounded.y);
	//		
	//	}
	//	void LateUpdate(){
	//		transform.position = new Vector3(Mathf.Lerp(transform.position.x, resourceGrid.TileCoordToWorldCoord (posX, posY).x, movementSpeed * Time.time),
	//		                                 Mathf.Lerp(transform.position.x, resourceGrid.TileCoordToWorldCoord (posX, posY).y, movementSpeed * Time.time) , 0);
	//	}
	
	
	public void MoveToNextTile(){
		if (currentPath == null) {
			
			return;
		}
		// Remove the old first node from the path
		currentPath.RemoveAt (0);
		
		if (resourceGrid.UnitCanEnterTile (currentPath [0].x, currentPath [0].y) == false) {
			Debug.Log (gameObject.name + "'s path is BLOCKED!");
			moving = false;
			anim.SetTrigger("idle");
			anim.ResetTrigger("movingRight");
			anim.ResetTrigger("movingLeft");
			currentPath = null;
			return;
		}
		
		
		//		// Check if the next tile is a UNWAKABLE tile, if it is: DISCOVER TILE & clear path
		//		if (resourceGrid.UnitCanEnterTile (currentPath [0].x, currentPath [0].y) == true) {
		//			resourceGrid.DiscoverTile (currentPath [0].x, currentPath [0].y, false);
		//		} else {
		//			resourceGrid.DiscoverTile(currentPath[0].x, currentPath[0].y, false);
		//			// check again, now that it has been discovered
		//			if (resourceGrid.UnitCanEnterTile (currentPath [0].x, currentPath [0].y) == false){
		//				currentPath = null;
		//				return;
		//			}
		//		}	
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
			// This unit has finished walking its path so it no longer has to be selected
			isSelected = false;
			
		}
	}
}
