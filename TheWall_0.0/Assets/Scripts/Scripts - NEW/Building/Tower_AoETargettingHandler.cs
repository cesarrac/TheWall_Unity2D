using UnityEngine;
using System.Collections;

public class Tower_AoETargettingHandler : Unit_Base {

	/// <summary>
	/// Tower AoE Targetting. 
	/// Enemy Units that enter the circle collider get added to an array and, in turn, 
	/// is dealt damage. This tower only does damage the right when a group of enemies enter its collider.
	/// </summary>

	
	public ObjectPool objPool;
	
	bool canShoot, enemyInRange = false;
	
	private GameObject targetUnit;


	public GameObject[] enemiesInRange;

	public int maxTargets; // caps out how many enemies it can damage at the same time
	int enemiesCount = 0;

	public bool starvedMode; // MANIPULATED BY THE RESOURCE MANAGER

	void Start () {
		if (resourceGrid == null)
			resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();

		// Initialize building stats
		stats.Init ();
		InitTileStats((int)transform.position.x, (int)transform.position.y);

		if (objPool == null) {
			objPool = GameObject.FindGameObjectWithTag("Pool").GetComponent<ObjectPool>();
		}

		enemiesInRange = new GameObject[maxTargets];
	}

	
	void Update(){

		if (canShoot && !starvedMode){
			StartCoroutine(WaitToShoot());
		}
	}
	
	IEnumerator WaitToShoot(){
		canShoot = false;
		yield return new WaitForSeconds (stats.curRateOfAttk);
		if (unitToPool != null) {
			PoolTarget(unitToPool);
		} 
		if (enemiesInRange[0] != null){
//			VisualShooting ();
			HandleDamageToUnits ();
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
	void HandleDamageToUnits(){
		if (enemiesInRange [0] != null) { // only loop if there's at least the first guy detected
			for (int x =0; x < enemiesInRange.Length; x++) {
				if (enemiesInRange [x] != null) { // target hasn't already been killed
					AttackOtherUnit (enemiesInRange [x].GetComponent<Unit_Base> ());
					VisualShooting(enemiesInRange[x].transform.position);
				}
			}
			canShoot = true;
		}else {
			canShoot = false;
			enemiesCount = 0;
			enemyInRange = false;
		}
		
	}
	
	void PoolTarget(GameObject target){
		objPool.PoolObject (target); // Pool the Dead Unit
		string deathName = "dead";
		GameObject deadE = objPool.GetObjectForType(deathName, true); // Get the dead unit object
		deadE.GetComponent<EasyPool> ().objPool = objPool;
		deadE.transform.position = unitToPool.transform.position;
		unitToPool = null;
		// if we are pooling it means its dead so we should check for target again
		for (int y = 0; y < enemiesInRange.Length; y++){
			if (enemiesInRange[y] == target){
				enemiesInRange[y] = null;
				break;
			}
		}
		enemyInRange = false;
	}
	
//	void OnTriggerStay2D(Collider2D coll){
//		if (coll.gameObject.CompareTag("Enemy")){
//			float z = Mathf.Atan2((coll.transform.position.y - sightStart.position.y), (coll.transform.position.x - sightStart.position.x)) * Mathf.Rad2Deg - 90;		
//			//		myTransform.eulerAngles = new Vector3 (0,0,z);
//			sightStart.rotation = Quaternion.AngleAxis(z, Vector3.forward);
//			enemyInRange = true;
//		}
//	}
	void OnTriggerEnter2D(Collider2D coll){
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
//				VisualShooting();
			}
		}

	}

}
