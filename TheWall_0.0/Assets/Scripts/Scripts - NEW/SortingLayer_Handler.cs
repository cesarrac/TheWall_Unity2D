using UnityEngine;
using System.Collections;

public class SortingLayer_Handler : MonoBehaviour {
	/// <summary>
	/// Sort the Sprite layers so objects lower on scree appear above objects that are higher.
	/// </summary>

	SpriteRenderer sr;
	string above = "Above", middle = "Middle", below = "Below";
	string myLayer;

	public GameObject objectAbove, objectBelow; // set the first time they enter trigger

	void Awake(){
		sr = GetComponent<SpriteRenderer> ();
		myLayer = sr.sortingLayerName;
	}

	void OnTriggerEnter2D(Collider2D coll){
		Debug.Log ("entered trigger");
		if (coll.gameObject.CompareTag ("Building")) {
			// Get its current sorting layer
			string nameOfOthersLayer = GetSortingLayer (coll.gameObject);
			if (nameOfOthersLayer != "none") {
				// Check if its above me
				if (coll.transform.position.y > transform.position.y) {

					sr.sortingOrder = GetSortingOrder (coll.gameObject) + 1;

					if (objectBelow != null) {
						// set the one above me to be -1 as well
						int calc = sr.sortingOrder + 1;
						GetSprite (objectBelow).sortingOrder = calc;
						
					}
					 //store this as an object above to later be able to change its layer if necessary
					if (objectAbove != null) {
						SpriteRenderer otherSr = GetSprite (objectAbove);
						// if the objectabove sorting layer is higher than mine, set mine to be even higher
						if (otherSr.sortingOrder > sr.sortingOrder){
							sr.sortingOrder = otherSr.sortingOrder + 1;
						}
					} else {
						objectAbove = coll.gameObject;
					}
//				if (nameOfOthersLayer == above){
//					SetOtherLayer(coll.gameObject, middle, 0);
//					// then set my layer to be above if im not middle
//					if (myLayer != middle){
//						myLayer = above;
//					}
//				}
//				if (nameOfOthersLayer == below){
//					// set me to above
//					myLayer = above;
//				}
				}
				// Check if its below me
				if (coll.transform.position.y < transform.position.y) {

					sr.sortingOrder = GetSortingOrder (coll.gameObject) - 1;

					if (objectBelow != null) {
						// set the one above me to be -1 as well
						int calc = sr.sortingOrder + 2;
						GetSprite (objectBelow).sortingOrder = calc;
					
					} else {
						objectBelow = coll.gameObject;
					} 
				 

//				if (objectBelow == null) {
//					objectBelow = coll.gameObject;
//				}
//
//				if (nameOfOthersLayer == middle){
//					myLayer = below;
//				}
//				if (nameOfOthersLayer == above){
//					myLayer = middle;
//				}
//				if (nameOfOthersLayer == below){
//					SetOtherLayer(coll.gameObject, below, 1);
//					// also set the other objects below
//					if (objectBelow != null)
//						SetOtherLayer(objectBelow, below, 1);
//				}
				}
			}

		}
	}

//	void OnTriggerExit2D(Collider2D coll){
////		if (coll.transform.position.y > transform.position.y) {
////			sr.sortingOrder = originalSorting;
////		}
//	}
	SpriteRenderer GetSprite(GameObject other){
		if (other.GetComponent<SpriteRenderer> () != null) {
			return other.GetComponent<SpriteRenderer> ();
		} else if (other.GetComponentInParent<SpriteRenderer> () != null) { // check the parent
			return other.GetComponentInParent<SpriteRenderer> ();
		} else {
			return null;
		}
	}

	int GetSortingOrder (GameObject other){

		if (other.GetComponent<SpriteRenderer> () != null) {
			return other.GetComponent<SpriteRenderer> ().sortingOrder;
		} else if (other.GetComponentInParent<SpriteRenderer> () != null) { // check the parent
			return other.GetComponentInParent<SpriteRenderer> ().sortingOrder;
		} else {
			return 0;
		}
		
	}

	string GetSortingLayer (GameObject other){
		if (other.gameObject.GetComponent<SpriteRenderer> () != null) {
			return other.gameObject.GetComponent<SpriteRenderer> ().sortingLayerName;
		} else if (other.gameObject.GetComponentInParent<SpriteRenderer> () != null) { // check the parent
			return other.gameObject.GetComponentInParent<SpriteRenderer> ().sortingLayerName;
		} else {
			return "none";
		}
	}

	void SetOtherLayer(GameObject other, string layerToSet, int order){
		if (other.gameObject.GetComponent<SpriteRenderer> () != null) {
			other.gameObject.GetComponent<SpriteRenderer> ().sortingLayerName = layerToSet;
			other.gameObject.GetComponent<SpriteRenderer> ().sortingOrder += order;
		} else if (other.gameObject.GetComponentInParent<SpriteRenderer> () != null) { // check the parent
			other.gameObject.GetComponentInParent<SpriteRenderer> ().sortingOrder += order;
		}
	}
}
