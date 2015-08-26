using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeToPool : MonoBehaviour {
	public ObjectPool objPool;
	bool continueFade;
	public float fadeTime;
	SpriteRenderer sr;

	public bool trueIfSprite;
	Image img;

	// Use this for initialization
	void Start () {
		if (objPool == null) {
			objPool = GameObject.FindGameObjectWithTag("Pool").GetComponent<ObjectPool>();
		}
		if (trueIfSprite) {
			sr = GetComponent<SpriteRenderer> ();
		} else {
			img = GetComponent<Image>();
		}

		if (sr != null || img != null)
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
		yield return new WaitForSeconds (fadeTime);
		if (trueIfSprite) {
			//fade a sprite
			FadeSprite ();
		} else {
			// fade an UI image
			FadeImg();
		}

		
	}
	
	void Die(){
		objPool.PoolObject (gameObject);
	}
	
	void FadeSprite(){
		sr.color = new Color (sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.6f);
		if (sr.color.a <= 0) {
			Die ();
		} else {
			continueFade = true;
		}
	}

	void FadeImg(){
		img.color = new Color (img.color.r, img.color.g, img.color.b, img.color.a - 0.6f);
		if (img.color.a <= 0) {
			Die ();
		} else {
			continueFade = true;
		}
	}
}
