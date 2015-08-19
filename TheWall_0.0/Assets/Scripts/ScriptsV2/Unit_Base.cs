using UnityEngine;
using System.Collections;

public class Unit_Base : MonoBehaviour {

	public float hp, defence, attack, shield, damage, rateOfAttack;

	public ResourceGrid resourceGrid;

	public GameObject targetUnit;

	public bool canAttack;

	// ENEMY UNITS ONLY:
	// recording the tile they are attacking so we don't have to check its stats with each individual unit
	TileData tileUnderAttack = null;
	float tileDefence, tileShield, tileHP;

	public void AttackTile(int x, int y, Enemy_MoveHandler enemyMove){
						// if no tile has been attacked OR this unit is attacking ANOTHER TILE, then we fill the tile and store it for calcs
		if (tileUnderAttack == null || tileUnderAttack != resourceGrid.tiles[x,y]) { 
			tileUnderAttack = resourceGrid.tiles[x,y]; 
			tileDefence = tileUnderAttack.def;
			tileShield = tileUnderAttack.shield;
			tileHP = tileUnderAttack.hp;
		} 
		Debug.Log ("TileHP: " + tileUnderAttack.hp);

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
			canAttack = true;
		} else {
			canAttack = false;
			enemyMove.isAttacking = false;
			enemyMove.CreateEnemyPath(); // start on path again!
			tileUnderAttack = null;
		}
//		Debug.Log("Tile: " + resourceGrid.tiles [x, y].tileType + " hp: " + resourceGrid.tiles [x, y].hp);
	}

	public void AttackOtherUnit(Unit_Base unit){
		if (unit != null) {
			float calc = (unit.defence - attack) - unit.shield;
			if (calc > 0) {
				// Apply full damage
				unit.TakeDamage (damage);  
			} else {
				// Apply just 1 damage
				unit.TakeDamage (1f);   
			}
		} else {
			// target is probably dead by now
			targetUnit = null;
		}
	
	}

	public void TakeDamage(float dmg){
		hp = hp - dmg;
		if (hp <= 0){
			Die();
		}
	}

	void Die(){
		Destroy (gameObject);
	}
}
