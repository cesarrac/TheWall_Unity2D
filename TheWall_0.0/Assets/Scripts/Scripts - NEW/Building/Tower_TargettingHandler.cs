using UnityEngine;
using System.Collections;

public class Tower_TargettingHandler : Unit_Base {
	/// <summary>
	/// Tower Targetting. 
	/// Seeks Enemy units using a Linecast
	/// </summary>
	public Transform sightStart, sightEnd;

	public LayerMask mask;

	public ObjectPool objPool;

	bool canShoot, enemyInRange = false;

	public GameObject targetUnit;

	LineRenderer lineR;

	public bool starvedMode; // MANIPULATED BY THE RESOURCE MANAGER

	SpriteRenderer sr;

	void Start () {
		if (objPool == null) {
			objPool = GameObject.FindGameObjectWithTag("Pool").GetComponent<ObjectPool>();
		}

		if (GetComponentInParent<SpriteRenderer> () != null) {
			sr = GetComponentInParent<SpriteRenderer> ();
			
			lineR = sightStart.GetComponent<LineRenderer> ();
			lineR.sortingLayerName = sr.sortingLayerName;
			lineR.sortingOrder = sr.sortingOrder;
		}


	}


	void FixedUpdate () {
		if (enemyInRange && !starvedMode){
			SeekEnemies ();
		}
	}

	void SeekEnemies(){
		RaycastHit2D hit = Physics2D.Linecast (sightStart.position, sightEnd.position, mask.value);
		if (hit.collider != null) {
			if (hit.collider.CompareTag("Enemy")){
				if (targetUnit == null) {// DONT GET TARGET if you already have one!
					targetUnit = hit.collider.gameObject;
				// shoot one shot quickly then start coroutine
					VisualShooting ();
					HandleDamageToUnit ();
				}
			}
		}
	}

	void Update(){
		if (canShoot){
			StartCoroutine(WaitToShoot());
		}
//		Debug.DrawLine (sightStart.position, sightEnd.position, Color.magenta);
		if (!enemyInRange && !starvedMode) 
			sightStart.Rotate (Vector3.forward * 90 * Time.deltaTime);
	}

	IEnumerator WaitToShoot(){
		canShoot = false;
		yield return new WaitForSeconds (rateOfAttack);
		if (unitToPool != null) {
			PoolTarget(unitToPool);
		} else if (targetUnit != null){
			VisualShooting ();
			HandleDamageToUnit ();
		}

	}
	//TODO: Don't need to spawn a bullet at all!! Just create a shooting animation that starts up when it shoots!!!
	/// <summary>
	/// Gets a bullet from the pool of bullets and shoots it.
	/// The bullet itself is just for visual reference and will just Pool itself when it hits.
	/// </summary>
	void VisualShooting(){
		GameObject explosion = objPool.GetObjectForType ("Explosion Particles", false);
		if (explosion != null) {
			explosion.transform.position = targetUnit.transform.position;
		}
	}

	/// <summary>
	/// Handles the damage to unit by using
	/// method from Unit_Base class.
	/// </summary>
	void HandleDamageToUnit(){
		if (targetUnit != null) {
			Unit_Base unitToHit = targetUnit.GetComponent<Unit_Base> ();
			AttackOtherUnit (unitToHit);
			canShoot = true;
		} else {
			canShoot = false;
		}

	}

	void PoolTarget(GameObject target){
		objPool.PoolObject (target); // Pool the Dead Unit
		string deathName = "dead";
		GameObject deadE = objPool.GetObjectForType(deathName, false); // Get the dead unit object
		deadE.GetComponent<FadeToPool> ().objPool = objPool;
		deadE.transform.position = unitToPool.transform.position;
		unitToPool = null;
		// if we are pooling it means its dead so we should check for target again
		targetUnit = null;
		enemyInRange = false;
	}

	void OnTriggerStay2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Enemy") && targetUnit == null) {
			// Rotate to the new target
			float z = Mathf.Atan2 ((coll.transform.position.y - sightStart.position.y), (coll.transform.position.x - sightStart.position.x)) * Mathf.Rad2Deg - 90;		
			sightStart.rotation = Quaternion.AngleAxis (z, Vector3.forward);
			enemyInRange = true;

		} else if (coll.gameObject.CompareTag ("Enemy") && targetUnit != null) {
			// Already have a target, keep rotating with it
			float z = Mathf.Atan2 ((targetUnit.transform.position.y - sightStart.position.y), (targetUnit.transform.position.x - sightStart.position.x)) * Mathf.Rad2Deg - 90;		
			sightStart.rotation = Quaternion.AngleAxis (z, Vector3.forward);
		}
	}

	void OnTriggerExit2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Enemy") && targetUnit != null) {
			if (coll.gameObject == targetUnit){
				targetUnit = null;
				enemyInRange = false;
			}
		}
	}


}
