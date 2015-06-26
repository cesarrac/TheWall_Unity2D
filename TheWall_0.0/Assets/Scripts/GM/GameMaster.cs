using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameMaster : MonoBehaviour {
	//Arrays to be used for Unit targetting and for checking if battle is over
	public GameObject[] monsterArray;
	public GameObject[] captainArray;

	public List<GameObject> monsterList = new List<GameObject>();
	public List<GameObject> captainList = new List<GameObject>();

	public bool battleOver;
	public bool battleStarted;
	// for testing the instantiate captain button
	public GameObject captainOne;
	public GameObject captainTwo;



	void Awake () {
		StartGame ();
	}
	
	// Update is called once per frame
	void Update () {
		if (battleOver){
			print ("Battle is over.");

		}
	}

	void StartGame(){
		// this creates the first five captains and puts them in a list
		float xOffset = -4;
		for (int x = 0; x < 5; x++) {
			int randomUnit = Random.Range(0, 2);
			if ( randomUnit == 0){
				Instantiate(captainOne, new Vector3(xOffset, -2, 0), Quaternion.identity);
				xOffset += 2;
			}else{
				Instantiate(captainTwo, new Vector3(xOffset, -2, 0), Quaternion.identity);
				xOffset += 1;
			}

		}
	}

	public void AddUnit(){
		// this is called by the pressing of the button
		// the captain is then instantiated at the mouse position and follows it until the player clicks again

	}

	public void InitLists(){
		// this fills up the MonsterArray with monsters, CaptainArray with Captain
		print ("Initializing lists!!");
		captainArray = GameObject.FindGameObjectsWithTag ("Captain");
		// TODO: Instead of filling up the Captains list like this, I can fill it up when Player places them on wall
		for (int y = 0; y < captainArray.Length; y++) {
			captainList.Add(captainArray[y]);
		}

		// monsters are going to be pre-determined depending on the Attackers units
		// TODO: Attacker units as a list or array of GameObjects that can be given to the monsterList
		monsterArray = GameObject.FindGameObjectsWithTag ("Enemy");
		for (int x = 0; x < monsterArray.Length; x++) {
			monsterList.Add(monsterArray[x]);
		}

		// tell the Units the battle has started
		battleStarted = true;
	}


	// ******** BATTLE TOOLS ( HitCheck, KillTarget, BattleOverCheck) ****************

	public bool HitCheck(int attackRating, int defenseRating){
		int attack100 = attackRating * 10;
		int defense100 = defenseRating * 10;
		int attackRoll = Random.Range (0, attack100);
		int defenseRoll = Random.Range (0, defense100);
		bool hit = (attackRoll > defenseRoll) ? true : false;
		if (hit) print ("Attack: " + attack100 + " vs Defense: "+ defense100 + " HITS!!!");
		print ("Monsters left: " + monsterList.Count + " Captains left: " + captainList.Count);		
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
		print ("Monsters left: " + monsterList.Count + " Captains left: " + captainList.Count);
		if (monsterList.Count < 1 || captainList.Count < 1 )
			battleOver = true;
	}

	// ***********************************************************************************

}
