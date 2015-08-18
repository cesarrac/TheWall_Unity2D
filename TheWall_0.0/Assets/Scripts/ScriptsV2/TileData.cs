using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileData  {
	
	public enum Types{
		rock,
		buildable,
		empty,
		building,
		capital,
		sfoodfactory,
		mfoodfactory,
		lfoodfactory,
		sfarm,
		mfarm,
		lfarm,
		sextractor,
		mextractor,
		lextractor,
		shouse,
		mhouse,
		lhouse
	}
	public Types tileType;

	public bool hasBeenSpawned = false;
	
	public int maxResourceQuantity;

	public int movementCost = 1;
	public GameObject tileAsGObj;
	
	public bool isWalkable = true;

	public float hp = 10, def = 0, attk = 0, shield = 0; 

	public TileData(Types type, int resourceQuantity, int moveCost, GameObject tileGameFab, float _hp, float _defence, float _attk, float _shield){
		tileType = type;
		maxResourceQuantity = resourceQuantity;
		movementCost = moveCost;
		tileAsGObj = tileGameFab;
		if (type != Types.rock && type != Types.empty) {
			isWalkable = false;
		}
		hp = _hp;
		def = _defence;
		attk = _attk;
		shield = _shield;
	}
	public TileData(Types type, int resourceQuantity, int moveCost, GameObject tileGameFab){
		tileType = type;
		maxResourceQuantity = resourceQuantity;
		movementCost = moveCost;
		tileAsGObj = tileGameFab;
		if (type != Types.rock && type != Types.empty) {
			isWalkable = false;
		}
	}
}
