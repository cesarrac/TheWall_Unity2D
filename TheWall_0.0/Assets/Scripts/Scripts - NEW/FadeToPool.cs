using UnityEngine;
using System.Collections;

public class FadeToPool : MonoBehaviour {
	public ObjectPool objPool;
	bool continueFade;
	
	SpriteRenderer sr;
	// Use this for initialization
	void Start () {
		if (objPool == null) {
			objPool = GameObject.FindGameObjectWithTag("Pool").GetComponent<ObjectPool>();
		}
		sr = GetComponent<SpriteRenderer> ();
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
		objPool.PoolObject (gameObject);
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
