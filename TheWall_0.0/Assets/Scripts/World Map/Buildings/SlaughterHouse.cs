using UnityEngine;
using System.Collections;

public class SlaughterHouse : MonoBehaviour {
	TownResources townRes;

	public int grainBoost;

	// Use this for initialization
	void Start () {
		townRes = GameObject.FindWithTag("Town_Central").GetComponent<TownResources> ();
		if (townRes != null) townRes.grain = townRes.grain + grainBoost;

	}
	

}
