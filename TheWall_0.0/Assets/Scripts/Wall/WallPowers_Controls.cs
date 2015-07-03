using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallPowers_Controls : MonoBehaviour {

	Wall_Targets wallTargetsScript;
	GameMaster gmScript;

	public float wallDamageBoostAmmount = 5f;
	float damageBoostCurrentTime;
	public float damageBoostMaxTime;
	bool boostingDamage;

	void Start () {
		wallTargetsScript = GetComponent<Wall_Targets> ();
		gmScript = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();
	}
	
	// Update is called once per frame
	void Update () {

		// ** ***   vvv REPLACE THIS WITH UI BUTTONS vvvvv *******
		if (gmScript.battleStarted) {
			// Wall Push: Push back enemy units close to the wall
			if (Input.GetKeyDown (KeyCode.Space)) {
				foreach (GameObject obj in wallTargetsScript.wallTargets){
					Rigidbody2D rbody = obj.GetComponent<Rigidbody2D>();
					rbody.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);
				}
			}
			
			// Wall Energize: Add damage to the units on the wall
			
			if (Input.GetKeyUp (KeyCode.E)) {
				// this will start a timer to determine how long the boost will last
				WallPower_DamageBoost();
			}

			if (boostingDamage){
				DamageBoostTimer();
			}
		}
		// ***** ^^^ REPLACE THIS WITH UI BUTTONS ^^^^^^ **********
	}


	void WallPower_DamageBoost(){
		// set the current time for the boost timer
		damageBoostCurrentTime = Time.time;
		//start the timer
		boostingDamage = true;
		// now we need to add a damage bonus according to a percent (default damage boost will be 5%)
		// the GM script takes care of giving the bonus to the units
		gmScript.WeaponDamageBoost (wallDamageBoostAmmount);
		print ("BOOSTING DAMAGE!");

	}

	void DamageBoostTimer(){
		if (Time.time - damageBoostCurrentTime >= damageBoostMaxTime) {
			// call the gmscript to cancel out the damage boost
			gmScript.WeaponDamageBoost(0f);
			boostingDamage = false;
		}
	}
}
