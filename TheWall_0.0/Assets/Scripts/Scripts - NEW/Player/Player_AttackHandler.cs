using UnityEngine;
using System.Collections;

public class Player_AttackHandler : Unit_Base {

	public int targetTilePosX, targetTilePosY;
	
	private GameObject enemyUnit;

	public bool canAttack { private get; set;}
	bool continueCounter;

	public GameObject unitParent;

	public SelectedUnit_MoveHandler moveHandler;

	Animator anim;

	public Barracks_SpawnHandler myBarracks;

	void Start () {

		// Initialize Unit stats
		stats.Init ();

		anim = GetComponentInParent<Animator> ();
		unitParent = gameObject.transform.parent.gameObject;
		continueCounter = true;

		// This receives the Object Pool from its Spawner, but just in case...
		if (objPool == null)
			objPool = GameObject.FindGameObjectWithTag ("Pool").GetComponent<ObjectPool> ();
	}
	
	// Update is called once per frame
	void Update () {

//		if (unitToPool != null)
//			PoolTarget (unitToPool);

		if (canAttack && continueCounter) {
			StartCoroutine(WaitToAttack());
		}
	}
	
	IEnumerator WaitToAttack(){
		continueCounter = false;
		yield return new WaitForSeconds (stats.curRateOfAttk);
		if (enemyUnit != null) {
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
			continueCounter = true;
		} else {
			canAttack = false;
			enemyUnit = null;
			continueCounter = true;
		}
		
	}

//	void MoveToAttack(){
//		if (enemyUnit != null) {
//			moveHandler.mX = Mathf.RoundToInt(enemyUnit.transform.position.x);
//		}
//	}
	
//	void PoolTarget(GameObject target){
//		unitToPool = null;
//
//		objPool.PoolObject (target); // Pool the Dead Unit
//
//		GameObject deadE = objPool.GetObjectForType("dead", false); // Get the dead unit object
//		if (deadE != null) {
//			deadE.GetComponent<EasyPool> ().objPool = objPool;
//			deadE.transform.position = target.transform.position;
//		}
//	
//		// if we are pooling it means its dead so we should check for target again
//		enemyUnit = null;
//		moveHandler.moving = false;
//		moveHandler.movingToAttack = false;
//		moveHandler.attackTarget = null;
//		continueCounter = true;
//
//	}

	// To Add and enemy unit, this unit has to detect it Entering its Circle Collider
	void OnTriggerStay2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Enemy") && enemyUnit == null) {
//			Debug.Log ("Enemy entered collider!");
			enemyUnit = coll.gameObject;

			// Tell the enemy that it's being attacked by me
			enemyUnit.GetComponent<Enemy_AttackHandler>().playerUnit = gameObject;

			// Change its state to Attack Unit
			enemyUnit.GetComponent<Enemy_AttackHandler>().state = Enemy_AttackHandler.State.ATTACK_UNIT;

			canAttack = true;
			moveHandler.attackTarget = enemyUnit;
			moveHandler.moving = true;
			moveHandler.movingToAttack = true;
			moveHandler.mX = Mathf.RoundToInt(enemyUnit.transform.position.x);
			moveHandler.mY = Mathf.RoundToInt(enemyUnit.transform.position.y);


		}
	}

	void OnTriggerExit2D(Collider2D coll){
		if (coll.gameObject.CompareTag ("Enemy") && enemyUnit != null) {
			canAttack = false;
			enemyUnit.GetComponent<Enemy_AttackHandler>().playerUnit = null;
			enemyUnit.GetComponent<Enemy_AttackHandler>().state = Enemy_AttackHandler.State.MOVING;

			enemyUnit = null;
			moveHandler.attackTarget = null;
			moveHandler.moving = false;
			moveHandler.movingToAttack = false;


		}
	}
}
