using UnityEngine;
using System.Collections;

public class DeSalt_Plant : MonoBehaviour {
	LineRenderer lineR;
	public bool selecting;
	public float pumpRate;
	public int waterPumped;
	bool canPump = false;
	Vector3 mouseEnd;

	public ResourceGrid resourceGrid;

	Storage myStorage; // is set when player connects the plant to a storage building

	Building_UIHandler buildingUI;

	public bool starvedMode; // MANIPULATED BY THE RESOURCE MANAGER

	public Player_ResourceManager playerResources;

	bool statsInitialized;

	void Awake(){
		lineR = GetComponent<LineRenderer> ();
	
	}
	void Start () {
		if (resourceGrid == null) {
			resourceGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<ResourceGrid>();
		}
		if (buildingUI == null) {
			buildingUI = GameObject.FindGameObjectWithTag ("UI").GetComponent<Building_UIHandler> ();
		}

		if (playerResources == null) {
			playerResources = GameObject.FindGameObjectWithTag("Capital").GetComponent<Player_ResourceManager>();
		}
		lineR.SetPosition (0, transform.position);
		selecting = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (selecting)
		{
			lineR.enabled = true;
			myStorage = null;
			LineFollowMouse();
			buildingUI.currentlyBuilding = true;
			if (Input.GetMouseButtonUp (0)) 
			{
				SetStorageAndPump();
			}
		}

	

		if (canPump && !starvedMode) {
			StartCoroutine(WaitToPump());
		}

		if (!selecting && myStorage == null) {
			lineR.enabled = false;
			canPump = false;
			Debug.Log ("Need STORAGE!");
		} else if (!selecting && myStorage != null) {
			if (!statsInitialized){
				playerResources.CalculateWaterProduction(waterPumped, pumpRate, false);
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

	public void ActivateSelecting(){
		if (!selecting) {
			selecting = true;
		}
	}

	void SetStorageAndPump(){
		int mX = Mathf.RoundToInt(mouseEnd.x);
		int mY = Mathf.RoundToInt(mouseEnd.y);
		if (mX > 2 && mX < resourceGrid.mapSizeX - 2 && mY > 2 && mY < resourceGrid.mapSizeY - 2) {
			if (resourceGrid.GetTileType (mX, mY) == TileData.Types.storage) {
				selecting = false;

				buildingUI.currentlyBuilding = false;
//			lineR.enabled = false;
				// set my storage
				myStorage = resourceGrid.GetTileGameObj (mX, mY).GetComponent<Storage> ();
				// start pumping!!
				canPump = true;
			} else {
				Debug.Log ("Need a place to store the water!");
			}
		}
	}

	IEnumerator WaitToPump(){
		canPump = false;
		yield return new WaitForSeconds(pumpRate);
		if (myStorage != null) {
			PumpIt ();
		} 
	}

	void PumpIt(){
		// check that storage is not full
		if (!myStorage.CheckIfFull (waterPumped, true)) {
			// add it to Storage
			myStorage.AddOreOrWater (waterPumped, true);
			// keep pumping
			canPump = true;
		} else {
			// storage is full and pump stops until it gets a new storage
			myStorage = null;
		}
	}
}
