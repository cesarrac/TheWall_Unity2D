using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TownResources : MonoBehaviour {
	public int ore;
	public int food;
	public int credits;

	// OLD RESOURCES SO THINGS DONT BREAK
	public int wood;
	public int metal;
	public int stone;
	public int xp;

	//Text components to display resource ammounts
	public Text oreText;
	public Text foodText;
	public Text creditText;


	void Update(){
		oreText.text = "ORE: " + ore;
		foodText.text = "FOOD: " + food;
		creditText.text = "CREDITS: " + credits;
	}

	public void AddResource (string id, int quantityToAdd){
		switch (id) {
		case "Ore":
			ore = ore + quantityToAdd;
			break;
		case "Food":
			food = food + quantityToAdd;
			break;
		case "Credits":
			credits = credits + quantityToAdd;
			break;
//		case "xp":
//			float xpCalc = xp + (float)quantityToAdd;
//			xp = Mathf.Round(xpCalc);
//			break;
		default:
			print ("Cant find that resource type!");
			break;
		}
	}
}
