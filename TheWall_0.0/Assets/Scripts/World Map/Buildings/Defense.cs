using UnityEngine;
using System.Collections;

[System.Serializable]
public class Defense : MonoBehaviour {

	// **** Using this script on every object that boosts a tile defenses, all it REQUIRES is to specify
							// the TYPE OF DEFENSE this is in the Editor


	// how much HP to boost, determined by the defense type below
	float hitPointsBoost;
	
	// access to Town Properties to boost its HP
	TownTile_Properties townTileProps;

	// store and view in the Editor what type of Defense this is (Basic, Stone or Metal)
	public DefenseType myDefenseType;

	public enum DefenseType
	{
		basic,
		stone,
		metal
	}

	void Start () {
		
		townTileProps = GetComponentInParent<TownTile_Properties> ();
//		townTileProps.tileHitPoints = townTileProps.tileHitPoints + hitPointsBoost;
		if (townTileProps != null) {
			myDefenseType = myDefenseType;
			BoostTileHitPoints(myDefenseType);
		}
	}

	void BoostTileHitPoints(DefenseType type){
		switch (type) {
		case DefenseType.basic:
			hitPointsBoost = 10f;
			townTileProps.tileHitPoints = townTileProps.tileHitPoints + hitPointsBoost;
			break;
		case DefenseType.stone:
			hitPointsBoost = 20f;
			townTileProps.tileHitPoints = townTileProps.tileHitPoints + hitPointsBoost;
			break;
		case DefenseType.metal:
			hitPointsBoost = 30f;
			townTileProps.tileHitPoints = townTileProps.tileHitPoints + hitPointsBoost;
			break;
		default:
			hitPointsBoost = 10f;
			townTileProps.tileHitPoints = townTileProps.tileHitPoints + hitPointsBoost;
			break;
		}

	}
	
	
}
