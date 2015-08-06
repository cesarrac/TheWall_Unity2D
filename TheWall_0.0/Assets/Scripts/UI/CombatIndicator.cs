using UnityEngine;
using System.Collections;

public class CombatIndicator : MonoBehaviour {

	public Transform tileUnderAttack;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Rotate ();
	}

	void Rotate(){
		if (tileUnderAttack != null) {
			Vector3 targetPos = tileUnderAttack.position;
//			float z = Mathf.Atan2((Mathf.Abs(myTransform.position.y) - Mathf.Abs(targetPos.y)), (Mathf.Abs(myTransform.position.x) - Mathf.Abs(targetPos.x))) * Mathf.Rad2Deg;		
//			myTransform.eulerAngles = new Vector3 (0,0,z);

			float z = Mathf.Atan2((targetPos.y - transform.position.y), (targetPos.x - transform.position.x)) * Mathf.Rad2Deg - 90;		
			transform.rotation = Quaternion.AngleAxis(z, Vector3.forward);

		} else {
			Destroy(this.gameObject);
		}
	
	}
}
