using UnityEngine;
using System.Collections;

public class EasyPool : MonoBehaviour {


	public ObjectPool objPool;

	public float timeBeforePool;

	private float poolCountdown;

	private bool haveStartedToCount;

	enum State { INIT, COUNTING, POOLED };

	private State state = State.POOLED;

	void Update () {

		if (objPool == null)
			objPool = GameObject.FindGameObjectWithTag ("Pool").GetComponent<ObjectPool> ();

		if (state == State.POOLED)
			poolCountdown = timeBeforePool;


		if (poolCountdown <= 0 && state == State.COUNTING) {

			// Pool object
			objPool.PoolObject (this.gameObject);
			state = State.POOLED;


		} else {
			state = State.COUNTING;
			poolCountdown -= Time.deltaTime;
		}
		
	}
}
