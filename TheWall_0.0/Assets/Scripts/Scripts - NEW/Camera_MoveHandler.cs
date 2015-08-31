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
//		float horzExtent = vertExtent * Screen.width / Screen.height;
//	
//		leftBound = (float)(horzExtent - grid.mapSizeX/ 2.0f);
//		rightBound = (float)(grid.mapSizeX / 2.0f - horzExtent);
//		bottomBound = (float)(vertExtent - grid.mapSizeY / 2.0f);
//		topBound = (float)(grid.mapSizeY  / 2.0f - vertExtent);
		horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
//		leftBound = (transform.position.x + (float)grid.mapSizeX / 2) - horzExtent;
//		rightBound = (transform.position.x - (float)grid.mapSizeX / 2) + horzExtent;
//		bottomBound = (transform.position.y + (float)grid.mapSizeY / 2) - vertExtent;
//		topBound = (transform.position.y - (float)grid.mapSizeY / 2) + vertExtent;
	
	}
	// Update is called once per frame
	void Update () {
//		leftBound = (horzExtent - (mapX / 2.0f)) + horzExtent / 2;
//		rightBound = mapX / 2.0f - horzExtent;
//		bottomBound = vertExtent - mapY / 2.0f;
//		topBound = mapY / 2.0f - vertExtent;
//		leftBound = transform.position.x - horzExtent / 2;
//		bottomBound = transform.position.y - vertExtent / 2;
//		topBound = transform.position.y + vertExtent / 2;
//		rightBound = transform.position.x + horzExtent / 2;
		//		float inputY = Mathf.Clamp( Input.GetAxis ("Vertical"), bottomBound, topBound);
//		float inputX = Mathf.Clamp(Input.GetAxis ("Horizontal"), leftBound, rightBound);
		float inputX = Input.GetAxis ("Horizontal");
		float inputY = Input.GetAxis ("Vertical");
		Vector3 move = new Vector3(inputX, inputY, 0);

		transform.position += move * 8f * Time.deltaTime;

		// clamp movement
//		transform.position = new Vector3(Mathf.Clamp(transform.position.x, leftBound, rightBound), 
//		                                 Mathf.Clamp(transform.position.y, bottomBound, topBound), -10f);


//		if (Input.GetMouseButton (2)) {
//			Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//			target = m;
//		}else {
//			target = Vector3.zero;
//		}
//
//		if (target != Vector3.zero)
//		{
////			pos.x = Mathf.Clamp(target.x, leftBound, rightBound);
////			pos.y = Mathf.Clamp(target.y, bottomBound, topBound);
//
//			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target);
//			Vector3 delta = target - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
//			Vector3 destination = transform.position + delta;
//			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
//		}
		
	}
}
