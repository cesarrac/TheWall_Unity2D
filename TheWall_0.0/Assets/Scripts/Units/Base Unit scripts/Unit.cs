using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit: MonoBehaviour {

	public string name;
	public string description;

	string pName;
	string pDescription;

//	public Human_DescDB descriptionDatabase;
//	List<Description> descList = new List<Description>();
	string[] monsterNames = new string[]{"Mort", "Sotted", "Grrax", "Lumped"};
	string[] humanNames = new string[]{"Tomas", "Fenix", "Dorean", "Tyrion"};

	void Start(){

//			descriptions = GameObject.FindGameObjectWithTag("Human Descriptions").GetComponent<Human_DescDB>().humDescriptions;
//			descList = descriptionDatabase.humDescriptions;
	

		pName = name;
		pDescription = description;
	}
	public string GetName(bool IsThisAnEnemy){
		int nameSelect = Random.Range (0, 4);
		string randomName;
		if (IsThisAnEnemy) {
			randomName = monsterNames [nameSelect];
		} else {
			randomName = humanNames [nameSelect];
		}
		return randomName;
	}

	public string GunName(){
		string randomName = "This Gun.";
		return randomName;
	}	
	//TODO: A Description class that is just a long string. It will be constructed in two Description Databases (one for Human one for Monsters)
	// A method HERE will access one of the descriptions and fill up the description property

//	public string GetDesc(){
//		int descSelect = Random.Range (0, descList.Count);
//		string randomDescription = descList [2].descBody;
//
//		return randomDescription;
//
//	}
}
