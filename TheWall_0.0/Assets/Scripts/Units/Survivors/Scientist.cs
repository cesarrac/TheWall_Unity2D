using UnityEngine;
using System.Collections;

public class Scientist : MonoBehaviour {
	UI_Master uiMaster;
	Survivor mySurvivor;

	
	void Start () {
		mySurvivor = GetComponent<Survivor> ();
		uiMaster = mySurvivor.uiMaster;
	}
	
	void AbilityButtons(){
		uiMaster.CreateAbilityButtons (survivorClass: "Scientist", ability1Action: AbilityOne, ability2Action: AbilityTwo);
	}
	
	void AbilityOne(){
		
	}
	
	void AbilityTwo(){
		
	}
}
