using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wall_Targets : MonoBehaviour {
	public List<GameObject> wallTargets = new List<GameObject>();

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	// when an enemy unit enters the wall's collider, they get added to a list of wall targets
	void OnTriggerEnter2D(Collider2D coll){
		if (coll.tag == "Enemy") {
			wallTargets.Add(coll.gameObject);
			print (coll.gameObject.name + " is now a wall target.");
			print ("Target count: " + wallTargets.Count);
		}
	}

	// when an enemy exits the collider they are no longer a wall target
	void OnTriggerExit2D(Collider2D coll){
		if (coll.tag == "Enemy") {
			for (int x = 0; x < wallTargets.Count; x++){
				if (wallTargets[x] == coll.gameObject){
					wallTargets.RemoveAt(x);
				}
			}
			print ("Target count: " + wallTargets.Count);
		}
	}


}
