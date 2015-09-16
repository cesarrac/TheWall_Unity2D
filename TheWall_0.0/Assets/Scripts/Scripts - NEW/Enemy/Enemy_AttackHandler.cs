using UnityEngine;
using System.Collections;

public class Enemy_AttackHandler : Unit_Base {

	public Enemy_MoveHandler moveHandler;


	public int targetTilePosX, targetTilePosY;

	public GameObject playerUnit;

//	public bool canAttack;

	public bool startCounter = true;

	// KAMIKAZE:
	public bool isKamikaze;
	private float countDownToPool = 1f;

	public enum State { MOVING, ATTACK_TILE, ATTACK_UNIT, POOLING_TARGET };

	private State _state = State.MOVING;
	
	[HideInInspector]
	public State state { get { return _state; } set { _state = value; } }

	private float attackCountDown;

	void Start () {

		// Get the value of isKamikaze set by the public bool in Move Handler
		isKamikaze = moveHandler.isKamikaze;

		// Initialize Unit stats
		stats.Init ();

		// Get the Grid from the Move Handler
		if (resourceGrid == null)
			resourceGrid = GetComponent<Enemy_MoveHandler> ().resourceGrid;

		// This receives the Object Pool from the Wave Spawner, but just in case...
		if (objPool == null)
			objPool = GameObject.FindGameObjectWithTag ("Pool").GetComponent<ObjectPool> ();

		// Set attack Countdown to this Unit's starting attack rate
		attackCountDown = stats.startAttack;
	}
	
	// Update is called once per frame
	void Update () {

		// If I don't have any HP left, Pool myself
		if (stats.curHP <= 0)
			Suicide ();

		MyStateMachine (_state);
	}

	void MyStateMachine (State _curState)
	{
		switch (_curState) {
		case State.MOVING:
			// Not attacking
			break;
		case State.ATTACK_TILE:
			CountDownToAttack(false);
			break;
		case State.ATTACK_UNIT:
			CountDownToAttack(true);
			break;
//		case State.POOLING_TARGET:
//			if (unitToPool != null){
//				PoolTarget(unitToPool);
//			}else{
//				_state = State.MOVING;
//			}
//			break;
		default:
			// Unit is not attacking
			break;
		}
	}


	void CountDownToAttack(bool trueIfUnit)
	{
		if (attackCountDown <= 0) {
			// Attack
			if (trueIfUnit){

				// attack unit
				HandleDamageToUnit ();

			}else{

				// attack tile
				HandleDamageToTile();
			}
			// reset countdown
			attackCountDown = stats.curRateOfAttk;

		} else {

			attackCountDown -= Time.deltaTime;
		}
	}

	void HandleDamageToTile()
	{
		if(moveHandler != null){
			// Check if tile can still take damage, if so Unit_Base damages it
			if(AttackTile(targetTilePosX, targetTilePosY, moveHandler)){

				// Change Move Handler state to stop movement
//				moveHandler.state = Enemy_MoveHandler.State.ATTACKING;

			}else{

				// Tile has been destroyed, start Moving again
				moveHandler.state = Enemy_MoveHandler.State.MOVING_BACK;

				// Set state back to moving to stop attacking
				_state = State.MOVING;

			}
			
		}
	}

	/// <summary>
	/// Handles the damage to unit by using
	/// method from Unit_Base class.
	/// </summary>
	void HandleDamageToUnit()
	{

		// Verify that Player Unit is not null
		if (playerUnit != null) {

			// Store the unit's Unit_Base to access its stats
			Unit_Base unitToHit = playerUnit.GetComponent<Unit_Base> ();

			// Call the attack
			AttackOtherUnit (unitToHit);

		} else {

			// Player unit is dead, we can STOP attack
			_state = State.MOVING;
		}
		
	}

	void Suicide()
	{
		// get a Dead sprite to mark my death spot
		GameObject deadE = objPool.GetObjectForType("dead", false); // Get the dead unit object
		if (deadE != null) {
			deadE.GetComponent<EasyPool> ().objPool = objPool;
			deadE.transform.position = transform.position;
		}

		// and Pool myself
		objPool.PoolObject (this.gameObject);
	}

//	void PoolTarget(GameObject target)
//	{
//		unitToPool = null;
//
//		Destroy (target.GetComponent<Player_AttackHandler>().unitParent);
//
//		GameObject deadE = objPool.GetObjectForType("dead", false); // Get the dead unit object
//		if (deadE != null) {
//			deadE.GetComponent<EasyPool> ().objPool = objPool;
//			deadE.transform.position = target.transform.position;
//		}
//
//		if (!isKamikaze) {
//			// if we are pooling it means its dead so we should check for target again
//			playerUnit = null;
//
//			// Tell move handler we are no longer attacking
//			moveHandler.state = Enemy_MoveHandler.State.MOVING;
//
//			// Set state back to moving (if there's another target to attack the Unit or Tile will cause state to change to attacking)
//			_state = State.MOVING;
//
//		} else {
//			// kamikaze units just pool themselves when they hit
//			objPool.PoolObject(this.gameObject);
//		}
//	
//	}

	/// <summary>
	/// Special Attack for when Enemy reaches the Capital,
	/// suicide bombs the building doing max special damage.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void SpecialAttack(int x, int y)
	{

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
	public void KamikazeAttack(int x, int y, Unit_Base unit = null)
	{
		
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

	void OnTriggerEnter2D(Collider2D coll)
	{
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
