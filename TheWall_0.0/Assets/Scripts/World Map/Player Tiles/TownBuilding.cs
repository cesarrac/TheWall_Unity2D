using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TownBuilding : MonoBehaviour {

	public Map_Manager mapScript;
	Transform myTransform;
	// buttons
	public Button firstBuildBtn;

	//images
	public Sprite sprite;

	//prefab Buildings
	public GameObject houseFab;

	//current town tile
	public GameObject townTile;
	GameObject storedTile;

	// button Text
	Text firstText;

	// bool turns true if this tile has a building
	public bool hasBuilding;

	void Awake () {
		mapScript = GetComponent<Map_Manager> ();
	}
	void Start(){
		myTransform = transform;

		firstText = firstBuildBtn.GetComponentInChildren<Text>();
		CheckWhatTile ();
//		townTile = mapScript.GetTownTile ();
		ShowBasicBuildings();

	}
	
	// Update is called once per frame
	void Update () {
	
		if (townTile != null) {
			hasBuilding = townTile.GetComponent<TownTile_Properties>().tileHasBuilding;
			if (hasBuilding){
				// show advanced
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
		// show basic buildings
		firstBuildBtn.image.sprite = sprite;
		// get the text
		string houseText = "House";
		
		firstText.text = houseText;
	}

	public void FirstBuild(){
		string name = firstText.text;
		if (townTile != null) {
			if (name == "House") {
				GameObject building = Instantiate (houseFab, myTransform.position, Quaternion.identity) as GameObject;
				// parent it to the town tile this is on
				building.transform.parent = townTile.transform;
			}
		}

	}
}
