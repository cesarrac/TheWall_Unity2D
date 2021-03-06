﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Captain : Battle_Unit {
	int[] myStats = new int[3];

	GameMaster gameMaster;

//	public Captain(){
//		name = GetName (false);
//		description = "Default captain guy";
//		// Random stat init, ** This is for now
//		myStats = initStats ();
//		hitPoints = myStats [0];
//		attackRating = myStats [1];
//		defenseRating = myStats [2];
//	}

	void Start () {
		gameMaster = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();
		myWeapon = GetComponentInChildren<Weapon> (); // this works only if this is instantiated as a prefab with wpn as child

		// THIS ONLY HAPPENS IF FOR SOME REASON THIS UNIT DOESN'T ALREADY HAVE QUALITY AND STATS
		if (myStats [0] <= 0) { // easiest check is for HP since it HAS to be more than ONE if this unit has already initialized
			// get a name, false for human
			name = GetName (false);
			description = "Default captain guy";
			// first Time a Unit is created we determine its Quality
			quality = initQuality ();
			// Random stat init, ** This is for now
			myStats = initStats (quality);
			hitPoints = (float)myStats [0];
			attackRating = myStats [1];
			defenseRating = myStats [2];
			allegiance = Allegiance.captain;
		}
	

		//the weapon needs to know my attack rating
		myWeapon.myAttackRating = attackRating;



		myTransform = transform;

	}
	

	void Update () {
		// we need to check if battle has started
		// once it has we need to quickly assign a target and start shooting

		CheckIfDead ();
	}

	void TargetAssign(){
			
		if (gameMaster != null) target = TargetSelection (gameMaster.monsterList);
		// give my Weapon its target
		myWeapon.targetDead = false;
		myWeapon.AssignTarget(target, true); // must specify target and TRUE if Captain, FALSE for monster

		
	}

	void CheckIfDead(){
		bool gameOver = gameMaster.battleOver;
		bool battleHasStarted = gameMaster.battleStarted;
		if (myWeapon.targetDead || target == null ) {
			if (!gameOver && battleHasStarted) {  	
				TargetAssign ();
			} else {
				print (this.name + " is waiting.");
				
			}
		}
		
	}

	//This next function is called from MouseControls
		// When this Unit is selected Mouse_Controls will give a target
		// and this Unit will assign it to its Weapon
	public void PlayerTargetAssign(GameObject playerTarget){
		target = playerTarget;
		print ("Player selected a new target: " + playerTarget.name);
		// give my Weapon its target
		myWeapon.targetDead = false;
		myWeapon.AssignTarget(target, true);
		// I want to add here a way to shut off the selection box of this unit, once a target is selected
	}

}
