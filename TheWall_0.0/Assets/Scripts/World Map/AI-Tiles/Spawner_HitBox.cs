using UnityEngine;
using System.Collections;

public class Spawner_HitBox : MonoBehaviour {
	HordeSpawner hordeSpwn;
	//access to town central for damage var
	Town_Central townCentral;
	// bool to tell if this spawner is visible to Player
	public bool visible;

	void Awake () {
		hordeSpwn = GetComponentInParent<HordeSpawner> ();
		townCentral = GameObject.FindGameObjectWithTag ("Town_Central").GetComponent<Town_Central> ();
	}

	void Update(){
		visible = hordeSpwn.visible;
	}
	
	// using this Mouse Over to detect when player hits this Spawner
	void OnMouseOver(){
		print ("Mouse over Spawner");
		if (Input.GetMouseButtonDown (0)) {
			// get the current town damage
			hordeSpwn.TakeDamage (townCentral.shortRangeDamage);
		}
	}
}
