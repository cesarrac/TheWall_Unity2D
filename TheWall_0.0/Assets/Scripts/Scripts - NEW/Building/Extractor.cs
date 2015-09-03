using UnityEngine;
using System.Collections;

public class Extractor : MonoBehaviour {
	public int mapPosX, mapPosY;
	public ResourceGrid resourceGrid;
	bool rockFound, canExtract = false;
	// Storing the rock tiles that this building can find (EXTRACTOR ONLY!)
	public Vector2[] rocksDetected;

	int rockPosX, rockPosY, currRockIndex;

	public float extractRate;

	public int extractAmmnt;

	public Player_ResourceManager playerResources;

	public bool starvedMode; // MANIPULATED BY THE RESOURCE MANAGER

	LineRenderer lineR;
	public bool selecting;
	Vector3 mouseEnd;

	Building_UIHandler buildingUI;

	Storage myStorage; // is set when player connects the plant to a storage building

	bool statsInitialized;

	SpriteRenderer sr;

	void Awake(){
		lineR = GetComponent<LineRenderer> ();
	}

	void Start(){
		// INIT rocksdetected array
		// This assumes that we are only checking tiles ONE TILE OVER in all directions
		rocksDetected = new Vector2[8]; 

//		if (SearchForRock ()) {
//			CycleRocksArray();
//		}
		if (resourceGrid == null) {
			resourceGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<ResourceGrid>();
		}

		if (buildingUI == null) {
			buildingUI = GameObject.FindGameObjectWithTag ("UI").GetComponent<Building_UIHandler> ();
		}

		sr = GetComponent<SpriteRenderer> ();
		lineR.sortingLayerName = sr.sortingLayerName;
		lineR.sortingOrder = sr.sortingOrder - 1;
		lineR.SetPosition (0, transform.position);
		selecting = true;
	}
	

	void Update () {
		if (selecting)
		{
			lineR.enabled = true;
			myStorage = null;
			LineFollowMouse();
			buildingUI.currentlyBuilding = true;
			if (Input.GetMouseButtonUp (0)) 
			{
				SetStorageAndExtract();
			}
		}



		if (canExtract && !starvedMode) {
			StartCoroutine (WaitToExtract ());
		}

		if (!selecting && myStorage == null) {
			lineR.enabled = false;
			canExtract = false;
			Debug.Log ("Need STORAGE!");
		} else if (!selecting && myStorage != null) {
			if (!statsInitialized){
				playerResources.CalculateOreProduction(extractAmmnt, extractRate, false);
				statsInitialized = true;
			}
		}
	}

	void LineFollowMouse(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mouseEnd = new Vector3 (Mathf.Clamp(m.x, transform.position.x - 10f, transform.position.x + 10f), 
		                        Mathf.Clamp(m.y, transform.position.y - 10f, transform.position.y + 10f), 
		                        0.0f);
		lineR.SetPosition (1, mouseEnd);
	}

	/// <summary>
	/// Sets selecting bool to true. Useful for attaching to
	/// an On-Click Event on a Button for Resetting this extractor's storage.
	/// </summary>
	public void ActivateSelecting(){
		if (!selecting) {
			selecting = true;
		}
	}

	void SetStorageAndExtract(){
		int mX = Mathf.RoundToInt(mouseEnd.x);
		int mY = Mathf.RoundToInt(mouseEnd.y);
		if (mX > 2 && mX < resourceGrid.mapSizeX - 2 && mY > 2 && mY < resourceGrid.mapSizeY - 2) {
			if (resourceGrid.GetTileType (mX, mY) == TileData.Types.storage) {
				Debug.Log("Storage found for ore!");
				selecting = false;
			
				buildingUI.currentlyBuilding = false;
				//			lineR.enabled = false;
				// set my storage
				myStorage = resourceGrid.GetTileGameObj (mX, mY).GetComponent<Storage> ();

				// start extracting by finding which direction our rock is
				if (SearchForRock ()) {
					CycleRocksArray ();
				}
			} else {
				Debug.Log ("Need a place to store the ore!");
			}
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
			if (resourceGrid.GetTileType(x, y) == TileData.Types.rock){
				return true;
			}else{
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
		yield return new WaitForSeconds(extractRate);
		if (myStorage != null) {
			Extract ();
		} 
	}

//	void Extract(){
//		int q = resourceGrid.tiles [rockPosX, rockPosY].maxResourceQuantity;
//		int calc = q - extractAmmnt;
//		// subtract it from the tile
//		resourceGrid.tiles [rockPosX, rockPosY].maxResourceQuantity = calc;
//		// add it to Player resources
//		playerResources.ChangeResource ("Ore", extractAmmnt);
//		// check if tile is depleted
//		int newQ = resourceGrid.tiles [rockPosX, rockPosY].maxResourceQuantity;
//		Debug.Log ("Extracting!");
//		if (newQ <= 0) {
//			DepleteRock (rockPosX, rockPosY);
//		} else {
//			canExtract = true;
//		}
//	}

	void Extract(){
		int q = resourceGrid.tiles [rockPosX, rockPosY].maxResourceQuantity;
		int calc = q - extractAmmnt;
		if (calc > 0) { // theres still ore left in this rock
			Debug.Log ("Extracting!");
		
			// check that storage is not full
			if (!myStorage.CheckIfFull (extractAmmnt, false)) {

				// add it to Storage
				myStorage.AddOreOrWater (extractAmmnt, false);

				// subtract it from the tile
				resourceGrid.tiles [rockPosX, rockPosY].maxResourceQuantity = calc;

				// check if tile is depleted
				int newQ = resourceGrid.tiles [rockPosX, rockPosY].maxResourceQuantity;

				if (newQ <= 0) {
					// it is Depleted
					DepleteRock (rockPosX, rockPosY);
				} else {
					// it's not Depleted so continue extracting
					canExtract = true;
				}

			} else {

				// storage is full and extractor stops until it gets a new storage
				myStorage = null;

//				// keep extracting
//				if (SearchForRock ()) {
//					CycleRocksArray();
//				}
			}


		} else { // tile is depleted
			DepleteRock(rockPosX, rockPosY);
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
