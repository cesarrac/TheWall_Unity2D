using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player_UIHandler : MonoBehaviour {
	public Text oreText, foodText, creditsText;
	int ore, food, credits;
	public Player_ResourceManager resourceManager;

	void Start () {
		if (resourceManager == null) {
			resourceManager = GetComponent<Player_ResourceManager>();
		}
		GetResourcesText ();
	}

	void GetResourcesText(){
		oreText.text = "Ore: " + resourceManager.ore.ToString();
		foodText.text = "Food: " +resourceManager.food.ToString();
		creditsText.text = "Credits: " + resourceManager.credits.ToString();
		ore = resourceManager.ore;
		food=resourceManager.food;
		credits =resourceManager.credits;
	}
	

	void Update () {
		if (resourceManager.ore != ore || resourceManager.food != food || resourceManager.credits != credits) {
			GetResourcesText();
		}
	}
}
