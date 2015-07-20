using UnityEngine;
using System.Collections;

public class Town_HordeDetect : MonoBehaviour {

	Horde horde;

	//fill this up in awake to make sure
	TownTile_HP townTile;

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
			horde = coll.GetComponent<Horde>();
			horde.nextToTownTile = false;
		}
	}

}
