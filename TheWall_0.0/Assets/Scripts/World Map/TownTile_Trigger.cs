using UnityEngine;
using System.Collections;

public class TownTile_Trigger : MonoBehaviour {

	Horde horde;
	
	//fill this up in awake to make sure
	TownTile_HP townTile;

	// true if it has a building on it
	public bool tileHasBuilding;
	
	void Awake(){
		townTile = GetComponent<TownTile_HP> ();
	}
	
	
	void OnTriggerEnter2D (Collider2D coll){
		if (coll.CompareTag ("Badge")) {
			horde = coll.GetComponent<Horde>();
			horde.nextToTownTile = true;
			horde.townTile = townTile; // gives the horde the proper tile to do damage to.
		}
	}
	void OnTriggerExit2D (Collider2D coll){
		if (coll.CompareTag ("Badge")) {
			horde = coll.GetComponent<Horde> ();
			horde.nextToTownTile = false;
		} else if (coll.CompareTag ("Building")) {
			tileHasBuilding = false;
		}
	}

	void OnTriggerStay2D(Collider2D coll){
		if (coll.CompareTag ("Building")) {
			tileHasBuilding = true;
		}
	}
}
