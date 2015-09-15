using UnityEngine;
using System.Collections;

public class Kamikaze_AttackHandler : Unit_Base {

	public Enemy_MoveHandler moveHandler;

//	public ObjectPool objPool;

	private float countDownToPool = 1f;

	void Start () {

		// Initialize Unit stats
		stats.Init ();

		if (resourceGrid == null)
			resourceGrid = GetComponent<Enemy_MoveHandler> ().resourceGrid;

		// This receives the Object Pool from the Wave Spawner, but just in case...
		if (objPool == null)
			objPool = GameObject.FindGameObjectWithTag ("Pool").GetComponent<ObjectPool> ();
	}
	

	void Update () {
		// once per second we can check if we need to pool any player units we hit before pooling ourselves
		if (this.unitToPool != null) {
			if (countDownToPool <= 0) {
				KillTarget (this.unitToPool.transform.position);
			} else {
				countDownToPool -= Time.deltaTime;
			}
		}
	
	}


	void OnCollisionEnter2D(Collision2D coll){
		Debug.Log ("KAM ATTACK: Something is colliding!");
		if (coll.gameObject.tag == "Building"){
			// if unit hits a building, we blow up
			KamikazeAttack ((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.y);
		}
		if (coll.gameObject.tag == "Citizen"){
			// if unit hits a Player Unit, 
				// we get the Unit base and blow up
			if (coll.gameObject.GetComponentInChildren<Unit_Base>() != null){
				KamikazeAttack(0,0, coll.gameObject.GetComponentInChildren<Unit_Base>()); 
			}else{
				Debug.Log("KAM ATTACK: Could not find Player unit's attack handler!");
			}
		}
	}

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

	void KillTarget(Vector3 deathPos){
		// Currently NOT pooling Player units
//		objPool.PoolObject(unitToPool);
		Destroy (unitToPool.GetComponent<Player_AttackHandler>().unitParent);
		// Get dead sprite
		GameObject deadE = objPool.GetObjectForType("dead", false);
		if (deadE != null) {
			deadE.GetComponent<EasyPool> ().objPool = objPool;
			deadE.transform.position = deathPos;
		}

		this.unitToPool = null;

		// then pool myself
		objPool.PoolObject (this.gameObject);

	}
}
