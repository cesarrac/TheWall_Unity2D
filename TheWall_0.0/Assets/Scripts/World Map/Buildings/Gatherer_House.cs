using UnityEngine;
using System.Collections;

public class Gatherer_House : MonoBehaviour {
	public int maxHabitants = 2;
	public float timeBoost = -0.3f;
	// this building will add more Gatherers and boost their gather time slightly

	//access Town Central
	Town_Central townCentral;

	//access to the Town Tile this building is on to modify its HP
	public float hpLoss = 5f;
	TownTile_Properties townTileProps;

	void Start () {
		townCentral = GameObject.FindGameObjectWithTag ("Town_Central").GetComponent<Town_Central> ();
		// boost the gather timer
		townCentral.gatherTimeBoost = townCentral.gatherTimeBoost + timeBoost;
		// add habitants to max Gatherers
		townCentral.maxGatherers = townCentral.maxGatherers + maxHabitants;

		townTileProps = GetComponentInParent<TownTile_Properties> ();
		townTileProps.tileHitPoints = townTileProps.tileHitPoints - hpLoss;
	}

}
