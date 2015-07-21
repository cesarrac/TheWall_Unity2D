using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TownTile_Properties : MonoBehaviour {

	// this holds all the interactable properties of a town tile game object

	// index to hold its ID in Map Manager's towntileDataList
	public int listIndex;

	// bool to tell if this tile has a building on it
	public bool tileHasBuilding;

	// float for tile's Hit Points
	public float tileHitPoints = 10f;

	// stored transform
	Transform myTransform;


	void Start () {
		myTransform = transform;
	}
	

	void Update () {
	
	}

	public void TakeDamage(float damage){
		tileHitPoints = tileHitPoints - damage;
		if (tileHitPoints <= 0) {
			KillTile();
		}
	}
	
	void KillTile(){
		Destroy (this.gameObject);
	}
}
