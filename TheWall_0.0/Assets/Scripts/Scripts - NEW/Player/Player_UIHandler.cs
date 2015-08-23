using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player_UIHandler : MonoBehaviour {
	public Text oreText, foodText, foodCostText, creditsText;
	int ore, food, credits, totalFood;
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
		oreText.text = "Ore: " + resourceManager.ore.ToString();
		foodText.text = "Food: " + resourceManager.food.ToString ();
		creditsText.text = "Credits: " + resourceManager.credits.ToString();
		ore = resourceManager.ore;
		food=resourceManager.food;
		credits =resourceManager.credits;
		totalFood = resourceManager.totalFoodCost;
		if (resourceManager.totalFoodCost > 0) {
			foodCostText.text = "/ - " + resourceManager.totalFoodCost.ToString ();
		}

	}
	

	void Update () {
		if (resourceManager.ore != ore || resourceManager.food != food || resourceManager.credits != credits || resourceManager.totalFoodCost != totalFood) {
			GetResourcesText();
		}
	}
}
