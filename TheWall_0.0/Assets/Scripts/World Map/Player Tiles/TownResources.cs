using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TownResources : MonoBehaviour {
	public int wood;
	public int metal;
	public int grain;
	public int stone;
	public float xp = 2;

	// rate at which xp is gained each turn
	public float xpGainRate = 1;

	//Text components to display resource ammounts
	public Text woodText;
	public Text metalText;
	public Text grainText;
	public Text stoneText;
	public Text xpText;


	void Update(){
		woodText.text = "WOOD: " + wood;
		metalText.text = "METAL: " + metal;
		grainText.text = "GRAIN: " + grain;
		stoneText.text = "STONE: " + stone;
		xpText.text = "XP: " + xp;
	}

	public void AddResource (string id, int quantityToAdd){
		switch (id) {
		case "wood":
			wood = wood + quantityToAdd;
			break;
		case "metal":
			metal = metal + quantityToAdd;
			break;
		case "grain":
			grain = grain + quantityToAdd;
			break;
		case "stone":
			stone = stone + quantityToAdd;
			break;
		case "xp":
			xp = xp + (float)quantityToAdd;
			break;
		default:
			print ("Cant find that resource type!");
			break;
		}
	}
}
