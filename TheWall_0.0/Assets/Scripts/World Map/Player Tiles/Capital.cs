using UnityEngine;
using System.Collections;

public class Capital : MonoBehaviour {
	GameMaster gm;
//	public bool gameover;

	void Start () {
		gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();
	}

//	void Update(){
//		if (gameover) {
//			CallGameOver ();
//
//		}
//	}
	public void CallGameOver(){
		gm.gameOver = true;
	}
}
