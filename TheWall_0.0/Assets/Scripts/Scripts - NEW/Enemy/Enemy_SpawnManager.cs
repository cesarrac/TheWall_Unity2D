using UnityEngine;
using System.Collections;

public class Enemy_SpawnManager : MonoBehaviour {
	/// <summary>
	/// This script determines when an Enemy Spawner should be instantiated and where - 
	/// the WHERE can be determined by fixed positions on the map (position that are not tiles).
	/// Collections of enemies to spawn will be stored in arrays
	/// </summary>

	// level, currentEnemies[# of wave], a method with a switch by level to fill the array of enemies
	public int level;
	public GameObject[] currEnemies; // this array is filled with Spawn_Handlers
	int maxWaves = 10; // default for level 0 is 20
	public float timeBetweenWaves = 80f;
	// a TOTAL OF 5 SPAWN POSITONS!
	public Vector3[] spawnPositions; // this would need to be filled in by resource manager, for now just manually
	bool canSpawn;
	// ENEMY SPAWNER PREFABS:
	public GameObject easyBasic, easyThieves, easyPythons, easyMixed;
	// Names of Prefabs
	string eBasicName, eThvsName, ePythName, eMixName;
	int spawnCount =0;

	public ResourceGrid resourceGrid;

	public ObjectPool objPool;

	public SpawnPoint_Handler spwnPointHandler;

	void Start () {
		InitNames ();
		InitLevelCount ();
		InitWaveTime ();
		InitMaxEnemies ();
		currEnemies = new GameObject[maxWaves];
		InitSpawners ();
		StartCoroutine (WaitToSpawn ());
	}
	void InitNames(){
		eBasicName = easyBasic.name;
		eThvsName = easyThieves.name;
		ePythName = easyPythons.name;
		eMixName = easyMixed.name;
	}

	int InitLevelCount(){
		level++;
		return level;
	}

	float InitWaveTime(){
		if (level > 1) {
			timeBetweenWaves -= 1; // making it 1 second less between waves every level
		}
		return timeBetweenWaves;
	}
	int InitMaxEnemies(){
		if (level > 1) {
			maxWaves += 10;
		}
		return maxWaves;
	}
	void InitSpawners(){
		switch (level) {
		case 1:
			currEnemies[0] = easyBasic;
			currEnemies[1] =  easyBasic;
			currEnemies[2] =  easyBasic;
			currEnemies[3] =  easyBasic;
//			currEnemies[4] =  easyThieves;
//			currEnemies[5] =  easyThieves;
//			currEnemies[6] =  easyBasic;
//			currEnemies[7] =  easyMixed;
//			currEnemies[8] = easyMixed;
//			currEnemies[9] = easyPythons;
			break;
		default:
			currEnemies[0] = easyBasic;
			currEnemies[1] =  easyBasic;
			currEnemies[2] =  easyBasic;
			currEnemies[3] =  easyBasic;
			currEnemies[4] =  easyThieves;
			currEnemies[5] =  easyThieves;
			currEnemies[6] =  easyBasic;
			currEnemies[7] =  easyMixed;
			currEnemies[8] = easyMixed;
			currEnemies[9] = easyPythons;
			break;
		}
	}
	void Update(){
		if (canSpawn) {
			StartCoroutine(WaitToSpawn());
		}
	}

	IEnumerator WaitToSpawn(){
		canSpawn = false;
		yield return new WaitForSeconds (timeBetweenWaves);
		spawnCount++;
		SpawnFromPool (spawnCount);
	}

	void SpawnFromPool(int count){
		//TOTAL OF 5 SPAWN POSITONS!
		int randomPos = Random.Range (0, spawnPositions.Length);
		if (count <= currEnemies.Length && currEnemies[count -1] != null) {
			GameObject e = objPool.GetObjectForType(eBasicName, true);
			if (e != null){
				e.transform.position = spawnPositions[randomPos];
				e.GetComponent<Enemy_SpawnHandler>().resourceGrid = resourceGrid;
				e.GetComponent<Enemy_SpawnHandler>().objPool = objPool;
				e.GetComponent<Enemy_SpawnHandler>().indexForPath = randomPos;
				e.GetComponent<Enemy_SpawnHandler>().spwnPointHandler = spwnPointHandler;
				canSpawn = true;
			}else{
				Debug.Log ("Spawn Manager can't find what you're trying to spawn!");
			}
		} else {
			// GAME OVER! YOU WON!!
			Debug.Log("No more enemies to spawn!");
		}
	}	
}
