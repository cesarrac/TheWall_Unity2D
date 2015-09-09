using UnityEngine;
using System.Collections;

public class Camera_MoveHandler : MonoBehaviour {
	private float rightBound;
	private float leftBound;
	private float topBound;
	private float bottomBound;
	public ResourceGrid grid; // for keeping camera within map size
	Vector3 pos;

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	Vector3 target;

	float vertExtent, horzExtent;

	public float mapX, mapY;

	void Start(){
		vertExtent = Camera.main.orthographicSize;  

		horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;

		leftBound = (horzExtent - (mapX / 2.0f)) + horzExtent / 2;
		
		rightBound = (horzExtent + (mapX / 2.0f)) - horzExtent /2;
		
		topBound = (vertExtent + (mapY / 2.0f)) + vertExtent / 2;
		
		bottomBound = (vertExtent - (mapY / 2.0f)) + vertExtent;
	
	}
	// Update is called once per frame
	void Update () {
	
//		target = transform.position;

		float inputX = Input.GetAxis ("Horizontal");
		float inputY = Input.GetAxis ("Vertical");

		Vector3 move = new Vector3(Mathf.Round(inputX), Mathf.Round( inputY), 0);

//		Vector3.Normalize (move);


//		if (move != Vector3.zero) {
//			target += move * 8f;
//		}
		transform.position += move * 4f *  Time.deltaTime;
//
		// clamp movement
		transform.position = new Vector3 (Mathf.Clamp (transform.position.x, (leftBound + horzExtent / 2) - 10f, (rightBound - horzExtent / 2) + 10f),
		                                 Mathf.Clamp(transform.position.y, vertExtent /2, topBound - vertExtent /2 + 10f), 
		                                  -10f);



//		if (Input.GetMouseButton (2)) {
//			Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//			target = m;
//		}else {
//			target = Vector3.zero;
//		}

//		if (move != Vector3.zero)
//		{
//
//
//			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target);
//			Vector3 delta = target - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
//			Vector3 destination = transform.position + delta;
//			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
//			//		// clamp movement
//			transform.position = new Vector3 (Mathf.Clamp (transform.position.x, leftBound + horzExtent / 2, rightBound - horzExtent / 2),
//			                                 Mathf.Clamp(transform.position.y, 9f + vertExtent /2, topBound - vertExtent /2), 
//			                                  -10f);
//		}
		
	}
}
