using UnityEngine;
using System.Collections;

public class Enemy_AttackHandler : Unit_Base {

	public Enemy_MoveHandler moveHandler;


	public int targetTilePosX, targetTilePosY;


	// Use this for initialization
	void Start () {

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
			if(moveHandler != null){
				AttackTile(targetTilePosX, targetTilePosY, moveHandler);
				Debug.Log("Attacking Tile!");
			}
		}

	}
}
