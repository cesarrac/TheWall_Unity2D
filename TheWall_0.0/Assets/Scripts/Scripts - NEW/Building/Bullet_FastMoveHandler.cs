using UnityEngine;
using System.Collections;

public class Bullet_FastMoveHandler : MonoBehaviour {

	/// <summary>
	/// The bullet quickly travels to its target, if it hits it will pool itself. 
	/// If it misses for some reason, it will just pool itself.
	/// </summary>
	public float bulletSpeed;
	Rigidbody2D rb;
	public ObjectPool objPool;

	public float timeToDie;
	float startTime;

	void Awake () {
		startTime = Time.time;

		rb = GetComponent<Rigidbody2D> ();
	}
	void Update(){
		if (Time.time - startTime > timeToDie)
			objPool.PoolObject(gameObject);
	}
	
	void FixedUpdate(){
		if (rb != null)
			rb.AddForce (transform.forward * bulletSpeed);
	}
	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.CompareTag("Enemy")) {
			objPool.PoolObject(gameObject);
		}
	}
}
