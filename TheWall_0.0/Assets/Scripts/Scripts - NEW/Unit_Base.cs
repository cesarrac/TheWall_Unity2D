using UnityEngine;
using System.Collections;

public class Unit_Base : MonoBehaviour {

	public float maxHP, defence, attack, shield, damage, rateOfAttack;

	private float _curHP;
	public float curHP { get { return _curHP; } set { _curHP = Mathf.Clamp (value, 0f, maxHP); } }

	public ResourceGrid resourceGrid;

	public GameObject unitToPool;

	// ENEMY UNITS ONLY:
	// recording the tile they are attacking so we don't have to check its stats with each individual unit
	TileData tileUnderAttack = null;
	float tileDefence, tileShield, tileHP;
	public bool canAttackTile;

//	public Damage_PopUp dmgPopUp; 

	[Header("Optional: ")]
	[SerializeField]
	private Unit_StatusIndicator statusIndicator;

	void Awake(){
		// finds its own dmgPopUp
//		dmgPopUp = GetComponent<Damage_PopUp> ();

		curHP = maxHP;
	}

	void Start(){
		if (statusIndicator != null) {
			statusIndicator.SetHealth(curHP, maxHP);
		}
	}



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

		if (unit.curHP > 0) {
			float def = (unit.defence + unit.shield);

			if (attack > def){
				Debug.Log("Attacking " + unit.name + " DEF: " + def + " ATTK: " + attack);

				// Apply full damage
				TakeDamage(unit, damage);

//				unit.hp = unit.hp - damage;
//
//				if (unit.hp <=0){
//					Die(unit.gameObject);
//				}
			}else{
				// hit for difference between def and attack
				float calc = def - attack;
				float damageCalc = damage - calc;
				Debug.Log("Can't beat " + unit.name + "'s Attack, so I hit for " + damageCalc);

				// always do MINIMUM 1 pt of damage
				float clampedDamage = Mathf.Clamp(damageCalc, 1f, damage);

				TakeDamage (unit, clampedDamage);
//				unit.hp = unit.hp - Mathf.Clamp(damageCalc, 1f, damage); 
//				if (unit.hp <=0){
//					Die(unit.gameObject);
//				} 
			}
		} else {
			// target is dead by now
			Die (unit.gameObject);
		}
	
	}

	void TakeDamage(Unit_Base unit, float damage){

		unit.curHP = unit.curHP - damage;
		if (unit.curHP <= 0f) {
			Die (unit.gameObject);
		} else {
			if (unit.statusIndicator != null) {
				unit.statusIndicator.SetHealth(unit.curHP, unit.maxHP, damage);
			}
		}

		// pop up the damage
//		unit.dmgPopUp.PopUpDamage (damage);


	}

	void Die(GameObject target){
		unitToPool = target; // Pool the unit for later use (the attacker handles the pooling)
		target = null;
	}
}
