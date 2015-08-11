using UnityEngine;
using System.Collections;

[System.Serializable]
public class House : Building {
				// *******					BASE HOUSE 					****************
	//Houses will provide space for survivors. With each upgrade to a house the more
	// comfort it will provide to the survivor living there.

	public int maxHabitants;
	public float hpLoss;

	void Start () {

		if (townCentral == null || townTProps == null) {
			townCentral = GameObject.FindGameObjectWithTag ("Town_Central").GetComponent<Town_Central> ();
			townTProps = GetComponentInParent<TownTile_Properties> ();
			myBuildMaterial = myBuildMaterial;
			ApplyBonus(myBuildMaterial);
		}
	}

	// NOW HOUSES ADD TO HOUSING NOT VACANCY(this will make survivors more comfortable)
	void ApplyBonus(BuildMaterialType type){
		switch (type) {
		case BuildMaterialType.basic:
			Debug.Log("Adding House bonuses!!");
			maxHabitants = 1;
			intBonus1 = maxHabitants;
			townCentral.housingCount = townCentral.housingCount + maxHabitants;
			hpLoss = 5f;
			floatPenalty1 = hpLoss;
			townTProps.tileHitPoints = townTProps.tileHitPoints - hpLoss;
			break;
		case BuildMaterialType.stone:
			maxHabitants = 2;
			intBonus1 = maxHabitants;
			townCentral.housingCount = townCentral.housingCount + maxHabitants;
			hpLoss = 3f;
			floatPenalty1 = hpLoss;
			townTProps.tileHitPoints = townTProps.tileHitPoints - hpLoss;			
			break;
		case BuildMaterialType.metal:
			maxHabitants = 3;
			intBonus1 = maxHabitants;
			townCentral.housingCount = townCentral.housingCount + maxHabitants;
			hpLoss = 1f;
			floatPenalty1 = hpLoss;
			townTProps.tileHitPoints = townTProps.tileHitPoints - hpLoss;			
			break;
		default:
			maxHabitants = 1;
			intBonus1 = maxHabitants;
			townCentral.housingCount = townCentral.housingCount + maxHabitants;
			hpLoss = 5f;
			floatPenalty1 = hpLoss;
			townTProps.tileHitPoints = townTProps.tileHitPoints - hpLoss;			
			break;
		}
		
	}
}
