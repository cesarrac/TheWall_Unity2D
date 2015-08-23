using UnityEngine;
using System.Collections;

public class Unit_Base : MonoBehaviour {

	public float hp, defence, attack, shield, damage, rateOfAttack;

	public ResourceGrid resourceGrid;

	public GameObject unitToPool;

	// ENEMY UNITS ONLY:
	// recording the tile they are attacking so we don't have to check its stats with each individual unit
	TileData tileUnderAttack = null;
	float tileDefence, tileShield, tileHP;
	public bool canAttackTile;

	public void AttackTile(int x, int y, Enemy_MoveHandler enemyMove){
		if (resourceGrid.tiles [x, y] != null) {
			// if no tile has been attacked OR this unit is attacking ANOTHER TILE, then we fill the tile and store it for calcs
			if (tileUnderAttack == null || tileUnderAttack != resourceGrid.tiles [x, y]) { 
				tileUnderAttack = resourceGrid.tiles [x, y]; 
				tileDefence = tileUnderAttack.def;
				tileShield = tileUnderAttack.shield;
				tileHP = tileUnderAttack.hp;
			} 
//		Debug.Log ("TileHP: " + tileUnderAttack.hp);

			// hit Calc =  (defense - attack) - armor
			// if hitCalc > 0, damage = full damage
			// if hitCalc <=0, damage = 1
			if (tileHP > 0) {
				float calc = (tileDefence - attack) - tileShield;
//			
				if (calc > 0) {
					// Apply full damage
					resourceGrid.DamageTile (x, y, damage);  
				} else {
					// Apply just 1 damage
					resourceGrid.DamageTile (x, y, 1f);  
				}
				canAttackTile = true;
			} else {
				canAttackTile = false;
				enemyMove.isAttacking = false;
//				enemyMove.GetPath (); // start on path again!
				tileUnderAttack = null;
			}
		} else {
			canAttackTile = false;
			enemyMove.isAttacking = false;
//			enemyMove.GetPath (); // start on path again!
		}
	}

	public void AttackOtherUnit(Unit_Base unit){

		if (unit.hp > 0) {
			float def = (unit.defence + unit.shield);
			if (attack > def){
				// Apply full damage
				unit.hp = unit.hp - damage;
				if (unit.hp <=0){
					Die(unit.gameObject);
				}
			}else{
				// hit for difference between def and attack
				float calc = def - attack;
				float damageCalc = damage - calc;
				unit.hp = unit.hp - Mathf.Clamp(damageCalc, 1f, damage); // always do MINIMUM 1 pt of damage
				if (unit.hp <=0){
					Die(unit.gameObject);
				} 
			}
		} else {
			// target is dead by now
			Die (unit.gameObject);
		}
	
	}

	void Die(GameObject target){
		unitToPool = target; // Pool the unit for later use (the attacker handles the pooling)
		target = null;
	}
}
