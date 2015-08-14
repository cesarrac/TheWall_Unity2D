using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType {

	public enum Types{
		grain,
		wood,
		stone,
		metal,
		empty
	}

	public Types tileType;

	[HideInInspector]
	public bool hasBeenSpawned;

	public int maxResourceQuantity;
	public Vector3 gridPosition;
	public int movementCost = 1;
	public GameObject tileAsGObj;

	public bool isWalkable = true;

}
