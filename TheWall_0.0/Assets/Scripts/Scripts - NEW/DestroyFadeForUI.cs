using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DestroyFadeForUI : MonoBehaviour {

	bool continueFade;
	
	Image img;
	

	void Start () {

		img = GetComponent<Image>();
		continueFade = true;
	}
	

	void Update () {
		if (continueFade) {
			StartCoroutine(FadeOut());
		}
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
		img.color = new Color (img.color.r, img.color.g, img.color.b, img.color.a - 0.6f);
		if (img.color.a <= 0) {
			Die ();
		} else {
			continueFade = true;
		}
	}

}
