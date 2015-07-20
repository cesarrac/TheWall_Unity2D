using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Horde : MonoBehaviour {
//	public List<AI_Enemy> hordeMembers = new List<AI_Enemy>();
	public List<Unit_Data> hordeMembers = new List<Unit_Data> ();

	// TODO: this needs a variable given to them by the faction that is spawning this horde

	// it will tell them what to spawn
//	public GameObject unitToSpawn; // this public GameObject is blank except for the components needed for a Battle Unit
//	SpriteRenderer sr;

	//this bool will be true when this horde is next to a town tile
	public bool nextToTownTile;

	bool canHitTile = true;

	// GM script access needed to call on the battle view load
	GameMaster gmScript;

	// Tile that gets filled with a Town tile when this Horde is next to one
	public TownTile_HP townTile;

	public GameObject deadHordeFab;

	void Start () {
		gmScript = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();

		// TEST: create 5 AI elite units
//		for (int x = 0; x <= 5; x++) {
//			hordeMembers.Add(new Unit_Data(description: "Some description.", allegiance: Unit_Data.Allegiance.monster, quality: Unit_Data.Quality.high, fireRate: 1, sDamage: 2f, mDamage: 3f, lDamage: 5f));
//		}

		// TEST2: create 1 AI high
		hordeMembers.Add(new Unit_Data(description: "Some description.", allegiance: Unit_Data.Allegiance.monster, quality: Unit_Data.Quality.high, fireRate: 1, sDamage: 2f, mDamage: 3f, lDamage: 5f));

//		unitHolder.gameObject.AddComponent <AI_Enemy> ();
//		leader = unitHolder.GetComponent<AI_Enemy> ();
//		leader.ForceInit (Battle_Unit.Quality.high);

	}

	void Update () {
		if (nextToTownTile && townTile != null) {
			if (canHitTile) {
				StartCoroutine (TileHit ());
			}
		} else {
			nextToTownTile = false;
		}
	}


	
	// HORDE MOVEMENT IS DONE BY THE GM

	// If next to tile do damage
	IEnumerator TileHit(){
		canHitTile = false;
		yield return new WaitForSeconds(2);
		print ("Horde is waiting to do damage");
		DamageTile (hordeMembers[0].shortDamage);
	}

	void DamageTile(float damage){
		print ("Horde hits tile");
		townTile.DoDamage (damage);
		canHitTile = true;
	}

	// This next function takes care of damaging the Horde once its clicked (called by Mouse_Control)
	public void TakeDamage(float damage){
		hordeMembers [0].hitPoints = hordeMembers [0].hitPoints - damage;
		if (hordeMembers [0].hitPoints <= 0) {
			Die();
		}

	}

	void Die(){
		// first instantiate my dead prefab self
		GameObject deadHorde = Instantiate(deadHordeFab, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Quaternion.identity) as GameObject;
		// then die
		Destroy (this.gameObject);
	}


	// *****BATTLEVIEW ****
	// that load battleview function will then call the Spawn function below to instantiate the proper units
//	public void GoToBattle(){
//		if (nextToTownTile) { // make sure this Horde tile is next to a player wall
//			gmScript.LoadBattleView(hordeMembers);
//		}
//	}
	// *****BATTLEVIEW ****







	//for testing: Im calling this from Mouse_controls, when you click on the badge we spawn the units with the proper values from the Unit Data
//	public void Spawn(){
//		int maxMembersOfHorde = hordeMembers.Count;
//		for (int x=0; x < maxMembersOfHorde; x++) {
//			GameObject hordeSpawn = Instantiate(unitToSpawn, new Vector3(10, x, 0), Quaternion.identity) as GameObject;
//			sr = hordeSpawn.GetComponent<SpriteRenderer>();
//			sr.sprite = hordeMembers[x].mySprite;
//			AI_Enemy ai = hordeSpawn.GetComponent<AI_Enemy>();
//			Weapon weapon = hordeSpawn.GetComponentInChildren<Weapon>();
//			weapon.ForcedInit(hordeMembers[x].rateOfFire, hordeMembers[x].shortDamage, hordeMembers[x].midDamage, hordeMembers[x].longDamage); 
//			ai.ForceInit((Battle_Unit.Quality)hordeMembers[x].myQuality, hordeMembers[x].myName, hordeMembers[x].myStats, (Battle_Unit.Allegiance) hordeMembers[x].myAllegiance);
//		}
//	}
}
