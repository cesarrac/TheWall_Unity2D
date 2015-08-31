using UnityEngine;
using System.Collections;

public class Storage : MonoBehaviour {

	public int waterStored { get; private set; }
	public int waterCapacity;
	public int waterCapacityLeft;

	public int oreStored { get; private set; }
	public int oreCapacity;
	public int oreCapacityLeft;

	public Player_ResourceManager playerResources;
	public Player_UIHandler playerUI;

	// Use this for initialization
	void Start () {
		waterCapacityLeft = waterCapacity;
		oreCapacityLeft = oreCapacity;

		if (playerResources == null) {
			playerResources = GameObject.FindGameObjectWithTag("Capital").GetComponent<Player_ResourceManager>();
		}

		if (playerResources != null) {
			playerResources.storageBuildings.Add(this);
		}

		if (playerUI == null) {
			playerUI = GameObject.FindGameObjectWithTag("Capital").GetComponent<Player_UIHandler>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseOver(){
		if (Input.GetMouseButtonUp (1)) {
			if (playerUI != null) {
				playerUI.DisplayStorageInfo (this);
			}
		}
	}

	public void AddOreOrWater(int ammt, bool trueIfWater)
	{
		if (trueIfWater) {
			int calcW = waterStored + ammt;
			if (calcW <= waterCapacity) {
				waterStored = calcW;
				waterCapacityLeft = waterCapacity - waterStored;

				// ADD TO THE PLAYER RESOURCES TO DISPLAY ON UI
				playerResources.ChangeResource("Water", ammt);

			} else {
				// cant store more water
			}
			Debug.Log ("Total water in storage = " + waterStored);
		} else {
			int calcW = oreStored + ammt;
			if (calcW <= oreCapacity) {
				oreStored = calcW;
				oreCapacityLeft = oreCapacity - oreStored;

				// ADD TO THE PLAYER RESOURCES TO DISPLAY ON UI
				playerResources.ChangeResource("Ore", ammt);

			} else {
				// cant store more water
			}
			Debug.Log ("Total ore in storage = " + oreStored);
		}
	}

	public bool CheckIfFull(int ammntToStore, bool trueIfWater){
		if (trueIfWater) {
			if (ammntToStore > waterCapacityLeft) {
				return true;
			} else {
				return false;
			}
		} else {
			if (ammntToStore > oreCapacityLeft){
				return true;
			}else{
				return false;
			}
		}
	}

	/// <summary>
	/// Charges the resource from this storage
	/// </summary>
	/// <param name="ammnt">Ammnt.</param>
	/// <param name="id">Identifier.</param>
	public void ChargeResource(int ammnt, string id){
		if (id == "Ore") {
			oreStored = oreStored + ammnt;
			Debug.Log("Charging stored ore for " + ammnt);
			playerResources.ChangeResource(id, ammnt);
		} else if (id == "Water") {
			waterStored = waterStored + ammnt;
			playerResources.ChangeResource(id, ammnt);

		}
	}
}
