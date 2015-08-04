using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	Transform myTransform;
	public float bulletSpeed;
	Rigidbody2D rb;
	public float damage;

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
		myTransform = transform;
	}
	
	void FixedUpdate(){
//				Vector3 myP = Camera.main.WorldToScreenPoint(myTransform.position);
//		var dir = (myTransform.up - myTransform.right).normalized;
		rb.AddForce (transform.parent.up * bulletSpeed);
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.tag == "Badge") {
			Horde horde = coll.gameObject.GetComponent<Horde> ();
			horde.TakeDamage (damage);

			Destroy (gameObject);
		}
	}
}
