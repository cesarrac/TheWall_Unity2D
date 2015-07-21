using UnityEngine;
using System.Collections;

public class BuildingBuilt : MonoBehaviour {

	bool buildingCheck;

	// Use this for initialization
	void Start () {
		buildingCheck = GetComponentInParent<TownTile_Properties> ().tileHasBuilding;

		if (!buildingCheck) {
			buildingCheck = true;
		}
	}

}
