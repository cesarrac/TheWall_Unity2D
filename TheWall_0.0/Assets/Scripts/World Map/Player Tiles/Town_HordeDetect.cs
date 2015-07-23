using UnityEngine;
using System.Collections;

public class Town_HordeDetect : MonoBehaviour {

	Horde horde;

	//fill this up in awake to make sure
	TownTile_Properties townTile;

	void Awake(){
		townTile = GetComponent<TownTile_Properties> ();
	}


	void OnTriggerEnter2D (Collider2D coll){
		if (coll.CompareTag ("Badge")) {
			horde = coll.GetComponent<Horde> ();
			horde.nextToTownTile = true;
			horde.townTile = townTile; // gives the horde the proper tile to do damage to.
			townTile.beingAttacked = true;
		} else {
			townTile.beingAttacked = false;
		}
	}
	void OnTriggerExit2D (Collider2D coll){
		if (coll.CompareTag ("Badge")) {
			horde = coll.GetComponent<Horde>();
			horde.nextToTownTile = false;
			townTile.beingAttacked = false;
		}
		else {
			townTile.beingAttacked = false;
		}
	}

}
