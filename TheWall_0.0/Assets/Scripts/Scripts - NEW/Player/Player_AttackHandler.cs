using UnityEngine;
using System.Collections;

public class Player_AttackHandler : Unit_Base {

	public int targetTilePosX, targetTilePosY;
	
	private GameObject enemyUnit;

	public ObjectPool objPool;

	public bool canAttack { private get; set;}

	public GameObject unitParent;

	public SelectedUnit_MoveHandler moveHandler;

	Animator anim;

	void Start () {
		anim = GetComponentInParent<Animator> ();
		unitParent = gameObject.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (canAttack) {
			StartCoroutine(WaitToAttack());
		}
	}
	
	IEnumerator WaitToAttack(){
		canAttack = false;
		yield return new WaitForSeconds (rateOfAttack);
		if (unitToPool != null) {
			PoolTarget(unitToPool);
		}else if (enemyUnit != null) {
			HandleDamageToUnit ();
		} 
	}

	/// <summary>
	/// Handles the damage to unit by using
	/// method from Unit_Base class.
	/// </summary>
	void HandleDamageToUnit(){
//		Debug.Log ("Damaging enemy!");
		if (enemyUnit != null) {
		
			Unit_Base unitToHit = enemyUnit.GetComponent<Unit_Base> ();
			AttackOtherUnit (unitToHit);
			canAttack = true;
		} else {
			canAttack = false;
			enemyUnit = null;
		}
		
	}
	
	void PoolTarget(GameObject target){
		objPool.PoolObject (target); // Pool the Dead Unit
		GameObject deadE = objPool.GetObjectForType("dead", false); // Get the dead unit object
		if (deadE != null) {
			deadE.GetComponent<FadeToPool> ().objPool = objPool;
			deadE.transform.position = unitToPool.transform.position;
		}
		unitToPool = null;
		// if we are pooling it means its dead so we should check for target again
		enemyUnit = null;
		moveHandler.moving = false;
		moveHandler.movingToAttack = false;

	}

	// To Add and enemy unit, this unit has to detect it Entering its Circle Collider
	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Enemy")) {
//			Debug.Log ("Enemy entered collider!");
			enemyUnit = coll.gameObject;
			enemyUnit.GetComponent<Enemy_AttackHandler>().playerUnit = gameObject;
			enemyUnit.GetComponent<Enemy_AttackHandler>().canAttack = true;
			canAttack = true;
//			moveHandler.moving = true;
//			moveHandler.movingToAttack = true;


		}
	}

	void OnTriggerExit2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Enemy") && enemyUnit != null) {
			canAttack = false;
			enemyUnit.GetComponent<Enemy_AttackHandler>().playerUnit = null;
			enemyUnit.GetComponent<Enemy_AttackHandler>().canAttack = false;
			enemyUnit = null;
		}
	}
}
