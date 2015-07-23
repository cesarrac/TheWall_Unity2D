using UnityEngine;
using System.Collections;

public class HordeSpawner : MonoBehaviour {
	//Spawns a Horde at the location of this capital, with specified stats, and sticks it to a holder transform

	Transform myTransform;

	public GameObject hordeFab;

	public Transform hordeHolder;

	public float timeToSpawn;

	bool spawning;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		SpawnHorde ();
	}

	void Update(){
		if (spawning) {
			StartCoroutine(CountToSpawn());
		}
	}
	IEnumerator CountToSpawn(){
		// stop counting
		spawning = false;
		// wait
		yield return new WaitForSeconds (timeToSpawn);
		// spawn
		SpawnHorde ();
	}

	public void SpawnHorde(){
		Vector3 hordePos = new Vector3 (myTransform.position.x, myTransform.position.y, 0);
		GameObject horde = Instantiate (hordeFab, hordePos, Quaternion.identity) as GameObject;
		horde.transform.parent = hordeHolder;
		Horde hordeScript = horde.GetComponent<Horde> ();
		hordeScript.hordeMembers.Add(new Unit_Data(description: "Some description.", allegiance: Unit_Data.Allegiance.monster, quality: Unit_Data.Quality.high, fireRate: 1, sDamage: 2f, mDamage: 3f, lDamage: 5f));

		spawning = true;
	}
}
