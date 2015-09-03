using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Enemy_WaveSpawner : MonoBehaviour {

	public enum SpawnState { SPAWNING, WAITING, COUNTING, STOP };

	[System.Serializable]
	public class Wave
	{
		public string name;
		public string enemyName;
		public int count;
		public float rate;
		public int spawnPosIndex;
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

	private int nextGroup = 0;

	void Start()
	{
		if (objPool == null)
			objPool = GameObject.FindGameObjectWithTag ("Pool").GetComponent<ObjectPool> ();

		startingCountDown = startingPeaceTime;
		peaceCountDown = peaceTime;
		waveCountDown = timeBetweenWaves;

		displayTime.text = "Next Wave in: ";



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
	}

	public void ForceStartAttack()
	{
		startingCountDown = 0;
		peaceCountDown = 0;
		if (state != SpawnState.SPAWNING) {
			waveCountDown = 0;
		}
	}

	IEnumerator SpawnWave (Wave _wave)
	{
		state = SpawnState.SPAWNING;

		// Spawn
		for (int i =0; i < _wave.count; i++) 
		{
			SpawnEnemy(_wave.enemyName, _wave.spawnPosIndex);
//			Debug.Log ("SPAWNING " + _wave.enemyName);
			yield return new WaitForSeconds( 1f / _wave.rate);
		}

		nextWave ++;

		if (groupCount > 0) 
		{		// CHECK IF THERE ARE ANY GROUPS
			if (nextWave <= wavesInGroup - 1) 
			{
				// keep spawning
				waveCountDown = timeBetweenWaves;

				state = SpawnState.WAITING;

			} 
			else
			{
				// this GROUP is done spawning, go to next group
				nextGroup++;
				if (nextGroup <= groupCount - 1)
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

			// and give it the Object Pool
			_attkHandler.objPool = objPool;

			// store its position for its neighbors
			neighborEnemyPosition = _enemy.transform.position;

			// pathfinding variables
			Enemy_MoveHandler _moveHandler = _enemy.GetComponent<Enemy_MoveHandler>();
			_moveHandler.resourceGrid = resourceGrid;
			_moveHandler.spwnPtIndex = _spawnIndex;
			_moveHandler.spwnPtHandler = spwnPtHandler; 

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


