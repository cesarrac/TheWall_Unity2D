using UnityEngine;
using System.Collections;

public class Tower_DeBuffer : Unit_Base {
	

	bool canShoot;
	
	private GameObject targetUnit;
	
	
	private GameObject[] enemiesInRange;
	
	public int maxTargets; // caps out how many enemies it can damage at the same time
	int enemiesCount = 0;
	
	enum debuffType { DEFENCE, ATTACK, SPEED}

	[SerializeField]
	private debuffType type = debuffType.DEFENCE;

	public float debuffAmmt, slowDownDebuffAmmnt = 0.3f;

	public enum State { DEBUFFING, SEEKING, STARVED } ;

	private State _state = State.SEEKING;

	[HideInInspector]
	public State state { get { return _state; } set { _state = value; } }

	[SerializeField]
	private Building_StatusIndicator bStatusIndicator;
	
	private bool statusIndicated = false;



	void Start () {

		if (bStatusIndicator == null)
			Debug.Log ("GUN: Building Status Indicator NOT SET!");

		if (resourceGrid == null)
			resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();
		
		// Initialize building stats
		stats.Init ();
		InitTileStats((int)transform.position.x, (int)transform.position.y);

		// If Object Pool is null we can get it from the Click Handler
		if (objPool == null)
			objPool = GetComponentInParent<Building_ClickHandler> ().objPool;
		
		enemiesInRange = new GameObject[maxTargets];
	}
	

	void Update () {

		MyStateMachine (state);

	}

	void MyStateMachine(State curState)
	{
		switch (curState) {
		case State.SEEKING:
			if (enemiesCount > 1)
				state = State.DEBUFFING;
			break;
		case State.DEBUFFING:
			HandleDeBuffToUnits();

			if (!statusIndicated){
				if (type == debuffType.ATTACK){

					IndicateStatus("Debuffing " + type.ToString().ToLower(), Color.magenta);

				}else if (type == debuffType.DEFENCE){

					IndicateStatus("Debuffing " + type.ToString().ToLower(), Color.blue);

				}else{

					IndicateStatus("Debuffing " + type.ToString().ToLower(), Color.cyan);
				}
			}
				

			break;
		default:
			// building is starved
			break;
		}
	}

	void IndicateStatus(string status, Color _color = default(Color))
	{
		if (buildingStatusIndicator != null) {
			buildingStatusIndicator.CreateStatusMessage(status, _color);
			
			statusIndicated = true;
		}
	}


	void HandleDeBuffToUnits()
	{
		if (enemiesInRange [0] != null) { // only loop if there's at least the first guy detected

			// indicate that we are debuffing
			statusIndicated = false;

			for (int x =0; x < enemiesInRange.Length; x++) {

				if (enemiesInRange [x] != null) { // target hasn't already been killed

					DebuffUnit (enemiesInRange [x].GetComponent<Unit_Base> ());

					// after DeBuffing each one, remove them from the array
					enemiesInRange[x] = null;

				}

				// after looping everyone reset enemycount
				enemiesCount = 0;

				// If NOT starved
				if (_state != State.STARVED){
					// State goes back to seeking, waiting for more targets to enter trigger
					_state = State.SEEKING;

				}
			}

		}else {
			// There were NO eneies detected, reset enemies count
			enemiesCount = 0;

			// If NOT starved
			if (_state != State.STARVED)
				// State goes back to seeking, waiting for more targets to enter trigger
				state = State.SEEKING;

		}
	}

	void DebuffUnit(Unit_Base target)
	{

		if (type == debuffType.DEFENCE) {

			target.stats.curDefence = target.stats.curDefence - debuffAmmt;
			target.TakeDebuff(debuffAmmt, "Defence");

		} else if (type == debuffType.ATTACK) {

			target.stats.curAttack = target.stats.curAttack - debuffAmmt;
			target.TakeDebuff(debuffAmmt, "Attack");

		} else {

			// Get the move handler from the unit's gameobject
			Enemy_MoveHandler moveH = target.gameObject.GetComponent<Enemy_MoveHandler>();
			moveH.mStats.curMoveSpeed = moveH.mStats.curMoveSpeed - slowDownDebuffAmmnt;
			target.TakeDebuff(slowDownDebuffAmmnt, "Speed");
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.CompareTag("Enemy")){

			if (coll.gameObject.CompareTag ("Enemy")) {

				if (enemiesCount < maxTargets) {

					enemiesInRange[enemiesCount] = coll.gameObject;
					enemiesCount++;

				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		// when any enemy exits my trigger, their stat gets reset
		if (coll.gameObject.CompareTag ("Enemy")) {

			ResetEnemy(coll.gameObject.GetComponent<Unit_Base>());	

		}
	}

	void ResetEnemy(Unit_Base target)
	{
		if (type == debuffType.DEFENCE) {

			target.stats.curDefence = target.stats.startDefence;

		} else if (type == debuffType.ATTACK) {

			target.stats.curAttack = target.stats.startAttack;

		} else {

			Enemy_MoveHandler moveH = target.gameObject.GetComponent<Enemy_MoveHandler>();
			moveH.mStats.curMoveSpeed = moveH.mStats.startMoveSpeed;

		}
	}

	// Methods used by Buttons:
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
