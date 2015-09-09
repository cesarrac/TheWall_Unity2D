using UnityEngine;
using System.Collections;

public class PixelPerfectScale : MonoBehaviour {

	public int screenVerticalPixels = 256;

	public bool preferUncropped = true;

	private float screenPixelsY = 0;

	private bool currentCropped = false;
	
	void Update () 
	{
		if (screenPixelsY != (float)Screen.height || currentCropped != preferUncropped) {
			screenPixelsY = (float)Screen.height;
			currentCropped = preferUncropped;

			float screeRatio = screenPixelsY / screenVerticalPixels;
			float ratio;

			if (preferUncropped){
				ratio = Mathf.Floor(screeRatio)/screeRatio;
			}else{
				ratio = Mathf.Ceil(screeRatio)/screeRatio;
			}

			transform.localScale = Vector3.one * ratio;
		}
	}
}
