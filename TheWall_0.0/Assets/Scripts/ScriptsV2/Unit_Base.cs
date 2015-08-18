using UnityEngine;
using System.Collections;

public class Unit_Base : MonoBehaviour {

	public float hp, defence, attack, shield, damage, rateOfAttack;

	public ResourceGrid resourceGrid;

	public GameObject targetUnit;

	public bool canAttack;

//	public Unit_Base(float hitPoints, float defenceRating, float attackRating, float shieldRating, Sprite sprite){
//		hp = hitPoints;
//		defence = defenceRating;
//		attack = attackRating;
//		shield = shieldRating;
//		unitSprite = sprite;
//	}

	public void AttackTile(int x, int y, Enemy_SpawnHandler spawnHandler, Enemy_MoveHandler enemyMove){
		// hit Calc =  (defense - attack) - armor
		// if hitCalc > 0, damage = full damage
		// if hitCalc <=0, damage = 1
		if (resourceGrid.tiles [x, y].hp > 0) {
			float calc = (resourceGrid.tiles [x, y].def - attack) - resourceGrid.tiles [x, y].shield;
			Debug.Log("Hit for " + calc);
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
			enemyMove.currentPath = spawnHandler.currentPath; // start on path again!

		}
		Debug.Log("Tile: " + resourceGrid.tiles [x, y].tileType + " hp: " + resourceGrid.tiles [x, y].hp);
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
