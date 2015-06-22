﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Captain : Battle_Unit {
	int[] myStats = new int[3];



	GameMaster gameMaster;

	void Start () {
		// get a name, false for human
		name = GetName (false);
		description = "Default captain guy";
		// Random stat init, ** This is for now
		myStats = initStats ();
		hitPoints = myStats [0];
		attackRating = myStats [1];
		defenseRating = myStats [2];

		gameMaster = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();
//		targets = gameMaster != null ? gameMaster.monsterArray : null;

		if (gameMaster != null) TargetAssign ();
		//the weapon needs to know my attack rating

		myWeapon.myAttackRating = attackRating;

		allegiance = "captain";

		myTransform = transform;

	}
	
	// Update is called once per frame
	void Update () {
		CheckIfDead ();
	}

	void TargetAssign(){
		if (gameMaster != null) target = TargetSelection (gameMaster.monsterList);
		// give my Weapon its target
//		myWeapon.targetDead = false;
		myWeapon.AssignTarget(target); 
		
	}

	void CheckIfDead(){
		bool gameOver = gameMaster != null ? gameMaster.battleOver : false;
		if (myWeapon.targetDead && !gameOver) {
			TargetAssign ();
		} else {
			print (this.name + " is done.");
		}
	}

	//TODO: Ability to override target selection and replace it with Player's selection;


}