using UnityEngine;
using System.Collections;

public class DroneController : MonoBehaviour {
	/// <summary>
	/// The transform holding this class is going to be the Group of 3 drones.
	/// When I click somewhere I want it to move as a group.
	/// When a Horde enters my circle collider, move all the Drones to the target 
	/// and fill a Horde global variable. Once that Horde is dead or it exits my 
	/// collider, attack is false and the Drones will move back to their original position.
	/// </summary>

	private Vector3 velocity = Vector3.zero;

	public Vector3 target;

	public float xMin, xMax, yMin, yMax; // Boundaries for clamping the movement

	// My drones as an array of Transforms
	public Transform[] drones;

	public bool selectingSpawnPoint; // turns true when Player accesses a Drone Tower to move the drones

	public bool attacking; // turns true when this object's collider detects a Horde

	public Horde hordeAttacker; // holds the Horde that is attacking

	Vector3 firstDronePos0, firstDronePos1, firstDronePos2; // store drone Positions on Start and when Group moves

	public bool idling; // this is true whenever im not attacking
	bool justSpawned;

	void Awake () {
		// get the boundaries
				// since they are spawned at the bottom of the tower yMax and ymin have to be adjusted
		xMin = transform.position.x - 1.0f;
		xMax = transform.position.x + 0.5f;
		yMin = transform.position.y - 0.4f;
		yMax = transform.position.y + 1.0f;

		firstDronePos0 = drones [0].position;
		firstDronePos1 = drones [1].position;
		firstDronePos2 = drones [2].position;

		// Starts Idle
		idling = true;

		justSpawned = true;
	}

//	void OnMouseOver(){
//		if (Input.GetMouseButtonDown (0)) {
//			selectingSpawnPoint = true;
//		}
//	}
	
	void Update () {

		if (drones [0] != null) {
			if (selectingSpawnPoint) {
				justSpawned = false;
				if (Input.GetMouseButtonUp (0)) {
					// Select a Target
					target = SelectTarget ();
				}
				// make sure we continue to move
				if (target != Vector3.zero && transform.position != target) {
					MoveGroup (target);
				} else if (transform.position == target) {
					// stop moving
					target = Vector3.zero;
					selectingSpawnPoint = false;
				}
			} else if (idling && !justSpawned) {
				if (drones [0].position != firstDronePos0) {
					MoveBackToOriginalPositions ();
				}
			} else {
				
				if (attacking && hordeAttacker != null && !idling) {
					justSpawned = false;
					Vector3 hordeTarget = new Vector3 (hordeAttacker.transform.position.x, hordeAttacker.transform.position.y, 0.0f);
					if (drones [0].position != hordeTarget) {
						MoveIndividualDrones (drones [0], hordeTarget - drones [0].localPosition);
						if (drones [1] != null) {
							MoveIndividualDrones (drones [1], hordeTarget - drones [1].localPosition);
						}
						if (drones [2] != null) {
							MoveIndividualDrones (drones [2], hordeTarget - drones [2].localPosition);
						}


					}
					
				} else if (hordeAttacker == null) {
					attacking = false;
					// AFTER A HORDE EXITS COLLIDER, Move the drones back to original positions
				
					idling = true;

				}
			}
		}// end if drones != null
		else {
			Destroy(gameObject);
		}

	
	}

	Vector3 SelectTarget(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		// Clamp the value of target to the boundaries set on Start
		Vector3 mWithNoZ = new Vector3 (Mathf.Clamp(m.x, xMin, xMax), Mathf.Clamp(m.y, yMin, yMax), 0.0f);
		return mWithNoZ;
	}

				// Here we are moving the group as a whole, altering its transform
	void MoveGroup(Vector3 newTarget){
		Vector3 t = new Vector3 (newTarget.x, newTarget.y, 0);
		transform.position = Vector3.SmoothDamp (transform.position, t, ref velocity, 0.18f);
		// Update the spawn positions for each drone
		firstDronePos0 = drones [0].position;
		if (drones[1] != null){
			firstDronePos1 = drones [1].position;
		}
		if (drones[2] != null){
			firstDronePos2 = drones [2].position;
		}
	}
	
	void MoveIndividualDrones(Transform drone, Vector3 newTarget){
		Vector3 t = new Vector3 (drone.localPosition.x + newTarget.x, drone.localPosition.y + newTarget.y, 0);
		drone.position = Vector3.SmoothDamp (drone.position, t, ref velocity, 0.3f);
	}

	void MoveBackToOriginalPositions(){
		MoveIndividualDrones (drones [0], firstDronePos0 - drones[0].localPosition);
		if (drones[1] != null){
			MoveIndividualDrones(drones[1], firstDronePos1 - drones[1].localPosition);
		}
		if (drones[2] != null){
			MoveIndividualDrones(drones[2], firstDronePos2 - drones[2].localPosition);
		}
	}

	// DETECT HORDES
	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Badge")) {
			attacking = true;
			// Not idling anymore!
			idling = false;
			if (hordeAttacker == null){
				hordeAttacker = coll.gameObject.GetComponent<Horde>();
			}

			
		}
	}
	void OnTriggerExit2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Badge")) {
			attacking = false;
//			hordeAttacker = null;
			idling = true;
		}
	}
}
