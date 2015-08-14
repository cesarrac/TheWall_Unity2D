using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapV2 : MonoBehaviour {
	/// <summary>
	/// Creates the grid of possible positions the Player can move to according to # of colums and rows.
	/// Stores those positions in a list.
	/// Every time this script is loaded and a new map is created a level variable will increase.
	/// It then calls ResourceGrid with a level and it will generate resources according to the grid and level.
	/// </summary>

				// list of possible positions on the grid
//	[HideInInspector]
//	public List<Vector3> gridPositions = new List<Vector3>();

	ResourceGrid resourceGrid;
	int level;
				// rows and colums determined by level
	public int mapSizeX;
	public int mapSizeY;
				// Transforms for holding the Tiles in the Hierarchy
	public Transform tileHolder;

	public Vector3 playerSpawnPos;

	public TileType[] tileTypes;


	int[,] tiles;

	void Awake(){


		if (level == 0) {
			mapSizeX = 5;
			mapSizeY = 5;
		}

		resourceGrid = GetComponent<ResourceGrid> ();
	}

	void Start () {
		// initialize the tiles array
		tiles = new int[mapSizeX, mapSizeY];

		InitGrid ();

	}

	void InitGrid(){
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++){
//				gridPositions.Add(new Vector3(x, y, 0.0f)); // default Z to -2 so resource tiles get detected first by linecasts
				tiles[x,y] = 0;

			}
		}
	}


}
