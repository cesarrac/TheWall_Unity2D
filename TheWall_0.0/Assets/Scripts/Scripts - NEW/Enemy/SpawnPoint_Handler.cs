using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpawnPoint_Handler : MonoBehaviour {
	public ResourceGrid resourceGrid;
	public Vector2[] spawnPositions;

	public List<Node>[] kamikazePath;
	public List<Node>[] path;

	public Vector2[] kamikazeDestinations;

	[Header("Kamikaze Min. and Max. X/Y positions")]
	public float minKamiX, maxKamiX, minKamiY, maxKamiY;

	private float randomKamikazeX;
	private float randomKamikazeY;

	void Awake(){
		path = new List<Node>[spawnPositions.Length];
		kamikazePath = new List<Node>[spawnPositions.Length]; 

		// Create some random Kamikaze positions
		RandomKamikaze ();
	}

	void RandomKamikaze()
	{
		// Loop through the Kamikaze Destinations array and fill each with a random X and Y
		for (int x = 0; x < kamikazeDestinations.Length; x++) {
			randomKamikazeX = Random.Range(minKamiX, maxKamiX);
			randomKamikazeY = Random.Range(minKamiY, maxKamiY);
			kamikazeDestinations[x] = new Vector2(Mathf.Round(randomKamikazeX), Mathf.Round(randomKamikazeY));
		}
	}

	void Start () {

		if (resourceGrid == null)
			resourceGrid = GameObject.FindGameObjectWithTag ("Map").GetComponent<ResourceGrid> ();

		if (resourceGrid != null) {
			// Get Paths to capital from all Spawn Positions
			for (int x =0; x< spawnPositions.Length; x++) {
				resourceGrid.GenerateWalkPath (resourceGrid.capitalSpawnX, resourceGrid.capitalSpawnY, false, 
			                              (int)spawnPositions [x].x, (int)spawnPositions [x].y);
				if (resourceGrid.pathForEnemy != null)
					FillPath (resourceGrid.pathForEnemy, x, false);
			}

			// Then get Paths to kamikaze destinations
			for (int x =0; x< spawnPositions.Length; x++) {
				resourceGrid.GenerateWalkPath ((int)kamikazeDestinations[x].x, (int)kamikazeDestinations[x].y, false, 
				                               (int)spawnPositions [x].x, (int)spawnPositions [x].y);
				if (resourceGrid.pathForEnemy != null)
					FillPath (resourceGrid.pathForEnemy, x, true);
			}
		}


	}
	void FillPath(List<Node> currPath, int i, bool trueIfKamikaze){

		if (!trueIfKamikaze) {
			path [i] = new List<Node> ();
			for (int y = 0; y < currPath.Count; y++) {
				path [i].Add (currPath [y]);
			}

			Debug.Log ("PATH TO CAPITAL: " + i + " From: " + path [i] [0].x + " " + path [i] [0].y + " To: " + path [i] [path [i].Count - 1].x + " " + path [i] [path [i].Count - 1].y);

		} else {
			kamikazePath [i] = new List<Node> ();
			for (int y = 0; y < currPath.Count; y++) {
				kamikazePath [i].Add (currPath [y]);
			}
			Debug.Log ("KAMIKAZE PATH: " + i + " From: " + kamikazePath [i] [0].x + " " + kamikazePath [i] [0].y + " To: " + kamikazePath [i] [kamikazePath [i].Count - 1].x + " " + kamikazePath [i] [kamikazePath [i].Count - 1].y);

		}
	}
	
}
