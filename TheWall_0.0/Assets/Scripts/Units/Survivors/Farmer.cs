using UnityEngine;
using System.Collections;

public class Farmer : MonoBehaviour {
										// FARMER:
						// ABILITY 1: Plant Seeds, 2: Slash and Burn
	// access to UIMaster for UI controls
	UI_Master uiMaster;
	Survivor mySurvivor;
	
	// Plant Seeds
	Farm myFarm;
	bool seedsPlanted;

	void Start () {
		mySurvivor = GetComponent<Survivor> ();
		uiMaster = mySurvivor.uiMaster;
		myFarm = mySurvivor.townTile.GetComponentInChildren<Farm>();
	}

	void AbilityButtons(){
		uiMaster.CreateAbilityButtons (survivorClass: "Farmer", ability1Action: AbilityOne, ability2Action: AbilityTwo);
	}

	void AbilityOne(){
		// call on farm to plant seeds
		if (myFarm != null) {
			myFarm.PlantCrops();
			Debug.Log("Farmer plants crops!");
		}
		Destroy(uiMaster.abBttn1.gameObject);
		Destroy(uiMaster.abBttn2.gameObject);
	}

	void AbilityTwo(){

	}

	void OnMouseOver(){
		if (Input.GetMouseButtonDown (0)) {
			if (uiMaster != null) {
				if (uiMaster.abBttn1 != null && uiMaster.abBttn2 != null){
					Destroy(uiMaster.abBttn1.gameObject);
					Destroy(uiMaster.abBttn2.gameObject);
//					AbilityButtons ();
				}else{
					AbilityButtons ();
				}
				
			}
		}

	}

	// Update is called once per frame
	void Update () {
	
	}
}
