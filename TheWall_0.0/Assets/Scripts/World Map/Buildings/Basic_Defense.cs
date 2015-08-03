using UnityEngine;
using System.Collections;

public class Basic_Defense : Building {

	public float hitPointsBoost = 10f;


	TownTile_Properties townTileProps;

	// Use this for initialization
	void Start () {
		
		townTileProps = GetComponentInParent<TownTile_Properties> ();
		townTileProps.tileHitPoints = townTileProps.tileHitPoints + hitPointsBoost;
	}
	

}
