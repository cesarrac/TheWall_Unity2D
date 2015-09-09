using UnityEngine;
using System.Collections;

public class Gatherer_House : Building {
	// *****this building will add more Gatherers and boost their gather time slightly****
	// EDIT: ^^^ (07/31/2015) Now Houses will provide space for survivors. With each upgrade to a house the more
	// comfort it will provide to the survivor living there.
	// BUILDING BASE CLASS WILL HOLD THE SUBSTRACT BONUS METHOD

	public int maxHabitants = 2;
//	public float timeBoost = -0.3f;

	//access Town Central
	Town_Central townCentral;

	//access to the Town Tile this building is on to modify its HP
	public float hpLoss = 5f;
	TownTile_Properties townTileProps;

	void Start () {
		townCentral = GameObject.FindGameObjectWithTag ("Town_Central").GetComponent<Town_Central> ();
		// boost the gather timer
//		townCentral.gatherTimeBoost = townCentral.gatherTimeBoost + timeBoost;

		// add habitants to Survivor Vacancies
//		townCentral.survivorVacancies = townCentral.survivorVacancies + maxHabitants;
//		townCentral.maxGatherers = townCentral.maxGatherers + maxHabitants;

		// decrease this town tile's HP because house's lower their defenses
		townTileProps = GetComponentInParent<TownTile_Properties> ();
		townTileProps.tileHitPoints = townTileProps.tileHitPoints - hpLoss;
	}



}
