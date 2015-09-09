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
		none,						// none is default
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
	public RectTransform mySurvivorSlot;
	public Button mySurvivorButton;

	// Access to Town Central
	public Town_Central townCentral;

	// ** BASIC BONUSES **
	public float basicTimeBoost = -0.3f;
	public float basicHPBoost = 3f;
	// ** BASIC BONUSES **

	public bool beingMoved;
	public bool partOfTown; // turns true when Player recruits this survivor
	public bool hasBeenToTown; // turns true when Survivor has visited Town

	public UI_Master uiMaster;

	// Building and Town Tile that im standing on when placed
	public TownTile_Properties townTile;
	Building currBuilding;

	// ID # to be able to compare a Spawned Survivor with a Survivor from data list (survivors In Town)
	public int id;

	public void ForcedStart(string sname, Sprite ssprite, float smood, SurvivorClass sClass, int myID){
		name = sname;
		mySprite = ssprite;
		mood = smood;
		mySurvivorClass = sClass;
		partOfTown = true;
		// add the class script to this survivor gameobject
//		AddSurvivorClass (mySurvivorClass);
		id = myID;
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
			currMood = mood;
		}

		if (beingMoved) {
			FollowMouse();
		}

		if (townTile != null) {
			CheckIfDead ();
			if (mood <= traitor){	// IF MOOD IS REALLY LOW, this Survivor will act out
				TraitorAction();			// Violently...
			}
		}
	}

	void FollowMouse(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		myTransform.position = new Vector3 (Mathf.Round (m.x), Mathf.Round (m.y), -2f);
		//		myTransform.position = new Vector3 (m.x, m.y, -2f);
		
		if (Input.GetMouseButtonDown (0)) {
			if (townTile != null){
				if (!townTile.hasASurvivor){ // cant place more than one in a building
					ApplyBasicBonus(townTile); // give tile basic bonuses
					beingMoved = false;
				}
			}

		}
	}
	// for testing im making this survivor talk when clicked on
	void OnMouseOver(){
		if (Input.GetMouseButtonDown (1)) {
			Talk ("Salute");
		}
	}

	// checks what Building this survivor has been placed on
	void ApplyBasicBonus(TownTile_Properties townTile){
			// BASIC TILE HP BOOST: All survivors give a slight HP boost when placed
		// tell the town tile that it has a Survivor on it
		townTile.hasASurvivor = true;
		townTile.tileHitPoints = townTile.tileHitPoints + basicHPBoost;
	
		// ** CHECK BUILDING FROM EACH OF THE SURVIVOR CLASSES SCRIPT SINCE EACH PROVIDE UNIQUE BONUSES
	//Check Building and Add Class
		if (townTile.tileHasTier1 || townTile.tileHasTier2 || townTile.tileHasTier3) {
			currBuilding = townTile.GetComponentInChildren<Building> ();
			CheckBuilding (currBuilding);
		}

	}

	void CheckIfDead(){
		float tileHP = townTile.tileHitPoints;
		if (tileHP <= 0) {
			KillSurvivor();
		}

	}

	void KillSurvivor(){
		// identify & remove this survivor in Town Central's survivors in town list & survivorsSpawned List
		townCentral.ClearDeadSurvivor (id);
		// remove it from Survivor slots
		townCentral.RemoveSurvivorSlot (mySurvivorSlot.position.y, mySurvivorSlot.gameObject);
		// add the name to the Book of Dead
		uiMaster.AddDeadSurvivor (name);
		if (townTile != null) {
			townTile.hasASurvivor = false;
		}
		// then destroy this gameObject
		Destroy (gameObject);

	}
						// GO HOME: spawned survivor goes back to its slot
	public void GoHome(){
		// identify and remove from Town Central's spawned survivors
		townCentral.ClearForGoHome (id);
		// activate this survivor's button so it can be spawned again
		mySurvivorButton.enabled = true;
		// tell the tile I was on it no longer has a survivor
		if (townTile != null) {
			townTile.hasASurvivor = false;
		}
		// then destroy this object
		Destroy (gameObject);
	}


	public void CheckBuilding(Building building){
		Building.BuildingType bType = building.myBuildingType;
		switch (bType) {
		case Building.BuildingType.house:
			// apply Special House bonuses

			break;
		case Building.BuildingType.defense:
			// add Soldier class
			AddSurvivorClass(SurvivorClass.soldier);
			break;
		case Building.BuildingType.food:

			AddSurvivorClass(SurvivorClass.farmer);
			break;
		case Building.BuildingType.workshop:

			AddSurvivorClass(SurvivorClass.scientist);
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
			Debug.Log("WELCOME dialogue is on");
			if (!partOfTown && !hasBeenToTown){
				uiMaster.CreateAnswers(type);
				uiMaster.currentSurvivor = GetComponent<Survivor>();
				uiMaster.currSurvivor = gameObject;
				uiMaster.charName.text = name;
//				// first activate the dialogue panel
				dialoguePanel.SetActive (true);
				survivorPanel.SetActive(true);
				// then display response and portrait
				dialogueText.text = GetResponse (type);
				charPortrait.sprite = mySprite;
//				// then create appropriate Player AnswerCloths
//				CreateAnswerButton(playerAnswerBttn, dialoguePanel, corners, corners,new Vector3(26f, 30f, 0) , "Join me!");
				hasBeenToTown = true;
			}

			break;
		case "Salute":
			Debug.Log(type  + " dialogue is on");
			uiMaster.CreateAnswers(type, partOfTown);

			uiMaster.currentSurvivor = GetComponent<Survivor>();
			uiMaster.currSurvivor = gameObject;
			uiMaster.charName.text = name;

			// then display response and portrait
			dialogueText.text = GetResponse (type);
			charPortrait.sprite = mySprite;
			hasBeenToTown = true;
			break;
		default:
			Debug.Log(type  + " dialogue is on");

			uiMaster.CreateAnswers(type, partOfTown);
			uiMaster.charName.text = name;
//			// first activate the dialogue panel

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

	// ADD SURVIVOR CLASS: this is called when the Survivor is spawned by the player
	void AddSurvivorClass(SurvivorClass survivorClass){
		switch (survivorClass) {
		case SurvivorClass.farmer:
			gameObject.AddComponent<Farmer>();
			mySurvivorClass = SurvivorClass.farmer;
			// change name
			name = name + " the " + mySurvivorClass;
			gameObject.name = name;
			break;
		case SurvivorClass.soldier:
			gameObject.AddComponent<Soldier>();
			mySurvivorClass = SurvivorClass.soldier;
			// change name
			name = name + " the " + mySurvivorClass;
			gameObject.name = name;
			break;
		case SurvivorClass.scientist:
			gameObject.AddComponent<Scientist>();
			mySurvivorClass = SurvivorClass.scientist;
			// change name
			name = name + " the " + mySurvivorClass;
			gameObject.name = name;
			break;
		default:
			print ("This surivor ain't got no class!");
			break;
		}
	}

	void TraitorAction(){
		print (name + " has become a TRAITOR!!!");
	
		townTile.TakeDamage(townTile.tileHitPoints);
		Debug.Log("Destroying " + townTile.name);
		KillSurvivor ();

	}
}
