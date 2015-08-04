using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {

	// This script can both INSTANTLY DESTROY a gameObject after time is up 
	// 			OR
	// FADE a gameObject with a Sprite Renderer out over time before destroying them

	// option for instant destroy or fade
	public bool checkTrueForInstantDestroy;

	public float timeToDestroy;

	bool continueFade;

	SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		if (checkTrueForInstantDestroy) {
			StartCoroutine (DestructionTimer ()); // only needs to be called once
		} else {
			sr = GetComponent<SpriteRenderer>();
			continueFade = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (continueFade) {
			StartCoroutine(FadeOut());
		}
	}

	IEnumerator DestructionTimer(){
		// waits determined time
		yield return new WaitForSeconds(timeToDestroy);
		Die (); // and destroys this object

	}

	IEnumerator FadeOut(){
		// stop from calling again
		continueFade = false;
		//wait
		yield return new WaitForSeconds (0.6f);
		//fade a bit
		Fade ();

	}

	void Die(){
		Destroy (this.gameObject);
	}

	void Fade(){
		sr.color = new Color (sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.6f);
		if (sr.color.a <= 0) {
			Die ();
		} else {
			continueFade = true;
		}
	}

}
