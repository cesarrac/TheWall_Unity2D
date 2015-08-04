using UnityEngine;
using System.Collections;

public class DayNight_Cycler : MonoBehaviour {


// Rotates a directional light on its Y axis and holds while at 0 (NOON) for a time, then continues rotating,
	// and stops again at 180 (NIGHT), then continues to rotate in the opposite direction and so on

	public float timeSunStops;
	public float night = 180.0f;
	public float noon = 0.0f;
	float positiveRot = 1f, negativeRot = -1f;
	bool dayGoesOn;
	Transform myTransform;
	Quaternion myRot;
	bool rotateSun;

	void Start () {
		myTransform = transform;
		myRot = myTransform.rotation;
		Debug.Log ("rot: " + myRot);
		dayGoesOn = true;
	}
	

	void Update () {


		if (dayGoesOn) {
		
			StartCoroutine (SunStops ());
		
		} else if (rotateSun) {
			myTransform.Rotate(Vector3.up * 180);
		}

	}
	
	IEnumerator SunStops(){
		// waits during the day / night
		dayGoesOn = false;
		yield return new WaitForSeconds(timeSunStops);
		rotateSun = true;


		
	}

	void Rotate(bool trueForClockWise){
		Debug.Log ("Rotating sun!");
		float myRotY = myRot.y;
		if (trueForClockWise) {
			// goes from 180 back to 0
			if (myRotY > 0) {
				Quaternion newRot = new Quaternion (0, 0, 0, 0);
				myRot = Quaternion.RotateTowards(myRot, newRot, 0);
				// keep calling itself until it's done

			}else{
				dayGoesOn = true;
			}
		} else {
			// go from 0 to 180
			if (myRotY < 180) {
				myRotY++;
				myRot = new Quaternion (0, myRotY, 0, 0);
				// keep calling itself until it's done

			}else{
				dayGoesOn = true;
			}
		}
	}
}
