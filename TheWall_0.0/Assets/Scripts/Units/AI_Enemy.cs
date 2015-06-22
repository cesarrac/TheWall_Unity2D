using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Enemy : Battle_Unit {


	int[] myStats = new int[3];

	GameMaster gameMaster;
	public bool canMove;


	void Start () {
		gameMaster = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();

		// get a name, true for monster
		name = GetName (true);
		description = "Default monster thing.";
		// Random stat init, ** This is for now
		myStats = initStats ();
		hitPoints = myStats [0];
		attackRating = myStats [1];
		defenseRating = myStats [2];

//		targets = gameMaster != null ? gameMaster.captainArray : null;
		 
		if (gameMaster != null) TargetAssign ();
		
		//the weapon needs to know my attack rating
		myWeapon.myAttackRating = attackRating;

		allegiance = "monster";

		myTransform = transform;
		speed = 0.2f;
		canMove = true;
	}
	

	void Update () {
		// call to check if my current target is dead
		CheckIfDead ();
		if (canMove) {
			MoveTowardsWall ();
		} 
	}

	void TargetAssign(){

		if (gameMaster != null) target = TargetSelection (gameMaster.captainList);
		// give my Weapon its target
		myWeapon.targetDead = false;
		myWeapon.AssignTarget(target); 

		canMove = true;

	}


	void CheckIfDead(){
//		bool gameOver = gameMaster != null ? gameMaster.battleOver : false;
		bool gameOver = gameMaster.battleOver;
		if (myWeapon.targetDead || target == null ) {
			if (!gameOver ) {
				TargetAssign ();
			} else {
				print (this.name + " is done.");
				
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
