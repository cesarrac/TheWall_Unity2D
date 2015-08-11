using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TownBuilding : MonoBehaviour {

	// BUILDINGS have 3 tiers
	// tier 1 is Basic, tier 2 advanced, and tier 3 are upgrades/add-ons that go on those buildings

	public Map_Manager mapScript;
	Transform myTransform;
	Vector3 lastPos;

	// buttons
	public Button buildControlBttn;
	public Button firstBuildBtn, secondBuildBtn, thirdBuidBtn, fourthBuildBtn;

	//TIER 1: 
	//sprites
	public Sprite houseSprite, basicDefenseSprite, slaughterSprite, workSprite, farmSprite;
	//prefabs
	public GameObject houseFab, basicDefenseFab, slaughterHouseFab, workshopFab, farmFab;
	//costs
	public int[] houseCost = new int[3];
	public int[] basicDefenseCost = new int[3];
	public int[] workshopCost= new int[3];
	public int[] farmCost= new int[3];
	//TIER 2:
	// sprites
	public Sprite stoneHouseSprite, metalHouseSprite, stoneDefenseSprite, metalDefenseSprite, stoneWorkSprite, metalWorkSprite;
	public Sprite stoneFarmSprite, metalFarmSprite;
	// prefabs
	public GameObject stoneHouseFab, metalHouseFab, stoneDefenseFab, metalDefenseFab, stoneWorkFab, metalWorkFab;
	public GameObject stoneFarmFab, metalFarmFab;
	// costs
	public int[] stoneHouseCost = new int[3];
	public int[]metalHouseCost = new int[3];
	// defense costs
	public int[] stoneDefenseCost = new int[3];
	public int[] metalDefenseCost = new int[3];
	// slaughterhouse cost
	public int[] slaughterCost= new int[3];
	// workshop costs
	public int[] metalWorkshopCost= new int[3];
	public int[] stoneWorkshopCost= new int[3];
	// farm costs
	public int[] stoneFarmCost= new int[3];
	public int[] metalFarmCost= new int[3];

	//TIER 3:
	//sprites:
	public Sprite autoCannonSprite, throwerSprite, catapultSprite;
	//prefabs:
	public GameObject autoCanFab, throwFab, catapFab;
	//costs:
	public int[] autoCannonCost, throwerCost, catapultCost;

	//destroy / cancel building image
	public Sprite cancelSprite;
	// empty button sprite
	public Sprite emptyButtonSprite;
	

	//current town tile
	public GameObject townTile;
	GameObject storedTile;

	// button Text
	Text firstText, secondText, thirdText, fourthText;
	Text buildControlText;

	// bool turns true if this tile has a building
	public bool hasBuildingT1, hasBuildingT2, hasBuildingT3;

	// access to Town Resources
	public GameObject townRes;
	TownResources townResources;

	// we have resources check
	bool weHaveResources;

	// Layer mask so we dont hit ourselves
	public LayerMask mask;

	bool showBuildButtons;

	// to control tile/ui ray selection
	Mouse_Controls mouseControls;
	// my own bool to store and give a value to mouse control's bool
	bool stopMouse;

	void Awake () {
		mapScript = GetComponent<Map_Manager> ();
		mouseControls = GetComponent<Mouse_Controls> ();
		stopMouse = mouseControls.stopSelecting;
	}

	void Start(){
		myTransform = transform;
		lastPos = myTransform.position; // record this position


		buildControlText = buildControlBttn.GetComponentInChildren<Text> ();

//		townTile = mapScript.GetTownTile ();
//		ShowBasicBuildings();

		townResources = townRes.GetComponent<TownResources> ();

//		CheckWhatTile ();
		// INSTEAD OF CHECKING TILE AT THE BEGINNING, CHECK TILE WHEN PLAYER PRESSES BUILD CONTROL BUTTON
		buildControlBttn.image.sprite = emptyButtonSprite;
		buildControlText.text = "BUILD";
	}

	void Update () {
		if (mouseControls != null) {
			mouseControls.stopSelecting = stopMouse;
		}

		// if I've moved,destroy unwanted tiles and stop showing build buttons
		if (myTransform.position != lastPos){
			DestroyUnwantedTiles ();
			lastPos = myTransform.position;
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
			fourthBuildBtn.gameObject.SetActive (false);
			showBuildButtons = false;
		}
	}

	public void CallBuild(){	// called when Build Control button is pressed
		showBuildButtons = !showBuildButtons;
		// toggle build buttons
		if (showBuildButtons) {
			CheckWhatTile ();
			// stop mouse from selecting tiles
			stopMouse = true;
		} else {
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
			fourthBuildBtn.gameObject.SetActive (false);

			stopMouse = false;
		}

	}

	// raycast from this position to check what town tile we are on
	public void CheckWhatTile(){

//		RaycastHit2D hit = Physics2D.Linecast (new Vector2 (myTransform.position.x, myTransform.position.y), -Vector2.up, mask.value);
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(myTransform.position.x, myTransform.position.y), -Vector2.up, 100, mask.value);  

		if (hit.collider != null) {
//			print ("Camera hits " + hit.collider.name);
			if (hit.collider.CompareTag ("Town_Tile")) {
				townTile = hit.collider.gameObject; // grab the gameobject the camera is detecting
				GetOptionsToShow (townTile);// SHOW BUILD OPTIONS

			}else if(hit.collider.CompareTag("Capital")){
				// show empty buttons
				ShowNone();
			}
		}
	}

	public void DestroyUnwantedTiles(){
		
		//		RaycastHit2D hit = Physics2D.Linecast (new Vector2 (myTransform.position.x, myTransform.position.y), -Vector2.up, mask.value);
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(myTransform.position.x, myTransform.position.y), -Vector2.up, 100, mask.value);  
		
		if (hit.collider != null) {
			//			print ("Camera hits " + hit.collider.name);
			if(hit.collider.CompareTag("Tile") || hit.collider.CompareTag("Food Source") ){
				//destroy this, this is under a town tile
				//				Destroy(hit.collider.gameObject);
				mapScript.ClearResourceTilesUnderTown(myTransform.position, hit.collider.gameObject);
//				CheckWhatTile();
				
			}else if(hit.collider.CompareTag("Tile Under Attack")){
				// moves the player out of that tile so you cant access it
				//				myTransform.position = new Vector3(myTransform.position.x + 1f, myTransform.position.y, myTransform.position.z);
			}else if (hit.collider.CompareTag("Destroyed Town")){
				Destroy(hit.collider.gameObject);
			}else if (hit.collider.CompareTag("Depleted")){
				Destroy(hit.collider.gameObject);
			}
		}
	}

	void GetOptionsToShow(GameObject ttile){
		if (ttile != null) {
			hasBuildingT1 = ttile.GetComponent<TownTile_Properties>().tileHasTier1;
			hasBuildingT2 = ttile.GetComponent<TownTile_Properties>().tileHasTier2;
			hasBuildingT3 = ttile.GetComponent<TownTile_Properties>().tileHasTier3;
			if (hasBuildingT1){
				ShowTier2Builds(ttile.name);
			}else if (hasBuildingT2){
				ShowTier3Builds(ttile.name);
			}else if(hasBuildingT3){
				ShowOnlyDestroyed();
			}
			else{
				ShowTier1Builds();
			}
		}
	}

	void ShowNone(){
		firstBuildBtn.gameObject.SetActive (true);
		secondBuildBtn.gameObject.SetActive (true);
		thirdBuidBtn.gameObject.SetActive (true);
		fourthBuildBtn.gameObject.SetActive (true);
		firstText = firstBuildBtn.GetComponentInChildren<Text>();
		secondText = secondBuildBtn.GetComponentInChildren<Text>();
		thirdText = thirdBuidBtn.GetComponentInChildren<Text>();
		fourthText = fourthBuildBtn.GetComponentInChildren<Text> ();

		firstBuildBtn.image.sprite = emptyButtonSprite;
		secondBuildBtn.image.sprite = emptyButtonSprite;
		thirdBuidBtn.image.sprite = emptyButtonSprite;
		fourthBuildBtn.image.sprite = emptyButtonSprite;

		firstText.text = "Can't Build";
		secondText.text = "Can't Build";
		thirdText.text = "Can't Build";
		fourthText.text = "Can't Build";

	}

	void ShowTier1Builds(){
		firstBuildBtn.gameObject.SetActive (true);
		secondBuildBtn.gameObject.SetActive (true);
		thirdBuidBtn.gameObject.SetActive (true);
		fourthBuildBtn.gameObject.SetActive (true);
		firstText = firstBuildBtn.GetComponentInChildren<Text>();
		secondText = secondBuildBtn.GetComponentInChildren<Text>();
		thirdText = thirdBuidBtn.GetComponentInChildren<Text>();
		fourthText = fourthBuildBtn.GetComponentInChildren<Text> ();

		firstBuildBtn.enabled = true;
		secondBuildBtn.enabled = true;
		thirdBuidBtn.enabled = true;

		// show basic buildings
		firstBuildBtn.image.sprite = houseSprite;
		// get the text
		string houseText = "House";
		firstText.text = houseText;

		secondBuildBtn.image.sprite = basicDefenseSprite;
		string defenseText = "Basic Defense";
		secondText.text = defenseText;

		thirdBuidBtn.image.sprite = workSprite ;
		string workText = "Workshop";
		thirdText.text = workText;

		fourthBuildBtn.image.sprite = workSprite ;
		string farmText = "Farm";
		fourthText.text = farmText;
	}

	// to show the right Advanced building options, I need to get the name of the building on this town tile
	void ShowTier2Builds(string buildingName){
		string destroyText = "Destroy";
		firstBuildBtn.gameObject.SetActive (true);
		secondBuildBtn.gameObject.SetActive (true);
		thirdBuidBtn.gameObject.SetActive (true);
		fourthBuildBtn.gameObject.SetActive (true);
		firstText = firstBuildBtn.GetComponentInChildren<Text>();
		secondText = secondBuildBtn.GetComponentInChildren<Text>();
		thirdText = thirdBuidBtn.GetComponentInChildren<Text>();
		fourthText = fourthBuildBtn.GetComponentInChildren<Text> ();

		firstBuildBtn.enabled = true;
		secondBuildBtn.enabled = true;
		thirdBuidBtn.enabled = true;
		switch (buildingName) {
		case "House":
			// show upgrades for house
			firstBuildBtn.image.sprite = stoneHouseSprite;
			// get the text
			string stoneHouseText = "Stone House";
			firstText.text = stoneHouseText;
			
			secondBuildBtn.image.sprite = metalHouseSprite;
			// get the text
			string metalHouseText = "Metal House";
			secondText.text = metalHouseText;
		
			thirdBuidBtn.image.sprite = cancelSprite;
			thirdText.text = destroyText;
			// dont show 4th button
			fourthBuildBtn.gameObject.SetActive(false);
			break;
		case "Basic Defense":
			firstBuildBtn.image.sprite = stoneDefenseSprite;
			string stoneDefenseText = "Stone Defense";
			firstText.text = stoneDefenseText;

			secondBuildBtn.image.sprite = metalDefenseSprite;
			string metalDefenseText = "Metal Defense";
			secondText.text = metalDefenseText;

			thirdBuidBtn.image.sprite = cancelSprite;
			thirdText.text = destroyText;
			// dont show 4th button
			fourthBuildBtn.gameObject.SetActive(false);
			break;
		case "Workshop":
			firstBuildBtn.image.sprite = stoneWorkSprite;
			string stoneWorkText = "Stone Workshop";
			firstText.text = stoneWorkText;
			
			secondBuildBtn.image.sprite = metalWorkSprite;
			string metalWorkText = "Metal Workshop";
			secondText.text = metalWorkText;
			
			thirdBuidBtn.image.sprite = cancelSprite;
			thirdText.text = destroyText;
			// dont show 4th button
			fourthBuildBtn.gameObject.SetActive(false);
			break;
		case "Farm":
			firstBuildBtn.image.sprite = stoneFarmSprite;
			string stoneFarmT = "Stone Farm";
			firstText.text = stoneFarmT;
			
			secondBuildBtn.image.sprite = metalFarmSprite;
			string metalFarmT = "Metal Farm";
			secondText.text = metalFarmT;
			
			thirdBuidBtn.image.sprite = cancelSprite;
			thirdText.text = destroyText;
			// dont show 4th button
			fourthBuildBtn.gameObject.SetActive(false);

			break;
		default:
			print ("No advanced options for this building found!");
			break;
		}
	}

	void ShowTier3Builds(string buildingName){
		string destroyText = "Destroy";
		string noUpgradeText = "No Upgrades";
		firstBuildBtn.gameObject.SetActive (true);
		secondBuildBtn.gameObject.SetActive (true);
		thirdBuidBtn.gameObject.SetActive (true);
		fourthBuildBtn.gameObject.SetActive (true);	
		firstText = firstBuildBtn.GetComponentInChildren<Text>();
		secondText = secondBuildBtn.GetComponentInChildren<Text>();
		thirdText = thirdBuidBtn.GetComponentInChildren<Text>();
		fourthText = fourthBuildBtn.GetComponentInChildren<Text> ();

		firstBuildBtn.enabled = true;
		secondBuildBtn.enabled = true;
		switch (buildingName) {
		case "Stone House":
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
//			// show no upgrades for a house
//			firstBuildBtn.image.sprite = emptyButtonSprite;
//			// get the text
//			firstText.text = noUpgradeText;
//			//			
//			secondBuildBtn.image.sprite = emptyButtonSprite;
//			// get the text
//			secondText.text = noUpgradeText;
//			
//			thirdBuidBtn.image.sprite = emptyButtonSprite;
//			thirdText.text = noUpgradeText;
			// as fourth show option for Destroy
			fourthBuildBtn.image.sprite = cancelSprite;
			// get the text
			fourthText.text = destroyText;
			break;
		case "Metal House":
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
//			// show no upgrades for a house
//			firstBuildBtn.image.sprite = emptyButtonSprite;
//			// get the text
//			firstText.text = noUpgradeText;
////			
//			secondBuildBtn.image.sprite = emptyButtonSprite;
//			// get the text
//			secondText.text = noUpgradeText;
//
//			thirdBuidBtn.image.sprite = emptyButtonSprite;
//			thirdText.text = noUpgradeText;
			// as fourth show option for Destroy
			fourthBuildBtn.image.sprite = cancelSprite;
			// get the text
			fourthText.text = destroyText;
			break;
		case "Stone Defense":
			firstBuildBtn.image.sprite = throwerSprite;
			string stoneThrowText = "Stone-Thrower";
			firstText.text = stoneThrowText;
//			
			secondBuildBtn.image.sprite = metalDefenseSprite;
			string metalDefenseText = "Metal Defense";
			secondText.text = metalDefenseText;
//			
			thirdBuidBtn.image.sprite = cancelSprite;
			thirdText.text = destroyText;
			// dont show 4th button
			fourthBuildBtn.gameObject.SetActive(false);
			break;
		case "Metal Defense":
			firstBuildBtn.image.sprite = autoCannonSprite;
			string cannonText = "Auto-Cannon";
			firstText.text = cannonText;
			
			secondBuildBtn.image.sprite = stoneDefenseSprite;
			string stoneDefenseText = "Stone Defense";
			secondText.text = stoneDefenseText;
			
			thirdBuidBtn.image.sprite = cancelSprite;
			thirdText.text = destroyText;
			// dont show 4th button
			fourthBuildBtn.gameObject.SetActive(false);
			break;
		case "Stone Workshop":
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
			// as fourth show option for Destroy
			fourthBuildBtn.image.sprite = cancelSprite;
			// get the text
			fourthText.text = destroyText;
			break;
		case "Metal Workshop":
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
			// as fourth show option for Destroy
			fourthBuildBtn.image.sprite = cancelSprite;
			// get the text
			fourthText.text = destroyText;
			break;
		case "Stone Farm":
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
			// as fourth show option for Destroy
			fourthBuildBtn.image.sprite = cancelSprite;
			// get the text
			fourthText.text = destroyText;
			break;
		case "Metal Farm":
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
			// as fourth show option for Destroy
			fourthBuildBtn.image.sprite = cancelSprite;
			// get the text
			fourthText.text = destroyText;
			break;
		default:
			print ("No advanced options for this building found!");
			break;
		}
	}

	void ShowOnlyDestroyed(){
		string destroyText = "Destroy";
		firstBuildBtn.enabled = false;

		secondBuildBtn.enabled = false;
		
		thirdBuidBtn.image.sprite = emptyButtonSprite;
		thirdBuidBtn.enabled = false;
		fourthBuildBtn.gameObject.SetActive (true);
		// as fourth show option for Destroy
		fourthBuildBtn.image.sprite = cancelSprite;
		// get the text
		fourthText.text = destroyText;

	}
	
	public void FirstBuild(){
		string name = firstText.text;
		if (townTile != null) {
			CheckBuildingRecipeAndBuild(name, townTile);
			// then deActivate build buttons
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
			fourthBuildBtn.gameObject.SetActive (false);
			showBuildButtons = false;
			// then allow mouse to select again
			stopMouse = false;
		}

	}

	public void SecondBuild(){
		string name = secondText.text;
		if (townTile != null) {
			CheckBuildingRecipeAndBuild(name, townTile);
			// then deActivate build buttons
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
			fourthBuildBtn.gameObject.SetActive (false);
			showBuildButtons = false;
			// then allow mouse to select again
			stopMouse = false;

		}
		
	}

	public void ThirdBuild(){
		string name = thirdText.text;
		if (townTile != null) {
			CheckBuildingRecipeAndBuild(name, townTile);
			// then deActivate build buttons
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
			fourthBuildBtn.gameObject.SetActive (false);
			showBuildButtons = false;
			// then allow mouse to select again
			stopMouse = false;
			
		}
	}
	public void FourthBuild(){
		string name = fourthText.text;
		if (townTile != null) {
			if (name != "Destroy"){
				CheckBuildingRecipeAndBuild(name, townTile);
				// then deActivate build buttons
				firstBuildBtn.gameObject.SetActive (false);
				secondBuildBtn.gameObject.SetActive (false);
				thirdBuidBtn.gameObject.SetActive (false);
				fourthBuildBtn.gameObject.SetActive (false);
				showBuildButtons = false;
				// then allow mouse to select again
				stopMouse = false;
			}else{
				DestroyBuilding(townTile);
			}
		}
	}

	void CheckBuildingRecipeAndBuild(string name, GameObject towntile){

		switch (name) {
//		case "Slaughterhouse":
//			if (townResources.wood >= slaughterCost){
//				GameObject building = Instantiate (slaughterHouseFab, myTransform.position, Quaternion.identity) as GameObject;
//				// parent it to the town tile this is on
//				building.transform.parent = towntile.transform;
//				// need to makes sure the new gameobject's name matches my hardcoded names
//				building.name = name;
//				towntile.name = name;
//				//then tell this tile that it has an Advanced building
//				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
//				townProps.tileHasBuilding = false; // no longer has basic building
//				townProps.tileHasAdvancedBuilding = true;
//				townResources.wood = townResources.wood - slaughterCost;
//			}
//			break;
		case "Workshop":
			if (CheckResourceCost(wood: workshopCost[0], stone: workshopCost[1], metal: workshopCost[2])){
				GameObject building = Instantiate (workshopFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				townProps.tileHasTier1 = true; 

				// CHECK to see what options to show next
				GetOptionsToShow(towntile);
			}
			break;
		case "Stone Workshop":
			if (CheckResourceCost(wood: stoneWorkshopCost[0], stone: stoneWorkshopCost[1], metal: stoneWorkshopCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				
				// Need to DESTROY the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedT1 = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);
				
				GameObject building = Instantiate (stoneWorkFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = true;
				// CHECK to see what options to show next
				GetOptionsToShow(towntile);
			}
			break;
		case "Metal Workshop":
			if (CheckResourceCost(wood: metalWorkshopCost[0], stone: metalWorkshopCost[1], metal: metalWorkshopCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				
				// Need to DESTROY the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedT1 = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);
				
				GameObject building = Instantiate (metalWorkFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = true;
				// CHECK to see what options to show next
				GetOptionsToShow(towntile);
			}
			break;
		case "House":
			if (CheckResourceCost(wood: houseCost[0], stone: houseCost[1], metal: houseCost[2])){
				GameObject building = Instantiate (houseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has a building
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				townProps.tileHasTier1 = true;

				GetOptionsToShow(towntile);
			}
			break;
		case "Stone House":
			if (CheckResourceCost(wood: stoneHouseCost[0], stone: stoneHouseCost[1], metal: stoneHouseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();

				// Need to DESTROY the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedT1 = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);

				GameObject building = Instantiate (stoneHouseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = true;
				// CHECK to see what options to show next
				GetOptionsToShow(towntile);

			}
			break;
		case "Metal House":
			if (CheckResourceCost(wood: metalHouseCost[0], stone: metalHouseCost[1], metal: metalHouseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				// Need to DESTROY the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedT1 = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);
		

				GameObject building = Instantiate (metalHouseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = true;
				// CHECK to see what options to show next
				GetOptionsToShow(towntile);
		
			}
			break;
		case "Basic Defense":
			if (CheckResourceCost(wood: basicDefenseCost[0], stone: basicDefenseCost[1], metal: basicDefenseCost[2])){
				GameObject building = Instantiate (basicDefenseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has a building
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				townProps.tileHasTier1 = true;
				GetOptionsToShow(towntile);
			}
			break;
		case "Stone Defense":
			if (CheckResourceCost(wood: stoneDefenseCost[0], stone: stoneDefenseCost[1], metal: stoneDefenseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();

				// Need to DESTROY the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedT1 = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);

				GameObject building = Instantiate (stoneDefenseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = true;
				// CHECK to see what options to show next
				GetOptionsToShow(towntile);
			}
			break;
		case "Metal Defense":
			if (CheckResourceCost(wood: metalDefenseCost[0], stone: metalDefenseCost[1], metal: metalDefenseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				// Need to DISABLE the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store the Tier 1 bulding
				townProps.deactivatedT1 = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				// and turn it off
				oldBuilding[1].gameObject.SetActive(false);

				GameObject building = Instantiate (metalDefenseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = true;
				// CHECK to see what options to show next
				GetOptionsToShow(towntile);
			}
			break;
		case "Auto-Cannon":
			if (CheckResourceCost(wood: metalDefenseCost[0], stone: metalDefenseCost[1], metal: metalDefenseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				// Need to DISABLE the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store the Tier 2 bulding
				townProps.deactivatedT2 = oldBuilding[1].gameObject;
				// DONT actually turn it off so we can have a nice wall background for now
//				oldBuilding[1].gameObject.SetActive(false);
				
				GameObject building = Instantiate (autoCanFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = false;
				townProps.tileHasTier3 = true; // now has Tier 3 buiding
			}
			break;
		case "Stone-Thrower":
			if (CheckResourceCost(wood: metalDefenseCost[0], stone: metalDefenseCost[1], metal: metalDefenseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				// Need to DISABLE the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store the Tier 2 bulding
				townProps.deactivatedT2 = oldBuilding[1].gameObject;
				// DONT actually turn it off so we can have a nice wall background for now
				//				oldBuilding[1].gameObject.SetActive(false);
				
				GameObject building = Instantiate (throwFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = false;
				townProps.tileHasTier3 = true; // now has Tier 3 buiding
			}
			break;
		case "Catapult":
			if (CheckResourceCost(wood: metalDefenseCost[0], stone: metalDefenseCost[1], metal: metalDefenseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				// Need to DISABLE the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store the Tier 1 bulding
				townProps.deactivatedT2 = oldBuilding[1].gameObject;
				// DONT actually turn it off so we can have a nice wall background for now
				//				oldBuilding[1].gameObject.SetActive(false);
				
				GameObject building = Instantiate (catapFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = false;
				townProps.tileHasTier3 = true; // now has Tier 3 buiding
			}
			break;
		case "Farm":
			if (CheckResourceCost(wood: farmCost[0], stone: farmCost[1], metal: farmCost[2])){
				GameObject building = Instantiate (farmFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has a building
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				townProps.tileHasTier1 = true;
				GetOptionsToShow(towntile);
			}
			break;
		case "Stone Farm":
			if (CheckResourceCost(wood: stoneFarmCost[0], stone: stoneFarmCost[1], metal: stoneFarmCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				
				// Need to DESTROY the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedT1 = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);
				
				GameObject building = Instantiate (stoneFarmFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = true;
				// CHECK to see what options to show next
				GetOptionsToShow(towntile);
			}
			break;
		case "Metal Farm":
			if (CheckResourceCost(wood: metalFarmCost[0], stone: metalFarmCost[1], metal: metalFarmCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				// Need to DISABLE the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store the Tier 1 bulding
				townProps.deactivatedT1 = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				// and turn it off
				oldBuilding[1].gameObject.SetActive(false);
				
				GameObject building = Instantiate (metalFarmFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasTier1 = false; // no longer has basic building
				townProps.tileHasTier2 = true;
				// CHECK to see what options to show next
				GetOptionsToShow(towntile);
			}
			break;
		default:
			print ("Not enough resources!");
			break;
		}

	}

	bool CheckResourceCost(int wood, int stone, int metal){
		if (townResources.wood >= wood && townResources.stone >= stone && townResources.metal >= metal) {
			townResources.wood = townResources.wood - wood;
			townResources.stone = townResources.stone - stone;
			townResources.metal = townResources.metal - metal;
			return true;
		} else {
			return false;
		}
	}

	void DestroyBuilding(GameObject town){
		Transform[] children = town.GetComponentsInChildren<Transform> ();
		TownTile_Properties townProps = town.GetComponent<TownTile_Properties>();
								// find the BUILDING on the town tile
		Building building = town.GetComponentInChildren<Building> ();

						// subtract ALL BONUSES and give back ALL PENALTIES
		building.SubtractBonuses (building.myBuildingType);

		// IF TILE ONLY HAS 1 BUILDING then when we destroy we need to deactivate build buttons

		if (townProps.tileHasTier1) { // only destroy Tier 1 building
			Destroy (children [1].gameObject);
			// Change parent name to something else
			townTile.name = "Town X";
			townProps.tileHasTier1 = false;
			//		// then deActivate build buttons
			firstBuildBtn.gameObject.SetActive (false);
			secondBuildBtn.gameObject.SetActive (false);
			thirdBuidBtn.gameObject.SetActive (false);
			fourthBuildBtn.gameObject.SetActive (false);
			showBuildButtons = false;
			// then allow mouse to select again
			stopMouse = false;
		}else if (townProps.tileHasTier2){
			Destroy(children [1].gameObject); // destroy the Tier 2 building
			if (townProps.deactivatedT1 != null){
				townProps.deactivatedT1.SetActive(true); // activate Tier 1 building
									// Change name back to Tier 1 building name
				townTile.name = townProps.deactivatedT1.name;
				townProps.tileHasTier2 = false;// tile no longer has Tier 2 building
				townProps.tileHasTier1 = true;// tile now has Tier 1 building
				// CHECK to see what options to show next
				GetOptionsToShow(town);
			}else{
				townTile.name = "Town S"; // in case it can't find the T1 building, everything goes false
				townProps.tileHasTier2 = false;
				townProps.tileHasTier1 = false;
				// CHECK to see what options to show next
				GetOptionsToShow(town);
			}

		}else if (townProps.tileHasTier3){
			Destroy(children [2].gameObject); // destroy the Tier 3 building (using index 2 because im not turning off the wall)
			if (townProps.deactivatedT2 != null){
//				townProps.deactivatedT2.SetActive(true); 
				// Change name back to Tier 2 building name
				townTile.name = townProps.deactivatedT2.name;
				townProps.tileHasTier3 = false;// tile no longer has Tier 3 building
				townProps.tileHasTier2 = true;// tile now has Tier 2 building
				// CHECK to see what options to show next
				GetOptionsToShow(town);
			}else{
				townTile.name = "Town S"; // in case it can't find the T2 building, everything goes false
				townProps.tileHasTier3 = false;
				townProps.tileHasTier2 = false;
				townProps.tileHasTier1 = false;
				// CHECK to see what options to show next
				GetOptionsToShow(town);
			}
			
		}
	}
}
