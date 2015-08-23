using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpawnPoint_Handler : MonoBehaviour {
	public ResourceGrid resourceGrid;
	public Vector2[] spawnPositions;

	public List<Node>[] path;

	void Awake(){
		path = new List<Node>[spawnPositions.Length];
	}
	void Start () {
		for (int x =0; x< spawnPositions.Length; x++) {
			resourceGrid.GenerateWalkPath(resourceGrid.capitalSpawnX,resourceGrid.capitalSpawnY, false, 
			                              (int)spawnPositions[x].x, (int)spawnPositions[x].y);
			if (resourceGrid.pathToCapital != null)
				FillPath(resourceGrid.pathToCapital, x);
		}

	}
	void FillPath(List<Node> currPath, int i){

		path[i] = new List<Node>();
		for (int y = 0; y < currPath.Count; y++) {
			path [i].Add (currPath [y]);
		}

	}
}
