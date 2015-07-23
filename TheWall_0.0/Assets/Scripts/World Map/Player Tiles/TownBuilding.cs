using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TownBuilding : MonoBehaviour {

	public Map_Manager mapScript;
	Transform myTransform;
	// buttons
	public Button firstBuildBtn;
	public Button secondBuildBtn;
	//images
	public Sprite houseSprite, basicDefenseSprite;
	//prefab Buildings
	public GameObject houseFab, basicDefenseFab;

	//current town tile
	public GameObject townTile;
	GameObject storedTile;

	// button Text
	Text firstText;
	Text secondText;

	// bool turns true if this tile has a building
	public bool hasBuilding;

	// access to Town Resources
	public GameObject townRes;
	TownResources townResources;

	// costs for building
	int woodCost, metalCost, stoneCost;
	public int houseCost = 5;
	public int basicDefenseCost = 10;

	// we have resources check
	bool weHaveResources;

	void Awake () {
		mapScript = GetComponent<Map_Manager> ();
	}
	void Start(){
		myTransform = transform;

		firstText = firstBuildBtn.GetComponentInChildren<Text>();
		secondText = secondBuildBtn.GetComponentInChildren<Text>();
//		CheckWhatTile ();
//		townTile = mapScript.GetTownTile ();
		ShowBasicBuildings();

		townResources = townRes.GetComponent<TownResources> ();

	}
	
	// Update is called once per frame
	void Update () {
		CheckWhatTile ();


		if (townTile != null) {
			hasBuilding = townTile.GetComponent<TownTile_Properties>().tileHasBuilding;
			if (hasBuilding){
				ShowAdvancedBuildings();
			}else{
				ShowBasicBuildings();
			}
		}
	}

	// linecast from this position to check what town tile we are on
	public void CheckWhatTile(){

		RaycastHit2D hit = Physics2D.Linecast (new Vector2 (myTransform.position.x, myTransform.position.y), Vector2.up);
		if (hit.collider != null) {
			print ("Camera hits " + hit.collider.name);
			if (hit.collider.CompareTag("Town_Tile")){
				townTile = hit.collider.gameObject; // grab the gameobject the camera is detecting

			}else if(hit.collider.CompareTag("Tile")){
				//destroy this, this is under a town tile
//				Destroy(hit.collider.gameObject);
				mapScript.ClearResourceTilesUnderTown(myTransform.position, hit.collider.gameObject);

			}else if(hit.collider.CompareTag("Tile Under Attack")){
				// moves the player out of that tile so you cant access it
//				myTransform.position = new Vector3(myTransform.position.x + 1f, myTransform.position.y, myTransform.position.z);
			}else if (hit.collider.CompareTag("Destroyed Town")){
				Destroy(hit.collider.gameObject);
			}
		}
	}

	// Everytime we move from TileToTile (keyboard) this is called
	public void CheckWhatToShow(){

//		if (mapScript.CheckTownTileForBuilding(myTransform.position) == false) {
//			townTile = mapScript.GetTownTile ();
//			firstBuildBtn.enabled = true;
//			ShowBasicBuildings();
//
//
//		} else if (mapScript.CheckTownTileForBuilding(myTransform.position) == true) {
//			firstBuildBtn.enabled = false;
//			//check what buildings they are
//			// to show upgrade options
//		}
	}

	void ShowBasicBuildings(){
//		firstBuildBtn.image.color = new Color (firstBuildBtn.image.color.r, firstBuildBtn.image.color.g, firstBuildBtn.image.color.b, firstBuildBtn.image.color.a + 50f);
//		secondBuildBtn.image.color = new Color (secondBuildBtn.image.color.r, secondBuildBtn.image.color.g, secondBuildBtn.image.color.b, secondBuildBtn.image.color.a + 50f);

		// show basic buildings
		firstBuildBtn.image.sprite = houseSprite;
		// get the text
		string houseText = "House";
		firstText.text = houseText;

		secondBuildBtn.image.sprite = basicDefenseSprite;
		string defenseText = "Basic Defense";
		secondText.text = defenseText;
	}

	void ShowAdvancedBuildings(){
//		firstBuildBtn.image.color = new Color (firstBuildBtn.image.color.r, firstBuildBtn.image.color.g, firstBuildBtn.image.color.b, firstBuildBtn.image.color.a - 50f);
//		secondBuildBtn.image.color = new Color (secondBuildBtn.image.color.r, secondBuildBtn.image.color.g, secondBuildBtn.image.color.b, secondBuildBtn.image.color.a - 50f);

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

	void CheckBuildingRecipeAndBuild(string name, GameObject towntile){

		switch (name) {
		case "House":
			if (townResources.wood >= houseCost){
				GameObject building = Instantiate (houseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				//then tell this tile that it has a building
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				townProps.tileHasBuilding = true;
				townResources.wood = townResources.wood - houseCost;
			}
			break;
		case "Basic Defense":
			if (townResources.wood >= basicDefenseCost){
				GameObject building = Instantiate (basicDefenseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = towntile.transform;
				//then tell this tile that it has a building
				TownTile_Properties townProps = towntile.GetComponent<TownTile_Properties>();
				townProps.tileHasBuilding = true;
				townResources.wood = townResources.wood - basicDefenseCost;
			}
			break;
		default:
			weHaveResources = false;
			break;
		}

	}
}
