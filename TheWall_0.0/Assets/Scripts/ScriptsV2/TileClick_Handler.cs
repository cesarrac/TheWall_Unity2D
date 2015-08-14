using UnityEngine;
using System.Collections;

public class TileClick_Handler : MonoBehaviour {

	public int mapPosX;	// when the map instantiates this tile it feeds it its map coordinates
	public int mapPosY;


	public ResourceGrid resourceGrid;

	public GameObject playerUnit; // this is the Player's selected Unit that will move

	public bool isWalkable = true;

	void Start () {
	
	}

//	void OnMouseUpAsButton() {
//	
//		if (resourceGrid != null) {
//			// The Selected Unit Move Handler on the unit gameobject feeds Grid with
//			// the unit to path
//			// BUT! IN CASE THE OBJECT IS NOT A PLAYER UNIT we are just NOT going to call the generate walk path
////			if (resourceGrid.unitOnPath.CompareTag("Survivor"))
////				resourceGrid.GenerateWalkPath(mapPosX, mapPosY, true);
//
//		}
//	}
}
