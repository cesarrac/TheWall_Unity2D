using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	Transform myTransform;
	public float bulletSpeed;
	Rigidbody2D rb;
	public float damage;
	int attackR;
	float dmg;

	public void Initialize(int aR, float baseDmg){
		attackR = aR;
		dmg = baseDmg;
	}
	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
		myTransform = transform;
	}
	
	void FixedUpdate(){
//				Vector3 myP = Camera.main.WorldToScreenPoint(myTransform.position);
//		var dir = (myTransform.up - myTransform.right).normalized;
		rb.AddForce (transform.parent.up * bulletSpeed);
	}


	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.tag == "Badge") {
			Horde horde = coll.gameObject.GetComponent<Horde> ();
			CalcDamage(horde);
		}
	}
	void CalcDamage(Horde target){
		int defense = target.hordeUnit.defenseRating;
		int dmgRoll = (Random.Range (0, attackR) + 1) - defense;
		Debug.Log (gameObject.name + " rolls: " + dmgRoll);
		if (dmgRoll > 0) {
			float damage = (float)dmgRoll;
			target.TakeDamage (damage);
			Destroy (gameObject);
		} else {
			print ("Miss!");
			Destroy (gameObject);
		}
		
	}
}
