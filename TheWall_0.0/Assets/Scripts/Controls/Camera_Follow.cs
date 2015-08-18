using UnityEngine;
using System.Collections;

public class Camera_Follow : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	Transform target;

	// have two targets that the Camera can follow
	// One will be the town tile the player last selected / the initial town tile
	Transform townTarget;
	// a bool to snap the camera back to town when scouting is done
	public bool scouting;
	// Access to the map manager to spawn tiles around mouse
	Map_Manager mapScript; // when can just get the transform from this object anyway

	void Start(){
		// store the initial map manager position
		GameObject mapMan = GameObject.FindGameObjectWithTag ("Map_Manager");
		mapScript = mapMan.GetComponent<Map_Manager> ();
		townTarget = mapMan.GetComponent<Transform> ();
		target = townTarget;
	}

//	void Update () 
//	{
//		if (!scouting) {
//			mapScript.ClearScoutedTiles();
//			if (target) {
//				dampTime = 0.15f;
//				Vector3 point = GetComponent<Camera> ().WorldToViewportPoint (target.position);
//				Vector3 delta = target.position - GetComponent<Camera> ().ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
//				Vector3 destination = transform.position + delta;
//				transform.position = Vector3.SmoothDamp (transform.position, destination, ref velocity, dampTime);
//			} else {
//				target = GameObject.FindGameObjectWithTag ("Map_Manager").GetComponent<Transform> ();
//			}
//		} else {
//
//		}
//
//	}
//
	public void ScoutCam(Vector3 mousePos){
		dampTime = 1f;
		Vector3 point = GetComponent<Camera> ().WorldToViewportPoint (mousePos);
		Vector3 delta = mousePos - GetComponent<Camera> ().ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
		Vector3 destination = transform.position + delta;
		transform.position = Vector3.SmoothDamp (transform.position, destination, ref velocity, dampTime);
			
		mapScript.SpawnTilesForScout (mousePos);
	
	}
}
