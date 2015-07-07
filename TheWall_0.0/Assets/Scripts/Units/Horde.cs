using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Horde : MonoBehaviour {
//	public List<AI_Enemy> hordeMembers = new List<AI_Enemy>();
	public List<Unit_Data> hordeMembers = new List<Unit_Data> ();

	// this needs a variable given to them by the faction that is spawning this horde
	// it will tell them what to spawn
	public GameObject unitToSpawn;
	SpriteRenderer sr;


	void Start () {
		// TEST: create 5 AI elite units
		for (int x = 0; x <= 5; x++) {
			hordeMembers.Add(new Unit_Data(description: "Some description.", allegiance: Unit_Data.Allegiance.monster, quality: Unit_Data.Quality.high));
		}
//		unitHolder.gameObject.AddComponent <AI_Enemy> ();
//		leader = unitHolder.GetComponent<AI_Enemy> ();
//		leader.ForceInit (Battle_Unit.Quality.high);
		print (hordeMembers [0].myAllegiance.ToString ());

	}

	// this needs to tell the GM to spawn the number of units with this data when the Player clicks to go to battle





	// Update is called once per frame
	void Update () {
	
	}

	//for testing
	public void Spawn(){
		int maxMembersOfHorde = hordeMembers.Count;
		for (int x=0; x < maxMembersOfHorde; x++) {
			GameObject hordeSpawn = Instantiate(unitToSpawn, new Vector3(10, x, 0), Quaternion.identity) as GameObject;
			sr = hordeSpawn.GetComponent<SpriteRenderer>();
			sr.sprite = hordeMembers[x].mySprite;
			hordeSpawn.AddComponent<AI_Enemy>();
			AI_Enemy ai = hordeSpawn.GetComponent<AI_Enemy>();
			ai.ForceInit((Battle_Unit.Quality)hordeMembers[x].myQuality, hordeMembers[x].myName, hordeMembers[x].myStats);
		}
	}
}
