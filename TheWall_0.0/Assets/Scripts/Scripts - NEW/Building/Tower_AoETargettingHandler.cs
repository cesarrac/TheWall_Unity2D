using UnityEngine;
using System.Collections;

public class Tower_AoETargettingHandler : Unit_Base {

	/// <summary>
	/// Tower AoE Targetting. 
	/// Enemy Units that enter the circle collider get added to an array and, in turn, 
	/// is dealt damage. This tower only does damage the right when a group of enemies enter its collider.
	/// </summary>

	
	public ObjectPool objPool;
	
	bool enemyInRange = false;

	public GameObject[] enemiesInRange;

	public int maxTargets; // caps out how many enemies it can damage at the same time
	int enemiesCount = 0;

	public bool starvedMode; // MANIPULATED BY THE RESOURCE MANAGER

	enum State 
	{
		SEEKING,
		SHOOTING,
		STARVED
	}

	State state = State.SEEKING;

	private float shootCountDown;

	void Start () 
	{
		// In case Resource Grid is null
		if (resourceGrid == null)
			resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();

		// Initialize building stats
		stats.Init ();
		InitTileStats((int)transform.position.x, (int)transform.position.y);

		// In case Object Pool is null
		if (objPool == null) {
			objPool = GameObject.FindGameObjectWithTag("Pool").GetComponent<ObjectPool>();
		}

		// Set Length of enemies in range array to Maximum Targets
		enemiesInRange = new GameObject[maxTargets];

		// set the count down to Shoot to this Tower's fire rate
		shootCountDown = stats.startRate;
	}

	
	void Update(){

		if (unitToPool != null)
			PoolTarget (unitToPool);

		MyStateManager (state);
	}

	void MyStateManager(State curState)
	{
		switch (curState) {
		case State.SEEKING:
			if (enemiesInRange[0] !=null)
				state = State.SHOOTING;
			break;
		case State.SHOOTING:
			CountDownToShoot();
			break;
		default:
			Debug.Log("Starved!");
			break;
		}
		
	}
	
	void CountDownToShoot()
	{
		
		if (shootCountDown <= 0) {
			
			// SHOOT
			if (enemiesInRange[0] != null){ // If we don't have at least ONE Target, don't shoot

				HandleDamageToUnits ();

			}else{
				// Target is null so we can go back to seeking
				state = State.SEEKING;
			}
			
			shootCountDown = stats.curRateOfAttk;
			
		} else {
			shootCountDown -= Time.deltaTime;
		}
		
	}



	void VisualShooting(Vector3 position){
		GameObject explosion = objPool.GetObjectForType ("Explosion Particles", true);
		if (explosion != null) {
			// the Explosion's sorting layer must match the target's layer

			// Get the target's layer
			string targetLayer =  enemiesInRange[0].GetComponent<SpriteRenderer>().sortingLayerName;

			// apply layer to Particle System Renderer
			explosion.GetComponent<ParticleSystemRenderer>().sortingLayerName = targetLayer;

			explosion.transform.position = position;
		}
	}

	
	/// <summary>
	/// Handles the damage each unit detected by using
	/// method from Unit_Base class.
	/// </summary>
	void HandleDamageToUnits()
	{
		if (enemiesInRange [0] != null) { 

			for (int x =0; x < enemiesInRange.Length; x++) {

				if (enemiesInRange [x] != null) { // target hasn't already been killed
					AttackOtherUnit (enemiesInRange [x].GetComponent<Unit_Base> ());
					VisualShooting(enemiesInRange[x].transform.position);
				}

			}

		}else {

			// No target in range so go back to Seeking
			state = State.SEEKING;

			// Reset all enemy vars
			enemiesCount = 0;
			enemyInRange = false;
		}
		
	}
	
	void PoolTarget(GameObject target)
	{
		unitToPool = null;

		objPool.PoolObject (target); // Pool the Dead Unit


		string deathName = "dead";
		GameObject deadE = objPool.GetObjectForType(deathName, true); // Get the dead unit object
		deadE.GetComponent<EasyPool> ().objPool = objPool;
		deadE.transform.position = target.transform.position;

		// if we are pooling it means its dead so we should check for target again
		for (int y = 0; y < enemiesInRange.Length; y++){
			if (enemiesInRange[y] == target){
				enemiesInRange[y] = null;
				break;
			}
		}
		enemyInRange = false;
	}
	

	void OnTriggerEnter2D(Collider2D coll)
	{
		// Everytime one enters they get added to the array
		if (coll.gameObject.CompareTag ("Enemy")) {
			Debug.Log ("target in range!");
			if (enemiesCount < maxTargets) {
				enemiesInRange[enemiesCount] = coll.gameObject;
				enemiesCount++;
			}
			if (!enemyInRange){
				enemyInRange = true;
				HandleDamageToUnits();
			}
		}

	}

}
