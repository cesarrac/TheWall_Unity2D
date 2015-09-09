using UnityEngine;
using System.Collections;

public class DroneWpn : MonoBehaviour {
	public float damage;

	public float hitPoints;

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.CompareTag("Badge")) {
			Horde horde = coll.gameObject.GetComponent<Horde>();
			horde.TakeDamage(damage: 5f);
			Debug.Log("Hit for damage: " + damage);
		}

	}

	public void TakeDamage(float damage){
		hitPoints = hitPoints - damage;
		if (hitPoints <= 0) {
			Die();
		}
	}

	void Die(){
		Destroy (gameObject);
	}
}
