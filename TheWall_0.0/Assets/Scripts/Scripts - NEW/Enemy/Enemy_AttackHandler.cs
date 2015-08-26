using UnityEngine;
using System.Collections;

public class Enemy_AttackHandler : Unit_Base {

	public Enemy_MoveHandler moveHandler;


	public int targetTilePosX, targetTilePosY;

	public GameObject playerUnit;

	public ObjectPool objPool;

	public bool canAttack { private get; set;}

	void Start () {
		resourceGrid = GetComponentInParent<Enemy_MoveHandler> ().resourceGrid;
	}
	
	// Update is called once per frame
	void Update () {
		if (canAttack || canAttackTile) {
			StartCoroutine (WaitToAttack ());

		} 
	}

	IEnumerator WaitToAttack(){
		canAttack = false;
		canAttackTile = false;
		yield return new WaitForSeconds (rateOfAttack);
		if (unitToPool != null) {
			PoolTarget(unitToPool);
		}else if (playerUnit != null) {
			HandleDamageToUnit ();
		} else {
			if(moveHandler != null){
				AttackTile(targetTilePosX, targetTilePosY, moveHandler);
			}
		}
	}
	/// <summary>
	/// Handles the damage to unit by using
	/// method from Unit_Base class.
	/// </summary>
	void HandleDamageToUnit(){
//		Debug.Log ("Damaging the player!");
		if (playerUnit != null) {
			Unit_Base unitToHit = playerUnit.GetComponent<Unit_Base> ();
			AttackOtherUnit (unitToHit);
			canAttack = true;
		} else {
			canAttack = false;
		}
		
	}
	
//	void PoolTarget(GameObject target){
//		// Have to get the parent of the target because the attack handler is on a child object of the actual unit
//		GameObject targetParent = target.GetComponent<Player_AttackHandler> ().unitParent;
//		objPool.PoolObject (targetParent); // Pool the Dead Unit
//		string deathName = "dead";
//		GameObject deadE = objPool.GetObjectForType(deathName, true); // Get the dead unit object
//		deadE.GetComponent<FadeToPool> ().objPool = objPool;
//		deadE.transform.position = unitToPool.transform.position;
//		unitToPool = null;
//		// if we are pooling it means its dead so we should check for target again
//		playerUnit = null;
//		canAttack = false;
//		moveHandler.isAttacking = false;
//	}
	void PoolTarget(GameObject target){
		Destroy (target.GetComponent<Player_AttackHandler>().unitParent);
		GameObject deadE = objPool.GetObjectForType("dead", false); // Get the dead unit object
		if (deadE != null) {
			deadE.GetComponent<FadeToPool> ().objPool = objPool;
			deadE.transform.position = unitToPool.transform.position;
		}
		unitToPool = null;
		// if we are pooling it means its dead so we should check for target again
		playerUnit = null;
		canAttack = false;
		moveHandler.isAttacking = false;
	}
}
