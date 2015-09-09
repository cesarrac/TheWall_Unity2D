using UnityEngine;
using System.Collections;

public class DroneTower : Building {
	/// <summary>
	/// This tower controls the spawning of a Drone group.
	/// When first built it will spawn the first group and store it as a GameObject
	/// The Drone group destoys itself when all 3 of its drones are gone.
	/// When that happens this tower starts a Spawn Timer, that when done, spawns a new group of drones.
	/// The player can change the position where these drones spawn by clicking on the tower, this will 
	/// activate the drone group's select bool.
	/// </summary>


	public GameObject droneGroupFab;

	public GameObject spawnedDrones;

	public bool spawning;

	public float timeToSpawn;

	public Vector3 spawnPosition, newPosition;
	private Vector3 velocity = Vector3.zero;

	public bool moving;

	Mouse_Controls mouse;
	void Start () {
		floatBonus1 = 0;
		intBonus1 = 0;
		// first Spawn them at the default spawnPosition
		spawnPosition = new Vector3(transform.position.x, transform.position.y - 1, 0);
		newPosition = spawnPosition;
		Spawn ();

		mouse = GameObject.FindGameObjectWithTag ("Map_Manager").GetComponent<Mouse_Controls> ();
		townTProps = GetComponentInParent<TownTile_Properties> ();
	}
	

	void Update () {
		attachedObject = spawnedDrones; // for destroying when tower is destroyed

		if (!spawning) {
			if (spawnedDrones == null){
				StartCoroutine(WaitForSpawn());
			}
		}
	
		if (Input.GetMouseButtonDown (0)) {
			if (spawnedDrones != null) {
				DroneController dC = spawnedDrones.GetComponent<DroneController>();
				newPosition = SelectTarget(dC.xMin, dC.xMax, dC.yMin, dC.yMax);			
			}
		}

		if (moving) {
			MoveToNewPos(newPosition);
		}


	}


	Vector3 SelectTarget(float xMin, float xMax, float yMin, float yMax){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		
		// Clamp the value of target to the boundaries set on Start
		Vector3 mWithNoZ = new Vector3 (Mathf.Clamp(m.x, xMin, xMax), Mathf.Clamp(m.y, yMin, yMax), 0.0f);
		return mWithNoZ;
	}


	IEnumerator WaitForSpawn(){

		spawning = true;
		Debug.Log ("Counting to spawn");
		yield return new WaitForSeconds(timeToSpawn);

		Spawn ();
	}

	void Spawn(){
		spawnedDrones = Instantiate (droneGroupFab, spawnPosition, Quaternion.identity) as GameObject;
		if (spawnPosition != newPosition) {
			moving = true;

		} 
		spawning = false;
	}

	void MoveToNewPos(Vector3 target){
		Debug.Log ("Moving the new spawn to the right location");
		Vector3 t = new Vector3 (target.x, target.y, 0);
		spawnedDrones.transform.position = Vector3.SmoothDamp (spawnedDrones.transform.position, t, ref velocity, 0.2f);
		if (spawnedDrones.transform.position == newPosition) {
			moving = false;
		}
	}
}
