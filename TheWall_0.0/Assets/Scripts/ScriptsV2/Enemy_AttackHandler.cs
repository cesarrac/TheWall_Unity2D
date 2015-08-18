using UnityEngine;
using System.Collections;

public class Enemy_AttackHandler : Unit_Base {

	public Enemy_MoveHandler moveHandler;
	public Enemy_SpawnHandler mySpawnHandler;

	public int targetTilePosX, targetTilePosY;

	// Use this for initialization
	void Start () {
		moveHandler = GetComponent<Enemy_MoveHandler> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (canAttack) {
			StartCoroutine(WaitToAttack());
		}
	}

	IEnumerator WaitToAttack(){
		canAttack = false;
		yield return new WaitForSeconds (rateOfAttack);
		if (targetUnit != null) {
			AttackOtherUnit (targetUnit.GetComponent<Unit_Base> ());
		} else {
			AttackTile(targetTilePosX, targetTilePosY, mySpawnHandler, moveHandler);
			Debug.Log("Attacking Tile!");
		}

	}
}
