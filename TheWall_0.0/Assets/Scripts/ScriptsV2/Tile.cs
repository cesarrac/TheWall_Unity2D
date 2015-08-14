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

	public enum tileType
	{
		grain,
		stone,
		wood,
		metal,
		empty
	}
	public tileType myType;

	public tileType rType (int resourceID){
		tileType type;
		switch (resourceID) {
		case 1:
			type = tileType.wood;
			break;
		case 2:
			type = tileType.grain;
			break;
		case 3:
			type = tileType.metal;
			break;
		case 4:
			type = tileType.stone;
			break;
		case 5:
			type = tileType.empty;
			break;
		default:
			type = tileType.empty;
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
		myType = rType (id);
		if (myType == tileType.empty)
			movementCost = 10000;
	}

	public bool hasBeenSpawned;
	public int movementCost = 1;

}
