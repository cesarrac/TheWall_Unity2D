using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameMaster : MonoBehaviour {
	//Arrays to be used for Unit targetting and for checking if battle is over
//	public GameObject[] monsterArray;
//	public GameObject[] captainArray;

	public List<GameObject> monsterList = new List<GameObject>();
	public List<GameObject> captainList = new List<GameObject>();

	public bool battleOver;
	public bool battleStarted;
	// for testing the instantiate captain button
	public GameObject captainOne;
	public GameObject captainTwo;

	// penalties/bonuses for Hit Check based on Distance
	public int longDistancePenalty, shortDistanceBonus;
	public int longDistance_is, midDistance_is, shortDistance_is;

	// floats to keep track of Wall stats
	public float maxWallEnergy;	// this gets affected by upgrades and new buildings during Town View

	//for spawning Horde members (Monsters)
	public List<Unit_Data> hordeMembers = new List<Unit_Data> ();
	public GameObject unitToSpawn; // this public GameObject is blank except for the components needed for a Battle Unit
	SpriteRenderer sr;

	void Awake () {
//		SpawnCaptains ();
		DontDestroyOnLoad (this.gameObject);
		monsterList.Clear ();
		captainList.Clear ();
		battleStarted = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Application.loadedLevel == 1) {
			if (battleOver){
				print ("Battle is over.");
				
			}
		}

	}

	public void LoadBattleView(List<Unit_Data> members){
		foreach (Unit_Data unit in members) {
			hordeMembers.Add(unit); // populate the GM list of hordeMembers to spawn
		}
		Application.LoadLevel (1);
		StartCoroutine (SetUpBattle ());

	}

	IEnumerator SetUpBattle(){
		OnLoadedLevel ();
		yield return new WaitForSeconds (1);
		SpawnCaptains ();
		StopCoroutine (SetUpBattle ());

	}
	void OnLoadedLevel(){
		// spawn the members of the enemy horde
		print ("Loading level!");
			int maxMembersOfHorde = hordeMembers.Count;
		for (int x=0; x < maxMembersOfHorde; x++) {
			GameObject hordeSpawn = Instantiate(unitToSpawn, new Vector3(x, 3.6f, 0), Quaternion.identity) as GameObject;
			DontDestroyOnLoad(hordeSpawn.gameObject);
			sr = hordeSpawn.GetComponent<SpriteRenderer>();
			sr.sprite = hordeMembers[x].mySprite;
			AI_Enemy ai = hordeSpawn.GetComponent<AI_Enemy>();
			Weapon weapon = hordeSpawn.GetComponentInChildren<Weapon>();
			weapon.ForcedInit(hordeMembers[x].rateOfFire, hordeMembers[x].shortDamage, hordeMembers[x].midDamage, hordeMembers[x].longDamage); 
			ai.ForceInit((Battle_Unit.Quality)hordeMembers[x].myQuality, hordeMembers[x].myName, hordeMembers[x].myStats, (Battle_Unit.Allegiance) hordeMembers[x].myAllegiance);

			// add them to the monster list
			monsterList.Add(hordeSpawn);
		}



	}

	void SpawnCaptains(){
		// ***************** TEMPORARY CAPTAINS SPAWN / REPLACE WITH ADD UNITS UI **************
		// this creates the first 5 captains
		float xOffset = -4;
		GameObject cptSpawn;
		for (int x = 0; x < 5; x++) {
			int randomUnit = Random.Range(0, 2);
			if ( randomUnit == 0){
				cptSpawn = Instantiate(captainOne, new Vector3(xOffset, -2, 0), Quaternion.identity)as GameObject;
				xOffset += 2;
				// add to captain list
				captainList.Add(cptSpawn);
			}else{
				cptSpawn = Instantiate(captainTwo, new Vector3(xOffset, -2, 0), Quaternion.identity)as GameObject;
				xOffset += 1;
				// add to captain list
				captainList.Add(cptSpawn);
			}


		}
		// ***************** TEMPORARY CAPTAINS SPAWN / REPLACE WITH ADD UNITS UI **************
	}

	public void AddUnit(){
		// this is called by the pressing of the button
		// the captain is then instantiated at the mouse position and follows it until the player clicks again

	}

	public void InitLists(){
		// this fills up the MonsterArray with monsters, CaptainArray with Captain
		print ("Initializing lists!!");
//		captainArray = GameObject.FindGameObjectsWithTag ("Captain");
//		// TODO: Instead of filling up the Captains list like this, I can fill it up when Player places them on wall
//		for (int y = 0; y < captainArray.Length; y++) {
//			captainList.Add(captainArray[y]);
//		}
//
//		// monsters are going to be pre-determined depending on the Attackers units
//		// TODO: Use the list of Horde Members to fill up this monsterList
//		monsterArray = GameObject.FindGameObjectsWithTag ("Enemy");
//		for (int x = 0; x < monsterArray.Length; x++) {
//			monsterList.Add(monsterArray[x]);
//		}

		// tell the Units the battle has started
		battleStarted = true;
	}


	// ******** BATTLE TOOLS ( HitCheck, KillTarget, BattleOverCheck) ****************

	public bool HitCheck(int attackRating, int defenseRating, int distance){
		// multiply attack and defense ratings by 10 to make a percentile die throw
		int attack100 = attackRating * 10;
		int defense100 = defenseRating * 10;
		// percentile die roll
		int attackRoll = Random.Range (attackRating, attack100);
		int defenseRoll = Random.Range (defenseRating, defense100);

//		print ("Attack Roll before distance: " + attackRoll);

		// now Distance steps in to the equation, long distance has a penalty while short distance has a bonus
		// im using public int vars named exactly like the ones in Weapon (longDistance_is, midDistance_is)
		if (distance <= longDistance_is && distance > midDistance_is) {
			attackRoll = attackRoll - longDistancePenalty; // apply penalty
			if (attackRoll < 0) attackRoll = 0; // no negative numbers

		} else if (distance <= shortDistance_is) {
			attackRoll = attackRoll + shortDistanceBonus; // apply bonus
		}

//		print ("Attack Roll after distance: " + attackRoll);

		bool hit = (attackRoll > defenseRoll) ? true : false; // now check to see if it HITS
		if (hit) {
			// prints out console messages for debugging
//			print ("Attack: " + attackRoll + " vs Defense: "+ defenseRoll + " HITS!!!");
//			print ("Monsters left: " + monsterList.Count + " Captains left: " + captainList.Count);
		} else {
//			print ("I missed. Attack: " + attackRoll + "defense: " + defenseRoll);

		}
				
		return hit;
	}

	public void KillTarget(GameObject target, string unitAllegiance){
		int currCptCount = captainList.Count;
		int currMonsterCount = monsterList.Count;
		if (unitAllegiance == "captain") {
			for (int x = 0; x < captainList.Count; x++){
				if (captainList[x] == target){
					print (target.name + " KIA");
					captainList.RemoveAt(x);
					Destroy(target);

				}
			}


		} else if (unitAllegiance == "monster") {

			for (int x = 0; x < monsterList.Count; x++){
				if (monsterList[x] == target){
					print (target.name + " KIA");
					monsterList.RemoveAt(x);
					Destroy(target);

				}
			}

		}
		// after killing one target we must check if the battle is over
		BattleOverCheck();
	}

	void BattleOverCheck(){
//		print ("Monsters left: " + monsterList.Count + " Captains left: " + captainList.Count);
		if (monsterList.Count <= 0 || captainList.Count <= 0) {
			battleStarted = false;
			battleOver = true;
		}
	}

	// damage boost called by weather conditions,  wall powers, and other upgrades
	public void WeaponDamageBoost(float damageBoostPercent){
		foreach (GameObject obj in captainList) {
			Captain captain = obj.GetComponent<Captain>();
			captain.myWeapon.DamageBoost(damageBoostPercent); // this calls the Weapon script to figure out the bonus
		}
	}

	// ***********************************************************************************

}
