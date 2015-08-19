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
	public Sprite buildingSprite, sExtractSprite, mExtractSprite, lExtractSprite;
	// Building Names
	public string buildingName;
	// PreFabs (alpha set to 1/2 to represent un-built building)
	public GameObject sExtractFab, mExtractFab, lExtractFab;

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
	public int sExtractCost, mExtractCost, lExtractCost;

	// Indicator that pops up just over the build panel, destroys itself
	public GameObject indicatorFab;

	public Player_SpawnHandler playerSpawn;

	void Awake(){
		canvasRectTransform = canvas.transform as RectTransform;
	}

	void Start () {
		activeButtons = new Button[4]; // there will only be 4 build buttons
		CreateBuildingButtons ();
	}

	void CreateBuildingButtons(){
		Vector2 corners = new Vector2(0, 1);
		// TEST BUTTON
		BuildButton (buildBttnFab,sExtractSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (16f, 10f, 0), new Vector2 (32, 32), "S-Extractor"); 
		BuildButton (buildBttnFab,mExtractSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (16f, 54f, 0), new Vector2 (32, 32), "M-Extractor"); 
		BuildButton (buildBttnFab,lExtractSprite, mainBuildPanel, corners, corners, 
		             new Vector3 (16f, 101f, 0), new Vector2 (32, 32), "L-Extractor"); 
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
	public void BuildThis(string buildingName){
		// Mouse position so I can instantiate on the mouse!
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 spawnPos = new Vector3 (Mathf.Round (m.x), Mathf.Round (m.y), 0.0f);
		// To build we are just using the function SwapTileType to change this tile's type to that of the building
		switch (buildingName) {
		case "S-Extractor":
			if (resourceManager.ore >= sExtractCost){
				GameObject sExtractor = Instantiate(sExtractFab, spawnPos, Quaternion.identity) as GameObject;
				// this new Building will need: 
				// follow mouse is true
				// the Grid to swap the tile where you place the building
				// the tiletype it will be swapping to
				sExtractor.GetComponent<Building_PositionHandler>().resourceGrid = resourceGrid;
				sExtractor.GetComponent<Building_PositionHandler>().followMouse = true;
				sExtractor.GetComponent<Building_PositionHandler>().tileType = TileData.Types.sextractor;
				sExtractor.GetComponent<Building_PositionHandler>().resourceManager = resourceManager;
				sExtractor.GetComponent<Building_PositionHandler>().currBuildingCost = sExtractCost;
			}else{
				CreateIndicator("Not enough ore!");
			}
			break;
		case "M-Extractor":
			if (resourceManager.ore >= mExtractCost){
				GameObject mExtractor = Instantiate(mExtractFab, spawnPos, Quaternion.identity) as GameObject;
				mExtractor.GetComponent<Building_PositionHandler>().resourceGrid = resourceGrid;
				mExtractor.GetComponent<Building_PositionHandler>().followMouse = true;
				mExtractor.GetComponent<Building_PositionHandler>().tileType = TileData.Types.mextractor;
				mExtractor.GetComponent<Building_PositionHandler>().resourceManager = resourceManager;
				mExtractor.GetComponent<Building_PositionHandler>().currBuildingCost = mExtractCost;
			}else{
				CreateIndicator("Not enough ore!");
			}
			break;
		case "L-Extractor":
			if (resourceManager.ore >= lExtractCost){
				GameObject lExtractor = Instantiate(lExtractFab, spawnPos, Quaternion.identity) as GameObject;
				lExtractor.GetComponent<Building_PositionHandler>().resourceGrid = resourceGrid;
				lExtractor.GetComponent<Building_PositionHandler>().followMouse = true;
				lExtractor.GetComponent<Building_PositionHandler>().tileType = TileData.Types.lextractor;
				lExtractor.GetComponent<Building_PositionHandler>().resourceManager = resourceManager;
				lExtractor.GetComponent<Building_PositionHandler>().currBuildingCost = lExtractCost;
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
		Vector2 corners = new Vector2(0f, 0f);
		GameObject panel = Instantiate (buildUpgradePanelFab, newPos, Quaternion.identity) as GameObject;
		RectTransform rectTransform = panel.GetComponent<RectTransform> ();
		rectTransform.SetParent (canvas.transform);
		rectTransform.anchoredPosition3D = position;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.sizeDelta = new Vector2(111, 76);
		rectTransform.localPosition = new Vector3(rectTransform.localPosition.x + newPos.x, rectTransform.localPosition.y + newPos.y + 65f, 0);
		return panel;
	}
	public void CreateUpgradeButtons(Vector3 buildPosition, TileData.Types tileType, int posX, int posY){
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
			CreateButton (buildButton, 1, buildUpgradePanel, corners, corners, new Vector3 (27, 38, 0), new Vector2 (32, 32), "Basic", CallUpgrade); 
		} 
		if (currentTileType == TileData.Types.capital) {
			buildUpgradePanel = CreateNewPanel (buildPosition);

			foreach (Button button in activeButtons) {
				if (button != null) {
					Destroy (button.gameObject);
				}
			}
			CreateButton (closeBttn, 0, buildUpgradePanel, new Vector2 (1, 1), new Vector2 (1, 1), new Vector3 (-4f, -1.6f, 0), new Vector2 (8, 8), " ", ClosePanel); 
			CreateButton (buildButton, 1, buildUpgradePanel, corners, corners, new Vector3 (27, 36, 0), new Vector2 (32, 32), "Spawn", CallSpawn); 
		}
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
	void CallUpgrade(){
		UpgradeThis ();
	}
	void UpgradeThis(){
		// To build we are just using the function SwapTileType to change this tile's type to that of the building
		switch (currentTileType) {
		case TileData.Types.buildable:
			resourceGrid.SwapTileType(currPosX, currPosY, TileData.Types.building); 
			ClosePanel();
			break;
		default:
			print("UI handler cant find this type!");
			break;
		}
	}

	void CallSpawn(){
		playerSpawn.Spawn ();
		ClosePanel();
	}

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
