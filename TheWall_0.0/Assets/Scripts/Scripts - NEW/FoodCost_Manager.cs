using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodCost_Manager : MonoBehaviour {
	/// <summary>

	/// Stores an List of buildings that cost food. Every cycle/turn it checks which ones are still not null
	/// and adds all their food costs together. Once it has the TOTAL food cost, it subtracts it from Food.

	/// </summary>

	public List<GameObject> foodCostBuildings;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
