using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tile {

	public GameObject tileGameObject;
	public string resourceType;
	public string type (int resourceID){
		string type;
		switch (resourceID) {
		case 1:
			type = "wood";
			break;
		case 2:
			type = "grain";
			break;
		case 3:
			type = "metal";
			break;
		case 4:
			type = "stone";
			break;
		case 5:
			type = "empty";
			break;
		default:
			type = "empty";
			break;
		}
		return type;
	}
//	public string resourceQuantityType;
	public int maxResourceQuantity;
	public Vector3 gridPosition;

	public Tile (int id, int maxQuantity, Vector3 position, GameObject tileGameObj){
		resourceType = type (id);
		maxResourceQuantity = maxQuantity;
		gridPosition = position;
		tileGameObject = tileGameObj;
	}

}
