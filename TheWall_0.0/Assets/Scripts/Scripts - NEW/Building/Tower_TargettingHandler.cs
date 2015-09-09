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

	bool enemyInRange = false;

	public GameObject targetUnit;

	LineRenderer lineR;

	public bool starvedMode; // MANIPULATED BY THE RESOURCE MANAGER

	SpriteRenderer sr;

	enum State 
	{
		SEEKING,
		SHOOTING,
		STARVED
	}

	State state = State.SEEKING;

	public float shootCountDown;

	void Start ()
	{

		// In case Resource Grid is null
		if (resourceGrid == null)
			resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();

		// Initialize building stats
		stats.Init ();
		InitTileStats((int)transform.position.x, (int)transform.position.y);

		// In case Object Pool is null
		if (objPool == null)
			objPool = GameObject.FindGameObjectWithTag("Pool").GetComponent<ObjectPool>();

		// Get our Sprite Renderer from Parent ( this is how the prefab is set-up )
		if (GetComponentInParent<SpriteRenderer> () != null) {

			sr = GetComponentInParent<SpriteRenderer> ();

			// Get the Line Renderer Component from Child object sightStart
			lineR = sightStart.GetComponent<LineRenderer> ();
			lineR.sortingLayerName = sr.sortingLayerName;
			lineR.sortingOrder = sr.sortingOrder;

		}

		// set the count down to Shoot to this Tower's fire rate
		shootCountDown = stats.startRate;

	}

	// NOTE: Using Fixed Update becuase SeekEnemies() is controlling a Physics 2D Linecast
	void FixedUpdate () 
	{

		if (enemyInRange && !starvedMode){
			SeekEnemies ();
		}

	}

	void SeekEnemies()
	{
		RaycastHit2D hit = Physics2D.Linecast (sightStart.position, sightEnd.position, mask.value);
		if (hit.collider != null) {
			if (hit.collider.CompareTag("Enemy")){
				if (targetUnit == null) {// DONT GET TARGET if you already have one!
					targetUnit = hit.collider.gameObject;
				// shoot one shot quickly then start coroutine
					VisualShooting ();
					HandleDamageToUnit ();

					// After shooting once begin change state to begin countdown
					// Can't Shoot if I'm in a Starved State!
					if (state != State.STARVED)
						state = State.SHOOTING;
				}
			}
		}
	}

	void Update()
	{

		// Check that my line renderer's sorting layer is the same as mine
		if (lineR.sortingLayerName != sr.sortingLayerName) {
			lineR.sortingLayerName = sr.sortingLayerName;
			lineR.sortingOrder = sr.sortingOrder + 1;
		}

		if (unitToPool != null)
			PoolTarget (unitToPool);

//		Debug.DrawLine (sightStart.position, sightEnd.position, Color.magenta);

		MyStateManager (state);
	}

	void MyStateManager(State curState)
	{
		switch (curState) {
		case State.SEEKING:
			sightStart.Rotate (Vector3.forward * 90 * Time.deltaTime);
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
			if (targetUnit != null){
				VisualShooting ();
				HandleDamageToUnit ();
				Debug.Log("MACHINE GUN: Shooting!");
			}else{
				// Target is null so we can go back to seeking
				state = State.SEEKING;
			}

			shootCountDown = stats.curRateOfAttk;

		} else {
			shootCountDown -= Time.deltaTime;
		}

	}
	

	/// <summary>
	/// Gets a bullet from the pool of bullets and shoots it.
	/// The bullet itself is just for visual reference and will just Pool itself when it hits.
	/// </summary>
	void VisualShooting(){
		GameObject explosion = objPool.GetObjectForType ("Burst Particles", true);
		if (explosion != null) {
			// Explosion must match the target's layer

			// get the target layer
			string targetLayer = targetUnit.GetComponent<SpriteRenderer>().sortingLayerName;

			// assign layer to Particle Renderer
			explosion.GetComponent<ParticleSystemRenderer>().sortingLayerName = targetLayer;

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
		} else {
			// Target is null, go back to Seeking
			state = State.SEEKING;
		}

	}

	void PoolTarget(GameObject target){
		unitToPool = null;

		objPool.PoolObject (target); // Pool the Dead Unit

		string deathName = "dead";
		GameObject deadE = objPool.GetObjectForType(deathName, false); // Get the dead unit object
		deadE.GetComponent<EasyPool> ().objPool = objPool;
		deadE.transform.position = target.transform.position;
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

			targetUnit = null;
			enemyInRange = false;

			// Change state back to seeking
			state = State.SEEKING;
		}
	}


}
