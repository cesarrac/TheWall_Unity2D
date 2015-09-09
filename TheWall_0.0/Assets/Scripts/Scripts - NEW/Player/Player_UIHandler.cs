using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player_UIHandler : MonoBehaviour {
	// Resources Panel 
	public Text oreText, foodText, foodCostText, creditsText, waterText;
	int ore, food, credits, totalFood, farmCount, water, extractCount, waterPumpCount;

	// Food Production Panel
	public Text foodProd_txt, farmCount_txt, foodCost_txt;

	// Ore & Water Production Panel
	public Text oreProd_txt, extractCount_txt, waterProd_txt, waterCount_txt, waterCost_txt;

	// Storage Info Panel
	public Text storageName_txt, oreStored_txt, oreCap_txt, waterStored_txt, waterCap_txt;


	public Player_ResourceManager resourceManager;

	void Start () {
		if (resourceManager == null) {
			resourceManager = GetComponent<Player_ResourceManager>();
		}
		GetResourcesText ();
		foodCostText.color = Color.red;
	}

	void GetResourcesText(){
		if (resourceManager.food < resourceManager.totalFoodCost) {
			foodText.color = Color.red;
		} else {
			foodText.color = Color.green;
		}

		waterText.color = Color.blue;

		oreText.text = "Ore: " + resourceManager.ore.ToString();
		foodText.text = "Food: " + resourceManager.food.ToString ();
		creditsText.text = "Credits: " + resourceManager.credits.ToString();
		waterText.text = "Water: " + resourceManager.water.ToString();

		ore = resourceManager.ore;
		food=resourceManager.food;
		credits =resourceManager.credits;
		totalFood = resourceManager.totalFoodCost;
		water =resourceManager.water;

		if (resourceManager.totalFoodCost > 0) {
			foodCostText.gameObject.SetActive(true);
			foodCostText.text = "/ - " + resourceManager.totalFoodCost.ToString ();
		} else {
			foodCostText.gameObject.SetActive(false);
		}

		// food production panel
		farmCount_txt.text = resourceManager.farmCount.ToString ();
		foodCost_txt.text = resourceManager.totalFoodCost.ToString ();
		foodProd_txt.text = resourceManager.foodProducedPerDay.ToString ();
		farmCount = resourceManager.farmCount;

		// ore and water production panel
		extractCount_txt.text = resourceManager.extractorCount.ToString ();
		oreProd_txt.text = resourceManager.oreExtractedPerDay.ToString ();

		waterCount_txt.text = resourceManager.waterPumpCount.ToString ();
		waterProd_txt.text = resourceManager.waterProducedPerDay.ToString ();
		waterCost_txt.text = resourceManager.totalWaterCost.ToString ();
		extractCount = resourceManager.extractorCount;
		waterPumpCount = resourceManager.waterPumpCount;

	}
	

	void Update () {
		if (resourceManager.ore != ore || resourceManager.food != food || resourceManager.credits != credits 
		    || resourceManager.totalFoodCost != totalFood || resourceManager.farmCount != farmCount || resourceManager.water != water
		    || resourceManager.extractorCount != extractCount || resourceManager.waterPumpCount != waterPumpCount) {
			GetResourcesText();
		}
	}

	public void DisplayStorageInfo(Storage storageSelected){
		if (storageSelected != null) {
			oreStored_txt.text = storageSelected.oreStored.ToString();
			oreCap_txt.text = "/ " + storageSelected.oreCapacity.ToString();

			waterStored_txt.text = storageSelected.waterStored.ToString();
			waterCap_txt.text = "/ " + storageSelected.waterCapacity.ToString();
		}
	}
}
