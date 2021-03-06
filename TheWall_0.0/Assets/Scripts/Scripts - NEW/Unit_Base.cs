﻿using UnityEngine;
using System.Collections;

public class Unit_Base : MonoBehaviour {

	[System.Serializable]
	public class Stats{
		public float maxHP, startDefence, startAttack, startShield, startRate, startDamage, startSpecialDmg;
		private float _hitPoints, _defence, _attack, _shield, _damage, _specialDamage, _rateOfAttack;
	
		public float curHP { get { return _hitPoints; } set { _hitPoints = Mathf.Clamp (value, 0f, maxHP); } }
		public float curDefence {get {return _defence;} set { _defence = Mathf.Clamp (value, 0f, 100f); }}
		public float curAttack {get {return _attack;} set { _attack = Mathf.Clamp (value, 0f, 100f); }}
		public float curShield {get {return _shield;} set { _shield = Mathf.Clamp (value, 0f, 100f); }}
		public float curRateOfAttk {get {return _rateOfAttack;} set { _rateOfAttack = Mathf.Clamp (value, 0f, 5f); }}
		public float curDamage {get {return _damage;} set { _damage = Mathf.Clamp (value, 0f, 100f); }}
		public float curSPdamage { get {return _specialDamage;} set {_specialDamage = Mathf.Clamp (value, 0f, 100f);}}


		public void Init(){
			curHP = maxHP;
			curDefence = startDefence;
			curAttack = startAttack;
			curShield = startShield;
			curRateOfAttk = startRate;
			curDamage = startDamage;
			curSPdamage = startSpecialDmg;
		}
	}

	public Stats stats = new Stats ();

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
	private Unit_StatusIndicator _statusIndi;


	public Unit_StatusIndicator statusIndicator { get {return _statusIndi;} set{_statusIndi = value;}}



	void Start(){

		if (_statusIndi != null) {
			_statusIndi.SetHealth(stats.curHP, stats.maxHP);
		}
	}

	public void InitTileStats(int x, int y){
//		Debug.Log ("BASE: Tile stats initialized!");
		resourceGrid.tiles [x, y].hp = stats.curHP;
		resourceGrid.tiles [x, y].def = stats.curDefence;
		resourceGrid.tiles [x, y].attk = stats.curAttack;
		resourceGrid.tiles [x, y].shield = stats.curShield;
	}

	public bool AttackTile(int x, int y, Enemy_MoveHandler enemyMove){
		if (resourceGrid.tiles [x, y] != null) {
			// if no tile has been attacked OR this unit is attacking ANOTHER TILE, then we fill the tile and store it for calcs
			if (tileUnderAttack == null || tileUnderAttack != resourceGrid.tiles [x, y]) { 
				tileUnderAttack = resourceGrid.tiles [x, y]; 
				tileDefence = tileUnderAttack.def;
				tileShield = tileUnderAttack.shield;
				tileHP = tileUnderAttack.hp;
			} 
		Debug.Log ("TileHP: " + tileUnderAttack.hp);

			if (tileHP > 0) {
				float calc = (tileDefence - stats.curAttack) - tileShield;
//			
				if (calc > 0) {
					// Apply full damage
					resourceGrid.DamageTile (x, y, stats.curDamage);  
				} else {
					// Apply just 1 damage
					resourceGrid.DamageTile (x, y, 1f);  
				}
				canAttackTile = true;
				return true;

			} else {
				canAttackTile = false;
				enemyMove.isAttacking = false;
//				enemyMove.GetPath (); // start on path again!
				tileUnderAttack = null;
				return false;
			}
		} else {
			canAttackTile = false;
			enemyMove.isAttacking = false;
			return false;
//			enemyMove.GetPath (); // start on path again!
		}
	}

	public void AttackOtherUnit(Unit_Base unit){

		if (unit.stats.curHP >= 1f) {
			float def = (unit.stats.curDefence + unit.stats.curShield);

			if (stats.curAttack > def){
//				Debug.Log("Attacking " + unit.name + " DEF: " + def + " ATTK: " + stats.curAttack);

				// Apply full damage
				TakeDamage(unit, stats.curDamage);

//				unit.hp = unit.hp - damage;
//
//				if (unit.hp <=0){
//					Die(unit.gameObject);
//				}
			}else{
				// hit for difference between def and attack
				float calc = def - stats.curAttack;
				float damageCalc = stats.curDamage - calc;


				// always do MINIMUM 1 pt of damage
				float clampedDamage = Mathf.Clamp(damageCalc, 1f, stats.curDamage);

//				Debug.Log("Can't beat " + unit.name + "'s Defence, so I hit for " + clampedDamage);

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

	// ONLY USED BY KAMIKAZE UNITS ATTACKING PLAYER UNITS
	public void SpecialAttackOtherUnit(Unit_Base unit){
		if (unit.stats.curHP >= 1) {
			TakeDamage (unit, stats.curSPdamage);
		} else {
			Die (unit.gameObject);
		}
	}

	void TakeDamage(Unit_Base unit, float damage){

		unit.stats.curHP = unit.stats.curHP - damage;
		if (unit.stats.curHP < 1f) {
			Debug.Log ("UNIT BASE: Killing target!");
			Die (unit.gameObject);
		} else {
			if (unit.statusIndicator != null) {
				unit.statusIndicator.SetHealth(unit.stats.curHP, unit.stats.maxHP, damage);
			}
		}

		// pop up the damage
//		unit.dmgPopUp.PopUpDamage (damage);
	}

	public void TakeDebuff(float debuffAmmnt, string statID){
		statusIndicator.CreateDamageText (debuffAmmnt, statID);
	}

	void Die(GameObject target){
		unitToPool = target; // Pool the unit for later use (the attacker handles the pooling)
		target = null;
	}
}
