using UnityEngine;
using System.Collections;

public class NoFadePool : MonoBehaviour {
	public ObjectPool objPool;

	private IEnumerator _coroutine;
	// Use this for initialization
	void Start () {
		if (objPool == null) {
			objPool = GameObject.FindGameObjectWithTag("Pool").GetComponent<ObjectPool>();
		}
		_coroutine = WaitToPool (1.2f);
		StartCoroutine(_coroutine);
	}
	
	IEnumerator WaitToPool(float time){
		yield return new WaitForSeconds(time);
		objPool.PoolObject (this.gameObject);

	}
}
