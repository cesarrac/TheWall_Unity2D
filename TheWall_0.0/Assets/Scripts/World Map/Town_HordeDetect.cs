using UnityEngine;
using System.Collections;

public class Town_HordeDetect : MonoBehaviour {

	Horde horde;

	void OnTriggerEnter2D (Collider2D coll){
		if (coll.CompareTag ("Badge")) {
			horde = coll.GetComponent<Horde>();
			horde.nextToTownTile = true;
		}
	}
	void OnTriggerExit2D (Collider2D coll){
		if (coll.CompareTag ("Badge")) {
			horde = coll.GetComponent<Horde>();
			horde.nextToTownTile = false;
		}
	}

}
