using UnityEngine;
using System.Collections;

public class Building_ClickHandler : MonoBehaviour {

	// We'll eventually make it so that when Player clicks on a building a Panel with its options will
	// pop up. It'll have buttons for spawning different tiles and doing other cool stuff
	// For now im just going to use a temporary button on screen to spawn the first units

//	public Building myBuilding;

	public int mapPosX;
	public int mapPosY;
	public Building_UIHandler buildingUIhandler;
	public ResourceGrid resourceGrid;

	// UI Handler feeds this when this is a new building so it may Swap Tiles
	public TileData.Types tileType;

	// get the bounds of this collider to know where to place the options panel
	BoxCollider2D myCollider;
	float vertExtents;

	void Start () {
					// IF THIS BUILDING is spawned by the UI Handler, it won't need to make this search
		if (buildingUIhandler == null) {
			buildingUIhandler = GameObject.FindGameObjectWithTag ("UI").GetComponent<Building_UIHandler> ();
		}

		myCollider = GetComponent<BoxCollider2D> ();
		vertExtents = myCollider.bounds.extents.y;
	}

	void OnMouseUpAsButton(){
		Debug.Log("You clicked on " + gameObject.name);
		ActivateBuildingUI ();
	}

	public void ActivateBuildingUI(){
		Vector3 offset = new Vector3 (transform.position.x, transform.position.y + vertExtents);
		buildingUIhandler.CreateOptionsButtons (offset, CheckTileType(mapPosX, mapPosY), mapPosX, mapPosY);
		Debug.Log (transform.position);
	}

	TileData.Types CheckTileType(int x, int y){
		TileData.Types type = resourceGrid.tiles [x, y].tileType;
		return type;
	}
}
