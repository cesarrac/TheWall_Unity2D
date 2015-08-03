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
		
			if (!horde.nextToTownTile) {
				horde.nextToTownTile = true;
				horde.townTile = townTile; // gives the horde the proper tile to do damage to.
				townTile.beingAttacked = true;
			}
			
		}

		if (coll.CompareTag ("Survivor")) {
			Survivor survivor = coll.gameObject.GetComponent<Survivor>();
			if(!survivor.partOfTown){
				survivor.Talk("Welcome"); // Only call Welcome when survivor is not part of town
			}
		}
	}

	void OnTriggerStay2D(Collider2D coll){
		if (coll.CompareTag ("Badge")) {
			horde = coll.GetComponent<Horde> ();
			
			if (!horde.nextToTownTile) {
				horde.nextToTownTile = true;
				horde.townTile = townTile; // gives the horde the proper tile to do damage to.
				townTile.beingAttacked = true;
			}
		}
	}
	void OnTriggerExit2D (Collider2D coll){
		if (coll.CompareTag ("Badge")) {
			horde = coll.GetComponent<Horde>();
			horde.nextToTownTile = false;
			townTile.beingAttacked = false;
		}

	}

}
