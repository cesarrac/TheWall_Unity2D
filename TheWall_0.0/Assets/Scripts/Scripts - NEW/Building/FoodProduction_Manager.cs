using UnityEngine;
using System.Collections;

public class FoodProduction_Manager : MonoBehaviour {
	/// <summary>
	/// Extracts x # of food each production cycle and adds it to the Player's resources.
	/// </summary>

	public float productionRate;
	public int foodProduced;
	bool farming;

	public Player_ResourceManager resourceManager;

	public bool starvedMode; // MANIPULATED BY THE RESOURCE MANAGER

	void Start () {
		resourceManager = GameObject.FindGameObjectWithTag ("Capital").GetComponent<Player_ResourceManager> ();
		farming = true;

		// Tell the Resource Manager how much I produce per cycle
		resourceManager.CalculateFoodProduction (foodProduced, productionRate, false);
	}
	
	// Update is called once per frame
	void Update () {
		if (farming && !starvedMode) {
			StartCoroutine(WaitToFarm());
		}
	}

	IEnumerator WaitToFarm(){
		farming = false;
		yield return new WaitForSeconds (productionRate);
		Farm ();
	}

	void Farm(){
		resourceManager.food = resourceManager.food + foodProduced;
		farming = true;
	}

}
