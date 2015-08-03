using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_Master : MonoBehaviour {
	public Text dialogueText;
	public Image charPortrait;
	public GameObject dialoguePanel, survivorPanel;
	public Text charName;

	public List<Button> answerButtons = new List<Button>();
	public Button playerAnswerBttn;

	Town_Central townCentral;

	public Survivor currentSurvivor;
	public GameObject currSurvivor;

	void Start () {
		// check that we're not in the game over screen
		if (Application.loadedLevel == 0) {
			townCentral = GameObject.FindGameObjectWithTag("Town_Central").GetComponent<Town_Central>();
		}
	}
	


	public void CreateAnswers(string type){
		Vector2 corners = new Vector2(1, 1);

		switch (type) {
		case "Welcome":
			// clear any previous answer  buttons
			foreach (Button answerButton in answerButtons){
				Destroy(answerButton.gameObject);
			}
			answerButtons.Clear(); // clear the list
			CreateAnswerButton (playerAnswerBttn, dialoguePanel, corners, corners, new Vector3 (26f, 30f, 0), "Join me!", CallAddSurvivor);
			break;
		case "Salute":
			// clear any previous answer  buttons
			foreach (Button answerButton in answerButtons){
				Destroy(answerButton.gameObject);
			}
			answerButtons.Clear(); // clear the list
			CreateAnswerButton (playerAnswerBttn, dialoguePanel, corners, corners, new Vector3 (26f, 30f, 0), "Nothing.", CloseDialogue);
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
}
