using UnityEngine;
using System.Collections;

public class FoodProduction_Manager : MonoBehaviour {
	/// <summary>
	/// Extracts x # of food each production cycle and adds it to the Player's resources. Will not start or continue
	/// to produce food if the player has no water in storage.
	/// </summary>

	public float productionRate;
	public int foodProduced;
	public int waterConsumed; // water Consumed every farming cycle in order to produce a harvest
	bool farming;

	public Player_ResourceManager resourceManager;

	public bool starvedMode; // MANIPULATED BY THE RESOURCE MANAGER

	bool foodStatsInitialized = false;

	void Start () {
		resourceManager = GameObject.FindGameObjectWithTag ("Capital").GetComponent<Player_ResourceManager> ();
		farming = true;

		// MAKE SURE THE PLAYER HAS WATER BEFORE ADDING THIS FARM'S PRODUCTION TO THE STATS
		if (resourceManager.water > 0) {
			// Tell the Resource Manager how much I produce per cycle
			resourceManager.CalculateFoodProduction (foodProduced, productionRate, waterConsumed, false);
			foodStatsInitialized = true;
		}
	}
	

	void Update () {
		if (!foodStatsInitialized) {
			if (resourceManager.water > 0){
				resourceManager.CalculateFoodProduction (foodProduced, productionRate, waterConsumed, false);
				foodStatsInitialized = true;
			}
		}

		if (farming && !starvedMode) {
			StartCoroutine(WaitToFarm());
		}
	}

	IEnumerator WaitToFarm(){
		farming = false;
		yield return new WaitForSeconds (productionRate);
		// Farms NEED WATER! Here we have to check with Resources to see if Player has enough water
		if (resourceManager.water >= waterConsumed) {
			Farm ();
		} else {
			// NOT enough water to farm!
			Debug.Log ("FOOD PRODUCTION: Not enough Water!");
			farming = true;
		}
	}

	void Farm(){
		Debug.Log ("FOOD PRODUCTION: Farming!");
		resourceManager.ChangeResource ("Water", -waterConsumed);
//		resourceManager.ChargeOreorWater ("Water", -waterConsumed);

		// then add the food
		resourceManager.ChangeResource("Food", foodProduced);

		farming = true;
	}

}
