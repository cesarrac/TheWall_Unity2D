using UnityEngine;
using System.Collections;

public class DestroyResource : MonoBehaviour {

	// ANY RESOURCE TILE THAT ENTERS THIS COLLIDER WILL BE DESTROYED

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.tag == "Tile") {
			Destroy(coll.gameObject);
		}
	}
}
