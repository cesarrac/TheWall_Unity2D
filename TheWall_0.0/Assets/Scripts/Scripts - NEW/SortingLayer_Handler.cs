using UnityEngine;
using System.Collections;

public class SortingLayer_Handler : MonoBehaviour {
	/// <summary>
	/// Anything that enters this collider, if it's above me, will set my sorting layer to be on top of theirs.
	/// </summary>

	SpriteRenderer sr;
	int originalSorting;

	void Awake(){
		sr = GetComponent<SpriteRenderer> ();
		originalSorting = sr.sortingOrder;
	}

	void OnTriggerEnter2D(Collider2D coll){
		Debug.Log ("entered trigger");
		if (coll.transform.position.y > transform.position.y) {
			sr.sortingOrder = 1;
		}
//		if (coll.transform.position.y < transform.position.y) {
//			if (coll.gameObject.GetComponent<SpriteRenderer>() != null){
//				coll.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
//			}else if (coll.gameObject.GetComponentInParent<SpriteRenderer>() != null){ // check the parent
//				coll.gameObject.GetComponentInParent<SpriteRenderer>().sortingOrder = 2;
//			}
//		}
	}

	void OnTriggerExit2D(Collider2D coll){
		if (coll.transform.position.y > transform.position.y) {
			sr.sortingOrder = originalSorting;
		}
	}
}
