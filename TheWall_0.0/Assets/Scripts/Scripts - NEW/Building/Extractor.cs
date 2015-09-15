using UnityEngine;
using System.Collections;

public class Extractor : MonoBehaviour {
	public int mapPosX, mapPosY;
	public ResourceGrid resourceGrid;
	bool rockFound;
	// Storing the rock tiles that this building can find (EXTRACTOR ONLY!)
	public Vector2[] rocksDetected;

	int rockPosX, rockPosY, currRockIndex;

	public float extractRate;

	public int extractAmmnt;

	public Player_ResourceManager playerResources;

	LineRenderer lineR;
	public bool selecting;
	Vector3 mouseEnd;

	Building_UIHandler buildingUI;

	Storage myStorage; // is set when player connects the plant to a storage building

	bool statsInitialized;

	SpriteRenderer sr;

	public enum State 
	{
		EXTRACTING,
		SEARCHING,
		NOSTORAGE,
		STARVED
	}
	
	private State _state = State.NOSTORAGE;

	[HideInInspector]
	public State state { get { return _state; } set { _state = value; } }

	private float extractCountDown;




	void Awake(){

		// Store the Line Renderer
		lineR = GetComponent<LineRenderer> ();

	}

	void Start()
	{

		// INIT rocksdetected array
		// This assumes that we are only checking tiles ONE TILE OVER in all directions
		rocksDetected = new Vector2[8]; 

		// In Case Grid is null
		if (resourceGrid == null) {
			resourceGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<ResourceGrid>();
		}

		// In case Building UI is null
		if (buildingUI == null) {
			buildingUI = GameObject.FindGameObjectWithTag ("UI").GetComponent<Building_UIHandler> ();
		}

		// Store the Sprite Renderer for layer management
		sr = GetComponent<SpriteRenderer> ();

		// Line Renderer's layer is set to be UNDER my sprite
		lineR.sortingLayerName = sr.sortingLayerName;
		lineR.sortingOrder = sr.sortingOrder - 1;
		lineR.SetPosition (0, transform.position);

		// Set selecting is true so Line follows Mouse from Start
//		selecting = true;

		// set Extract Countdown to extraction rate
		extractCountDown = extractRate;

	}
	

	void Update () {

		if (!selecting && myStorage == null) {

			lineR.enabled = false;

			// This means that either the Storage we were using was destroyed OR is full, so change state to stop extraction
			_state = State.NOSTORAGE;

		} else if (!selecting && myStorage != null) {

			// Give the Player Resource Manager our stats to show on Food Production panel
			if (!statsInitialized){
				playerResources.CalculateOreProduction(extractAmmnt, extractRate, false);
				statsInitialized = true;
			}
		}

		MyStateMachine (_state);
	}

	void MyStateMachine(State curState)
	{
		switch (curState) {

		case State.EXTRACTING:
			CountDownToExtract();
			break;

		case State.NOSTORAGE:
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
			break;

		case State.SEARCHING:
			Debug.Log ("EXTRACTOR: Searching for rock...");
			break;

		default:
			// starved
			break;
		}
	}

	void CountDownToExtract()
	{
		if (extractCountDown <= 0) {

			Extract();
			extractCountDown = extractRate;

		} else {

			extractCountDown -= Time.deltaTime;

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

			// Make sure that where we clicked on the Grid is a storage tile
			if (resourceGrid.GetTileType (mX, mY) == TileData.Types.storage) {
				Debug.Log("Storage found for ore!");

				// Selecting is now false to deactivate the Line Renderer gameobject
				selecting = false;
			
				// Give Building UI ability to access to building menus by clicking again
				buildingUI.currentlyBuilding = false;

				// Set my storage
				myStorage = resourceGrid.GetTileGameObj (mX, mY).GetComponent<Storage> ();

				// Start extracting by finding which direction rock was found
				if (SearchForRock ()) {

					// Cycle through the rocks found array to make sure we don't extract from a NULL (represented by Vector2.zero)
					CycleRocksArray ();
				}


			} else {
				Debug.Log ("Need a place to store the ore!");
				// state will stay on NO STORAGE
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

				// Rock has been detected so we can change state
				_state = State.EXTRACTING;

			}else{
				Debug.Log("No rock found at: " + rocksDetected[x]);
			}
		}
	}


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

					// This rock is Depleted, so change state to stop extraction while we Search for more rock
					_state = State.SEARCHING;

					// Deplete the rock and check for more
					DepleteRock (rockPosX, rockPosY);
				} 

			} else {

				// Storage is full and extractor stops until it gets a new storage
				myStorage = null;

				// Change state to stop extraction while we get a new Storage
				_state = State.NOSTORAGE;
			}


		} else { 

			// This rock is Depleted, so change state to stop extraction while we Search for more rock
			_state = State.SEARCHING;

			// Deplete the rock and check for more
			DepleteRock (rockPosX, rockPosY);
		}
	}

	void DepleteRock(int x, int y){

		// To Deplete a rock, swap tile to empty
		resourceGrid.SwapTileType (x, y, TileData.Types.empty);

		// change value of this rock in the array so we don't try extracting from it anymore
		rocksDetected [currRockIndex] = Vector3.zero;

		// Cycle the array to see if there is any more rock around us
		CycleRocksArray ();
	}
}
