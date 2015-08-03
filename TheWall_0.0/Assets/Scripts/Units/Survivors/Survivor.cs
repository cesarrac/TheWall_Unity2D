using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Survivor : Unit {
	// Unit base class gives it a name and description
	// Survivor vars:
	public Sprite mySprite;
	Transform myTransform;
	//Mood: (-10 traitorous - 0 OK - 10 Loyalist)
	public float mood = 0; // 0 = Neutral (OK)
	float currMood;
	float loyalist = 10f;
	float traitor = -10f;
	public string moodType;

	public string GetMoodType(){
		string moodT;
		if (mood == 0) {
			moodT = "OK";
		}
		else if (mood == loyalist){
			moodT = "Loyalist";
			
		}else if (mood < loyalist && mood > 0){ // positive response
			moodT = "Happy";
			
		}else if (mood < 0 && mood > traitor){
			moodT = "Mad"; // angry response
			
		}else{
			moodT = "Traitorous"; // traitor response
		}
		return moodT;
	}

	// Response: this is the text that will be displayed when Player interacts with this Survivor
	public string response;
	// TODO: Create a Database of responses that can be accesed by this method
	public string GetResponse(string responseType){
		switch (responseType) {
		case "Salute":
			// loyalist response
			if (mood == loyalist){
				response = "How may I serve our cause?";

			}else if (mood < loyalist && mood > 0){ // positive response
				response = "What's up?";

			}else if (mood < 0 && mood > traitor){
				response = "What do you want from me now?"; // angry response

			}else if (mood == 0){
				response = "Yeah?"; // neutral response
				
			}else{
				response = "Don't bother me. I'm busy."; // traitor response

			}
			return response;
			break;
		case "Welcome":
			// Mood doesn't matter here. This is the first time the Player meets this Survivor
			response = "Hello there."; // traitor response
			response += "\nMy name is " + name;
			return response;
			break;
		default:
			response = "What's up?";
			return response;
			break;
		}
	}

	// CLASS or Type of Survivor (Each Class carries unique bonuses and needs)
	public enum SurvivorClass{
		soldier,
		farmer,
		scientist,
		mechanic
	}

	public SurvivorClass mySurvivorClass;


	// UI elements. Used to display a response and a character portrait when talking
	public Text dialogueText;
	public Image charPortrait;
	public GameObject dialoguePanel;
	public Text charName;

	public Button playerAnswerBttn;
	GameObject[] answerBttns;
	public GameObject survivorPanel; // to make it pop-up when the dialogue window is up
	public Button mySurvivorSlot;

	// Access to Town Central
	public Town_Central townCentral;

	// ** BASIC BONUSES **
	public float basicTimeBoost = -0.3f;
	public float basicHPBoost = 3f;
	// ** BASIC BONUSES **

	public bool beingMoved;
	public bool partOfTown; // turns true when Player recruits this survivor
	public bool hasBeenToTown; // turns true when Survivor has visited Town

	UI_Master uiMaster;

	// Building and Town Tile that im standing on when placed
	TownTile_Properties townTile;
	Building currBuilding;

	public void ForcedStart(string sname, Sprite ssprite, float smood, SurvivorClass sClass){
		name = sname;
		mySprite = ssprite;
		mood = smood;
		mySurvivorClass = sClass;
	}

	void Start () {
		uiMaster = GameObject.FindGameObjectWithTag ("GM").GetComponent<UI_Master> ();
		dialoguePanel = uiMaster.dialoguePanel;
		dialogueText = uiMaster.dialogueText;
		charPortrait = uiMaster.charPortrait;
		survivorPanel = uiMaster.survivorPanel;
		charName = uiMaster.charName;

		myTransform = transform;

		if (mySprite == null) {
			// when this Survivor is spawned they need a name
			name = GetName (false);
			// get the Sprite (for the Player's UI if this Survivor joins a town)
			mySprite = GetComponent<SpriteRenderer> ().sprite;
		}
	
		
	
		// get the current mood Type
		moodType = GetMoodType ();
		// and store the current mood
		currMood = mood;

		if (townCentral == null) {
			townCentral = GameObject.FindGameObjectWithTag("Town_Central").GetComponent<Town_Central>();
		}
	}



	void Update () {
		// check the moodtype everytime Mood changes
		if (mood > currMood || mood < currMood){
			moodType = GetMoodType();
		}

		if (beingMoved) {
			FollowMouse();
		}
	}

	void FollowMouse(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		myTransform.position = new Vector3 (Mathf.Round (m.x), Mathf.Round (m.y), -2f);
		//		myTransform.position = new Vector3 (m.x, m.y, -2f);
		
		if (Input.GetMouseButtonDown (0)) {
			ApplyBasicBonus(); // give tile basic bonuses
			beingMoved = false;
		}
	}
	// for testing im making this survivor talk when clicked on
	void OnMouseOver(){
		if (Input.GetMouseButtonDown (1)) {
			Talk ("Salute");
		}
	}

	// checks what Building this survivor has been placed on
	void ApplyBasicBonus(){
		if (townTile != null) {
			townTile.tileHitPoints = townTile.tileHitPoints + basicHPBoost;
			if (townTile.tileHasBuilding || townTile.tileHasBuilding) {
				currBuilding = townTile.GetComponentInChildren<Building> ();
				CheckBuilding (currBuilding);
			}
		} else {
			Debug.Log("No Town Tile found by " + name);
		}
	}

	void CheckBuilding(Building building){
		Building.BuildingType bType = building.myBuildingType;
		switch (bType) {
		case Building.BuildingType.house:
			// apply Special House bonuses
			break;
		case Building.BuildingType.defense:
			// apply Special defense bonuses
			break;
		case Building.BuildingType.food:
			// apply Special food bonuses
			break;
		case Building.BuildingType.workshop:
			// apply Special workshop bonuses
			break;
		default:
			print("Building of this type NOT FOUND!");
			break;
		}
	}
	public void Talk(string type){
		Vector2 corners = new Vector2(1, 1);
		dialoguePanel.SetActive (!dialoguePanel.activeSelf);
		// Depending on the type of Talk interaction/type different responses and player options will be available
		switch (type) {
		case "Welcome":
			if (!partOfTown && !hasBeenToTown){
				uiMaster.CreateAnswers(type);
				uiMaster.currentSurvivor = GetComponent<Survivor>();
				uiMaster.currSurvivor = gameObject;
				uiMaster.charName.text = name;
//				// first activate the dialogue panel
//				dialoguePanel.SetActive (!dialoguePanel.activeSelf);
				survivorPanel.SetActive(true);
				// then display response and portrait
				dialogueText.text = GetResponse (type);
				charPortrait.sprite = mySprite;
//				// then create appropriate Player AnswerCloths
//				CreateAnswerButton(playerAnswerBttn, dialoguePanel, corners, corners,new Vector3(26f, 30f, 0) , "Join me!");
				hasBeenToTown = true;
			}

			break;
		default:
			uiMaster.CreateAnswers(type);
			uiMaster.charName.text = name;
//			// first activate the dialogue panel
//			dialoguePanel.SetActive (!dialoguePanel.activeSelf);
//			survivorPanel.SetActive(true);
//			// then display response and portrait
			dialogueText.text = GetResponse (type);
			charPortrait.sprite = mySprite;
			break;
		}

	}

	// when this survivor is placed on a building I need to find out what type it is
	void OnTriggerStay2D(Collider2D coll){
		if (coll.gameObject.tag == "Town_Tile") {
			townTile = coll.gameObject.GetComponent<TownTile_Properties>();
		}
	}
}
