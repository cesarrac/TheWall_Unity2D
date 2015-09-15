using UnityEngine;
using System.Collections;

public class Tower_AoETargettingHandler : Unit_Base {

	/// <summary>
	/// Tower AoE Targetting. 
	/// Enemy Units that enter the circle collider get added to an array and, in turn, 
	/// is dealt damage. This tower only does damage the right when a group of enemies enter its collider.
	/// </summary>

	
//	bool enemyInRange = false;

	private GameObject[] enemiesInRange;

	public int maxTargets; // caps out how many enemies it can damage at the same time

	[SerializeField]
	private int _enemyCount = 0;
	int enemiesCount {get {return _enemyCount;} set {_enemyCount = Mathf.Clamp(value, 0, maxTargets - 1);}}
	
	public enum State {SEEKING,SHOOTING,STARVED}

	private State _state = State.SEEKING;

	[HideInInspector]
	public State state { get { return _state; } set { _state = value; } }

	private float shootCountDown;

	[SerializeField]
	private Building_StatusIndicator bStatusIndicator;
	
	private bool statusIndicated = false;



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
	}

	void MyStateManager(State curState)
	{
		switch (curState) {
		case State.SEEKING:
			if (enemiesCount > 0) // we have at least one target
				_state = State.SHOOTING;
			break;
		case State.SHOOTING:
			if (this.unitToPool == null){
				CountDownToShoot();
			}else{
				PoolTarget (this.unitToPool, this.unitToPool.transform.position);
			}

			if (!statusIndicated)
				IndicateStatus("Targets acquired");
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

			// indicate that we are shooting
			statusIndicated = false;

			// Loop through enemies
			for (int x = 0; x <= enemiesCount; x++){
				if (enemiesInRange[x] != null && enemiesInRange[x].GetComponent<Unit_Base>().stats.curHP > 0){

					// Damage this unit
					HandleDamageToUnit(enemiesInRange[x]);

				}else{
					// if NOT null, pool it
					if (enemiesInRange[x] != null)
						// Pool this unit
						this.unitToPool = enemiesInRange[x];

					// Null it from the array in case it's not already
					enemiesInRange[x] = null;

					// subtract from count
					enemiesCount--;
				}
			}
			shootCountDown = stats.curRateOfAttk;
			
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
		if (target != null) {

			AttackOtherUnit (target.GetComponent<Unit_Base> ());
			VisualShooting (target);
		} 
	}
	
	void PoolTarget(GameObject target, Vector3 deathPos)
	{
		// Pool the unit
		objPool.PoolObject (target);
		
		this.unitToPool = null;
		
		// Instantiate a Dead sprite at its location
		GameObject deadE = objPool.GetObjectForType("dead", true); // Get the dead unit object
		if (deadE != null) {
			deadE.GetComponent<EasyPool> ().objPool = objPool;
			deadE.transform.position = deathPos;
		}

		// If NOT starved
		if (_state != State.STARVED)
			// Return state back to Seeking
			_state = State.SEEKING;

	}
	

	void OnTriggerEnter2D(Collider2D coll)
	{
		// Everytime one enters they get added to the array
		if (coll.gameObject.CompareTag ("Enemy")) {
			Debug.Log ("target in range!");
			if (enemiesCount < maxTargets - 1) {
				enemiesInRange[enemiesCount] = coll.gameObject;
				enemiesCount++;
			}
//			if (!enemyInRange){
//				enemyInRange = true;
//				HandleDamageToUnits();
//			}
		}

	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.gameObject.CompareTag ("Enemy")) {
			for (int y = 0; y < enemiesInRange.Length; y++){
				if (coll.gameObject == enemiesInRange[y]){
					enemiesInRange[y] = null;
					enemiesCount--;
					return;
				}
			}
		}
	}

}
