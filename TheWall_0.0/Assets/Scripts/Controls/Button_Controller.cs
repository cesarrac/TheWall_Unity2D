using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Button_Controller : MonoBehaviour {

//	public Canvas battleMenu;
//	public Button startBattleButton;
//
//	// Use this for initialization
//	void Start () {
//		battleMenu = battleMenu.GetComponent<Canvas> ();
//		startBattleButton = startBattleButton.GetComponent<Button> ();
//	}

	GameMaster gm;

	public void StartBattle(){
		gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();
		gm.InitLists ();
	}
}
