using UnityEngine;
using System.Collections;

public class Weapon : Unit {

	public Battle_Unit myTarget;
	GameObject targetAsGameObj;
	public int myAttackRating;
	public int rateOfFire;
	public int damage;

	bool rdyToFire;
	public bool targetDead;

	bool stopShooting;

	GameMaster gameMaster;

	// for distance meter
	public Transform distanceMeter;

	// Use this for initialization
	void Awake () {
		gameMaster = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();
		rdyToFire = true;
		stopShooting = gameMaster != null ? gameMaster.battleOver : false;
		targetDead = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (myTarget != null) {
			Shoot ();
		} else {
			print ("Weapon stopped");
		}
	
	}

	public void AssignTarget (GameObject target){
		stopShooting = gameMaster.battleOver;
		if (!stopShooting) {
			myTarget = target.GetComponent<Battle_Unit> ();
			targetAsGameObj = target;
		}
	}


	// the Shoot method will run according to the fireRate this is called by the Unit holding this weapon
	public void Shoot(){
		if (rdyToFire && !targetDead) {
			StartCoroutine (FireRate ());
		} 


	
	}

	IEnumerator FireRate(){
		ActualShoot (myTarget);
		rdyToFire = false;
		yield return new WaitForSeconds (rateOfFire);
		rdyToFire = true;
		
	}

	void ActualShoot(Battle_Unit target){
		// calls on the GameMaster through a bool
		//TODO: Add distance variable. The distance will add a penalty to the hit while also affecting dmg
		// The lower the Unit's attack rating, the harder it is to hit at long range
		bool hit = gameMaster.HitCheck(myAttackRating, target.defenseRating);
		if (hit) {
			DoDamage(target);
			print (this.name + " hit " + myTarget.name);
		} else {
			// i missed
			print ("I missed. Attack: " + myAttackRating + "defense: " + target.defenseRating);
		}
	
	}



	void DoDamage(Battle_Unit target){
		//TODO: also Add distance variable. The distance will determine what type of attack it was, therefore dmg.
		damage = GetTypeOfDamage ();
		target.hitPoints = target.hitPoints - damage;
		if (target.hitPoints <= 0) {
			targetDead = true;
			print (target.name + " is dead?");
			//tell GM to kill target object
			gameMaster.KillTarget (targetAsGameObj, target.allegiance);
		}

			
	}

	int GetTypeOfDamage(){
		int longDistanceDmg = 6;
		int medDistanceDmg = 3;
		int shortDistanceDmg = 2;

		float distance = distanceMeter.localPosition.y - targetAsGameObj.transform.position.y;
		if (distance < 0) {
			// it's long distance
			return longDistanceDmg;
		} else if (distance < 1) {
			//its med distance
			return medDistanceDmg;
		} else if (distance < 2) {
			// short distance
			return shortDistanceDmg;
		}
		return damage;
	}




}
