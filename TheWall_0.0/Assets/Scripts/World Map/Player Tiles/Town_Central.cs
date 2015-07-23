using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Town_Central : MonoBehaviour {

	// max and available number of Gatherers
	public int maxGatherers = 2;
	public int availableGatherers = 2;
	public int spawnedGatherers;
	// prefab Gatherer
	public GameObject gatherer;

	// Text displaying available gatherers
	public Text availableText;

	// Damages for tap hits (keeping it simple for now)
	public float shortRangeDamage;
	public float longRangeDamage;

	// A list of Gatherers that have been spawned
	public List<Gatherer> currentGatherers = new List<Gatherer>();

	// Boosts to Gatherers (from buildings or Upgrades
	public float gatherTimeBoost;
	public int gatherAmmntBoost;

	void Start () {
		availableGatherers = maxGatherers;
	}
	
	// Update is called once per frame
	void Update () {
		availableGatherers = maxGatherers;
		int calc = availableGatherers - spawnedGatherers;

		availableText.text = "Available: " + calc;

	}

	public void SpawnGatherer(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (availableGatherers > 0) {
			//Instantiate gatherer and stick it to the mouse position
			GameObject gathererToSpwn = Instantiate (gatherer, new Vector3 (Mathf.Round(m.x), Mathf.Round(m.y), -2f), Quaternion.identity) as GameObject;
			//take one out of available
			spawnedGatherers ++;

			Gatherer currGatherer = gathererToSpwn.GetComponent<Gatherer>();
			// give it any boosts available
			if (gatherAmmntBoost > 0 || gatherTimeBoost < 0){
				currGatherer.gatherAmmount = currGatherer.gatherAmmount + gatherAmmntBoost;
				currGatherer.gatherTime = currGatherer.gatherTime + gatherTimeBoost;

			}
			//add to list of current gatherers to store its info
		
		}

	}

}
