using UnityEngine;
using System.Collections;

public class Drone : MonoBehaviour {
	/// <summary>
	/// Drones spawn near a Control Tower. The tower has an area of effect (circle collider) that detects hordes.
	/// If it detects a Horde it sends a message to all Drones to move to the Horde.
	/// When a Drone touches a Horde it HITS it.
	/// 
	/// Drone needs:
	/// Rigidbody to collide with Hordes and cause a bit of bounce.
	/// Circle collider tight around their sprite (trigger for getting hit by a horde)
	/// ^ OnTriggerEnter2D if a Horde enters stop moving and do some damage
	/// </summary>

//	Rigidbody2D rb;
//
//	Vector3 target;



	void Start () {
//		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetMouseButton (0)) {
//			target = SelectTarget();
////			Debug.Log(target);
//		}
	}

//	void FixedUpdate(){
//		if (target != Vector3.zero) {
//			MoveToTarget (target);
//		}
//	}

//	Vector3 SelectTarget(){
//		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//		Vector3 direction = (transform.position - m);
//		return direction.normalized;
//	}
//
//	void MoveToTarget(Vector3 newTarget){
//		Vector2 t = new Vector2 (-newTarget.x * 10, -newTarget.y * 10);
////		Debug.Log ("new t: " + t);
//		rb.AddForce (t, ForceMode2D.Impulse);
//		target = Vector3.zero;
//	}


}
