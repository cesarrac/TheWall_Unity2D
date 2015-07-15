using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Battle_Unit : Unit {
	public float hitPoints;
	public int attackRating;
	public int defenseRating;
	public Quality quality;

	float pHitPoints;
	int pAttackRating;
	int pDefenseRating;

	// an array to be filled up either by captains or monsters
	List<GameObject> targets = new List<GameObject>();
	public GameObject target;
	public GameObject currentTarget;

	GameObject pTarget;

	public Weapon myWeapon;

	public Allegiance allegiance;

	public Transform myTransform;

	public float speed;

	public enum Quality{
		elite,
		high,
		medium,
		low
	}

	public enum Allegiance{
		monster,
		captain
	}

	void Awake () {
	
//		initStats ();
		this.pHitPoints = hitPoints;
		this.pAttackRating = attackRating;
		this.pDefenseRating = defenseRating;

	}

	void Update () {
		this.pTarget = target;
	}
		
	public Quality initQuality (){
		// for now I'm going to use a random int out of 50. <15 = low, <35 = medium, <45 = high, > 45 = elite
		int randomQuality = Random.Range (0, 50);
		Quality quality;
		if (randomQuality > 45) {
			quality = Quality.elite;
		} else if (randomQuality <= 45 && randomQuality > 35) {
			quality = Quality.high;
		} else if (randomQuality <= 35 && randomQuality > 15) {
			quality = Quality.medium;
		} else {
			quality = Quality.low;
		}
		return quality;
	}
	public int[] initStats(Quality quality){
		// Stats should be determined by QUALITY of Unit
		// Low , Medium, High, Elite
		// this would change the min and max of their stats (eg. Quality = elite; stats[0] = Random.Range(18, 32);
		// need to fill up HP, Attack Rating, and Defense Rating
		int[] stats = new int[3];
		switch (quality) 
		{
			case Quality.elite:
				stats [0] = Random.Range (18, 42); 
				for (int x =1; x< stats.Length; x++) {
					int randomStat = Random.Range(10, 20);
					stats[x] = randomStat;
				}
				break;
			case Quality.high:
				stats [0] = Random.Range (18, 25); 
				for (int x =1; x< stats.Length; x++) {
					int randomStat = Random.Range(10, 20);
					stats[x] = randomStat;
				}
				break;
			case Quality.medium:
				stats [0] = Random.Range (10, 22); 
				for (int x =1; x< stats.Length; x++) {
					int randomStat = Random.Range(10, 20);
					stats[x] = randomStat;
				}
				break;
			case Quality.low:
				stats [0] = Random.Range (9, 22); // added this to make sure HP is between 9-22
				for (int x =1; x< stats.Length; x++) {
					int randomStat = Random.Range(2, 11);
					stats[x] = randomStat;
				}
				break;
			default:
				stats [0] = Random.Range (9, 22); // added this to make sure HP is between 9-22
				for (int x =1; x< stats.Length; x++) {
					int randomStat = Random.Range(2, 11);
					stats[x] = randomStat;
				}
				break;
		}

	
		
		return stats;
	}

	//TODO: a way to use an int value as a priority, sort the targets by weakest to strongest, and select 
	// targets according to priority


	// I want to Unit to Select a target, store as a Current target, check its hitpoints everytime it HITS,
	// and if its HP is 0 or below, select a new target
	public GameObject TargetSelection(List<GameObject> myTargets){
		//this line just picks a random GameObject off the targets array
		targets = myTargets;
		int randomSelect = Random.Range (0, targets.Count);
		currentTarget = targets[randomSelect];
		return currentTarget;

	}


}
