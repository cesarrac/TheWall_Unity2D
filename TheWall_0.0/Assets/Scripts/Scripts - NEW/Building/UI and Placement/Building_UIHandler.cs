using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Building_UIHandler : MonoBehaviour {

	TileData.Types currentTileType;
	public int currPosX, currPosY;// storing the map pos from click handler 

	// this is the MAIN BUILD PANEL at the bottom of the screen
	public GameObject mainBuildPanel;
	public Button buildBttnFab;
	// Building Sprites
	public Sprite buildingSprite, extractSprite, machineGunSprite, seaWSprite, harpoonHSprite, cannonSprite;
	// Building Names
	public string buildingName;

	// PreFabs (alpha set to 1/2 to represent un-built building)
//	public GameObject sExtractFab, mExtractFab, lExtractFab;

	// these objects are for handling the upgrade panel that pop up on top of already built buildings
	public GameObject buildUpgradePanelFab;
	private GameObject buildUpgradePanel;


	public Button buildButton;
	public Button closeBttn;

//	string currBuildingName;

	Button[] activeButtons;

	public Canvas canvas;
	private RectTransform canvasRectTransform;

	public ResourceGrid resourceGrid;

	public Player_ResourceManager resourceManager;

	// Costs of Buildings:
	public int[] extractCost, mGunCost, seaWCost, hHallCost, cannonCost;

	// Indicator that pops up just over the build panel, destroys itself
	public GameObject indicatorFab;
//
//	public Player_SpawnHandler playerSpawn;

	public ObjectPool objPool;

	void Awake(){
		canvasRectTransform = canvas.transform as RectTransform;
	}

	void Start () {
		activeButtons = new Button[4]; // there will only be 4 build buttons
		CreateBuildingButtons ();
		// Init Building costs
		InitBuildCosts ();
	}

	void InitBuildCosts(){
		extractCost = resourceGrid.extractorCost;
		mGunCost = resourceGrid.machineGunCost;
		seaWCost = resourceGrid.seaWitchCost;
		cannonCost = resourceGrid.cannonCost;
		hHallCost = resourceGrid.harpoonHCost;
	}

	void CreateBuildingButtons(){
		Vector2 corners = new Vector2(0, 1);
		// TEST BUTTON
		BuildButton (buildBttnFab,extractSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (16f, 10f, 0), new Vector2 (32, 32), "Extractor"); 
		BuildButton (buildBttnFab,machineGunSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (96f, 10f, 0), new Vector2 (32, 32), "Machine Gun"); 
		BuildButton (buildBttnFab,cannonSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (96f, 90f, 0), new Vector2 (32, 32), "Cannons"); 
		BuildButton (buildBttnFab,harpoonHSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (184f, 10f, 0), new Vector2 (32, 32), "Harpooner's Hall"); 
	}

	void BuildButton(Button buttonPrefab,Sprite image, GameObject panel, Vector2 cornerTopR, Vector2 cornerBottL, Vector3 position, Vector2 size,  string text){
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
		txt.text = text;
		string currBuildingName = text;
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
					// add building pos handler
					sExtractor.AddComponent<Building_PositionHandler>();
					sExtractor.GetComponent<Building_PositionHandler>().resourceGrid = resourceGrid;
					sExtractor.GetComponent<Building_PositionHandler>().followMouse = true;
					sExtractor.GetComponent<Building_PositionHandler>().tileType = TileData.Types.extractor;
					sExtractor.GetComponent<Building_PositionHandler>().resourceManager = resourceManager;
					sExtractor.GetComponent<Building_PositionHandler>().currOreCost = extractCost[0];
					sExtractor.GetComponent<Building_PositionHandler>().objPool = objPool;
				}
			}else{
				CreateIndicator("Not enough ore!");
			}
			break;
		case "Machine Gun":
			if (resourceManager.ore >= mGunCost[0]){
				GameObject mGun = objPool.GetObjectForType(halfName, true);
				if(mGun != null){
					// set the sprite
					mGun.GetComponent<SpriteRenderer>().sprite = machineGunSprite;
					// add building pos handler
					mGun.AddComponent<Building_PositionHandler>();
					mGun.GetComponent<Building_PositionHandler>().resourceGrid = resourceGrid;
					mGun.GetComponent<Building_PositionHandler>().followMouse = true;
					mGun.GetComponent<Building_PositionHandler>().tileType = TileData.Types.machine_gun;
					mGun.GetComponent<Building_PositionHandler>().resourceManager = resourceManager;
					mGun.GetComponent<Building_PositionHandler>().currOreCost = mGunCost[0];
					mGun.GetComponent<Building_PositionHandler>().objPool = objPool;
				}
			}else{
				CreateIndicator("Not enough ore!");
			}
			break;
		case "Cannons":
			if (resourceManager.ore >= cannonCost[0]){		
				GameObject can = objPool.GetObjectForType(halfName, true);
				if(can != null){
					// set the sprite
					can.GetComponent<SpriteRenderer>().sprite = cannonSprite;
					// add building pos handler
					can.AddComponent<Building_PositionHandler>();
					can.GetComponent<Building_PositionHandler>().resourceGrid = resourceGrid;
					can.GetComponent<Building_PositionHandler>().followMouse = true;
					can.GetComponent<Building_PositionHandler>().tileType = TileData.Types.cannons;
					can.GetComponent<Building_PositionHandler>().resourceManager = resourceManager;
					can.GetComponent<Building_PositionHandler>().currOreCost = cannonCost[0];
					can.GetComponent<Building_PositionHandler>().objPool = objPool;
				}
			}else{
				CreateIndicator("Not enough ore!");
			}
			break;
		case "Harpooner's Hall":
			if (resourceManager.ore >= hHallCost[0]){		// ** HAS FOOD COST
				GameObject hHall = objPool.GetObjectForType(halfName, true);
				if(hHall != null){
					// set the sprite
					hHall.GetComponent<SpriteRenderer>().sprite = harpoonHSprite;
					// add building pos handler
					hHall.AddComponent<Building_PositionHandler>();
					hHall.GetComponent<Building_PositionHandler>().resourceGrid = resourceGrid;
					hHall.GetComponent<Building_PositionHandler>().followMouse = true;
					hHall.GetComponent<Building_PositionHandler>().tileType = TileData.Types.harpoonHall;
					hHall.GetComponent<Building_PositionHandler>().resourceManager = resourceManager;
					hHall.GetComponent<Building_PositionHandler>().currOreCost = hHallCost[0];
					hHall.GetComponent<Building_PositionHandler>().objPool = objPool;
				}
			}else{
				CreateIndicator("Not enough ore!");
			}
			break;
		default:
			print("UI handler cant find this type!");
			break;
		}
	}


	// THIS CREATES THE BUILDING UPGRADE PANEL THAT POPS UP ON TOP OF BUILDINGS
	GameObject CreateNewPanel(Vector3 position){
		Vector3 posInScreen = Camera.main.WorldToScreenPoint (position);
		Vector3 newPos = posInScreen - canvasRectTransform.position;
		Vector2 corners = new Vector2(0.5f, 1f);
		GameObject panel = Instantiate (buildUpgradePanelFab, newPos, Quaternion.identity) as GameObject;
		RectTransform rectTransform = panel.GetComponent<RectTransform> ();
		rectTransform.SetParent (canvas.transform);
		rectTransform.anchoredPosition3D = position;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.sizeDelta = new Vector2(111, 76);
		rectTransform.localPosition = new Vector3(rectTransform.localPosition.x + newPos.x, rectTransform.localPosition.y + newPos.y + 100f, 0);
		return panel;
	}
	public void CreateOptionsButtons(Vector3 buildPosition, TileData.Types tileType, int posX, int posY){
		Vector2 corners = new Vector2(0, 1);
		currentTileType = tileType;
		currPosX = posX;
		currPosY = posY;

		// This gets called by the clicked building and gives its position so we can re-position the build panel
		if (currentTileType != TileData.Types.empty && currentTileType != TileData.Types.rock && currentTileType != TileData.Types.capital) { 
			// Create a Panel on top of this building tile
			buildUpgradePanel = CreateNewPanel (buildPosition);

			// Clear the already active buttons so we don't create them on top of each other
			foreach (Button button in activeButtons) {
				if (button != null) {
					Destroy (button.gameObject);
				}
			}
			// First create the Close Panel button, then we can create the Buiding buttons
			// Close Panel button
			CreateButton (closeBttn, 0, buildUpgradePanel, new Vector2 (1, 1), new Vector2 (1, 1), new Vector3 (-4f, -1.6f, 0), new Vector2 (8, 8), " ", ClosePanel); 
			// Here we would need a switch for the types to determine what Upgrade buttons we need
			CreateButton (buildButton, 1, buildUpgradePanel, corners, corners, new Vector3 (27, 38, 0), new Vector2 (32, 32), "Sell", CallOption); 
		} 
//		if (currentTileType == TileData.Types.harpoonHall) {
//			buildUpgradePanel = CreateNewPanel (buildPosition);
//
//			foreach (Button button in activeButtons) {
//				if (button != null) {
//					Destroy (button.gameObject);
//				}
//			}
//			CreateButton (closeBttn, 0, buildUpgradePanel, new Vector2 (1, 1), new Vector2 (1, 1), new Vector3 (-4f, -1.6f, 0), new Vector2 (8, 8), " ", ClosePanel); 
//			CreateButton (buildButton, 1, buildUpgradePanel, corners, corners, new Vector3 (27, 36, 0), new Vector2 (32, 32), "Spawn", CallSpawn); 
//			CreateButton (buildButton, 1, buildUpgradePanel, corners, corners, new Vector3 (64, 38, 0), new Vector2 (32, 32), "Sell", CallOption); 
//		}
	}

	void CreateButton(Button buttonPrefab, int bttnArrayIndex, GameObject panel, Vector2 cornerTopR, Vector2 cornerBottL, Vector3 position, Vector2 size,  string text, UnityEngine.Events.UnityAction method){
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
		txt.text = text;
		string currBuildingName = text;
		buildB.onClick.AddListener (method);
		// add this button to the array of active buttons ( so we can later clear it out)
		activeButtons [bttnArrayIndex] = buildB;
	}
	void CallOption(){
		OptionsForThis ();
	}
	void OptionsForThis(){
		// To build we are just using the function SwapTileType to change this tile's type to that of the building
		switch (currentTileType) {
		case TileData.Types.capital:
			Debug.Log("No options for the capital right now!");
			ClosePanel();
			break;
		default:
			resourceGrid.SwapTileType(currPosX, currPosY, TileData.Types.empty); 
			ClosePanel();
			break;
		}
	}

//	void CallSpawn(){
//		playerSpawn.Spawn ();
//		ClosePanel();
//	}

	public void ClosePanel(){
		Destroy (buildUpgradePanel.gameObject);
	}

	public void CreateIndicator(string message){
		GameObject indicator = Instantiate (indicatorFab, mainBuildPanel.transform.position, Quaternion.identity) as GameObject;
		indicator.transform.SetParent (canvas.transform);
		RectTransform rectTransform = indicator.GetComponent<RectTransform> ();
		rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y + 175f, 0);
		indicator.GetComponentInChildren<Text> ().text = message;
	}
}
