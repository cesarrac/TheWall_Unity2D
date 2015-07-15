using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Town_Central : MonoBehaviour {

	// max and available number of Gatherers
	public int maxGatherers = 2;
	public int availableGatherers = 2;

	// prefab Gatherer
	public GameObject gatherer;

	// Text displaying available gatherers
	public Text availableText;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		availableText.text = "Available: " + availableGatherers;

	}

	public void SpawnGatherer(){
		Vector3 m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (availableGatherers > 0) {
			//Instantiate gatherer and stick it to the mouse position
			GameObject gathererToSpwn = Instantiate (gatherer, new Vector3 (Mathf.Round(m.x), Mathf.Round(m.y), 0), Quaternion.identity) as GameObject;
			//take one out of available
			availableGatherers = availableGatherers - 1;
		}

	}
}
