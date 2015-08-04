using UnityEngine;
using System.Collections;

public class CombatIndicator : MonoBehaviour {

	public Transform tileUnderAttack;
	Transform myTransform;

	// Use this for initialization
	void Start () {
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		Rotate ();
	}

	void Rotate(){
		if (tileUnderAttack != null) {
			Vector3 targetPos = tileUnderAttack.position;
			Quaternion rot = Quaternion.LookRotation (myTransform.position - targetPos, Vector3.forward);
			myTransform.rotation = rot;
			Vector3 facingRot = new Vector3 (0, 0, -myTransform.eulerAngles.z);
			myTransform.eulerAngles = facingRot;
		} else {
			Destroy(this.gameObject);
		}
	
	}
}
