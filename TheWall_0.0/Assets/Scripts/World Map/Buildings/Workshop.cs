using UnityEngine;
using System.Collections;

[System.Serializable]
public class Workshop : Building {

// 			**************************** BASE WORKSHOP *****************
	public int maxGatherers; // gatherers to add
	public int genGatherAmmt;
	// 				Survivors and other Upgrades can make specific resource gather times go faster, for others using General
	public float woodGatherT, stoneGatherT, metalGatherT, grainGatherT, genGatherT;

	// Use this for initialization
	void Start () {
		if (townCentral == null) {
			townCentral = GameObject.FindGameObjectWithTag ("Town_Central").GetComponent<Town_Central> ();
			myBuildMaterial = myBuildMaterial;
			ApplyBonus(myBuildMaterial);
		}
	}
	
	void ApplyBonus(BuildMaterialType type){
		switch (type) {
		case BuildMaterialType.basic:
			maxGatherers = 1;
			genGatherAmmt = 2;
			genGatherT = -0.3f;
			intBonus1 = maxGatherers;
			intBonus2 = genGatherAmmt;
			floatBonus5 = genGatherT;
			// fill vars
			townCentral.maxGatherers = townCentral.maxGatherers + maxGatherers;
			townCentral.gatherAmntGen = townCentral.gatherAmntGen + genGatherAmmt;
			townCentral.gatherTimeGen = townCentral.gatherTimeGen + genGatherT;
			// fill special upgrades
			if (woodGatherT > 0 || stoneGatherT > 0 || metalGatherT > 0 || grainGatherT > 0){
				ApplySpecialBonuses();
			}
			break;
		case BuildMaterialType.stone:
			maxGatherers = 1;
			genGatherAmmt = 2;
			genGatherT = -1f;
			intBonus1 = maxGatherers;
			intBonus2 = genGatherAmmt;
			floatBonus5 = genGatherT;
			townCentral.maxGatherers = townCentral.maxGatherers + maxGatherers;
			townCentral.gatherAmntGen = townCentral.gatherAmntGen + genGatherAmmt;
			townCentral.gatherTimeGen = townCentral.gatherTimeGen + genGatherT;
			// fill special upgrades
			if (woodGatherT > 0 || stoneGatherT > 0 || metalGatherT > 0 || grainGatherT > 0){
				ApplySpecialBonuses();
			}
			break;
		case BuildMaterialType.metal:
			maxGatherers = 2;
			genGatherAmmt = 2;
			genGatherT = -2f;
			intBonus1 = maxGatherers;
			intBonus2 = genGatherAmmt;
			floatBonus5 = genGatherT;
			townCentral.maxGatherers = townCentral.maxGatherers + maxGatherers;
			townCentral.gatherAmntGen = townCentral.gatherAmntGen + genGatherAmmt;
			townCentral.gatherTimeGen = townCentral.gatherTimeGen + genGatherT;
			// fill special upgrades
			if (woodGatherT > 0 || stoneGatherT > 0 || metalGatherT > 0 || grainGatherT > 0){
				ApplySpecialBonuses();
			}
			break;
		default:
			maxGatherers = 1;
			genGatherAmmt = 2;
			genGatherT = -0.3f;
			intBonus1 = maxGatherers;
			intBonus2 = genGatherAmmt;
			floatBonus5 = genGatherT;
			townCentral.maxGatherers = townCentral.maxGatherers + maxGatherers;
			townCentral.gatherAmntGen = townCentral.gatherAmntGen + genGatherAmmt;
			townCentral.gatherTimeGen = townCentral.gatherTimeGen + genGatherT;
			// fill special upgrades
			if (woodGatherT > 0 || stoneGatherT > 0 || metalGatherT > 0 || grainGatherT > 0){
				ApplySpecialBonuses();
			}
			break;
		}
		
	}

	void ApplySpecialBonuses(){
		// fill special upgrades
		floatBonus1 = woodGatherT;
		floatBonus2 = stoneGatherT;
		floatBonus3 = metalGatherT;
		floatBonus4 = grainGatherT;
		townCentral.gatherTimeWood = townCentral.gatherTimeWood + woodGatherT;
		townCentral.gatherTimeStone = townCentral.gatherTimeStone + stoneGatherT;
		townCentral.gatherTimeMetal = townCentral.gatherTimeMetal + metalGatherT;
		townCentral.gatherTimeGrain = townCentral.gatherTimeGrain + grainGatherT;
	}
}
