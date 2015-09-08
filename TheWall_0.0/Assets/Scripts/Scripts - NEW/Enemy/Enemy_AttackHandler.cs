using UnityEngine;
using System.Collections;

public class Enemy_AttackHandler : Unit_Base {

	public Enemy_MoveHandler moveHandler;


	public int targetTilePosX, targetTilePosY;

	public GameObject playerUnit;

	public ObjectPool objPool;

	public bool canAttack;

	public bool startCounter = true;

	// KAMIKAZE:
	public bool isKamikaze;
	private float countDownToPool = 1f;


	void Start () {

		isKamikaze = moveHandler.isKamikaze;

		// Initialize Unit stats
		stats.Init ();

		resourceGrid = GetComponentInParent<Enemy_MoveHandler> ().resourceGrid;
	}
	
	// Update is called once per frame
	void Update () {

		if (isKamikaze) {
			// Run kamikaze mode
			// once per second we can check if we need to pool any player units we hit before pooling ourselves
			if (unitToPool != null) {
				if (countDownToPool <= 0) {
					PoolTarget (unitToPool);
				} else {
					countDownToPool -= Time.deltaTime;
				}
			}

		} else {
			if (unitToPool != null)
				PoolTarget (unitToPool);

			// Run normal attack mode
			if (canAttack && startCounter || canAttackTile && startCounter) {
				StartCoroutine (WaitToAttack ());
				
			} 
		}


	}

	IEnumerator WaitToAttack(){
//		canAttack = false;
//		canAttackTile = false;
		startCounter = false;
	
		yield return new WaitForSeconds (stats.curRateOfAttk);
		if (playerUnit != null) {
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
		unitToPool = null;

		Destroy (target.GetComponent<Player_AttackHandler>().unitParent);

		GameObject deadE = objPool.GetObjectForType("dead", false); // Get the dead unit object
		if (deadE != null) {
			deadE.GetComponent<EasyPool> ().objPool = objPool;
			deadE.transform.position = target.transform.position;
		}

		if (!isKamikaze) {
			// if we are pooling it means its dead so we should check for target again
			playerUnit = null;
			canAttack = false;
			moveHandler.isAttacking = false;
			startCounter = true;
		} else {
			// kamikaze units just pool themselves when they hit
			objPool.PoolObject(this.gameObject);
		}
	
	}

	/// <summary>
	/// Special Attack for when Enemy reaches the Capital,
	/// suicide bombs the building doing max special damage.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void SpecialAttack(int x, int y){

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


	// KAMIKAZE ONLY:



	/// <summary>
	/// When hit with Player Unit or Building is detected,
	/// this unit does full Special Damage. If it was a building/tile it
	/// does damage through the Grid. If it was a Unit then it attacks through 
	/// Unit Base stats, spawns a dead sprite and pools itself
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void KamikazeAttack(int x, int y, Unit_Base unit = null){
		
		if (unit == null) {
			// Hit the tile with special damage
			resourceGrid.DamageTile (x, y, stats.curSPdamage);
			// then pool myself
			objPool.PoolObject (this.gameObject);
		} else {
			// Hit the Player unit with special damage
			SpecialAttackOtherUnit(unit);
		}
		
		
		
		// Spawn an explosion at my position
		GameObject explosion = objPool.GetObjectForType ("Explosion Particles", true);
		
		if (explosion != null) {
			// Explosion must match my layer
			string targetLayer = GetComponent<SpriteRenderer>().sortingLayerName;
			
			// assign it to Particle Renderer
			explosion.GetComponent<ParticleSystemRenderer>().sortingLayerName = targetLayer;
			
			explosion.transform.position = transform.position;
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (isKamikaze) {
			if (coll.gameObject.tag == "Building") {
				// if unit hits a building, we blow up
				KamikazeAttack ((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.y);
			}
			if (coll.gameObject.tag == "Citizen") {
				// if unit hits a Player Unit, 
				// we get the Unit base and blow up
				if (coll.gameObject.GetComponentInChildren<Unit_Base> () != null) {
					KamikazeAttack (0, 0, coll.gameObject.GetComponentInChildren<Unit_Base> ()); 
				} else {
					Debug.Log ("ENEMY ATTACK: Could not find Player unit's attack handler!");
				}
			}
		}
	}
}
