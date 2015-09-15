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
//	bool farming;

	public Player_ResourceManager resourceManager;

	bool foodStatsInitialized = false;

	public enum State { HARVESTING, NOWATER }
	
	private State _state = State.HARVESTING;

	[HideInInspector]
	public State state { get { return _state; } set { _state = value; } }



	private float harvestCountDown;

	void Start () 
	{

		// Get Resource managaer from Capital gameObject
		resourceManager = GameObject.FindGameObjectWithTag ("Capital").GetComponent<Player_ResourceManager> ();
		
		// MAKE SURE THE PLAYER HAS ENOUGH WATER BEFORE ADDING THIS FARM'S PRODUCTION TO THE STATS
		if (resourceManager.water >=  waterConsumed) {

			// Tell the Resource Manager how much I produce per cycle
			resourceManager.CalculateFoodProduction (foodProduced, productionRate, waterConsumed, false);
			foodStatsInitialized = true;

		}

		// Set harvest countdown to the starting production rate
		harvestCountDown = productionRate;

	}
	

	void Update () 
	{

		if (!foodStatsInitialized) {
			if (resourceManager.water > 0){
				resourceManager.CalculateFoodProduction (foodProduced, productionRate, waterConsumed, false);
				foodStatsInitialized = true;
			}
		}

		MyStateManager (_state);
	}

	void MyStateManager(State curState)
	{
		switch (curState) {
		case State.HARVESTING:

			// Farms NEED WATER! Here we have to check with Resources to see if Player has enough water
			if (resourceManager.water >= waterConsumed) {
				CountDownToHarvest();
			} else {
				_state = State.NOWATER;
			}

			break;
		case State.NOWATER:

			if (resourceManager.water >= waterConsumed)
				_state = State.HARVESTING;

			break;

		default:
			Debug.Log("Starving");
			break;
		}
	}

	void CountDownToHarvest()
	{
		if (harvestCountDown <= 0) {
		
			Farm ();
			harvestCountDown = productionRate;

		} else {
			harvestCountDown -= Time.deltaTime;
		}
	}

	void Farm(){

		// Take the water needed from Resources
		resourceManager.ChangeResource ("Water", -waterConsumed);

		// Then add the food to Resources
		resourceManager.ChangeResource("Food", foodProduced);

	}

}
