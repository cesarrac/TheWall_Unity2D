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

	
	void Start(){
		float vertExtent = Camera.main.orthographicSize;  
		float horzExtent = vertExtent * Screen.width / Screen.height;
	
		leftBound = (float)(horzExtent - grid.mapSizeX/ 2.0f);
		rightBound = (float)(grid.mapSizeX / 2.0f - horzExtent);
		bottomBound = (float)(vertExtent - grid.mapSizeY / 2.0f);
		topBound = (float)(grid.mapSizeY  / 2.0f - vertExtent);
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
//			pos.x = Mathf.Clamp(target.x, leftBound, rightBound);
//			pos.y = Mathf.Clamp(target.y, bottomBound, topBound);

			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target);
			Vector3 delta = target - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
		
	}
}
