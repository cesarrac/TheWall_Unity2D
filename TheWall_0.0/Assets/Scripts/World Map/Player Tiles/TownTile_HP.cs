using UnityEngine;
using System.Collections;

public class TownTile_HP : MonoBehaviour {

	public float tileHitPoints = 10f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DoDamage(float damage){
		tileHitPoints = tileHitPoints - damage;
		if (tileHitPoints <= 0) {
			KillTile();
		}
	}

	void KillTile(){
		Destroy (this.gameObject);
	}
}
