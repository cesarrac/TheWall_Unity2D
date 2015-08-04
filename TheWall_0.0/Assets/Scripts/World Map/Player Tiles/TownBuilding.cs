using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TownBuilding : MonoBehaviour {

	public Map_Manager mapScript;
	Transform myTransform;
	Vector3 lastPos;
	// buttons
	public Button firstBuildBtn, secondBuildBtn, thirdBuidBtn;
	//basic building images
	public Sprite houseSprite, basicDefenseSprite, slaughterSprite, workSprite;
	//advanced images
	public Sprite stoneHouseSprite, metalHouseSprite, stoneDefenseSprite, metalDefenseSprite;
	//destroy / cancel building image
	public Sprite cancelSprite;
	// empty button sprite
	public Sprite emptyButtonSprite;

	//prefab Buildings
	// basic:
	public GameObject houseFab, basicDefenseFab, slaughterHouseFab, workshopFab;
	// advanced:
	public GameObject stoneHouseFab, metalHouseFab, stoneDefenseFab, metalDefenseFab, stoneWorkFab, metalWorkFab;

	//current town tile
	public GameObject townTile;
	GameObject storedTile;

	// button Text
	Text firstText, secondText, thirdText;

	// bool turns true if this tile has a building
	public bool hasBuilding, hasAdvanced;

	// access to Town Resources
	public GameObject townRes;
	TownResources townResources;

	// costs for building as an array of int [], wood = [0], stone = [1], metal = [2]
	// house costs
	public int[] houseCost = new int[3];
	public int[] stoneHouseCost = new int[3];
	public int[]metalHouseCost = new int[3];
	// defense costs
	public int[] basicDefenseCost = new int[3];
	public int[] stoneDefenseCost = new int[3];
	public int[] metalDefenseCost = new int[3];
	// slaughterhouse cost
	public int[] slaughterCost= new int[3];
	// workshop costs
	public int[] workshopCost= new int[3];
	public int[] metalWorkshopCost= new int[3];
	public int[] stoneWorkshopCost= new int[3];


	// we have resources check
	bool weHaveResources;

	// Layer mask so we dont hit ourselves with LineCast
	public LayerMask mask;

	void Awake () {
		mapScript = GetComponent<Map_Manager> ();
	}
	void Start(){
		myTransform = transform;
		lastPos = myTransform.position; // record this position

		firstText = firstBuildBtn.GetComponentInChildren<Text>();
		secondText = secondBuildBtn.GetComponentInChildren<Text>();
		thirdText = thirdBuidBtn.GetComponentInChildren<Text>();


//		townTile = mapScript.GetTownTile ();
//		ShowBasicBuildings();

		townResources = townRes.GetComponent<TownResources> ();

		CheckWhatTile ();

	}



	void Update () {
		// if I've moved, check tile
		if (myTransform.position != lastPos){
			CheckWhatTile ();
			lastPos = myTransform.position;
		}

	}

	// raycast from this position to check what town tile we are on
	public void CheckWhatTile(){

//		RaycastHit2D hit = Physics2D.Linecast (new Vector2 (myTransform.position.x, myTransform.position.y), -Vector2.up, mask.value);
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(myTransform.position.x, myTransform.position.y), -Vector2.up, 100, mask.value);  

		if (hit.collider != null) {
//			print ("Camera hits " + hit.collider.name);
			if (hit.collider.CompareTag("Town_Tile")){
				townTile = hit.collider.gameObject; // grab the gameobject the camera is detecting
				GetOptionsToShow(townTile);// SHOW BUILD OPTIONS

			}else if(hit.collider.CompareTag("Tile")){
				//destroy this, this is under a town tile
//				Destroy(hit.collider.gameObject);
				mapScript.ClearResourceTilesUnderTown(myTransform.position, hit.collider.gameObject);
				CheckWhatTile();
		
			}else if(hit.collider.CompareTag("Capital")){
				// show empty buttons
				ShowNone();
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
			hasBuilding = ttile.GetComponent<TownTile_Properties>().tileHasBuilding;
			hasAdvanced = ttile.GetComponent<TownTile_Properties>().tileHasAdvancedBuilding;
			if (hasBuilding){
				ShowAdvancedBuildings(ttile.name);
			}else if (hasAdvanced){
				ShowOnlyDestroyed();
			}
			else{
				ShowBasicBuildings();
			}
		}
	}

	void ShowNone(){
//		firstBuildBtn.enabled = false;
//		secondBuildBtn.enabled = false;
//		thirdBuidBtn.enabled = false;

		firstBuildBtn.image.sprite = emptyButtonSprite;
		secondBuildBtn.image.sprite = emptyButtonSprite;
		thirdBuidBtn.image.sprite = emptyButtonSprite;

		firstText.text = "Can't Build";
		secondText.text = "Can't Build";
		thirdText.text = "Can't Build";
	}

	void ShowBasicBuildings(){
		firstBuildBtn.enabled = true;
		secondBuildBtn.enabled = true;
		// show basic buildings
		firstBuildBtn.image.sprite = houseSprite;
		// get the text
		string houseText = "House";
		firstText.text = houseText;

		secondBuildBtn.image.sprite = basicDefenseSprite;
		string defenseText = "Basic Defense";
		secondText.text = defenseText;

		thirdBuidBtn.image.sprite = workSprite ;
		string slaughterText = "Workshop";
		thirdText.text = slaughterText;
	}

	// to show the right Advanced building options, I need to get the name of the building on this town tile
	void ShowAdvancedBuildings(string buildingName){
		string destroyText = "Destroy";
		firstBuildBtn.enabled = true;
		secondBuildBtn.enabled = true;
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
			// as third show option for Destroy
			thirdBuidBtn.image.sprite = cancelSprite;
			// get the text

			thirdText.text = destroyText;
			break;
		case "Basic Defense":
			firstBuildBtn.image.sprite = stoneDefenseSprite;
			string stoneDefenseText = "Stone Defense";
			firstText.text = stoneDefenseText;

			secondBuildBtn.image.sprite = metalDefenseSprite;
			string metalDefenseText = "Metal Defense";
			secondText.text = metalDefenseText;

			// as third show option for Destroy
			thirdBuidBtn.image.sprite = cancelSprite;
			// get the text
			thirdText.text = destroyText;
			break;
		case "Workshop":
//			firstBuildBtn.image.sprite = stoneDefenseSprite;
			string stoneWorkText = "Stone Workshop";
			firstText.text = stoneWorkText;
			
//			secondBuildBtn.image.sprite = metalDefenseSprite;
			string metalWorkText = "Metal Workshop";
			secondText.text = metalWorkText;
			
			// as third show option for Destroy
			thirdBuidBtn.image.sprite = cancelSprite;
			// get the text
			thirdText.text = destroyText;
			break;
		default:
			print ("No advanced options for this building found!");
			break;
		}
	}

	void ShowOnlyDestroyed(){
		firstBuildBtn.enabled = false;

		secondBuildBtn.enabled = false;
		
		// as third show option for Destroy
		thirdBuidBtn.image.sprite = cancelSprite;
		// get the text
		string destroyText = "Destroy";
		thirdText.text = destroyText;
	}
	
	public void FirstBuild(){
		string name = firstText.text;
		if (townTile != null) {
			CheckBuildingRecipeAndBuild(name, townTile);
		}

	}

	public void SecondBuild(){
		string name = secondText.text;
		if (townTile != null) {
			CheckBuildingRecipeAndBuild(name, townTile);

		}
		
	}

	public void ThirdBuild(){
		string name = thirdText.text;
		if (townTile != null) {
			if (name != "Destroy"){
				CheckBuildingRecipeAndBuild(name, townTile);
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
				townProps.tileHasBuilding = true; 

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
				townProps.deactivatedBuilding = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);
				
				GameObject building = Instantiate (stoneWorkFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasBuilding = false; // no longer has basic building
				townProps.tileHasAdvancedBuilding = true;
			}
			break;
		case "Metal Workshop":
			if (CheckResourceCost(wood: metalWorkshopCost[0], stone: metalWorkshopCost[1], metal: metalWorkshopCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				
				// Need to DESTROY the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedBuilding = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);
				
				GameObject building = Instantiate (metalWorkFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasBuilding = false; // no longer has basic building
				townProps.tileHasAdvancedBuilding = true;
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
				townProps.tileHasBuilding = true;

				GetOptionsToShow(towntile);
			}
			break;
		case "Stone House":
			if (CheckResourceCost(wood: stoneHouseCost[0], stone: stoneHouseCost[1], metal: stoneHouseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();

				// Need to DESTROY the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedBuilding = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);

				GameObject building = Instantiate (stoneHouseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasBuilding = false; // no longer has basic building
				townProps.tileHasAdvancedBuilding = true;

			}
			break;
		case "Metal House":
			if (CheckResourceCost(wood: metalHouseCost[0], stone: metalHouseCost[1], metal: metalHouseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				// Need to DESTROY the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedBuilding = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);
		

				GameObject building = Instantiate (metalHouseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasBuilding = false; // no longer has basic building
				townProps.tileHasAdvancedBuilding = true;
		
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
				townProps.tileHasBuilding = true;
				GetOptionsToShow(towntile);
			}
			break;
		case "Stone Defense":
			if (CheckResourceCost(wood: stoneDefenseCost[0], stone: stoneDefenseCost[1], metal: stoneDefenseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();

				// Need to DESTROY the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedBuilding = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);

				GameObject building = Instantiate (stoneDefenseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasBuilding = false; // no longer has basic building
				townProps.tileHasAdvancedBuilding = true;
			}
			break;
		case "Metal Defense":
			if (CheckResourceCost(wood: metalDefenseCost[0], stone: metalDefenseCost[1], metal: metalDefenseCost[2])){
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				// Need to DISABLE the old house in this towntile (use an array to get the child)
				Transform[] oldBuilding = towntile.GetComponentsInChildren<Transform>();
				// store this oldBuilding in this town tile
				townProps.deactivatedBuilding = oldBuilding[1].gameObject;
				// since the first Transform of the array is always the parent, access the second item
				oldBuilding[1].gameObject.SetActive(false);

				GameObject building = Instantiate (metalDefenseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				// need to makes sure the new gameobject's name matches my hardcoded names
				building.name = name;
				towntile.name = name;
				//then tell this tile that it has an Advanced building
				townProps.tileHasBuilding = false; // no longer has basic building
				townProps.tileHasAdvancedBuilding = true;
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

		if (townProps.tileHasBuilding) { // only destroy basic building
			Destroy (children [1].gameObject);
			// Change parent name to something else
			townTile.name = "Town X";
			townProps.tileHasBuilding = false;
			print ("Children: " + children.Length);
		}else if (townProps.tileHasAdvancedBuilding){
			Destroy(children [1].gameObject); // destroy the advanced building
			if (townProps.deactivatedBuilding != null){
				townProps.deactivatedBuilding.SetActive(true); // activate old building
				// Change name back to old building name
				townTile.name = townProps.deactivatedBuilding.name;
				townProps.tileHasAdvancedBuilding = false;
				townProps.tileHasBuilding = true;
			}else{
				townTile.name = "Town S";
				townProps.tileHasAdvancedBuilding = false;
				townProps.tileHasBuilding = false;
			}

		}
	}
}
