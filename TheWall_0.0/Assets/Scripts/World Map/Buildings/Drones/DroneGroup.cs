using UnityEngine;
using System.Collections;

public class DroneGroup : MonoBehaviour {
	Rigidbody2D rb;
	
	Vector3 target;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)) {
			target = SelectTarget();
			//			Debug.Log(target);
		}
	}

	void FixedUpdate(){
		if (target != Vector3.zero) {
			MoveToTarget (target);
		}
	}

	Vector3 SelectTarget(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 direction = (transform.position - m);
		return direction.normalized;
	}
	
	void MoveToTarget(Vector3 newTarget){
		Vector2 t = new Vector2 (-newTarget.x * 10, -newTarget.y * 10);
		//		Debug.Log ("new t: " + t);
		rb.AddForce (t, ForceMode2D.Impulse);
		target = Vector3.zero;
	}
}
