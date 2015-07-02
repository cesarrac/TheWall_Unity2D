using UnityEngine;
using System.Collections;

public class Weapon : Unit {

	public Battle_Unit myTarget;
	GameObject targetAsGameObj;
	public int myAttackRating;
	public int rateOfFire;
	public int shortDamage, midDamage, longDamage;



	bool rdyToFire;
	public bool targetDead;

	bool stopShooting;

	GameMaster gameMaster;

	Transform myParentTransform;
	Vector3 myParentPosition;

	// for distance checks:
	bool trueIfCaptain;
	public int longDistance_is, midDistance_is, shortDistance_is;
	int distanceToTarget;
	// for distance meter
//	public Transform distanceMeter;

	// Use this for initialization
	void Awake () {
		gameMaster = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();
		rdyToFire = true;
		stopShooting = gameMaster != null ? gameMaster.battleOver : false;
		targetDead = false;


	}
	void Start(){
		myParentTransform = GetComponentInParent<Transform>().transform;

	}
	
	// Update is called once per frame
	void Update () {
		if (myTarget != null) {
			Shoot ();
		} else {
			print ("Weapon stopped");
		}
	
	}

	public void AssignTarget (GameObject target, bool trueForCaptain){
		stopShooting = gameMaster.battleOver;
		if (!stopShooting) {
			myTarget = target.GetComponent<Battle_Unit> ();
			targetAsGameObj = target;
			trueIfCaptain = trueForCaptain; // fills bool TRUE if this is a Captain, FALSE if not

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

		// The lower the Unit's attack rating, the harder it is to hit at long range
		distanceToTarget = CalcDistanceToTarget (trueIfCaptain, targetAsGameObj); // this can get passed to the GM
		// calls on the GameMaster through a bool
		bool hit = gameMaster.HitCheck(myAttackRating, target.defenseRating, distanceToTarget);
		if (hit) DoDamage(target);
	
	}
	//this actually DOES the damage
	void DoDamage (Battle_Unit target){
		int damage = CalcDamage ();
		target.hitPoints = target.hitPoints - damage;
		print ("I hit " + target.name + " for " + damage);
		if (target.hitPoints <= 0) {
			targetDead = true;
			print (target.name + " is dead?");
			//tell GM to kill target object
			gameMaster.KillTarget (targetAsGameObj, target.allegiance);
		}
	}

	// this calculates the damage according to distance
	int CalcDamage(){
		//The distance will determine what type of attack it was, therefore dmg.
		int damageCalc = new int();
		int distance = distanceToTarget; // grabs the distance calculated when the shot was fired in ActualShoot

		if (distance <= longDistance_is && distance > midDistance_is) {
			// long distance dmg
			damageCalc = longDamage;

		} else if (distance <= midDistance_is && distance > shortDistance_is) {
			// mid distance dmg
			damageCalc = midDamage;
				} else {
			// short dmg
			damageCalc = shortDamage;
		}
		int theDamage = damageCalc;
		return theDamage;
	}



	// a function will as CalcDistanceToTarget with a bool that is TRUE if the unit is a Captain, and the target
	int CalcDistanceToTarget(bool trueForCaptain, GameObject target){
		float calc;
		myParentPosition = myParentTransform.position;
		if (trueForCaptain) { // this is a Captain calling this function
			// distance = my target Y position - my Y position (because the Captain will always be lower on screen)
			calc = target.transform.position.y - myParentPosition.y;
//			print (this.name + " is " + calc + " from " + targetAsGameObj.name);
		} else { // this is a Monster
			// distance = my Y pos - my target Y pos
			calc = myParentPosition.y - target.transform.position.y;

		}
		int distance = (int) calc;
	
		return distance;
	}


}
