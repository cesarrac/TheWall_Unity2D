using UnityEngine;
using System.Collections;

public class Tower_AoETargettingHandler : Unit_Base {

	/// <summary>
	/// Tower AoE Targetting. 
	/// Enemy Units that enter the circle collider get added to an array and, in turn, 
	/// is dealt damage. This tower only does damage the right when a group of enemies enter its collider.
	/// </summary>
	

	private GameObject[] enemiesInRange;

	public int maxTargets; // caps out how many enemies it can damage at the same time


	// Enemy Count should be treated as a length, meaning the actual total of enemies in range
	[SerializeField]
	private int _enemyCount = 0;
	int enemiesCount {get {return _enemyCount;} set {_enemyCount = Mathf.Clamp(value, 0, maxTargets);}}

	// This will represent the index of each enemy within the enemiesInRange array
	private int curEnemyIndex = 0;
	
	public enum State {SEEKING, COUNTING, SHOOTING, STARVED}

	private State _state = State.SEEKING;

	[HideInInspector]
	public State state { get { return _state; } set { _state = value; } }

	private float shootCountDown;

	[SerializeField]
	private Building_StatusIndicator bStatusIndicator;
	
	private bool statusIndicated = false;

	public State debugState;


	void Start () 
	{

		if (bStatusIndicator == null)
			Debug.Log ("GUN: Building Status Indicator NOT SET!");

		// In case Resource Grid is null
		if (resourceGrid == null)
			resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();

		// Initialize building stats
		stats.Init ();
		InitTileStats((int)transform.position.x, (int)transform.position.y);

		// If Object Pool is null we can get it from the Click Handler
		if (objPool == null)
			objPool = GetComponentInParent<Building_ClickHandler> ().objPool;
		

		// Set Length of enemies in range array to Maximum Targets
		enemiesInRange = new GameObject[maxTargets];

		// set the count down to Shoot to this Tower's fire rate
		shootCountDown = stats.startRate;
	}

	
	void Update(){

		MyStateManager (_state);

		debugState = _state;
	}

	void MyStateManager(State curState)
	{
		switch (curState) {

		case State.SEEKING:

			// From the moment the first enemy enters trigger, countdown begins to shoot
			if (enemiesCount > 0) {
				_state = State.COUNTING;

				// Indicate that this tower is shooting
				if (!statusIndicated)
					IndicateStatus("Targets acquired");
			}

			break;

		case State.COUNTING:

			// Begin countdown to shoot
			CountDownToShoot();

			break;

		case State.SHOOTING:

			// Loop through enemies
			for (int x = 0; x < enemiesCount; x++){
				if (enemiesInRange[x] != null && enemiesInRange[x].activeSelf){
					
					// Damage this unit
					HandleDamageToUnit(enemiesInRange[x]);
					
					// Null it from the array in case it's not already
					enemiesInRange[x] = null;
					
					// subtract from count
					enemiesCount--;
				}else{
					// if it's null or has less than 0 HP just remove from array and count

					// Null it from the array in case it's not already
					enemiesInRange[x] = null;
					
//					// subtract from count
					enemiesCount--;
				}
			}
			
			// After looping and Damaging all enemies in the array, the array is null and we can seek for more again
			// if NOT starved
			if (_state != State.STARVED && enemiesCount > 0){
				_state = State.COUNTING;
			}else if (state != State.STARVED && enemiesCount <=0){
				_state = State.SEEKING;
			}


			break;

		default:
			// building is starved
			break;
		}
		
	}


	void IndicateStatus(string status)
	{
		if (buildingStatusIndicator != null) {
			buildingStatusIndicator.CreateStatusMessage(status);
			
			statusIndicated = true;
		}
	}

	
	void CountDownToShoot()
	{
		
		if (shootCountDown <= 0) {

			// Reset Countdown
			shootCountDown = stats.curRateOfAttk;

			// Allow status to be indicated
			statusIndicated = false;

			// Change state to Shooting if not Starved
			if (_state != State.STARVED)
				_state = State.SHOOTING;

		} else {
			shootCountDown -= Time.deltaTime;
		}
		
	}



	void VisualShooting(GameObject target){
		GameObject explosion = objPool.GetObjectForType ("Explosion Particles", true);
		if (explosion != null) {
			// the Explosion's sorting layer must match the target's layer

			// Get the target's layer
			string targetLayer =  target.GetComponent<SpriteRenderer>().sortingLayerName;

			// apply layer to Particle System Renderer
			explosion.GetComponent<ParticleSystemRenderer>().sortingLayerName = targetLayer;

			explosion.transform.position = target.transform.position;
		}
	}

	
	/// <summary>
	/// Handles the damage each unit detected by using
	/// method from Unit_Base class.
	/// </summary>
//	void HandleDamageToUnits()
//	{
//		if (enemiesInRange [0] != null) { 
//
//			if (this.unitToPool == null){
//
//				for (int x =0; x < enemiesInRange.Length; x++) {
//
//					if (enemiesInRange [x] != null) { // target hasn't already been killed
//						AttackOtherUnit (enemiesInRange [x].GetComponent<Unit_Base> ());
//						VisualShooting(enemiesInRange[x].transform.position);
//					}
//
//				}
//			
//			}else{
//				// Unit to pool is not null
//				// Pool target, this will return state to seeking
//				PoolTarget(this.unitToPool, this.unitToPool.transform.position);
//			}
//
//		}else {
//
//			// Reset all enemy vars
//			enemiesCount = 0;
////			enemyInRange = false;
//		}
//		
//	}

	void HandleDamageToUnit(GameObject target)
	{
		if (target != null && target.activeSelf) {

			AttackOtherUnit (target.GetComponent<Unit_Base> ());
			VisualShooting (target);
		} 
	}
	
//	void PoolTarget(GameObject target, Vector3 deathPos)
//	{
//		// Pool the unit
//		objPool.PoolObject (target);
//		
//		this.unitToPool = null;
//		
//		// Instantiate a Dead sprite at its location
//		GameObject deadE = objPool.GetObjectForType("dead", true); // Get the dead unit object
//		if (deadE != null) {
//			deadE.GetComponent<EasyPool> ().objPool = objPool;
//			deadE.transform.position = deathPos;
//		}
//
////		// If NOT starved
////		if (_state != State.STARVED)
////			// Return state back to Seeking
////			_state = State.SEEKING;
//
//	}
	

	void OnTriggerEnter2D(Collider2D coll)
	{

		// Everytime one enters they get added to the array
		if (coll.gameObject.CompareTag ("Enemy")) {

			// Add targets to enemies array IF NOT more than maxTargets and state is neither Starved nor Shooting
			if (enemiesCount < maxTargets && _state != State.STARVED && _state != State.SHOOTING) {

				// Mark targets

				// Enemies count starts at 0
				enemiesCount++;

				// enemy index always has to be enemies count - 1
				curEnemyIndex = enemiesCount - 1;

				// Add enemy to array using current Enemy Index
				enemiesInRange[curEnemyIndex] = coll.gameObject;

			}

		}

	}

	void OnTriggerStay2D(Collider2D coll)
	{
		if (enemiesCount == 0 && _state == State.SEEKING) {
			if (coll.gameObject.CompareTag ("Enemy")) {
				// Enemies count starts at 0
				enemiesCount++;
				
				// enemy index always has to be enemies count - 1
				curEnemyIndex = enemiesCount - 1;
				
				// Add enemy to array using current Enemy Index
				enemiesInRange[curEnemyIndex] = coll.gameObject;
			}
		}
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.gameObject.CompareTag ("Enemy")) {
			for (int y = 0; y < enemiesCount; y++){
				if (coll.gameObject == enemiesInRange[y]){
					enemiesInRange[y] = null;
					enemiesCount--;
					return;
				}
			}
		}
	}

}
