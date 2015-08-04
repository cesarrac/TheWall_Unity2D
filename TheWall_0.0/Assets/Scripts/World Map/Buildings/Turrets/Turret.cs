using UnityEngine;
using System.Collections;

public class Turret : Building {

					// *********** 	BASE TURRET (Automatic weapons that can be built on top of defenses)*******
	// Turrets don't add bonuses, they pick targets in their range every other turn, and shoot them until someone dies
	public GameObject bulletFab;
	public float fireRate, seekTime;
	public bool seekPlayer, canShoot;
	Transform myTransform;
	public LayerMask mask;
	public float damage;
	public Transform sightStart, sightEnd;
	public bool spotted = false;
	Quaternion myRot;

	void Start () {
		seekPlayer = true;
		myTransform = transform;
		myRot = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (seekPlayer) {
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
		Debug.Log ("Counting");

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
		float z = Mathf.Atan2((target.y - myTransform.position.y), (target.x - myTransform.position.x)) * Mathf.Rad2Deg - 90;		
		myTransform.eulerAngles = new Vector3 (0,0,z);
		GameObject bullet = Instantiate (bulletFab, sightStart.position, Quaternion.identity) as GameObject;
		bullet.transform.parent = sightEnd;
		Bullet bull = bullet.GetComponent<Bullet> ();
		bull.damage = damage;
		Debug.Log ("BAM!");
		canShoot = true;

	}

	IEnumerator WaitToShoot(){
		canShoot = false;
		yield return new WaitForSeconds(fireRate);
		PickTarget();
		
	}
}
