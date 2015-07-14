using UnityEngine;
using System.Collections;

public class Weapon : Unit {

	public Battle_Unit myTarget;
	GameObject targetAsGameObj;
	public int myAttackRating;
	public int rateOfFire;
	public float shortDamage, midDamage, longDamage;

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

	// for damage boosts from wall or other powers
	bool damageIsBoosted;
	// damage Bonus can be applied when using a power (like the Wall power Energize), favorable weather conditions,
	// and the morale of that unit
	public float longDamageBonus = 0, midDamageBonus = 0, shortDamageBonus = 0;

	public void ForcedInit(int fireRate, float sDam, float mDam, float lDam){
		rateOfFire = fireRate;
		shortDamage = sDam;
		midDamage = mDam;
		longDamage = lDam;
	}
	void Awake () {
		gameMaster = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();
		rdyToFire = true;
		stopShooting = gameMaster != null ? gameMaster.battleOver : false;
		targetDead = false;


	}
	void Start(){
		myParentTransform = GetComponentInParent<Transform>().transform;

	}
	

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
		float damage = CalcDamage ();
		// apply damage to target
		target.hitPoints = target.hitPoints - damage;
		print ("I hit " + target.name + " for " + damage);
		// check if they died
		if (target.hitPoints <= 0) {
			targetDead = true;
			print (target.name + " is dead");
			//tell GM to kill target object
			gameMaster.KillTarget (targetAsGameObj, target.allegiance.ToString());
		}
	}

	// this calculates the damage according to distance
	float CalcDamage(){
		//The distance will determine what type of attack it was, therefore dmg.
		float damageCalc = new float();
		int distance = distanceToTarget; // grabs the distance calculated when the shot was fired in ActualShoot

		if (distance <= longDistance_is && distance > midDistance_is) {
			// long distance dmg
			damageCalc = longDamage + longDamageBonus;

		} else if (distance <= midDistance_is && distance > shortDistance_is) {
			// mid distance dmg
			damageCalc = midDamage + midDamageBonus;
				} else {
			// short dmg
			damageCalc = shortDamage + shortDamageBonus;
		}
		float theDamage = damageCalc;
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

	public void DamageBoost(float boostAmmount){
		// the boostAmmount is actually a percentage, so we need figure out what % of damage boostAmmount is
		// we need this for all 3 distances
		float onePercentLongDmg = longDamage / 100f;
		// now we just multiply the boost ammount by how much 1% is
		longDamageBonus = boostAmmount * onePercentLongDmg;
		print ("Long Dmg Bonus is now: " + longDamageBonus);
	}
}
