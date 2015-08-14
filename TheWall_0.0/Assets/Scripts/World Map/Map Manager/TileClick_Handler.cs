using UnityEngine;
using System.Collections;

public class TileClick_Handler : MonoBehaviour {

	public int mapPosX;	// when the map instantiates this tile it feeds it its map coordinates
	public int mapPosY;


	public ResourceGrid resourceGrid;

	public PlayerControls selectedUnit; // this is the Player's selected Unit that will move

	public bool isWalkable = true;

	void Start () {
	
	}

	void OnMouseUpAsButton() {
	
		if (resourceGrid != null) {
			resourceGrid.GenerateWalkPath(mapPosX, mapPosY);
		}
	}
}
