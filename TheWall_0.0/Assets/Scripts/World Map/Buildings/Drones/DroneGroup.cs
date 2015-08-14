using UnityEngine;
using System.Collections;

public class DroneGroup : MonoBehaviour {
	/// <summary>
	/// This holds a group of 3 drones as an array of Rigidbody2D. When it gets a target it applies force
	/// to each one of the drones in the direction of the target.
	/// Select a Position to move (when you have Drone tower selected) and click to select target for movement
	/// When a Horde enters this group's collider, all Drones go to attack
	/// </summary>

	public Rigidbody2D[] dronesRB;

	Rigidbody2D rb;

	Vector3 target = Vector3.zero;

	public float speed; // movement speed

	Transform boundaryTrans;

	public float xMin, xMax, yMin, yMax; // Boundaries for clamping the movement

	Vector3 originalPosition;
	Vector3 pos1, pos2, pos3;

	public bool attacking;

	Transform myTransform;
	private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		boundaryTrans = transform;
		myTransform = transform;
		// get the boundaries
		xMin = boundaryTrans.position.x - 1.5f;
		xMax = boundaryTrans.position.x + 1.5f;
		yMin = boundaryTrans.position.y - 1.5f;
		yMax = boundaryTrans.position.y + 1.5f;

		originalPosition = transform.position;

		rb = GetComponent<Rigidbody2D> ();

		pos1 = dronesRB [0].transform.position;
		pos2 = dronesRB [0].transform.position;
		pos3 = dronesRB [0].transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			if (attacking){
				MoveBackToOriginalPosition();
			}else{
				target = SelectTarget();
			}
		}

		if (!attacking && target != Vector3.zero) {
			MovementByClick (target);
		} 
		if (myTransform.position == target) {
			target = Vector3.zero;
		}

	}

	void FixedUpdate(){
//		if (target != Vector3.zero && !attacking) {
//			MovementByClick (dronesRB [0], target, false);
//			MovementByClick (dronesRB [1], target, false);
//			MovementByClick (dronesRB [2], target, true);
//		} else if (attacking) {
//			MoveToTarget (dronesRB [0], target);
//			MoveToTarget (dronesRB [1], target);
//			MoveToTarget (dronesRB [2], target);
//		}
		if (attacking) {
			//wake up rigidbodies
			if (dronesRB[0].IsSleeping() || dronesRB[1].IsSleeping() || dronesRB[2].IsSleeping()){
				dronesRB[0].WakeUp();
				dronesRB[1].WakeUp();
				dronesRB[2].WakeUp();
			} 
		
			MoveToTarget (dronesRB [0], target);
			MoveToTarget (dronesRB [1], target);
			MoveToTarget (dronesRB [2], target);
		}
	}

//	Vector3 SelectTarget(){
//		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//		Vector3 direction = (transform.position - m);
//		return direction.normalized;
//	}

	Vector3 SelectTarget(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (m.x > xMax) {
			m.x = xMax;
		}
		if (m.x < xMin) {
			m.x = xMin;
		}
		if (m.y > yMax) {
			m.y = yMax;
		}
		if (m.y < yMin) {
			m.y = yMin;
		}
		return m;
	}


	
	void MovementByClick(Vector3 newTarget){
		Debug.Log ("Moving by click!");
		Vector3 t = new Vector3 (newTarget.x, newTarget.y, 0);
		transform.position = Vector3.SmoothDamp (transform.position, t, ref velocity, 0.2f);
	}

	void MoveToTarget(Rigidbody2D rb, Vector3 newTarget){
		Debug.Log ("Moving to Horde!");
		Vector3 movement = new Vector3 (newTarget.x, newTarget.y,0.0f);
		rb.velocity = movement * speed;
		
		rb.position = new Vector3 (Mathf.Clamp (rb.position.x, xMin, xMax), Mathf.Clamp (rb.position.y, yMin, yMax), 0.0f);
	}

	void MoveBackToOriginalPosition(){
		attacking = false;
		dronesRB[0].Sleep();
		dronesRB[1].Sleep();
		dronesRB[2].Sleep();
		dronesRB[0].transform.position = Vector3.SmoothDamp (dronesRB[0].transform.position, pos1, ref velocity, 0.2f);
		dronesRB[1].transform.position = Vector3.SmoothDamp (dronesRB[1].transform.position, pos2, ref velocity, 0.2f);
		dronesRB[2].transform.position = Vector3.SmoothDamp (dronesRB[2].transform.position, pos3, ref velocity, 0.2f);
	}




	void OnTriggerStay2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Badge")) {
			attacking = true;
			// if a Horde is detected, make the Horde the target to move
			target = coll.gameObject.transform.position;
		
		} 
	}

	void OnTriggerExit2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Badge")) {
			// if a Horde is detected, make the Horde the target to move

			attacking = false;
			MoveBackToOriginalPosition();
			Debug.Log ("target exits");

		}
	}
}
