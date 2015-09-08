using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Enemy_WaveSpawner : MonoBehaviour {

	public enum SpawnState { SPAWNING, WAITING, COUNTING, STOP };

	[System.Serializable]
	public class EnemyUnit
	{
		public string enemyName;
		public int enemyCount;
		public Sprite enemySprite;
		public float spawnRate;
	}

	[System.Serializable]
	public class Wave
	{
		
		public string name;

		public EnemyUnit[] members;


		public int spawnPosIndex;
		public Sprite enemySprite;
	}



	public Wave[] waves;
	private int nextWave = 0;

	public float timeBetweenWaves = 5f;
	public float peaceTime = 60f;
	public float startingPeaceTime = 120f;

	[SerializeField]
	private float waveCountDown;

	[SerializeField]
	private float peaceCountDown;

	[SerializeField]
	private float startingCountDown;

	private SpawnState state = SpawnState.COUNTING;

	public ObjectPool objPool;

	[SerializeField]
	private Vector3[] spawnPositions;

	Vector3 neighborEnemyPosition = Vector3.zero;

	public ResourceGrid resourceGrid;

	public SpawnPoint_Handler spwnPtHandler;

	[SerializeField]
	private int maxWaves;

	private Enemy_MoveHandler lastEnemy;

	public Text displayTime;

	public List< Enemy_MoveHandler> spawnedEnemies = new List<Enemy_MoveHandler>();

	[SerializeField]
	private int wavesInGroup;

	[SerializeField]
	private int groupCount;

	private int nextGroup = 1;

	GameObject[] indicators;

	private bool indicatorsCreated = false;

	private int nextWaveInGroup = 0; // gets reset everytime a group is done spawning

	void Start()
	{
		if (objPool == null)
			objPool = GameObject.FindGameObjectWithTag ("Pool").GetComponent<ObjectPool> ();

		startingCountDown = startingPeaceTime;
		peaceCountDown = peaceTime;
		waveCountDown = timeBetweenWaves;

		displayTime.text = "Next Wave in: ";

		indicators = new GameObject[wavesInGroup];

		// create initial indicators
		CreateSpawnPointIndicators ();

	}

	void Update()
	{
		if (startingCountDown <= 0) {

			if (peaceCountDown <= 0) {
				displayTime.color = Color.red;

				// start the wave countdown	
				if (waveCountDown <= 0) {
					// check if we are already spawning
					if (state != SpawnState.SPAWNING && state != SpawnState.STOP) {

						displayTime.text = waves [nextWave].name + " INCOMING!";

						// get rid of the indicators
						for (int i =0; i < indicators.Length; i++){
							objPool.PoolObject(indicators[i]);
//							indicators[i] = null;
						}
						// reset bool
						indicatorsCreated = false;

						// Start spawning a wave
						StartCoroutine (SpawnWave (waves [nextWave]));
					}
				} else {
					waveCountDown -= Time.deltaTime;
					displayTime.text = "Wave Approaching in: " + waveCountDown.ToString ("f1");
				}
			} else {
				peaceCountDown -= Time.deltaTime;
				displayTime.text = "Next Wave in: " + peaceCountDown.ToString ("f1");
			}

		} else {
			startingCountDown -= Time.deltaTime;
			displayTime.color = Color.green;
			displayTime.text = "Attack Incoming in: " + startingCountDown.ToString("f1");
		}

		// Creates indicators as soon as we are on peace count down
		if (peaceCountDown > 0) {
			if (!indicatorsCreated)
				CreateSpawnPointIndicators();
		}

	}

	public void ForceStartAttack()
	{
		startingCountDown = 0;
		peaceCountDown = 0;
		if (state != SpawnState.SPAWNING) {
			waveCountDown = 0;
		}
	}

	public void CreateSpawnPointIndicators()
	{

		// create spawn point indicators for each wave in a group
		for (int i =0; i < wavesInGroup; i++) {
			// find the wave to get its info
			Wave thisWave = waves[nextWave + i];
			// get a new spawn indicator
			GameObject spwnIndicator = objPool.GetObjectForType("Spawn Indicator", true);

			if (spwnIndicator != null){

				// add this indicator to our array
				indicators[i] = spwnIndicator;

				// place it on the right location
				spwnIndicator.transform.position = spawnPositions[thisWave.spawnPosIndex];

				// then fill its information
				Enemy_ForceSpawn indicator = spwnIndicator.GetComponent<Enemy_ForceSpawn>();
				indicator.spwnIndicator.Init(thisWave.name, thisWave.members.Length, thisWave.enemySprite);

				// now that its info has been initialized, we can set the info
				indicator.SetIndicator();

				// and give it access to the functions on this script
				indicator.enemyWaveSpawner = this;

			}else{
				Debug.Log("WAVE SPAWNER: Pool can't find Spawn Indicator!");
			}
		}

		indicatorsCreated = true;
	}

	IEnumerator SpawnWave (Wave _wave)
	{
		Debug.Log ("WAVE: Spawning wave " + nextWave + ", group member: " + nextWaveInGroup);

		state = SpawnState.SPAWNING;

		// Spawn
			// Loop through members in a wave
		for (int x = 0; x < _wave.members.Length; x++) {
			// each member has a name and a count, loop through this count too
			for (int y = 0; y < _wave.members[x].enemyCount; y++){
				// spawn this enemy name
				SpawnEnemy(_wave.members[x].enemyName, _wave.spawnPosIndex);

				yield return new WaitForSeconds (1f / _wave.members[x].spawnRate);
			}
		}

		nextWave ++;

		if (groupCount > 0) 
		{		// CHECK IF THERE ARE ANY GROUPS
			if (nextWaveInGroup < wavesInGroup - 1) // this needs to say if it there has been at least wavesIngroup spawned
			{
				Debug.Log ("WAVE: still spawning from same group!");
				nextWaveInGroup ++;

				// keep spawning
				waveCountDown = timeBetweenWaves;

				state = SpawnState.WAITING;

			} 
			else
			{
				Debug.Log ("WAVE: spawning from NEW group!");
				// reset next wave in group
				nextWaveInGroup = 0;

				// this GROUP is done spawning, go to next group
				nextGroup++;

				if (nextGroup <= groupCount)
				{
					// start peace time and keep nextwave with its current value
					peaceCountDown = peaceTime;
					waveCountDown = timeBetweenWaves;
					state = SpawnState.COUNTING;
				}
				else
				{
					// No more groups, so STOP spawning
					state = SpawnState.STOP;

					displayTime.gameObject.SetActive(false);
				}

			}
		} 
		else 
		{					// THERE ARE NO GROUPS
			if (nextWave <= maxWaves - 1) 
			{
				waveCountDown = timeBetweenWaves;

				state = SpawnState.WAITING;
				
			} 
			else 
			{
				// No more Waves left, STOP Spawning
				state = SpawnState.STOP;


				displayTime.gameObject.SetActive(false);
			}
		}

		yield break;
	}

	void SpawnEnemy(string _enemyName, int _spawnIndex)
	{
		// Spawn and fill components
		GameObject _enemy = objPool.GetObjectForType (_enemyName, true);

		if (_enemy != null) {

			// Give it its starting position
			_enemy.transform.position = spawnPositions[_spawnIndex];

			// reset its Stats in case it just got brought back from the Pool
			Enemy_AttackHandler _attkHandler = _enemy.GetComponentInChildren<Enemy_AttackHandler>();
			_attkHandler.stats.Init();

			// also reset their Health Bar 
			_attkHandler.statusIndicator.SetHealth(_attkHandler.stats.curHP, _attkHandler.stats.maxHP);
			Debug.Log ("WAVESPAWNER: spawning unit with curHP: " + _attkHandler.stats.curHP + " and maxHP: " + _attkHandler.stats.maxHP);

			// and give it the Object Pool
			_attkHandler.objPool = objPool;

			// store its position for its neighbors
			neighborEnemyPosition = _enemy.transform.position;

			// pathfinding variables
			Enemy_MoveHandler _moveHandler = _enemy.GetComponent<Enemy_MoveHandler>();
			_moveHandler.resourceGrid = resourceGrid;
			_moveHandler.spwnPtIndex = _spawnIndex;
			_moveHandler.spwnPtHandler = spwnPtHandler; 

			// reset the current move speed in case this unit was affected by a DeBuffer
			_moveHandler.mStats.curMoveSpeed = _moveHandler.mStats.startMoveSpeed;

			// feed it the move handler from the enemy spawned before this one so it has a buddy
			if (lastEnemy != null){
				// make sure they belong to the same wave
				if (lastEnemy.gameObject.name == _enemyName){
					_moveHandler.myBuddy = lastEnemy;
				}else{
					// not the same name so this must be the first spawn of a new wave
					lastEnemy = null;
				}

			}

			lastEnemy = _moveHandler;

			// Add this newly Spawned enemy to the list of spawned enemies
			spawnedEnemies.Add(_moveHandler);
		}
	}

	/// <summary>
	/// Gets the spawned enemy and returns type of
	/// Enemy_MoveHandler.
	/// </summary>
	/// <returns>The spawned enemy.</returns>
	/// <param name="count">Count.</param>
	public Enemy_MoveHandler GetSpawnedEnemy ( int count){
		return spawnedEnemies[count];
	}


}


