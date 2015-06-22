﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Battle_Unit : Unit {
	public int hitPoints;
	public int attackRating;
	public int defenseRating;

	int pHitPoints;
	int pAttackRating;
	int pDefenseRating;

	// an array to be filled up either by captains or monsters
	List<GameObject> targets = new List<GameObject>();
	public GameObject target;
	public GameObject currentTarget;

	GameObject pTarget;

	public Weapon myWeapon;

	public string allegiance;
	public Transform myTransform;

	public float speed;

	void Start () {
		initStats ();
		this.pHitPoints = hitPoints;
		this.pAttackRating = attackRating;
		this.pDefenseRating = defenseRating;

		myWeapon = GetComponentInChildren<Weapon> ();

	}
	
	void Update () {
		this.pTarget = target;
	}


	//TODO: Create a method that determines INITIAL STATS
	// Stats should be determined by QUALITY of Unit
	// Low , Medium, High, Elite

	// For now I'm going to initialize stats with Random ints out of 10 (eg. hp: 4/10);
	public int[] initStats(){
		// need to fill up HP, Attack Rating, and Defense Rating (derived from Battle_Unit class)
		int[] stats = new int[3];
		stats [0] = Random.Range (9, 22);
		for (int x =1; x< stats.Length; x++) {
			int randomStat = Random.Range(2, 11);
			stats[x] = randomStat;
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