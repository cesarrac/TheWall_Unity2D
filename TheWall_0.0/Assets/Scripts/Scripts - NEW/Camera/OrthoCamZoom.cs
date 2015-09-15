using UnityEngine;
using System.Collections;

public class OrthoCamZoom : MonoBehaviour {

	private Camera _camera;

	private float _startOrthoSize, _zoomOrthoSize = 6.9f;
	public float curOrthoSize;

	void Start () {
		_camera = this.GetComponent<Camera> ();
		if(!_camera){
			Debug.LogWarning("No camera for Ortho Zoom cam to use");
		}else{
			_startOrthoSize = _camera.orthographicSize;
			curOrthoSize = _startOrthoSize;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetMouseButtonDown (1)) {
			if (curOrthoSize == _startOrthoSize){
				_camera.orthographicSize = _zoomOrthoSize;
				curOrthoSize = _camera.orthographicSize;
			}else{
				_camera.orthographicSize = _startOrthoSize;
				curOrthoSize = _camera.orthographicSize;
			}
		}

	}
}
