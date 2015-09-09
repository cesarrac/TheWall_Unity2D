using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_Master : MonoBehaviour {
						// ******* 			CONTROLS UI ELEMENTS 	***************
	// (except Survivor Panel(TownCentral), Building Buttons(TownBuilding) and Resources window(TownResources))


	public Text dialogueText;
	public Image charPortrait;
	public GameObject dialoguePanel, survivorPanel;
	public Text charName;

	public List<Button> answerButtons = new List<Button>();
	public Button playerAnswerBttn;

	Town_Central townCentral;

	public Survivor currentSurvivor;
	public GameObject currSurvivor;

	// for the Dead
	public Text deadText;
	public GameObject deadPanel;
	Vector3 lastDeadPos; // to track text positions

	// SURVIVOR ABILITIES:
	public GameObject abilitiesPanel;
	public Button abilityBttnFab;
	public Button abBttn1, abBttn2;
		// Soldier:
	public string soldier_ab1Text, soldier_ab2Text;
	public Sprite soldier_ab1Sprite, soldier_ab2Sprite;
	// Farmer:
	public string farmer_ab1Text, farmer_ab2Text;
	public Sprite farmer_ab1Sprite, farmer_ab2Sprite;
	// Mechanic:
	public string mech_ab1Text, mech_ab2Text;
	public Sprite mech_ab1Sprite, mech_ab2Sprite;


	void Start () {
		// check that we're not in the game over screen
		if (Application.loadedLevel == 0) {
			townCentral = GameObject.FindGameObjectWithTag("Town_Central").GetComponent<Town_Central>();
		}
	}

	void Update(){
		// PRESS ESCAPE TO CLOSE SURVIVOR ABILITIES
		if (Input.GetButtonDown ("Escape")) {
			if (abBttn1 != null && abBttn2 != null){
				Destroy(abBttn1.gameObject);
				Destroy(abBttn2.gameObject);
			}
		}
	}

	// 								*****Controls for DIALOGUE PANEL******
	public void CreateAnswers(string type, bool isSurvivorPartOfTown = false){
		Vector2 corners = new Vector2(1, 1);

		switch (type) {
		case "Welcome":
			// clear any previous answer  buttons
			foreach (Button answerButton in answerButtons){
				Destroy(answerButton.gameObject);
			}
			answerButtons.Clear(); // clear the list
			if (dialoguePanel.activeSelf){
				CreateAnswerButton (playerAnswerBttn, dialoguePanel, corners, corners, new Vector3 (26f, 30f, 0), "Join me!", CallAddSurvivor);
			}
			break;
		case "Salute":
			// clear any previous answer  buttons
			foreach (Button answerButton in answerButtons){
				Destroy(answerButton.gameObject);
			}
			answerButtons.Clear(); // clear the list
			if (dialoguePanel.activeSelf){
				CreateAnswerButton (playerAnswerBttn, dialoguePanel, corners, corners, new Vector3 (26f, 30f, 0), "Nothing.", CloseDialogue);
				if (!isSurvivorPartOfTown){
					// second answer button for Join me
					CreateAnswerButton (playerAnswerBttn, dialoguePanel, corners, corners, new Vector3 (26f, 90f, 0), "Join me!", CallAddSurvivor);
				}else{
					CreateAnswerButton (playerAnswerBttn, dialoguePanel, corners, corners, new Vector3 (26f, 60f, 0), "Go Home.", CallGoHome);
				}
			}

			break;
		}

	}


	
	void CreateAnswerButton(Button buttonPrefab, GameObject panel, Vector2 cornerTopR, Vector2 cornerBottL, Vector3 position,  string answer, UnityEngine.Events.UnityAction method){
		Button answerB = Instantiate (buttonPrefab, Vector3.zero, Quaternion.identity) as Button;
		RectTransform rectTransform = answerB.GetComponent<RectTransform> ();
		rectTransform.SetParent (panel.transform);
		rectTransform.anchorMax = cornerTopR;
		rectTransform.anchorMin = cornerBottL;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.sizeDelta = new Vector2 (160, 30);
		rectTransform.localPosition = new Vector3(rectTransform.localPosition.x - position.x, rectTransform.localPosition.y - position.y, 0);
		Text txt = answerB.gameObject.GetComponentInChildren<Text> ();
		txt.text = answer;
		answerB.onClick.AddListener(method);
		// add these answers to List of active answers
		answerButtons.Add (answerB);
	}

	void CallAddSurvivor(){
		if (currentSurvivor != null) {
			townCentral.AddSurvivor(currentSurvivor.mySprite, currentSurvivor.name,currentSurvivor);
			dialoguePanel.SetActive (false);
		}
	}

	void CloseDialogue(){
		dialoguePanel.SetActive (false);
	}

	void CallGoHome(){
		if (currentSurvivor != null) {
			currentSurvivor.GoHome();
			dialoguePanel.SetActive (false);
		}
	}


	// 								*****Controls for Book of dead Panel (shows names of Dead survivors)******
	public void AddDeadSurvivor(string name){
		Vector2 corners = new Vector2(0, 1);
		Text dead = Instantiate (deadText, Vector3.zero, Quaternion.identity) as Text;
		RectTransform rect = dead.GetComponent<RectTransform> ();
		rect.SetParent (deadPanel.transform);
		rect.anchorMax = corners;
		rect.anchorMin = corners;
		rect.offsetMax = Vector2.zero;
		rect.offsetMin = Vector2.zero;
		rect.sizeDelta = new Vector2 (93, 30);

		if (lastDeadPos != Vector3.zero) {
			rect.localPosition = new Vector3 (lastDeadPos.x, lastDeadPos.y - rect.sizeDelta.y , 0);
			dead.text = name;
			lastDeadPos = rect.localPosition;
		} else {
			rect.localPosition = new Vector3(rect.localPosition.x + 43, rect.localPosition.y -120f, 0);
			dead.text = name;
			lastDeadPos = rect.localPosition;
		}
	}

	// ************************* SURVIVOR ABILITIES

	public void CreateAbilityButtons(string survivorClass, UnityEngine.Events.UnityAction ability1Action, UnityEngine.Events.UnityAction ability2Action){
		Vector2 corners = new Vector2 (0, 0); // bottom left
		// Spawn Buttons
		abBttn1 = Instantiate (abilityBttnFab, Vector3.zero, Quaternion.identity) as Button;
		abBttn2 = Instantiate (abilityBttnFab, Vector3.zero, Quaternion.identity) as Button;
		// Get Rect Transform from each
		RectTransform rectTransform1 = abBttn1.GetComponent<RectTransform> ();
		RectTransform rectTransform2 = abBttn2.GetComponent<RectTransform> ();
		rectTransform1.SetParent (abilitiesPanel.transform);
		rectTransform1.anchorMax = corners;
		rectTransform1.anchorMin = corners;
		rectTransform1.offsetMax = Vector2.zero;
		rectTransform1.offsetMin = Vector2.zero;
		rectTransform1.sizeDelta = new Vector2 (64, 64);
		rectTransform2.SetParent (abilitiesPanel.transform);
		rectTransform2.anchorMax = corners;
		rectTransform2.anchorMin = corners;
		rectTransform2.offsetMax = Vector2.zero;
		rectTransform2.offsetMin = Vector2.zero;
		rectTransform2.sizeDelta = new Vector2 (64, 64);

		rectTransform1.localPosition = new Vector3 (rectTransform1.localPosition.x + 32, rectTransform1.localPosition.y + 16, 0);
		rectTransform2.localPosition = new Vector3 (rectTransform2.localPosition.x + 96, rectTransform2.localPosition.y + 16, 0);
		Text ab1text = abBttn1.GetComponentInChildren<Text>();
		Text ab2text = abBttn2.GetComponentInChildren<Text>();
		Image ab1sprite = abBttn1.GetComponent<Image> ();
		Image ab2sprite = abBttn2.GetComponent<Image> ();

		switch (survivorClass) {
		case "Soldier":
			ab1text.text = soldier_ab1Text;
			ab2text.text = soldier_ab2Text;
			ab1sprite.sprite = soldier_ab1Sprite;
			ab2sprite.sprite = soldier_ab2Sprite;
			abBttn1.onClick.AddListener(ability1Action);
			abBttn2.onClick.AddListener(ability2Action);
			break;
		case "Farmer":
			ab1text.text = farmer_ab1Text;
			ab2text.text = farmer_ab2Text;
			ab1sprite.sprite = farmer_ab1Sprite;
			ab2sprite.sprite = farmer_ab2Sprite;
			abBttn1.onClick.AddListener(ability1Action);
			abBttn2.onClick.AddListener(ability2Action);
			break;
		case "Mechanic":
			ab1text.text = mech_ab1Text;
			ab2text.text = mech_ab2Text;
			ab1sprite.sprite = mech_ab1Sprite;
			ab2sprite.sprite = mech_ab2Sprite;
			abBttn1.onClick.AddListener(ability1Action);
			abBttn2.onClick.AddListener(ability2Action);
			break;
		default:
			print ("No ABILITIES FOUND!!!");
			break;
		}
	}
}
