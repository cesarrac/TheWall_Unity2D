using UnityEngine;
using System.Collections;

public class Tower_DeBuffer : Unit_Base {

	public ObjectPool objPool;
	
	bool canShoot, enemyInRange = false;
	
	private GameObject targetUnit;
	
	
	public GameObject[] enemiesInRange;
	
	public int maxTargets; // caps out how many enemies it can damage at the same time
	int enemiesCount = 0;
	
	public bool starvedMode; // MANIPULATED BY THE RESOURCE MANAGER

	enum debuffType { DEFENCE, ATTACK, SPEED}

	[SerializeField]
	private debuffType type = debuffType.DEFENCE;

	public float debuffAmmt, slowDownDebuffAmmnt = 0.3f;

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
	

	void Update () {
		if (enemiesCount >= maxTargets){
			// enemies in range is full, debuff all of them
			HandleDeBuffToUnits();
		}
	}



	void HandleDeBuffToUnits(){
		if (enemiesInRange [0] != null) { // only loop if there's at least the first guy detected
			for (int x =0; x < enemiesInRange.Length; x++) {
				if (enemiesInRange [x] != null) { // target hasn't already been killed

					DebuffUnit (enemiesInRange [x].GetComponent<Unit_Base> ());

					// after DeBuffing each one, remove them from the array
					enemiesInRange[x] = null;

//					VisualShooting(enemiesInRange[x].transform.position);
				}

				// after looping everyone reset enemycount
				enemiesCount = 0;
			}

		}else {
			canShoot = false;
			enemiesCount = 0;
			enemyInRange = false;
		}
	}

	void DebuffUnit(Unit_Base target){
		if (type == debuffType.DEFENCE) {
			target.stats.curDefence = target.stats.curDefence - debuffAmmt;
			target.TakeDebuff(debuffAmmt, "Defence");
		} else if (type == debuffType.ATTACK) {
			target.stats.curAttack = target.stats.curAttack - debuffAmmt;
			target.TakeDebuff(debuffAmmt, "Attack");
		} else {
			// get the move handler from the unit's gameobject
			Enemy_MoveHandler moveH = target.gameObject.GetComponent<Enemy_MoveHandler>();
			moveH.mStats.curMoveSpeed = moveH.mStats.curMoveSpeed - slowDownDebuffAmmnt;
			target.TakeDebuff(slowDownDebuffAmmnt, "Speed");
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		Debug.Log ("DEBUFFER: Enemy entered collider!");
		if (coll.gameObject.CompareTag("Enemy")){
			if (coll.gameObject.CompareTag ("Enemy")) {
				Debug.Log ("target in range!");
				if (enemiesCount < maxTargets) {
					enemiesInRange[enemiesCount] = coll.gameObject;
					enemiesCount++;
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D coll){
		// when any enemy exits my trigger, their stat gets reset
		if (coll.gameObject.CompareTag ("Enemy")) {
			ResetEnemy(coll.gameObject.GetComponent<Unit_Base>());	
		}
	}

	void ResetEnemy(Unit_Base target){
		if (type == debuffType.DEFENCE) {
			target.stats.curDefence = target.stats.startDefence;
		} else if (type == debuffType.ATTACK) {
			target.stats.curAttack = target.stats.startAttack;
		} else {
			Enemy_MoveHandler moveH = target.gameObject.GetComponent<Enemy_MoveHandler>();
			moveH.mStats.curMoveSpeed = moveH.mStats.startMoveSpeed;
		}
	}

	public void SetToAttackDebuff(){
		type = debuffType.ATTACK;
	}
	public void SetToDefenceDebuff(){
		type = debuffType.DEFENCE;
	}
	public void SetToSpeedDebuff(){
		type = debuffType.SPEED;
	}
}
