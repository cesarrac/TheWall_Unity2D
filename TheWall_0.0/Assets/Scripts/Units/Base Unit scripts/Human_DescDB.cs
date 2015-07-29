using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Human_DescDB : MonoBehaviour {

	public List<Description> humDescriptions = new List<Description>();

	void Start () {

		// ************* HUMAN DESCRIPTIONS ************************
		humDescriptions.Add (new Description ("Born 4875, in the town of Menagua. \nA wife awaits for him at home."));
		humDescriptions.Add (new Description ("a Scottish landowner who became one of \nthe main leaders during the Wars of Scottish Independence."));
		humDescriptions.Add (new Description ("a French military and political leader."));

		// ************* HUMAN DESCRIPTIONS ************************
	}
	

}
