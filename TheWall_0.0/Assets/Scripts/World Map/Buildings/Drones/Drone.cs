using UnityEngine;
using System.Collections;

public class Drone : MonoBehaviour {
	public float damage;
	
	public float hitPoints;
	
	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.CompareTag("Badge")) {
			Horde horde = coll.gameObject.GetComponent<Horde>();
			if (!horde.nextToEnemy) {
				horde.myDrone = GetComponent<Drone>();
				horde.nextToEnemy = true;
			}
			horde.TakeDamage(damage: 5f);
			Debug.Log("Hit for damage: " + damage);
		}
		
	}

	void OnTriggerStay2D(Collider2D coll) {
		if (coll.gameObject.CompareTag("Badge")) {
			Horde horde = coll.gameObject.GetComponent<Horde>();
			if (!horde.nextToEnemy) {
				horde.myDrone = GetComponent<Drone>();
				horde.nextToEnemy = true;
			}
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
//	void OnMouseOver(){
//		if (Input.GetMouseButtonDown (0)) {
//			DroneController dC = GetComponentInParent<DroneController>();
//			if (!dC.attacking && !dC.selectingSpawnPoint){
//				dC.selectingSpawnPoint = true;
//			}
//		}
//	}

//	void OnTriggerExit2D (Collider2D coll){
//		if (coll.CompareTag ("Badge")) {
//			Horde horde = coll.GetComponent<Horde>();
//			horde.nextToEnemy = false;
//
//		}
//		
//	}

}
