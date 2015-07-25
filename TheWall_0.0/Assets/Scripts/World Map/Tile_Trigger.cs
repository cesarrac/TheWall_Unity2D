using UnityEngine;
using System.Collections;

public class Tile_Trigger : MonoBehaviour {
	// THIS SCRIPT IS ON RESOURCE TILES
	// It detects when a Horde enters this collider, gives the Horde access to my Collider
	// then lets the Horde turn it back on when it dies or moves

	BoxCollider2D myColl;

	void Awake(){
		myColl = GetComponent<BoxCollider2D> ();
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.tag == "Badge") {
			Horde horde = coll.gameObject.GetComponent<Horde>();
			horde.tileColl = myColl;
		}
	}



}
