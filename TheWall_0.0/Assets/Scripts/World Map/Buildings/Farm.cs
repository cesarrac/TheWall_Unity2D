using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Farm : Building {

	Transform myTransform;
	TownResources townResources;


	public int farmListID; // farms count -1

	public bool harvesting;
	public float timeToHarvest; // time it takes for this farm to harvest
	public int harvestAmmount; 

	void Awake () {
		InitFarmValues (myBuildMaterial); // init values according to material of this building

		myTransform = transform;
		GameObject town = GameObject.FindGameObjectWithTag ("Town_Central");
		townCentral = town.GetComponent<Town_Central> ();
		townResources = town.GetComponent<TownResources> ();
	

		if (townTProps == null) {
			townTProps = GetComponentInParent<TownTile_Properties> ();
		}

		if (townCentral != null) {
			townCentral.farms.Add(this);
			farmListID = townCentral.farms.Count -1;
		}
	}

	void InitFarmValues(BuildMaterialType type){
		switch (type) {
		case BuildMaterialType.basic:
			timeToHarvest = 5f;
			harvestAmmount = 5;
			break;
		case BuildMaterialType.stone:
			timeToHarvest = 90f;
			harvestAmmount = 8;
			break;
		case BuildMaterialType.metal:
			timeToHarvest = 60f;
			harvestAmmount = 5;
			break;
		default:
			timeToHarvest = 120f;
			harvestAmmount = 5;
			break;
		}
	}

	void Start(){
//		CheckTilesAroundMe (); // perfors a check with map manager to see if there are Food Sources around
	}

	public void PlantCrops(){
		Debug.Log ("Crops planted!");
		harvesting = true;
	}
	
	void Update () {
		if (harvesting) {
			StartCoroutine(WaitForHarvest());
		}
	}

	IEnumerator WaitForHarvest(){
		harvesting = false;
		yield return new WaitForSeconds (timeToHarvest);
		Harvest ();

	}

	void Harvest(){
		if (townResources != null) {
			townResources.food = townResources.food + harvestAmmount;
			print ("Harvested " + harvestAmmount + " food!!");
		}
		harvesting = true;
	}


//
//	public void CheckTilesAroundMe(){
//		Vector3 myPosition = new Vector3 (myTransform.position.x, myTransform.position.y, -2f); // my position refers to the tile to my  top right
//		for (int x = 0; x <  mapManager.tileDataList.Count; x++){
//			// find my position
//			if (mapManager.tileDataList[x].gridPosition == myPosition){
//					// grab the positions
//				//top left, top c, top right
//				int topL = x + (mapManager.colums - 1); int topC = x + (mapManager.colums); int topR = x + (mapManager.colums + 1);
//				// left, right
//				int left = x - 1; int right = x + 1;
//				// bott left, bott c, bott right
//				int bottL = x - (mapManager.colums + 1); int bottC = x - (mapManager.colums); int bottR = x - (mapManager.colums - 1);
//				// check the tile directly below me
//				if (mapManager.tileDataList[topR].resourceType == "grain"){
//					grainTiles.Add(mapManager.tileDataList[topR].tileGameObject);
//					// to not allow other farms to Harvest from this food, change its tag to Tile
//					mapManager.tileDataList[topR].tileGameObject.tag = "Tile";
//				}
//				if (mapManager.tileDataList[topC].resourceType == "grain"){
//					grainTiles.Add(mapManager.tileDataList[topC].tileGameObject);
//					mapManager.tileDataList[topC].tileGameObject.tag = "Tile";
//				}
//				if (mapManager.tileDataList[topL].resourceType == "grain"){
//					grainTiles.Add(mapManager.tileDataList[topL].tileGameObject);
//					mapManager.tileDataList[topL].tileGameObject.tag = "Tile";
//				}
//				if (mapManager.tileDataList[right].resourceType == "grain"){
//					grainTiles.Add(mapManager.tileDataList[right].tileGameObject);
//					mapManager.tileDataList[right].tileGameObject.tag = "Tile";
//				}
//				if (mapManager.tileDataList[left].resourceType == "grain"){
//					grainTiles.Add(mapManager.tileDataList[left].tileGameObject);
//					mapManager.tileDataList[left].tileGameObject.tag = "Tile";
//				}
//				if (mapManager.tileDataList[bottR].resourceType == "grain"){
//					grainTiles.Add(mapManager.tileDataList[bottR].tileGameObject);
//					mapManager.tileDataList[bottR].tileGameObject.tag = "Tile";
//				}
//				if (mapManager.tileDataList[bottC].resourceType == "grain"){
//					grainTiles.Add(mapManager.tileDataList[bottC].tileGameObject);
//					mapManager.tileDataList[bottC].tileGameObject.tag = "Tile";
//				}
//				if (mapManager.tileDataList[bottL].resourceType == "grain"){
//					grainTiles.Add(mapManager.tileDataList[bottL].tileGameObject);
//					mapManager.tileDataList[bottL].tileGameObject.tag = "Tile";
//				}
//				break;
//			}
//		}
//	}

//	public void Harvest(){
//		if (grainTiles.Count > 0) {
//			// add to town's food
//			int foodSourceCount = SourceCheck();
//			townResources.grain = townResources.grain + foodSourceCount;
//			print ("Harvested " + foodSourceCount + " food!!");
//		}
//	}
//	int SourceCheck(){
//		for (int x = 0; x < grainTiles.Count; x++) {
//			if (grainTiles[x] == null){
//				grainTiles.RemoveAt(x);
//			}
//		}
//		return grainTiles.Count;
//	}


}
