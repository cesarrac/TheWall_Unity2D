using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Farm : Building {
	// FARM: When Placed will check resource tiles around itself. It adds grain tiles to an array of GameObjects.
	// For every Grain Tile by this farm a harvestAmmount will be gained by Town Resources
	public List<GameObject>grainTiles = new List<GameObject>();
	Transform myTransform;
	TownResources townResources;
	Map_Manager mapManager;


	void Awake () {
		myTransform = transform;
		GameObject town = GameObject.FindGameObjectWithTag ("Town_Central");
		townCentral = town.GetComponent<Town_Central> ();
		townResources = town.GetComponent<TownResources> ();
		mapManager = GameObject.FindGameObjectWithTag ("Map_Manager").GetComponent<Map_Manager> ();

		if (townCentral != null) {
			townCentral.farms.Add(this);
		}
	}
	
	void Update () {

	}


	public void Harvest(){
		if (grainTiles.Count > 0) {
			// add to town's food
			int foodSourceCount = SourceCheck();
			townResources.grain = townResources.grain + foodSourceCount;
			print ("Harvested " + foodSourceCount + " food!!");
		}
	}
	int SourceCheck(){
		for (int x = 0; x < grainTiles.Count; x++) {
			if (grainTiles[x] == null){
				grainTiles.RemoveAt(x);
			}
		}
		return grainTiles.Count;
	}


	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Food Source")) {
			grainTiles.Add(coll.gameObject);
		}
	}
}
