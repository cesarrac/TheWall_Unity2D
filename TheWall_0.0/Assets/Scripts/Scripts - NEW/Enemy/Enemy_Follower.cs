using UnityEngine;
using System.Collections;

public class Enemy_Follower : MonoBehaviour {
	public Transform target;
	public float movementSpeed =1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Lerp (transform.position, new Vector3 (target.position.x - 1, target.position.y, 0.0f), 
		                                   movementSpeed * Time.deltaTime);
	}
}
