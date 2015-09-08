using UnityEngine;
/**
 * A camera to help with Orthagonal mode when you need it to lock to pixels.  Desiged to be used on android and retina devices.
 */
public class PixelPerfectCam : MonoBehaviour {
	/**
	 * The target size of the view port.
	 */
	public Vector2 targetViewportSizeInPixels = new Vector2(1424.0f, 890.0f);
	/**
	 * Snap movement of the camera to pixels.
	 */
	public bool lockToPixels = true;
	/**
	 * The number of target pixels in every Unity unit.
	 */
	public float pixelsPerUnit = 32.0f;
	/**
	 * A game object that the camera will follow the x and y position of.
	 */
//	public GameObject followTarget;
	Vector3 target;
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	
	private Camera _camera;
	private int _currentScreenWidth = 0;
	private int _currentScreenHeight = 0;
	
	private float _pixelLockedPPU = 32.0f;
	private Vector2 _winSize;

	float vertExtent, horzExtent, leftBound, rightBound, bottomBound, topBound;
	public float mapX, mapY;
	
	protected void Start(){

		_camera = this.GetComponent<Camera>();
		if(!_camera){
			Debug.LogWarning("No camera for pixel perfect cam to use");
		}else{
			_camera.orthographic = true;
			ResizeCamToTargetSize();
		}
	}
	
	public void ResizeCamToTargetSize(){
		if(_currentScreenWidth != Screen.width || _currentScreenHeight != Screen.height){
			// check our target size here to see how much we want to scale this camera
			float percentageX = Screen.width/targetViewportSizeInPixels.x;
			float percentageY = Screen.height/targetViewportSizeInPixels.y;
			float targetSize = 0.0f;
			if(percentageX > percentageY){
				targetSize = percentageY;
			}else{
				targetSize = percentageX;
			}
			int floored = Mathf.FloorToInt(targetSize);
			if(floored < 1){
				floored = 1;
			}
			// now we have our percentage let's make the viewport scale to that
			float camSize = ((Screen.height/2)/floored)/pixelsPerUnit;
			_camera.orthographicSize = camSize;
			_pixelLockedPPU = floored * pixelsPerUnit;


			vertExtent = camSize;  
			
			horzExtent = camSize * Screen.width / Screen.height;
			
			leftBound = (horzExtent - (mapX / 2.0f)) + horzExtent / 2;
			
			rightBound = (horzExtent + (mapX / 2.0f)) - horzExtent /2;
			
			topBound = (vertExtent + (mapY / 2.0f)) + vertExtent / 2;
			
			bottomBound = (vertExtent - (mapY / 2.0f)) + vertExtent;
		}
		_winSize = new Vector2(Screen.width, Screen.height);
	}
	
	public void Update(){

		if(_winSize.x != Screen.width || _winSize.y != Screen.height){
			ResizeCamToTargetSize();
		}
//		if(_camera && followTarget){
//			Vector2 newPosition = new Vector2(followTarget.transform.position.x, followTarget.transform.position.y);
//			float nextX = Mathf.Round(_pixelLockedPPU * newPosition.x);
//			float nextY = Mathf.Round(_pixelLockedPPU * newPosition.y);
//			_camera.transform.position = new Vector3(nextX/_pixelLockedPPU, nextY/_pixelLockedPPU, _camera.transform.position.z);
//		}

//		float inputX = Input.GetAxis ("Horizontal");
//		float inputY = Input.GetAxis ("Vertical");
//		if (_camera) {
//			if (inputX > 0 || inputX < 0 || inputY > 0 || inputY < 0){
//			
//				Vector3 move = new Vector3 (_camera.transform.position.x + inputX, _camera.transform.position.y + inputY, 0);
//				float nextX = Mathf.Round(_pixelLockedPPU * move.x);
//				float nextY = Mathf.Round(_pixelLockedPPU * move.y);
//
//				target = new Vector3 (Mathf.Clamp (nextX / pixelsPerUnit, (leftBound + horzExtent / 2), (rightBound - horzExtent / 2)),
//				                      Mathf.Clamp(nextY / pixelsPerUnit, vertExtent /2, topBound - vertExtent /2 + 10f), 
//				                      -10f);
//
////				target = new Vector3(nextX/_pixelLockedPPU, nextY/_pixelLockedPPU, 0);
//
//			}else {
//				target = Vector3.zero;
//			}
//		}
//
//		if (_camera && target != Vector3.zero) {
//			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target);
//			Vector3 delta = target - _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); 
//			Vector3 destination = _camera.transform.position + delta;
////			_camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, destination, ref velocity, dampTime);
//			_camera.transform.position = Vector3.MoveTowards(_camera.transform.position, destination, 6f * Time.deltaTime);
//
//		}
	}
}