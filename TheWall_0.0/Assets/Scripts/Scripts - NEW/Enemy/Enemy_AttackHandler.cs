using UnityEngine;
using System.Collections;

public class Enemy_AttackHandler : Unit_Base {

	public Enemy_MoveHandler moveHandler;


	public int targetTilePosX, targetTilePosY;

	public GameObject playerUnit;

	public ObjectPool objPool;

	public bool canAttack;

	public bool startCounter = true;

	void Start () {

		// Initialize Unit stats
		stats.Init ();

		resourceGrid = GetComponentInParent<Enemy_MoveHandler> ().resourceGrid;
	}
	
	// Update is called once per frame
	void Update () {
		if (canAttack && startCounter || canAttackTile && startCounter) {
			StartCoroutine (WaitToAttack ());

		} 
	}

	IEnumerator WaitToAttack(){
//		canAttack = false;
//		canAttackTile = false;
		startCounter = false;
	
		yield return new WaitForSeconds (stats.curRateOfAttk);
		if (unitToPool != null) {
			PoolTarget(unitToPool);
		}else if (playerUnit != null) {
			HandleDamageToUnit ();
		} else {
			if(moveHandler != null){
				if(AttackTile(targetTilePosX, targetTilePosY, moveHandler)){
					startCounter = true;
					canAttackTile = true;
					moveHandler.moving = false;
				}else{
					moveHandler.moving = true;
				}

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
			startCounter = true;
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
			deadE.GetComponent<EasyPool> ().objPool = objPool;
			deadE.transform.position = unitToPool.transform.position;
		}
		unitToPool = null;
		// if we are pooling it means its dead so we should check for target again
		playerUnit = null;
		canAttack = false;
		moveHandler.isAttacking = false;
		startCounter = true;
	}

	/// <summary>
	/// Special Attack for when Enemy reaches the Capital,
	/// suicide bombs the building doing max special damage.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void SpecialAttack(int x, int y){
		Debug.Log ("ENEMY: Doing special attack on Capital!");

		// Hit the tile with special damage
		resourceGrid.DamageTile (x, y, stats.curSPdamage);

		// Spawn an explosion at my position
		GameObject explosion = objPool.GetObjectForType ("Explosion Particles", true);

		if (explosion != null) {
			// Explosion must match my layer
			string targetLayer = GetComponent<SpriteRenderer>().sortingLayerName;

			// assign it to Particle Renderer
			explosion.GetComponent<ParticleSystemRenderer>().sortingLayerName = targetLayer;

			explosion.transform.position = transform.position;
		}


		// then pool myself
		objPool.PoolObject (this.gameObject);
	}
}
