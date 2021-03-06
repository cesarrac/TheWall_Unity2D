﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Enemy : Battle_Unit {


	int[] myStats = new int[3];

	GameMaster gameMaster;
	public bool canMove;

	public void ForceInit(Quality forcedQuality, string myName, int[] stats, Allegiance _allegiance){
		quality = forcedQuality;
		name = myName;
		myStats = stats;
		hitPoints = (float)myStats [0];
		attackRating = myStats [1];
		defenseRating = myStats [2];
		allegiance = _allegiance;
		description = "Default monster thing.";
	}

	void Awake(){
		gameMaster = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();
	}
	void Start () {
	
		myWeapon = GetComponentInChildren<Weapon> (); // this works only if this is instantiated as a prefab with wpn as child

		// THIS ONLY HAPPENS IF FOR SOME REASON THIS UNIT DOESN'T ALREADY HAVE QUALITY AND STATS
		if (myStats [0] <= 0) { // easiest check is for HP since it HAS to be more than ONE if this unit has already initialized
			// first Time a Unit is created we determine its Quality
			quality = initQuality ();
			myStats = initStats (quality);
			// get a name, true for monster
			name = GetName (true);
			description = "Default monster thing.";
			hitPoints = (float)myStats [0];
			attackRating = myStats [1];
			defenseRating = myStats [2];
			allegiance = Allegiance.monster;
		}
	

	

		// *v Commented this to make sure this unit waits for battle to start
//		if (gameMaster != null) TargetAssign ();
		
		//the weapon needs to know my attack rating
		myWeapon.myAttackRating = attackRating;

	

		myTransform = transform;
		speed = 0.2f;

	}
	

	void Update () {
		// call to check if my current target is dead
		CheckIfDead ();
		if (canMove) {
			MoveTowardsWall ();
		} 
	}

	void TargetAssign(){
		// now that you have a target, you can move
		if (gameMaster != null) target = TargetSelection (gameMaster.captainList);
		// give my Weapon its target
		myWeapon.targetDead = false;
		myWeapon.AssignTarget(target, false); // must specify target and TRUE if Captain, FALSE for monster

		canMove = true;

	}


	void CheckIfDead(){
		bool gameOver = gameMaster.battleOver;
		bool battleStarted = gameMaster.battleStarted;
		if (myWeapon.targetDead || target == null ) {
			if (!gameOver && battleStarted) {
				TargetAssign ();
			} else {
				print (this.name + " is waiting.");
				
			}
		}

	}


	//TODO: Method for moving towards the wall
	// they only need to move down in the Y ( the wall right now is at y:-2 world coords)
	void MoveTowardsWall (){
		// approx vector 2 location of wall
		Vector2 wallPostion = new Vector2 (0, -2);
		Vector2 movePosition = new Vector2 (myTransform.position.x, wallPostion.y);
		Vector2 moveResult = Vector2.MoveTowards (new Vector2(myTransform.position.x, myTransform.position.y), movePosition, speed * Time.deltaTime);
		myTransform.position = new Vector3 (myTransform.position.x, moveResult.y, 0);
	}


}
