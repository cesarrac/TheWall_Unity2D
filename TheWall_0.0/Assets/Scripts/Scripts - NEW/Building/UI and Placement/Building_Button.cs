using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Building_Button : MonoBehaviour {
	public Building_UIHandler buildingUI;
	public ResourceGrid resourceGrid;

	void OnMouseUpAsButton(){
		Debug.Log ("CLICK!");
		if (buildingUI != null) {
			string buildingName = GetComponentInChildren<Text>().text;
			buildingUI.BuildThis(buildingName);
		}
	}

}
