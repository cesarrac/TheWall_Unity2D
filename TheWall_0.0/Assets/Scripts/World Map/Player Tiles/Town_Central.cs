using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Town_Central : MonoBehaviour {

	// max and available number of Gatherers
	public int maxGatherers = 2, firstMax;
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
//	// # of survivor vacancies are determined by how many houses * by that house maxCapacity 
//	public int survivorVacancies;
//	int currVacancies; //keep track of vacancies
	public int housingCount;
	public int survivorCount, currSurvivorCount;
	// Access to the UI Panel that shows Survivor Portraits
	public GameObject survivorPanel;
	//Button / Survivor Slot (represents the survivor's in town)
	public Button survivorSlotBttn;
	public int slotBttnCount; // keep track of how many buttons have been created
	Vector3 bttnPos; // keep track of button/slots positions in Panel
	public List<RectTransform> slots = new List<RectTransform>();

	// Store the Survivors as Survivor Data
	public List<Survivor_Data> survivorsInTown = new List<Survivor_Data>();
	// and as Survivor 
	public List<Survivor> survivorsSpawned = new List<Survivor> ();

	Button currBtn; 
	// Survivor to spawn
	public GameObject survivorFab;
	Survivor newSurvivor;

	// number of Farms made
	public List<Farm> farms = new List<Farm>();

	void Start () {
		availableGatherers = maxGatherers;
		firstMax = maxGatherers;
//		currVacancies = survivorVacancies;
		currSurvivorCount = survivorCount;
	}
	

	void Update () {
		// Track SURVIVORS here
//		TrackSurvivorCount ();
		survivorCount = survivorsInTown.Count;

		// Track GATHERERS available and display it as Text under Gatherer Button
		if (maxGatherers > firstMax) {
			int diff = maxGatherers - firstMax;
			availableGatherers = availableGatherers + diff;
			firstMax = maxGatherers;
		}

		if (availableGatherers >= 0) {
			availableText.text = "Available: " + availableGatherers;
		} else {
			availableText.text = "Available: " + 0;
		}
	


	}

	// Track how many survivors have joined in order to create a new button

//	void TrackSurvivorCount(){
//		Vector2 corners = new Vector2(0.5f, 1);
//
//
//		// instead of tracking here the button that I would need to destroy, the survivor can store and destroy
//		// its own button/slot
//
////		if (survivorVacancies > currVacancies) {// # of vacancies is larger so ADD a new survivor slot
////			// calculate how much it increased
////			int calc = survivorVacancies - currVacancies;
////			// then add that many slots
////			for (int x = 1; x <= calc; x++){
////				CreateButton(survivorSlotBttn, survivorPanel, corners, corners, bttnPos);
////			} 
////			currVacancies = survivorVacancies;
////		} else if (survivorVacancies < currVacancies) {
////			// # of vacancies is less so SUBSTRACT a survivor slot
////				// difference b/w them
////			int calc = currVacancies - survivorVacancies;
////			for (int x = 1; x <= calc; x++){
////				if (slotBttnCount > 1){// check there is only ONE slot left
////					// get the position of the button created before the one being removed
////					// using the slotBttnCount -1 to determine the list index
////					bttnPos = slots[slotBttnCount - 2].position; // count -1 would give me the index of this button, -2 gives me the last
////					// then destroy the last button created
////					Destroy(slots[slotBttnCount-1].gameObject);
////					slots.RemoveAt(slotBttnCount-1);
////					currVacancies = survivorVacancies;
////					slotBttnCount--;
////				}else{ // only one slot left
////					Destroy(slots[slotBttnCount-1].gameObject);
////					currVacancies = survivorVacancies;
////					slots.Clear();
////					slotBttnCount--;
////				}
////			} 
////		}
//	}



	public void CreateButton (Button buttonPrefab, GameObject panel, Vector2 cornerTopR, Vector2 cornerBottL, Vector3 position, Survivor mySurvivor){
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
		survivorSlot.onClick.AddListener (() => SpawnSurvivor(rectTransform, survivorSlot));

		// fill up the button
		Text txt = survivorSlot.gameObject.GetComponentInChildren<Text> ();//Get the slot's text component
		Image moodBub = txt.gameObject.GetComponentInChildren<Image>();
		survivorSlot.image.sprite = mySurvivor.mySprite; // Fill Sprite
		Color newColor = AdaptSurvivorMood(mySurvivor.mood);
		moodBub.color = newColor; // Fill Mood Bubble with Color
		txt.text = mySurvivor.name;// Fill the text with the name

	}

	public void AddSurvivor(Sprite sprite, string name, Survivor thisSurvivor){
		// instead of Checking for vacancies, this will just ADD a new survivor slot every time a Survivor Joins
		// Create Survivor Slot / Button && Fill the button with Sprite, Name, and Mood Color
		Vector2 corners = new Vector2(0.5f, 1);
		CreateButton(survivorSlotBttn, survivorPanel, corners, corners, bttnPos, thisSurvivor);
		//Store & Destroy
		StoreSurvivor(thisSurvivor);
		Destroy(thisSurvivor.gameObject); // Destroy the gameobject in the scene
	
	}
	public void SpawnSurvivor(RectTransform slot, Button bttn){
		int count = 0;
		// these Survivor slots should have the same list index that each survivorInTown does
		for (int x =0; x < slots.Count; x++) {
			if (slots[x].gameObject == slot.gameObject){
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
		emptySurvivor.ForcedStart (survivorsInTown [count].myName, survivorsInTown [count].mySprite, survivorsInTown [count].myMood, (Survivor.SurvivorClass)survivorsInTown [count].myClass, survivorsInTown[count].myID);

		SpriteRenderer sr = survivorSpwn.GetComponent<SpriteRenderer> ();// add the right sprite
		sr.sprite = emptySurvivor.mySprite;

		survivorSpwn.name = emptySurvivor.name + " the " + emptySurvivor.mySurvivorClass;

		emptySurvivor.beingMoved = true; // make being moved true so it follows the mouse
		// give the survivor the slot it came from so it can enable it when not spawned
		emptySurvivor.mySurvivorSlot = slot;
		// and the slot as a button
		emptySurvivor.mySurvivorButton = bttn;
		bttn.enabled = false;// make bttn disabled

		// store this spawned survivor
		StoreSurvivorSpawn (emptySurvivor);
	
	}

	public void SpawnGatherer(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (availableGatherers > 0) {
			//Instantiate gatherer and stick it to the mouse position
			GameObject gathererToSpwn = Instantiate (gatherer, new Vector3 (Mathf.Round(m.x), Mathf.Round(m.y), -2f), Quaternion.identity) as GameObject;
			//take one out of available
			spawnedGatherers ++;

			availableGatherers = availableGatherers - 1;

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



	void StoreSurvivor(Survivor surv){ // Add this Survivor to the list
		// when adding a NEW survivor it will need a new id (using the list count as ID)
		survivorsInTown.Add (new Survivor_Data (surv.name, surv.mySprite, surv.mood, (Survivor_Data.SurvivorClass)surv.mySurvivorClass, id: survivorsInTown.Count));
	}

	void StoreSurvivorSpawn(Survivor surv){
		survivorsSpawned.Add (surv);
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
					// change spawned Survivor's mood according to ID
					MoodChangeForSpawnedSurvivors(survivorD.myID, survivorD.myMood);
				}
			}else{
				if (survivorD.myMood > -10f){
					survivorD.myMood = survivorD.myMood + change;
					MoodBubbleChange(survivorD.myMood);
					// change spawned Survivor's mood according to ID
					MoodChangeForSpawnedSurvivors(survivorD.myID, survivorD.myMood);
				}
			}
		}
	}

	// change Mood to SPECIFIC slot / survivor in town BY ID
	public void MoodChange(float change, int id){
		if (survivorsInTown.Count > 0) {
			foreach (Survivor_Data survivorD in survivorsInTown) {
				if (survivorD.myID == id) {
					if (change > 0) {				// Positive Mood change
						if (survivorD.myMood < 10){
							survivorD.myMood = survivorD.myMood + change;
							MoodBubbleChange(survivorD.myMood);
							// change spawned Survivor's mood according to ID
							MoodChangeForSpawnedSurvivors(survivorD.myID, survivorD.myMood);
						}
					} else {						// Negative Mood change
						if (survivorD.myMood > -10f){
							survivorD.myMood = survivorD.myMood + change;
							MoodBubbleChange(survivorD.myMood);
							// change spawned Survivor's mood according to ID
							MoodChangeForSpawnedSurvivors(survivorD.myID, survivorD.myMood);
						}
					}
				}
			}
		} else {
			print ("No Survivors in Town!");
		}

	}

	void MoodChangeForSpawnedSurvivors(int id, float newMood){
		Debug.Log ("changing spwned survivor. ID: " + id + " new mood: " + newMood);
		foreach (Survivor spwnedSurvivor in survivorsSpawned) {
			if (spwnedSurvivor.id == id){
//				Survivor survScript = spwnedSurvivor.gameObject.GetComponent<Survivor>();
//				survScript.mood = newMood;
				spwnedSurvivor.mood = newMood;
				Debug.Log("Found matching ID! Mood Changed.");
				break;
			}
		}
	}

	void MoodBubbleChange(float newMood){
		if (survivorPanel.activeSelf && slots.Count > 0) { // if panel is not active DONT change the bubble now, update when active
			foreach (RectTransform slot in slots) {
				Button btn = slot.gameObject.GetComponent<Button> ();// Fill up the slots img
				Text txt = btn.gameObject.GetComponentInChildren<Text> ();//Get the slot's text component
				Image moodBub = txt.gameObject.GetComponentInChildren<Image>();
				Color newColor = AdaptSurvivorMood(newMood);
				moodBub.color = newColor; // Fill Mood Bubble with Color
			}
		}

	}

	public void ClearDeadSurvivor(int id){
		// check the list of Survivors in town against this id
		for (int x =0; x < survivorsInTown.Count; x++) {
			if (survivorsInTown[x].myID == id){
				survivorsInTown.RemoveAt(x);
				break;
			}
		}
		for (int y= 0; y < survivorsSpawned.Count; y++) {
			if (survivorsSpawned[y].id == id){
				survivorsSpawned.RemoveAt(y);
				break;
			}
		}
	}

	public void ClearForGoHome(int id){
		for (int y= 0; y < survivorsSpawned.Count; y++) {
			if (survivorsSpawned[y].id == id){
				survivorsSpawned.RemoveAt(y);
				break;
			}
		}
	}

		// This clears TRAITORS who have not been spawned (they just leave town)
	void ClearTraitorFromList(int id){
//		foreach (RectTransform slot in slots) {
//			Survivor surv = slot.gameObject.GetComponent<Survivor>();
//		}
//		for (int x =0; x < survivorsInTown.Count; x++) {
//			if (survivorsInTown[x].myID == id){
//
//				survivorsInTown.RemoveAt(x);
//				break;
//			}
//		}
	}

	public void RemoveSurvivorSlot (float yPosOfSlotToRemove, GameObject slotObj){
		// we compare slotToRemove with the list of slots and store it
		for (int x =0; x < slots.Count; x++) {
			if (slots[x].position.y == yPosOfSlotToRemove){ // comparing the Y position of the slot
				slots.RemoveAt(x);
				Destroy(slotObj.gameObject);
				slotBttnCount--;
				// if there's more than one slot left they need to be moved up
				if (slots.Count > 0){
					MoveButtons(x);
				}
			}
		}
		// we Remove it from List and destroy the slotToRemove
		// then all slots that have a higher count on the list than the slot removed get moved up
	}
	
	void MoveButtons(int index){
		for (int x= index; x < slots.Count; x++) {
			// move them up
			slots[x].position = new Vector3(slots[x].position.x, slots[x].position.y + 70f, 0);
		}
	}
}
