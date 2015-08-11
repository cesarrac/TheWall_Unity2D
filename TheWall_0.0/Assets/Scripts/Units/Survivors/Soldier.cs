using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour {
					// PLACE SOLDIER on a Tile and Choose two abilities:
						// Sniper Mode (Timed Buff/ Double Damage on Target)
						// or Control Weapons ( ALLOWS PLAYER TO CONTROL AND SHOOT FROM A TURRET)
	// access to UIMaster for UI controls
	UI_Master uiMaster;
	
	Survivor mySurvivor;

	// SNIPER MODE:
	public bool boostDamage;
	public float damageBuff;
	public float buffTime = 5f;
	float tileBaseDamage;


	// WEAPON LINK
	Turret myTurret;
	bool controllingTurret;

	bool buildingChecked; // so I can stop update calling CheckBuilding
	void Start () {
		mySurvivor = GetComponent<Survivor> ();
		uiMaster = mySurvivor.uiMaster;
		myTurret = mySurvivor.townTile.GetComponentInChildren<Turret>();
//		myTurret.seekPlayer = true;

	}

//	void SoldierInit(){ // in case I need to initialize something from the survivor script
//		
//	}
	

										// each class gets TWO ABILITY buttons when placed. 
	void AbilityButtons(){
		uiMaster.CreateAbilityButtons (survivorClass: "Soldier", ability1Action: AbilityOne, ability2Action: AbilityTwo);
	}

	public void AbilityOne(){	// sniper mode (damage buff)
		boostDamage = true;
		// deactivate ability buttons
		uiMaster.abilityBttn1.gameObject.SetActive (false);
		uiMaster.abilityBttn2.gameObject.SetActive (false);
		Debug.Log ("Soldier ability 1 activated!!!!!");
	}

	public void AbilityTwo(){
		if (mySurvivor.townTile != null) {
			if (mySurvivor.townTile.tileHasTier3){
				myTurret = mySurvivor.townTile.GetComponentInChildren<Turret>();
				controllingTurret = true;
			}
		}

		// deactivate ability buttons
		uiMaster.abilityBttn1.gameObject.SetActive (false);
		uiMaster.abilityBttn2.gameObject.SetActive (false);
		Debug.Log ("Soldier ability 2 activated!!!!!");
	}


	void Update () {
		if (boostDamage) {
			StartCoroutine(DamageBuffTimer());
		}
		if (controllingTurret) {
			ControlTurret();
			if (Input.GetButtonDown("Escape")){
				controllingTurret = false;
				myTurret.beingControlled = false;
				myTurret.seekPlayer = true;
			}
		}
	}

	void OnMouseOver(){
		if (Input.GetMouseButtonDown (0)) {
			if (uiMaster != null) {
				AbilityButtons ();
			}
		}
	}

	IEnumerator DamageBuffTimer(){
		// add buff
		ApplyDamageBuff ();


		yield return new WaitForSeconds (buffTime);
		// remove buff
		RemoveDamageBuff ();

	} 


	public void ApplyDamageBuff(){
		if (mySurvivor.townTile != null && boostDamage) {
			tileBaseDamage = mySurvivor.townTile.baseDamage;
			damageBuff = tileBaseDamage * 2;
			mySurvivor.townTile.baseDamage = damageBuff;
			boostDamage = false;
			Debug.Log ("Buffing...");
		}
	}

	public void RemoveDamageBuff(){
		if (mySurvivor.townTile != null) {
			mySurvivor.townTile.baseDamage = tileBaseDamage;	
			Debug.Log("Stopped Buffing");
		}

	}

	void ControlTurret(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		myTurret.ManualControl (m);
	}




	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Turret")) {
			myTurret = coll.gameObject.GetComponent<Turret>();
			myTurret.seekPlayer = true;
		}
	}
}
