using UnityEngine;
using System.Collections;

[System.Serializable]
public class Defense : Building {

	// **** Using this script on every object that boosts a tile defenses, all it REQUIRES is to specify
							// the TYPE OF DEFENSE this is in the Editor


	// how much HP to boost, determined by the defense type below
	float hitPointsBoost;

	void Start () {
		if (townCentral == null) {
			townCentral = GameObject.FindGameObjectWithTag ("Town_Central").GetComponent<Town_Central> ();
		}
		if (townTProps == null) {
			townTProps = GetComponentInParent<TownTile_Properties> ();
		}
		
		if (townCentral != null && townTProps != null) {
			myBuildMaterial = myBuildMaterial;
			BoostTileHitPoints(myBuildMaterial);
		}
	}

	void BoostTileHitPoints(BuildMaterialType type){
		switch (type) {
		case BuildMaterialType.basic:
			hitPointsBoost = 10f;
			floatBonus1 = hitPointsBoost;
			townTProps.tileHitPoints = townTProps.tileHitPoints + hitPointsBoost;
			break;
		case BuildMaterialType.stone:
			hitPointsBoost = 20f;
			floatBonus1 = hitPointsBoost;
			townTProps.tileHitPoints = townTProps.tileHitPoints + hitPointsBoost;
			break;
		case BuildMaterialType.metal:
			hitPointsBoost = 30f;
			floatBonus1 = hitPointsBoost;
			townTProps.tileHitPoints = townTProps.tileHitPoints + hitPointsBoost;
			break;
		default:
			hitPointsBoost = 10f;
			floatBonus1 = hitPointsBoost;
			townTProps.tileHitPoints = townTProps.tileHitPoints + hitPointsBoost;
			break;
		}

	}


	
}
