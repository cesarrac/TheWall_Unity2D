using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameMaster : MonoBehaviour {
	//Arrays to be used for Unit targetting and for checking if battle is over
	public GameObject[] monsterArray;
	public GameObject[] captainArray;

	public List<GameObject> monsterList = new List<GameObject>();
	public List<GameObject> captainList = new List<GameObject>();

	public bool battleOver;

	// Use this for initialization
	void Awake () {
		// this fills up the MonsterArray with monsters, CaptainArray with Captain
		monsterArray = GameObject.FindGameObjectsWithTag ("Enemy");
		captainArray = GameObject.FindGameObjectsWithTag ("Captain");
		for (int x = 0; x < monsterArray.Length; x++) {
			monsterList.Add(monsterArray[x]);
		}
		for (int y = 0; y < captainArray.Length; y++) {
			captainList.Add(captainArray[y]);
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (battleOver){
			print ("Battle is over.");
		}
	}

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
}
