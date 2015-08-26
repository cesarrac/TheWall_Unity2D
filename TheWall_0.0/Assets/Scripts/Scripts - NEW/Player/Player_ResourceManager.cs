using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player_ResourceManager : MonoBehaviour {

	public int ore;
	public int food;
	public int credits;

	public int maxCitizenCount = 1;// the maximum citizens will increase with each level, bringing in new characters

	public int totalFoodCost; // this is the cost of food every turn/cycle. It gets added & subtracted by the resource grid whenever it 
													// Swaps tiles.

	public float dayTime; // How long a DAY is. Leaving this public for now but it can be set by the level the player is on
	bool feeding;
	bool starveMode;
	int turnsStarving = 0;

	Building_UIHandler buildingUI;

	public ResourceGrid resourceGrid;
	List <GameObject> buildingsStarved = new List<GameObject>();
	GameObject lastBuildingPicked, currStarvedBuilding;

	// Keep track of all Farms built and how much they produce per day
	int farmCount;
	public int foodProducedPerDay { get; private set; }

	
	/// <summary>
	/// Calculates the food production per day.
	/// Each farm takes X time to create Y food. A day takes T time.
	/// Production Rate = T divided by X ( how many times food is produced in a day).
	/// Total food produced per day = Y * Production Rate
	/// </summary>
	/// <param name="foodProduced">Food produced.</param>
	/// <param name="rateOfProd">Rate of prod.</param>
	/// <param name="trueIfSubtracting">Set to true if this Farm is being destroyed.</param>
	public void CalculateFoodProduction(int foodProduced, float rateOfProd, bool trueIfSubtracting){
	
		float productionRate = dayTime / rateOfProd;
//		Debug.Log ("Production Rate: " + productionRate);

		int perDay = Mathf.RoundToInt (foodProduced * productionRate);
//		Debug.Log ("PerDay: " + perDay);

		if (!trueIfSubtracting) {
			farmCount++;
			foodProducedPerDay = foodProducedPerDay + perDay;
		} else {
			farmCount--;
			foodProducedPerDay = foodProducedPerDay - perDay;
		}
		Debug.Log ("Food Produced Per Day = " + foodProducedPerDay + " from " + farmCount + " Farms.");
	}

	void Start(){
		buildingUI = GetComponentInChildren<Building_UIHandler> ();
	
		feeding = true;
	}
	
	void Update(){
	
		if (totalFoodCost > 0) {
			if (feeding)
				StartCoroutine(WaitToFeed());
		}
	}
	IEnumerator WaitToFeed(){
		feeding = false;
		yield return new WaitForSeconds (dayTime); // feeding ONCE per day
		FeedPopulation ();
	}

	void FeedPopulation(){
		if (food >= totalFoodCost) {

			if (buildingsStarved.Count > 0)
				UnStarveBuildings();

			int calc = food - totalFoodCost;
			Debug.Log ("Feeding population!");
			if (calc > 0){
				food = calc;
				feeding = true;
			}else{
				food = calc;
				feeding = true;
				buildingUI.CreateIndicator("NO FOOD LEFT!");
			}
		} else {
			Debug.Log("Not enough food!");
			feeding = true;

			// WAIT three cycles of the CoRoutine in starvation before turning off a building
				// Give 3 different WARNINGS to the Player informing them of the lack of food
			turnsStarving++;
			if (turnsStarving == 1){
				buildingUI.CreateIndicator("Population is HUNGRY!");
				GetBuildingToStarve();
			}else if (turnsStarving == 2){
				buildingUI.CreateIndicator("Population NEEDS FOOD!");
			}else if (turnsStarving == 3){
				buildingUI.CreateIndicator("Population is STARVING!");
				turnsStarving = 0;		// reset turns Starving ( this causes the manager to wait ANOTHER 3 turns before turning off anymore buildings)
				GetBuildingToStarve();
			}
		}

	}

	void GetBuildingToStarve(){

		foreach (GameObject tile in resourceGrid.spawnedTiles) {
			// check if another building had already been picked
			if (lastBuildingPicked != null){
				if (tile != null){
					if (tile.CompareTag("Building")){
						if (tile != lastBuildingPicked){
							// turn off the building
							currStarvedBuilding = tile;
							StarveBuildingControl(currStarvedBuilding, true);
							break;
						}
					}
				}
			}else{
				if (tile != null){
					if (tile.CompareTag("Building")){
						// turn off the building
						currStarvedBuilding = tile;
						StarveBuildingControl(currStarvedBuilding, true);
						break;
					}
				}
			}
		}
	}

	void UnStarveBuildings(){
		for (int i = 0; i < buildingsStarved.Count; i++) {
			// call starve control with starving as false
			StarveBuildingControl(buildingsStarved[i], false);
			// then remove that object from the list
			buildingsStarved.RemoveAt(i);
		}
	}


	/// <summary>
	/// Starves the building by using the object's name in a switch.
	/// It will make the starvedMode bool false in each component, depending on building type,
	/// unless starving is set to false or building name is not found.
	/// </summary>
	/// <param name="building">Building.</param>
	/// <param name="starving">If set to <c>true</c> turns starving mode true.</param>
	void StarveBuildingControl (GameObject building, bool starving){
		// first starve the building by finding what kind of building it is by name
		string buildingName = building.name;
		// add the current building to the list
		buildingsStarved.Add (building);
		// store it as the last building picked
		lastBuildingPicked = building;

		switch (buildingName) {						//** WARNING! ALL ATTACKING BUILDINGS HAVE THE COMPONENT IN CHILDREN!!!
		case "Extractor":	// extractor
			if (starving){
				building.GetComponent<Extractor>().starvedMode = true;
				buildingUI.CreateIndicator("An " + buildingName + " stopped working.");
			}else{
				building.GetComponent<Extractor>().starvedMode = false;
				buildingUI.CreateIndicator(buildingName + " back online!");
			}
			break;
		case "Machine Gun": // machine gun
			if (starving){
			building.GetComponentInChildren<Tower_TargettingHandler>().starvedMode = true;
			buildingUI.CreateIndicator("A " + buildingName + " stopped working.");
			}else{
				building.GetComponent<Tower_TargettingHandler>().starvedMode = false;
				buildingUI.CreateIndicator(buildingName + " back online!");
			}
			break;
		case "Cannons": // cannons
			if (starving){
			building.GetComponentInChildren<Tower_AoETargettingHandler>().starvedMode = true;
			buildingUI.CreateIndicator(buildingName + " stopped working.");
			}else{
				building.GetComponent<Tower_AoETargettingHandler>().starvedMode = false;
				buildingUI.CreateIndicator(buildingName + " back online!");
			}
			break;
		case "Harpooner's Hall": // harpooners hall
			if (starving){
			building.GetComponentInChildren<Barracks_SpawnHandler>().starvedMode = true;
			buildingUI.CreateIndicator("A " + buildingName + " stopped working.");
			}else{
				building.GetComponent<Barracks_SpawnHandler>().starvedMode = false;
				buildingUI.CreateIndicator(buildingName + " back online!");
			}
			break;
		case "Seaweed Farm": // seaweed farm
			if (starving){
			building.GetComponent<FoodProduction_Manager>().starvedMode = true;
			buildingUI.CreateIndicator("A " + buildingName + " stopped working.");
			}else{
				building.GetComponent<FoodProduction_Manager>().starvedMode = true;
				buildingUI.CreateIndicator(buildingName + " back online!");
			}
			break;
		default:
			Debug.Log("couldn't starve " + buildingName + " building!");
			break;
		}
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
