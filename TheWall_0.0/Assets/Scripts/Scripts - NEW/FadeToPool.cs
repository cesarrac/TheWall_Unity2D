using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeToPool : MonoBehaviour {
	public ObjectPool objPool;

	public float fadeTime;
	SpriteRenderer sr;

	public bool trueIfSprite;
	Image img;

	private IEnumerator _coroutine;

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

		if (sr != null || img != null) {
			_coroutine = FadeOut (fadeTime);
			StartCoroutine (_coroutine);
		}

	}



	IEnumerator FadeOut(float time){

		if (trueIfSprite) {
			//fade a sprite
			FadeSprite ();
		} else {
			// fade an UI image
			FadeImg();
		}

		yield return new WaitForSeconds (time);

		Die ();
	}



	void Die(){
		StopCoroutine (_coroutine);
		objPool.PoolObject (gameObject);
	}



	void FadeSprite(){
		sr.color = new Color (sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.6f);
	}



	void FadeImg(){
		img.color = new Color (img.color.r, img.color.g, img.color.b, img.color.a - 0.6f);
	}
}
