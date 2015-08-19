using UnityEngine;
using System.Collections;

public class Camera_MoveHandler : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	Vector3 target;
	
	void Start(){

	}
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (2)) {
			Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			target = m;
		} else {
			target = Vector3.zero;
		}

		if (target != Vector3.zero)
		{
			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target);
			Vector3 delta = target - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
		
	}
}
