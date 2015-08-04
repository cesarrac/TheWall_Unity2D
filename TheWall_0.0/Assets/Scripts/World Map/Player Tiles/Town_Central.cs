using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Town_Central : MonoBehaviour {

	// max and available number of Gatherers
	public int maxGatherers = 2;
	public int availableGatherers = 2;
	public int spawnedGatherers;
	// prefab Gatherer
	public GameObject gatherer;

	// Text displaying available gatherers
	public Text availableText;

	// Damages for tap hits (keeping it simple for now)
	public float shortRangeDamage;
	public float longRangeDamage;

	// A list of Gatherers that have been spawned
	public List<Gatherer> currentGatherers = new List<Gatherer>();

	// Boosts to Gatherers (from buildings or Upgrades
	public float gatherTimeGen, gatherTimeWood, gatherTimeStone, gatherTimeMetal, gatherTimeGrain;
	public int gatherAmntGen;

	// Survivors
	// # of survivor vacancies are determined by how many houses * by that house maxCapacity 
	public int survivorVacancies;
	int currVacancies; //keep track of vacancies
	// Access to the UI Panel that shows Survivor Portraits
	public GameObject survivorPanel;
	//Button / Survivor Slot (represents the survivor's in town)
	public Button survivorSlotBttn;
	public int slotBttnCount; // keep track of how many buttons have been created
	Vector3 bttnPos; // keep track of button/slots positions in Panel
	public List<RectTransform> slots = new List<RectTransform>();

	// Store the Survivors
	public List<Survivor_Data> survivorsInTown = new List<Survivor_Data>();
	Button currBtn; 
// Survivor to spawn
	public GameObject survivorFab;
	Survivor newSurvivor;

	void Start () {
		availableGatherers = maxGatherers;
		currVacancies = survivorVacancies;
	}
	

	void Update () {
		// Track VACANCIES here
		TrackSurvivorVacancy ();

		// Track GATHERERS available and display it as Text under Gatherer Button
		availableGatherers = maxGatherers;
		int calc = availableGatherers - spawnedGatherers;
		availableText.text = "Available: " + calc;

	}

	void TrackSurvivorVacancy(){
		Vector2 corners = new Vector2(0.5f, 1);
		if (survivorVacancies > currVacancies) {// # of vacancies is larger so ADD a new survivor slot
			// calculate how much it increased
			int calc = survivorVacancies - currVacancies;
			// then add that many slots
			for (int x = 1; x <= calc; x++){
				CreateButton(survivorSlotBttn, survivorPanel, corners, corners, bttnPos);
			} 
			currVacancies = survivorVacancies;
		} else if (survivorVacancies < currVacancies) {
			// # of vacancies is less so SUBSTRACT a survivor slot
				// difference b/w them
			int calc = currVacancies - survivorVacancies;
			for (int x = 1; x <= calc; x++){
				if (slotBttnCount > 1){// check there is only ONE slot left
					// get the position of the button created before the one being removed
					// using the slotBttnCount -1 to determine the list index
					bttnPos = slots[slotBttnCount - 2].position; // count -1 would give me the index of this button, -2 gives me the last
					// then destroy the last button created
					Destroy(slots[slotBttnCount-1].gameObject);
					slots.RemoveAt(slotBttnCount-1);
					currVacancies = survivorVacancies;
					slotBttnCount--;
				}else{ // only one slot left
					Destroy(slots[slotBttnCount-1].gameObject);
					currVacancies = survivorVacancies;
					slots.Clear();
					slotBttnCount--;
				}
			} 
		}
	}

	public void CreateButton (Button buttonPrefab, GameObject panel, Vector2 cornerTopR, Vector2 cornerBottL, Vector3 position){
		Button survivorSlot = Instantiate (buttonPrefab, Vector3.zero, Quaternion.identity) as Button;
		RectTransform rectTransform = survivorSlot.GetComponent<RectTransform> ();
		slots.Add (rectTransform); // ADD TO LIST OF BUTTONS
		rectTransform.SetParent (panel.transform);
		rectTransform.anchorMax = cornerTopR;
		rectTransform.anchorMin = cornerBottL;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.sizeDelta = new Vector2 (64, 64);
		// lock it to the initial button position
		Vector3 initbttnPos = new Vector3 (rectTransform.localPosition.x, -7f, 0);
		float buttonOffset = -6f; // space between buttons
		// check if this is button #2 or after
		if (slotBttnCount >= 1) {
			//topRightCorner + height + space between slots = top right position of new button 
			print ("New Survivor slot!");
			Vector3 newPos = new Vector3(position.x, (position.y - rectTransform.sizeDelta.y) + buttonOffset, 0);
			rectTransform.localPosition = newPos;
			bttnPos = newPos;
			slotBttnCount++;
		} else {
			bttnPos = initbttnPos;
			rectTransform.localPosition = bttnPos; // first button pos
			slotBttnCount++;
			print ("First Survivor Slot created!");
		}
		survivorSlot.onClick.AddListener (() => SpawnSurvivor(survivorSlot));

	}

	public void SpawnSurvivor(Button bttn){
		int count = 0;
		// these Survivor slots should have the same list index that each survivorInTown does
		for (int x =0; x < slots.Count; x++) {
			if (slots[x].gameObject == bttn.gameObject){
				count = x;
				Debug.Log(count);
				break;
			}
		}
//		count = (count > 0) ? count : 0;
		// mose pos
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);// get the mouse position
		GameObject survivorSpwn = Instantiate(survivorFab, new Vector3 (Mathf.Round(m.x), Mathf.Round(m.y)-2f), Quaternion.identity) as GameObject;// instantiate survivor prefab at mouse position
		Survivor emptySurvivor = survivorSpwn.GetComponent<Survivor> ();// add the survivor component
		emptySurvivor.ForcedStart (survivorsInTown [count].myName, survivorsInTown [count].mySprite, survivorsInTown [count].myMood, (Survivor.SurvivorClass)survivorsInTown [count].myClass);

		SpriteRenderer sr = survivorSpwn.GetComponent<SpriteRenderer> ();// add the right sprite
		sr.sprite = emptySurvivor.mySprite;

		survivorSpwn.name = emptySurvivor.name + " the " + emptySurvivor.mySurvivorClass;

		emptySurvivor.beingMoved = true; // make being moved true so it follows the mouse
		// give the survivor the slot it came from so it can enable it when not spawned
		emptySurvivor.mySurvivorSlot = bttn;
		bttn.enabled = false;// make bttn disabled
	
	}

	public void SpawnGatherer(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (availableGatherers > 0) {
			//Instantiate gatherer and stick it to the mouse position
			GameObject gathererToSpwn = Instantiate (gatherer, new Vector3 (Mathf.Round(m.x), Mathf.Round(m.y), -2f), Quaternion.identity) as GameObject;
			//take one out of available
			spawnedGatherers ++;

			Gatherer currGatherer = gathererToSpwn.GetComponent<Gatherer>();
			// give it any boosts available

			//Apply Boosts according to type of resource
			GathererBoosts(currGatherer);

			//add to list of current gatherers to store its info
			currentGatherers.Add(currGatherer);
		
		}

	}

	void GathererBoosts(Gatherer gatherer){
		gatherer.woodGatherTime = gatherer.woodGatherTime + gatherTimeWood;
		gatherer.stoneGatherTime = gatherer.stoneGatherTime + gatherTimeStone;
		gatherer.metalGatherTime = gatherer.metalGatherTime + gatherTimeMetal;
		gatherer.grainGatherTime = gatherer.grainGatherTime + gatherTimeGrain;
		gatherer.genGatherTime = gatherer.genGatherTime + gatherTimeGen;

		gatherer.gatherAmmount = gatherer.gatherAmmount + gatherAmntGen;
	}

	public void AddSurvivor(Sprite sprite, string name, Survivor thisSurvivor){

		// Are there any slots available?
		if (slots.Count > 0) {
			foreach (RectTransform slot in slots) { 	// Which slot is tagged empty?
				if (slot.gameObject.tag == "Empty Slot") {
					Button btn = slot.gameObject.GetComponent<Button> ();// Fill up the slots img
					Text txt = btn.gameObject.GetComponentInChildren<Text> ();//Get the slot's text component
					Image moodBub = txt.gameObject.GetComponentInChildren<Image>();
					btn.image.sprite = sprite; // Fill Sprite
					Color newColor = AdaptSurvivorMood(thisSurvivor.mood);
					moodBub.color = newColor; // Fill Mood Bubble with Color
					txt.text = name;// Fill the text with the name
					slot.gameObject.tag = "Full Slot";// Change this button's tag to Full Slot
					StoreSurvivor(thisSurvivor);
					Destroy(thisSurvivor.gameObject); // Destroy the gameobject in the scene
					break;
				}
			}
		} else {
			print ("Build Houses to Add more survivors!");
		}
	
	}

	void StoreSurvivor(Survivor surv){ // Add this Survivor to the list
		survivorsInTown.Add (new Survivor_Data (surv.name, surv.mySprite, surv.mood, (Survivor_Data.SurvivorClass)surv.mySurvivorClass));
	}

	Color AdaptSurvivorMood(float mood){
		float loyalist = 10f;
		float traitor = -10f;
		Color newColor = new Color();
		if (mood == 0) {
//			newColor = new Color(248, 233, 21);   // OK
			newColor = Color.yellow;
			Debug.Log ("Changing Color to OK!");
			return newColor;
		}
		else if (mood < loyalist && mood > 0){ // happy 
//			newColor= new Color(16, 255, 0);
			newColor = Color.green;
			return newColor;
		}
		else if (mood == loyalist){ 			// loyalist
//			newColor = new Color(6, 228, 236);
			newColor = Color.cyan;
			return newColor;
			
		}else if (mood < 0 && mood > traitor){// Mad
//			newColor = new Color(231, 90, 32);
			newColor = Color.red;
			return newColor;


		}else{
//			newColor = new Color(160, 0, 0);   // Traitor
			newColor = Color.black;
			return newColor;

		}
	}
					// THESE METHODS BELOW vvv AFFECT THE SURVIVOR DATA
				
	// this Method is called by GM or any other script that needs to change the moods of Survivors In Town
	public void MoodChange(float change){
		foreach (Survivor_Data survivorD in survivorsInTown) {
			if (change > 0){
				if (survivorD.myMood < 10){
					survivorD.myMood = survivorD.myMood + change;
					MoodBubbleChange(survivorD.myMood);
				}
			}else{
				if (survivorD.myMood > -10f){
					survivorD.myMood = survivorD.myMood + change;
					MoodBubbleChange(survivorD.myMood);
				}
			}
		}
	}

	// change Mood to SPECIFIC slot / survivor in town BY NAME
	public void MoodChange(float change, string name){
		if (survivorsInTown.Count > 0) {
			foreach (Survivor_Data survivorD in survivorsInTown) {
				if (survivorD.myName == name) {
					if (change > 0) {
						if (survivorD.myMood < 10){
							survivorD.myMood = survivorD.myMood + change;
							MoodBubbleChange(survivorD.myMood);
						}
					} else {
						if (survivorD.myMood > -10f){
							survivorD.myMood = survivorD.myMood + change;
							MoodBubbleChange(survivorD.myMood);
						}
					}
				}
			}
		} else {
			print ("No Survivors in Town!");
		}

	}

	void MoodBubbleChange(float newMood){
		foreach (RectTransform slot in slots) {
			Button btn = slot.gameObject.GetComponent<Button> ();// Fill up the slots img
			Text txt = btn.gameObject.GetComponentInChildren<Text> ();//Get the slot's text component
			Image moodBub = txt.gameObject.GetComponentInChildren<Image>();
			Color newColor = AdaptSurvivorMood(newMood);
			moodBub.color = newColor; // Fill Mood Bubble with Color
		}
	}
}
