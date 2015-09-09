using UnityEngine;
using System.Collections;

public class Turret : Building {

					// *********** 	BASE TURRET (Automatic weapons that can be built on top of defenses)*******
// Turrets add Attack Rating and base Damage, they pick targets in their range every other turn, and shoot them until someone dies
	public GameObject bulletFab;
	public float fireRate, seekTime;
	public bool seekPlayer, canShoot;
	Transform myTransform;
	public LayerMask mask;

	public Transform sightStart, sightEnd;
	public bool spotted = false;
	Quaternion myRot;
	public int attackBoost;
	public float damageBoost;

	bool tileUnderAttk;

	public bool beingControlled;
	bool canManualShoot;

	void Start () {
		canManualShoot = true;

		intBonus1 = attackBoost;
		floatBonus1 = damageBoost;

		seekPlayer = true;


		myTransform = transform;
		myRot = transform.rotation;
		if (townTProps == null) {
			townTProps = GetComponentInParent<TownTile_Properties> ();
			ApplyBonus (attackBoost, damageBoost);

		}
	}

	void ApplyBonus(int attack, float dmg){
		townTProps.attackRating = townTProps.attackRating + attack;
		townTProps.baseDamage = townTProps.baseDamage + dmg;
	}

	void Update () {
//		tileUnderAttk = townTProps.beingAttacked;


		if (seekPlayer && !beingControlled) {
			StartCoroutine (WaitToSeek ());
		}
		if (canShoot) {
			StartCoroutine (WaitToShoot ());
		}
	}

	IEnumerator WaitToSeek(){
		// seek
		// wait
		// stop seeking


		Patrol ();
		seekPlayer = false;
		yield return new WaitForSeconds(seekTime);
		PickTarget ();

	}

	void PickTarget(){
//		Debug.DrawLine (sightStart.position, sightEnd.position, Color.red);
//		Vector2 myPos = new Vector2 (myTransform.position.x, myTransform.position.y);
//		Debug.DrawRay (myTransform.position, Vector3.forward, Color.red);
		spotted = Physics2D.Linecast (sightStart.position, sightEnd.position, mask.value);
		if (spotted) {
			Shoot (target: sightEnd.position);
			seekPlayer = false;
		} else {
			seekPlayer = true;
			canShoot = false;
		}
//		RaycastHit2D hit = Physics2D.Raycast (sightStart.position, Vector3.up, 3, mask.value);
//		if (hit.collider != null) {
//			Shoot(hit.collider.gameObject.transform.position);
//			seekPlayer = false;
//		}
	}


	void Patrol(){ // this just makes the turret rotate 45 degrees
//		Vector3 target = new Vector3 (0, 0, 12);
//		float z = Mathf.Atan2((target.y - myTransform.position.y), (target.x - myTransform.position.x)) * Mathf.Rad2Deg - 90;		
//		myTransform.eulerAngles = new Vector3 (0,0,myTransform.eulerAngles.z + 25f);
		// euler angles affect z , x and y in that order
		myTransform.Rotate (Vector3.forward * 360 * Time.deltaTime);


	}

	void Shoot(Vector3 target){
		if (!beingControlled) { // IF NOT BEING CONTROLLED, NEED TO ROTATE TO TARGET
			float z = Mathf.Atan2((target.y - myTransform.position.y), (target.x - myTransform.position.x)) * Mathf.Rad2Deg - 90;		
			//		myTransform.eulerAngles = new Vector3 (0,0,z);
			transform.rotation = Quaternion.AngleAxis(z, Vector3.forward);
			canShoot = true;

		}
		GameObject bullet = Instantiate (bulletFab, sightStart.position, Quaternion.identity) as GameObject;
		bullet.transform.parent = sightEnd;
		Bullet bull = bullet.GetComponent<Bullet> ();
									// GIVE BULLET ABILITY TO CALCULATE DAMAGE
		bull.Initialize (townTProps.attackRating, townTProps.baseDamage);
		canShoot = true;
		canManualShoot = false;


	}

	IEnumerator WaitToShoot(){

		canShoot = false;

		yield return new WaitForSeconds(fireRate);
		if (beingControlled) {
			canManualShoot = true;		
		} else {
			PickTarget();
		}

	}

	public void ManualControl(Vector3 target){

		beingControlled = true;
		seekPlayer = false;


	
		if (Input.GetMouseButtonUp (0)) {
	
			if (canManualShoot) {
				Shoot (sightEnd.transform.position);
			}
		} else if (Input.GetMouseButton (0)) {
			float z = Mathf.Atan2((target.y - myTransform.position.y), (target.x - myTransform.position.x)) * Mathf.Rad2Deg - 90;		
			myTransform.eulerAngles = new Vector3 (0,0,z);
		} 
	}
}
