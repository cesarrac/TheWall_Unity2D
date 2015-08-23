using UnityEngine;
using System.Collections;


public class Player_ResourceManager : MonoBehaviour {

	public int ore;
	public int food;
	public int credits;

	public int maxCitizenCount = 1;// the maximum citizens will increase with each level, bringing in new characters

	public int totalFoodCost; // this is the cost of food every turn/cycle. It gets added & subtracted by the resource grid whenever it 
													// Swaps tiles.

	public float feedingRate; // leaving this public for now but it can be set by the level the player is on
	bool feeding;

	void Start(){
		if (totalFoodCost > 0) {
			feeding = true;
		}
	}

	void Update(){
		if (totalFoodCost > 0) {
			if (feeding)
				StartCoroutine(WaitToFeed());
		}
	}
	IEnumerator WaitToFeed(){
		feeding = false;
		yield return new WaitForSeconds (feedingRate);
		FeedPopulation ();
	}

	void FeedPopulation(){
		food = food - totalFoodCost;
		feeding = true;
	}

	public void ChangeResource (string id, int quantityToAdd){
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
