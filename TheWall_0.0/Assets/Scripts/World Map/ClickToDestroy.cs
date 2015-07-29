using UnityEngine;
using System.Collections;

public class ClickToDestroy : MonoBehaviour {
	Mouse_Controls mouse;
	// Use this for initialization
	void OnMouseOver () {
		mouse = GameObject.FindGameObjectWithTag ("Map_Manager").GetComponent<Mouse_Controls> ();
		mouse.resourceTileToDestroy = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
