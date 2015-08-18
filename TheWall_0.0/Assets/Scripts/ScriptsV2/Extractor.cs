using UnityEngine;
using System.Collections;

public class Extractor : MonoBehaviour {
	public int mapPosX, mapPosY;
	public ResourceGrid resourceGrid;
	bool rockFound, canExtract = false;
	// Storing the rock tiles that this building can find (EXTRACTOR ONLY!)
	public Vector2[] rocksDetected;

	int rockPosX, rockPosY, currRockIndex;

	public float extractTime;

	public int extractRate;

	public Player_ResourceManager playerResources;

	void Start(){
		// INIT rocksdetected array
		// This assumes that we are only checking tiles ONE TILE OVER in all directions
		rocksDetected = new Vector2[8]; 

		if (SearchForRock ()) {
			CycleRocksArray();
		}
	}
	

	void Update () {
		if (canExtract) {
			StartCoroutine(WaitToExtract());
		}
	}

	bool SearchForRock(){
		if (CheckTileType(mapPosX, mapPosY + 1)){ // top

			rockFound = true;
			// fill the array
			rocksDetected[0] = new Vector2(mapPosX, mapPosY + 1);
		}
		if (CheckTileType(mapPosX, mapPosY - 1)){ // bottom

			rockFound = true;
			rocksDetected[1] = new Vector2(mapPosX, mapPosY - 1);
		}
		if (CheckTileType(mapPosX - 1, mapPosY)){ // left
			rockFound = true;

			rocksDetected[2] = new Vector2(mapPosX - 1, mapPosY);
		}
		if (CheckTileType(mapPosX + 1, mapPosY)){ // right
			rockFound = true;

			rocksDetected[3] = new Vector2(mapPosX + 1, mapPosY);
		}
		if (CheckTileType(mapPosX - 1, mapPosY + 1)){ // top left
			rockFound = true;

			rocksDetected[4] = new Vector2(mapPosX - 1, mapPosY + 1);
		}
		if (CheckTileType(mapPosX + 1, mapPosY + 1)){ // top right
			rockFound = true;

			rocksDetected[5] = new Vector2(mapPosX + 1, mapPosY + 1);
		}
		if (CheckTileType(mapPosX - 1, mapPosY - 1)){ // bottom left
			rockFound = true;

			rocksDetected[6] = new Vector2(mapPosX - 1, mapPosY - 1);
		}
		if (CheckTileType(mapPosX + 1, mapPosY - 1)){ // bottom right
			rockFound = true;

			rocksDetected[7] = new Vector2(mapPosX + 1, mapPosY - 1);
		}
		return rockFound;
	}

	bool CheckTileType(int x, int y){
		if (x < resourceGrid.mapSizeX && y < resourceGrid.mapSizeY && x > 0 && y > 0) {
			if (resourceGrid.tiles [x, y].tileType == TileData.Types.rock) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	void CycleRocksArray(){
		for (int x =0; x< rocksDetected.Length; x++){
			if (rocksDetected[x] != Vector2.zero){
				rockPosX = (int) rocksDetected[x].x;
				rockPosY = (int) rocksDetected[x].y;
				currRockIndex = x;
				canExtract = true;
			}else{
				Debug.Log("No rock found at: " + rocksDetected[x]);
			}
		}
	}

	IEnumerator WaitToExtract(){
		canExtract = false;
		yield return new WaitForSeconds(extractTime);
		Extract ();
	}

	void Extract(){
		int q = resourceGrid.tiles [rockPosX, rockPosY].maxResourceQuantity;
		int calc = q - extractRate;
		// subtract it from the tile
		resourceGrid.tiles [rockPosX, rockPosY].maxResourceQuantity = calc;
		// add it to Player resources
		playerResources.ChangeResource ("Ore", extractRate);
		// check if tile is depleted
		int newQ = resourceGrid.tiles [rockPosX, rockPosY].maxResourceQuantity;
		Debug.Log ("Extracting!");
		if (newQ <= 0) {
			DepleteRock (rockPosX, rockPosY);
		} else {
			canExtract = true;
		}
	}

	void DepleteRock(int x, int y){
		Debug.Log ("Rock depleted at: " + x + ", " + y);
		resourceGrid.SwapTileType (x, y, TileData.Types.empty);
		// change value of this rock in the array
		rocksDetected [currRockIndex] = Vector3.zero;
		CycleRocksArray ();
	}
}
