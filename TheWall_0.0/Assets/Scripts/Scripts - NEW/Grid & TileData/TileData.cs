using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileData  {
	
	public enum Types{
		rock,
		empty,
		water,
		building,
		capital,
		farm_s,
		nutrient,
		extractor,
		desalt_s,
		desalt_m,
		desalt_l,
		house,
		seaWitch,
		cannons,
		machine_gun,
		harpoonHall,
		storage,
		sniper
	}
	public Types tileType;

	public bool hasBeenSpawned = false;
	
	public int maxResourceQuantity;

	public int movementCost = 1;

//	public GameObject tileAsGObj;
	
	public bool isWalkable = true;

	public float hp, def, attk, shield; 

	public string tileName;

	public int foodCost, oreCost;

	public TileData(string name, Types type, int resourceQuantity, int moveCost, float _hp, float _defence, float _attk, float _shield, int fCost, int oCost){
		tileType = type;
		maxResourceQuantity = resourceQuantity;
		movementCost = moveCost;

		if (type != Types.rock && type != Types.empty) {
			isWalkable = false;
		}
		hp = _hp;
		def = _defence;
		attk = _attk;
		shield = _shield;
		tileName = name;
		foodCost = fCost;
		oreCost = oCost;
	}
	public TileData(Types type, int resourceQuantity, int moveCost){
		tileType = type;
		maxResourceQuantity = resourceQuantity;
		movementCost = moveCost;

		if (type != Types.rock && type != Types.empty) {
			isWalkable = false;
		}
		tileName = type.ToString ();
	}
}
