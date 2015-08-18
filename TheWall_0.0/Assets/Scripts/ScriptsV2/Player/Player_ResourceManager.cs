using UnityEngine;
using System.Collections;


public class Player_ResourceManager : MonoBehaviour {

	public int ore;
	public int food;
	public int credits;

	public int maxCitizenCount = 1;// the maximum citizens will increase with each level, bringing in new characters

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
