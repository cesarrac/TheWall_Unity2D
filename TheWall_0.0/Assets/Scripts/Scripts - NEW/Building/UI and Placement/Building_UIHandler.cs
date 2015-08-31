using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Building_UIHandler : MonoBehaviour {

	TileData.Types currentTileType;
	public int currPosX, currPosY;// storing the map pos from click handler 

	// this is the MAIN BUILD PANEL at the bottom of the screen
	public GameObject mainBuildPanel;
	public Button buildBttnFab;
	// Building Sprites
	public Sprite buildingSprite, extractSprite, machineGunSprite, seaWSprite, harpoonHSprite, 
	cannonSprite, sFarmSprite, storSprite, sDesaltSprite;

	// Building Names
	public string buildingName;

	// PreFabs (alpha set to 1/2 to represent un-built building)
//	public GameObject sExtractFab, mExtractFab, lExtractFab;

	// these objects are for handling the upgrade panel that pop up on top of already built buildings
	public GameObject buildUpgradePanelFab;
	private GameObject buildUpgradePanel;


	public Button buildUpgradeBttn;
	public Button closeBttn;

//	string currBuildingName;

	Button[] activeButtons;

	public Canvas canvas;
	private RectTransform canvasRectTransform;

	public ResourceGrid resourceGrid;

	public Player_ResourceManager resourceManager;

	// Costs of Buildings:
	public int[] extractCost, mGunCost, seaWCost, hHallCost, cannonCost,sFarmCost, storageCost, sDesaltCost;

	// Indicator that pops up just over the build panel, destroys itself
	public GameObject indicatorFab;
//
//	public Player_SpawnHandler playerSpawn;

	public ObjectPool objPool;

	EventSystem eventSys;

	GameObject currIndicator; // store the instantiated indicator

	public string[] buildingNames;

	public bool currentlyBuilding; // turns true when Player has half-built building on mouse / is building


	void Awake(){
		canvasRectTransform = canvas.transform as RectTransform;
	}

	void Start () {

		// Init Building costs
		InitBuildCosts ();

		activeButtons = new Button[4]; // there will only be 4 Options buttons
		CreateBuildingButtons ();
	
	}

	void InitBuildCosts(){
		extractCost = resourceGrid.extractorCost;
		mGunCost = resourceGrid.machineGunCost;
		seaWCost = resourceGrid.seaWitchCost;
		cannonCost = resourceGrid.cannonCost;
		hHallCost = resourceGrid.harpoonHCost;
		sFarmCost = resourceGrid.sFarmCost;
		storageCost = resourceGrid.storageCost;
		sDesaltCost = resourceGrid.sDesaltCost;
	}

	void CreateBuildingButtons(){
		Vector2 corners = new Vector2(0, 1);
											// BUILD BUTTONS
		BuildButton (buildBttnFab,extractSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (16f, 10f, 0), new Vector2 (32, 32), buildingNames[0], extractCost[0], extractCost[1]); 
		BuildButton (buildBttnFab,machineGunSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (96f, 10f, 0), new Vector2 (32, 32), buildingNames[1], mGunCost[0], mGunCost[1]); 
		BuildButton (buildBttnFab,cannonSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (96f, 90f, 0), new Vector2 (32, 32), buildingNames[2], cannonCost[0], cannonCost[1]); 
		BuildButton (buildBttnFab,harpoonHSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (176f, 10f, 0), new Vector2 (32, 32), buildingNames[3], hHallCost[0], hHallCost[1]); 
		BuildButton (buildBttnFab,sFarmSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (256f, 10f, 0), new Vector2 (32, 32),buildingNames[4], sFarmCost[0], sFarmCost[1]);
		BuildButton (buildBttnFab,sDesaltSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (256f, 90f, 0), new Vector2 (32, 32),buildingNames[5], sDesaltCost[0], sDesaltCost[1]);
		BuildButton (buildBttnFab,storSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (336f, 10f, 0), new Vector2 (32, 32),buildingNames[6], storageCost[0], storageCost[1]);
	}

	void BuildButton(Button buttonPrefab,Sprite image, GameObject panel, Vector2 cornerTopR, Vector2 cornerBottL, Vector3 position, Vector2 size,  string bName, int oCost, int fCost){
		Button buildB = Instantiate (buttonPrefab, Vector3.zero, Quaternion.identity) as Button;
		RectTransform rectTransform = buildB.GetComponent<RectTransform> ();
		rectTransform.SetParent (panel.transform);
		rectTransform.anchorMax = cornerTopR;
		rectTransform.anchorMin = cornerBottL;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.sizeDelta = size;
		rectTransform.localPosition = new Vector3(rectTransform.localPosition.x + position.x, rectTransform.localPosition.y - position.y, 0);
		Text txt = buildB.gameObject.GetComponentInChildren<Text> ();
		txt.text = bName;
		string currBuildingName = bName;

		// Add a new TriggerEvent and add a listener
		buildB.gameObject.AddComponent<EventTrigger> ();
		EventTrigger eventTrigger = buildB.GetComponent<EventTrigger> ();
		EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
		EventTrigger.TriggerEvent pointerEnterEvent = new EventTrigger.TriggerEvent();
		pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
		pointerEnterEvent.AddListener((eventData) => CreateIndicator(oCost.ToString() + " ORE  " + fCost.ToString() + " FOOD/Day"));
		pointerEnterEntry.callback = pointerEnterEvent;
		eventTrigger.triggers.Add(pointerEnterEntry);

		EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
		EventTrigger.TriggerEvent pointerExitEvent = new EventTrigger.TriggerEvent();
		pointerExitEntry.eventID = EventTriggerType.PointerExit;
		pointerExitEvent.AddListener((eventData) => PoolIndicator());
		pointerExitEntry.callback = pointerExitEvent;
		eventTrigger.triggers.Add(pointerExitEntry);

		buildB.onClick.AddListener(() => BuildThis(currBuildingName));
		buildB.GetComponent<Image> ().sprite = image;


	}
	
	/// <summary>
	/// Builds the building according to variables set by the Button.
	/// Before building, depending if the building ORE-Only Cost or Both,
	/// the cost to build is checked. If Resources says the town has enough, 
	/// this allows Building Position handler to spawn a half opacity sprite of the building
	/// that follows the mouse cursor.
	/// ONLY IF the building has a Food cost is it added to Resources's array of spawned buildings to
	/// charge food cost each turn.
	/// </summary>
	/// <param name="buildingName">Building name.</param>
	public void BuildThis(string buildingName){
		// Set currBuilding to true so we can't create options menus
		currentlyBuilding = true;

		// Mouse position so I can instantiate on the mouse!
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 spawnPos = new Vector3 (Mathf.Round (m.x), Mathf.Round (m.y), 0.0f);
		// To build we are just using the function SwapTileType to change this tile's type to that of the building
			// we need the name of the Half Tile that gets called here
		string halfName = "half_Built";
		switch (buildingName) {
		case "Extractor":
			if (resourceManager.ore >= extractCost[0]){
				// in order to re-use half tiles all we need is to
				// grab the Half tile from the pool
				// Add Building Position Handler and fill its vars
				GameObject sExtractor = objPool.GetObjectForType(halfName, true);
				if (sExtractor != null){
					// set the sprite
					sExtractor.GetComponent<SpriteRenderer>().sprite = extractSprite;
//					// add building pos handler
//					sExtractor.AddComponent<Building_PositionHandler>();
					Building_PositionHandler bPosHand = sExtractor.GetComponent<Building_PositionHandler>(); 

					bPosHand.resourceGrid = resourceGrid;
					bPosHand.followMouse = true;
					bPosHand.tileType = TileData.Types.extractor;
					bPosHand.resourceManager = resourceManager;
					bPosHand.currOreCost = extractCost[0];
					bPosHand.objPool = objPool;
					bPosHand.buildingUI = this;
				}
			}else{
				int diff = extractCost[0] - resourceManager.ore;
				CreateIndicator("Need " + diff + " more Ore!");
			}
			break;
		case "Machine Gun":
			if (resourceManager.ore >= mGunCost[0]){
				GameObject mGun = objPool.GetObjectForType(halfName, true);
				if(mGun != null){
					// set the sprite
					mGun.GetComponent<SpriteRenderer>().sprite = machineGunSprite;
//					// add building pos handler
//					mGun.AddComponent<Building_PositionHandler>();
					Building_PositionHandler bPosHand = mGun.GetComponent<Building_PositionHandler>();

					bPosHand.resourceGrid = resourceGrid;
					bPosHand.followMouse = true;
					bPosHand.tileType = TileData.Types.machine_gun;
					bPosHand.resourceManager = resourceManager;
					bPosHand.currOreCost = mGunCost[0];
					bPosHand.objPool = objPool;
					bPosHand.buildingUI = this;
				}
			}else{
				int diff = mGunCost[0] - resourceManager.ore;
				CreateIndicator("Need " + diff + " more Ore!");
			}
			break;
		case "Cannons":
			if (resourceManager.ore >= cannonCost[0]){		
				GameObject can = objPool.GetObjectForType(halfName, true);
				if(can != null){
					// set the sprite
					can.GetComponent<SpriteRenderer>().sprite = cannonSprite;
//					// add building pos handler
//					can.AddComponent<Building_PositionHandler>();
					Building_PositionHandler bPosHand = can.GetComponent<Building_PositionHandler>();

					bPosHand.resourceGrid = resourceGrid;
					bPosHand.followMouse = true;
					bPosHand.tileType = TileData.Types.cannons;
					bPosHand.resourceManager = resourceManager;
					bPosHand.currOreCost = cannonCost[0];
					bPosHand.objPool = objPool;
					bPosHand.buildingUI = this;
				}
			}else{
				int diff = cannonCost[0] - resourceManager.ore;
				CreateIndicator("Need " + diff + " more Ore!");
			}
			break;
		case "Harpooner's Hall":
			if (resourceManager.ore >= hHallCost[0]){		// ** HAS FOOD COST
				GameObject hHall = objPool.GetObjectForType(halfName, true);
				if(hHall != null){
					// set the sprite
					hHall.GetComponent<SpriteRenderer>().sprite = harpoonHSprite;
					// add building pos handler
//					hHall.AddComponent<Building_PositionHandler>();
					Building_PositionHandler bPosHand = hHall.GetComponent<Building_PositionHandler>();

					bPosHand.resourceGrid = resourceGrid;
					bPosHand.followMouse = true;
					bPosHand.tileType = TileData.Types.harpoonHall;
					bPosHand.resourceManager = resourceManager;
					bPosHand.currOreCost = hHallCost[0];
					bPosHand.objPool = objPool;
					bPosHand.buildingUI = this;
				}
			}else{
				int diff = hHallCost[0] - resourceManager.ore;
				CreateIndicator("Need " + diff + " more Ore!");
			}
			break;
		case "Seaweed Farm":
			if (resourceManager.ore >= sFarmCost[0]){		// ** HAS FOOD COST
				GameObject sWeed = objPool.GetObjectForType(halfName, false);
				if(sWeed != null){
					// set the sprite
					sWeed.GetComponent<SpriteRenderer>().sprite = sFarmSprite;
//					// add building pos handler
//					sWeed.AddComponent<Building_PositionHandler>();
					Building_PositionHandler bPosHand = sWeed.GetComponent<Building_PositionHandler>();

					bPosHand.resourceGrid = resourceGrid;
					bPosHand.followMouse = true;
					bPosHand.tileType = TileData.Types.farm_s;
					bPosHand.resourceManager = resourceManager;
					bPosHand.currOreCost = sFarmCost[0];
					bPosHand.objPool = objPool;
					bPosHand.buildingUI = this;
				}
			}else{
				int diff = sFarmCost[0] - resourceManager.ore;
				CreateIndicator("Need " + diff + " more Ore!");
			}
			break;
		case "Storage":
			if (resourceManager.ore >= storageCost[0]){		
				GameObject storage = objPool.GetObjectForType(halfName, false);
				if(storage != null){
					// set the sprite
					storage.GetComponent<SpriteRenderer>().sprite = storSprite;
					//					// add building pos handler
					//					sWeed.AddComponent<Building_PositionHandler>();
					Building_PositionHandler bPosHand = storage.GetComponent<Building_PositionHandler>();
					
					bPosHand.resourceGrid = resourceGrid;
					bPosHand.followMouse = true;
					bPosHand.tileType = TileData.Types.storage;
					bPosHand.resourceManager = resourceManager;
					bPosHand.currOreCost = storageCost[0];
					bPosHand.objPool = objPool;
					bPosHand.buildingUI = this;
				}
			}else{
				int diff = storageCost[0] - resourceManager.ore;
				CreateIndicator("Need " + diff + " more Ore!");
			}
			break;
		case "Desalination Pump":
			if (resourceManager.ore >= sDesaltCost[0]){		
				GameObject dSalt = objPool.GetObjectForType(halfName, false);
				if(dSalt != null){
					// set the sprite
					dSalt.GetComponent<SpriteRenderer>().sprite = sDesaltSprite;
					//					// add building pos handler
					//					sWeed.AddComponent<Building_PositionHandler>();
					Building_PositionHandler bPosHand = dSalt.GetComponent<Building_PositionHandler>();
					
					bPosHand.resourceGrid = resourceGrid;
					bPosHand.followMouse = true;
					bPosHand.tileType = TileData.Types.desalt_s;
					bPosHand.resourceManager = resourceManager;
					bPosHand.currOreCost = sDesaltCost[0];
					bPosHand.objPool = objPool;
					bPosHand.buildingUI = this;
				}
			}else{
				int diff = sDesaltCost[0] - resourceManager.ore;
				CreateIndicator("Need " + diff + " more Ore!");
			}
			break;
		default:
			print("UI handler cant find this type!");
			break;
		}
	}





			// THIS CREATES THE BUILDING UPGRADE PANEL THAT POPS UP ON TOP OF BUILDINGS



//	GameObject CreateNewPanel(Vector3 position){
//		Vector3 posInScreen = Camera.main.WorldToScreenPoint (position);
//		Vector3 newPos = posInScreen - canvasRectTransform.position;
//		Vector2 corners = new Vector2(0.5f, 1f);
//		GameObject panel = Instantiate (buildUpgradePanelFab, newPos, Quaternion.identity) as GameObject;
//		RectTransform rectTransform = panel.GetComponent<RectTransform> ();
//		rectTransform.SetParent (canvas.transform);
//		rectTransform.anchoredPosition3D = position;
//		rectTransform.offsetMax = Vector2.zero;
//		rectTransform.offsetMin = Vector2.zero;
//		rectTransform.sizeDelta = new Vector2(111, 76);
//		rectTransform.localPosition = new Vector3(rectTransform.localPosition.x + newPos.x, rectTransform.localPosition.y + newPos.y + 100f, 0);
//		return panel;
//	}

//	public void CreateOptionsButtons(Vector3 buildPosition, TileData.Types tileType, int posX, int posY, GameObject buildingPanel, Canvas buildingCanvas){
//		if (!currentlyBuilding) {
//			Vector2 corners = new Vector2 (1, 1);
//			currentTileType = tileType;
//			currPosX = posX;
//			currPosY = posY;
//			
//			// This gets called by the clicked building and gives its position so we can re-position the build panel
//			if (currentTileType != TileData.Types.empty && currentTileType != TileData.Types.rock && 
//			    currentTileType != TileData.Types.capital && currentTileType != TileData.Types.desalt_s &&
//			    currentTileType != TileData.Types.extractor ) { 
//				
////				// Activate the building's panel
//				buildingPanel.SetActive(true);
//				buildUpgradePanel = buildingPanel;
//
//				// Clear the already active buttons so we don't create them on top of each other
//				foreach (Button button in activeButtons) {
//					if (button != null) {
//						Destroy (button.gameObject);
//					}
//				}
//
//				// First create the Close Panel button, then we can create the Buiding buttons
//				// Close Panel button
//				CreateButton (closeBttn, 0, buildingPanel, corners,corners, Vector3.zero, new Vector2 (32, 32), " ", ClosePanel); 
//				// Here we would need a switch for the types to determine what Upgrade buttons we need
//				CreateButton (buildUpgradeBttn, 1, buildingPanel, new Vector2(0,0.5f), new Vector2(0,0.5f), new Vector3 (113, 0, 0), new Vector2 (200, 100), "Sell", Sell); 
//			}
//			
////			if (currentTileType == TileData.Types.desalt_s || currentTileType == TileData.Types.extractor){
////				
////				if (buildUpgradePanel != null){
////					Destroy (buildUpgradePanel);
////					buildUpgradePanel = CreateNewPanel (buildPosition);
////				}else{
////					buildUpgradePanel = CreateNewPanel (buildPosition);
////				}
////				
////				foreach (Button button in activeButtons) {
////					if (button != null) {
////						Destroy (button.gameObject);
////					}
////				}
////				CreateButton (closeBttn, 0, buildUpgradePanel, new Vector2 (1, 1), new Vector2 (1, 1), new Vector3 (-4f, -1.6f, 0), new Vector2 (8, 8), " ", ClosePanel); 
////				
////				CreateButton (buildButton, 1, buildUpgradePanel, corners, corners, new Vector3 (64, 24, 0), new Vector2 (32, 32), "Sell", Sell); 
////				
////				CreateButton (buildButton, 1, buildUpgradePanel, corners, corners, new Vector3 (60, 56, 0), new Vector2 (64, 32), "Set Storage", CallOption); 
////				
////			}
//		}
//	}

//	void CreateButton(Button buttonPrefab, int bttnArrayIndex, GameObject panel, Vector2 anchorMin, Vector2 anchorMax, Vector3 position, Vector2 size,  string text, UnityAction method){
//		Button buildB = Instantiate (buttonPrefab, Vector3.zero, Quaternion.identity) as Button;
//		RectTransform rectTransform = buildB.GetComponent<RectTransform> ();
//		rectTransform.SetParent (panel.transform);
//		rectTransform.anchorMax = anchorMax;
//		rectTransform.anchorMin = anchorMin;
//		rectTransform.offsetMax = Vector2.zero;
//		rectTransform.offsetMin = Vector2.zero;
//		rectTransform.sizeDelta = size;
//		rectTransform.localPosition = new Vector3(rectTransform.localPosition.x + position.x, rectTransform.localPosition.y - position.y, 0);
//		Text txt = buildB.gameObject.GetComponentInChildren<Text> ();
//		txt.text = text;
//		string currBuildingName = text;
//		buildB.onClick.AddListener (method);
//
//		// add this button to the array of active buttons ( so we can later clear it out)
//		activeButtons [bttnArrayIndex] = buildB;
//	}

//	public void CreateOptionsButtons(Vector3 buildPosition, TileData.Types tileType, int posX, int posY){
//		if (!currentlyBuilding) {
//			Vector2 corners = new Vector2 (0, 1);
//			currentTileType = tileType;
//			currPosX = posX;
//			currPosY = posY;
//
//			// This gets called by the clicked building and gives its position so we can re-position the build panel
//			if (currentTileType != TileData.Types.empty && currentTileType != TileData.Types.rock && 
//			    currentTileType != TileData.Types.capital && currentTileType != TileData.Types.desalt_s &&
//			    currentTileType != TileData.Types.extractor ) { 
//
//				// Create a Panel on top of this building tile, if there's one already destroy it
//				if (buildUpgradePanel != null){
//					Destroy (buildUpgradePanel);
//					buildUpgradePanel = CreateNewPanel (buildPosition);
//				}else{
//					buildUpgradePanel = CreateNewPanel (buildPosition);
//				}
//
//				// Clear the already active buttons so we don't create them on top of each other
//				foreach (Button button in activeButtons) {
//					if (button != null) {
//						Destroy (button.gameObject);
//					}
//				}
//				// First create the Close Panel button, then we can create the Buiding buttons
//				// Close Panel button
//				CreateButton (closeBttn, 0, buildUpgradePanel, new Vector2 (1, 1), new Vector2 (1, 1), new Vector3 (-4f, -1.6f, 0), new Vector2 (8, 8), " ", ClosePanel); 
//				// Here we would need a switch for the types to determine what Upgrade buttons we need
//				CreateButton (buildButton, 1, buildUpgradePanel, corners, corners, new Vector3 (27, 38, 0), new Vector2 (32, 32), "Sell", Sell); 
//			}
//
//			if (currentTileType == TileData.Types.desalt_s || currentTileType == TileData.Types.extractor){
//		
//				if (buildUpgradePanel != null){
//					Destroy (buildUpgradePanel);
//					buildUpgradePanel = CreateNewPanel (buildPosition);
//				}else{
//					buildUpgradePanel = CreateNewPanel (buildPosition);
//				}
//
//				foreach (Button button in activeButtons) {
//					if (button != null) {
//						Destroy (button.gameObject);
//					}
//				}
//				CreateButton (closeBttn, 0, buildUpgradePanel, new Vector2 (1, 1), new Vector2 (1, 1), new Vector3 (-4f, -1.6f, 0), new Vector2 (8, 8), " ", ClosePanel); 
//
//				CreateButton (buildButton, 1, buildUpgradePanel, corners, corners, new Vector3 (64, 24, 0), new Vector2 (32, 32), "Sell", Sell); 
//
//				CreateButton (buildButton, 1, buildUpgradePanel, corners, corners, new Vector3 (60, 56, 0), new Vector2 (64, 32), "Set Storage", CallOption); 
//
//			}
//		}
//	}




//	void CreateButton(Button buttonPrefab, int bttnArrayIndex, GameObject panel, Vector2 cornerTopR, Vector2 cornerBottL, Vector3 position, Vector2 size,  string text, UnityAction method){
//		Button buildB = Instantiate (buttonPrefab, Vector3.zero, Quaternion.identity) as Button;
//		RectTransform rectTransform = buildB.GetComponent<RectTransform> ();
//		rectTransform.SetParent (panel.transform);
//		rectTransform.anchorMax = cornerTopR;
//		rectTransform.anchorMin = cornerBottL;
//		rectTransform.offsetMax = Vector2.zero;
//		rectTransform.offsetMin = Vector2.zero;
//		rectTransform.sizeDelta = size;
//		rectTransform.localPosition = new Vector3(rectTransform.localPosition.x + position.x, rectTransform.localPosition.y - position.y, 0);
//		Text txt = buildB.gameObject.GetComponentInChildren<Text> ();
//		txt.text = text;
//		string currBuildingName = text;
//		buildB.onClick.AddListener (method);
//		// add this button to the array of active buttons ( so we can later clear it out)
//		activeButtons [bttnArrayIndex] = buildB;
//	}

//
//	void CallOption(){
//		OptionsForThis ();
//	}
//	void OptionsForThis(){
//
//		switch (currentTileType) {
//		case TileData.Types.capital:
//			Debug.Log("No options for the capital right now!");
//			ClosePanel();
//			break;
//		case TileData.Types.desalt_s:
//			ResetStorage();
//			break;
//		case TileData.Types.extractor:
//			ResetStorage();
//			break;
//		default:
//			Sell ();
//			break;
//		}
//	}
//
//	public void Sell(){
//		resourceGrid.SwapTileType(currPosX, currPosY, TileData.Types.empty); 
//		ClosePanel();
//	}
//
//	public void ClosePanel(){
//		buildUpgradePanel.SetActive(false);
//	}
//
//	public void ResetStorage(){
//		if (resourceGrid.GetTileGameObj (currPosX, currPosY) != null) {
//			if (resourceGrid.GetTileType(currPosX, currPosY) == TileData.Types.desalt_s){
//				resourceGrid.GetTileGameObj (currPosX, currPosY).GetComponent<DeSalt_Plant>().selecting = true;
//			}else if (resourceGrid.GetTileType(currPosX, currPosY) == TileData.Types.extractor) {
//				resourceGrid.GetTileGameObj (currPosX, currPosY).GetComponent<Extractor>().selecting = true;
//			}
//		}
//		ClosePanel ();
//	}





	public void CreateIndicator(string message){
		if (currIndicator != null) {
			objPool.PoolObject(currIndicator);
		}
			currIndicator = objPool.GetObjectForType ("indicator", false);
			currIndicator.transform.position = mainBuildPanel.transform.position;
			currIndicator.transform.SetParent (canvas.transform);
			RectTransform rectTransform = currIndicator.GetComponent<RectTransform> ();
			rectTransform.sizeDelta = new Vector2 (182f, 32f);
			rectTransform.localPosition = new Vector3 (rectTransform.localPosition.x - 345f, rectTransform.localPosition.y + 113f, 0);
			currIndicator.GetComponentInChildren<Text> ().text = message;
	}
	public void PoolIndicator(){
		if (currIndicator != null) {
			objPool.PoolObject(currIndicator);
		}
	}
}
