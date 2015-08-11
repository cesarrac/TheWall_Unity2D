using UnityEngine;
using System.Collections;

[System.Serializable]
public class Building : MonoBehaviour {
	// THIS IS THE BASE FOR ALL BUILDINGS

	// Here let's hold the bonus the building provides and the penalty
	// there has to be 2 available bonus variables because in some cases (Defense) they are floats
	public int intBonus1, intBonus2;
	public float floatBonus1, floatBonus2, floatBonus3, floatBonus4, floatBonus5, floatBonus6;
	public int intPenalty1;
	public float floatPenalty1;

	// The variables affected by these bonuses are in either TownTileProperties or Town Central scripts
	public Town_Central townCentral;
	public TownTile_Properties townTProps;

	public enum BuildMaterialType{
		basic,
		stone,
		metal
	}
	public BuildMaterialType myBuildMaterial;

	public enum BuildingType
	{
		workshop,
		house,
		defense,
		weapon,
		food
	}
	public BuildingType myBuildingType;
	// Method in each building that is called when town tile is destroyed

	// Different constructors for different # of bonuses and pelalties

//	public void SubtractBonuses(){
//		if (intBonus1 > 0 && floatPenalty1 > 0 && floatBonus1 == 0) { // It's a house
//			townCentral.survivorVacancies = townCentral.survivorVacancies - intBonus1;
//			townTProps.tileHitPoints = townTProps.tileHitPoints + floatPenalty1;
//		} else if (floatBonus1 > 0 && intBonus1 == 0) { // Defense
//			townTProps.tileHitPoints = townTProps.tileHitPoints - floatBonus1;
//		}
//	}

	public void SubtractBonuses(BuildingType myType){
		switch (myType) {
		case BuildingType.defense:
			townTProps.tileHitPoints = townTProps.tileHitPoints - floatBonus1;
			townTProps.defenseRating = townTProps.defenseRating - intBonus1;
			break;
		case BuildingType.weapon:
			townTProps.baseDamage = townTProps.baseDamage - floatBonus1;
			townTProps.attackRating = townTProps.attackRating - intBonus1;
			break;
		case BuildingType.house:
//			townCentral.survivorVacancies = townCentral.survivorVacancies - intBonus1;
			townTProps.tileHitPoints = townTProps.tileHitPoints + floatPenalty1;
			break;
		case BuildingType.workshop:
			townCentral.gatherTimeWood = townCentral.gatherTimeWood - floatBonus1;
			townCentral.gatherTimeStone = townCentral.gatherTimeStone - floatBonus2;
			townCentral.gatherTimeMetal = townCentral.gatherTimeMetal - floatBonus3;
			townCentral.gatherTimeGrain = townCentral.gatherTimeGrain - floatBonus4;
			townCentral.gatherTimeGen = townCentral.gatherTimeGen - floatBonus5;

			townCentral.gatherAmntGen = townCentral.gatherAmntGen - intBonus2;

			townCentral.maxGatherers = townCentral.maxGatherers - intBonus1;
			break;
		default:
			print ("Building has no bonus!");
			break;
		}
	}


}
