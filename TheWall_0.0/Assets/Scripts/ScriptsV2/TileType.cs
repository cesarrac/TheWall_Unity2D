using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType {

	public enum Types{
		grain,
		wood,
		stone,
		metal,
		buildable,
		empty
	}

	public Types tileType;


	public bool hasBeenSpawned = false;

	public int maxResourceQuantity;
	public Vector3 gridPosition;
	public int movementCost = 1;
	public GameObject tileAsGObj;

	public bool isWalkable = true;

}
