using UnityEngine;
using System.Collections;

[System.Serializable]
public class HordeSpawner : MonoBehaviour {
	//Spawns a Horde at the location of this capital, with specified stats, and sticks it to a holder transform

	Transform myTransform;

	public GameObject hordeFab;

	public Transform hordeHolder;

	public float timeToSpawn;

	bool spawning;

	public float myHitPoints;

	// available level types to determine what quality of units to spawn
	public enum HordeLevel{
		Low,
		Medium,
		High,
		Elite
	}

	public HordeLevel myLevel; // stored level of this spawn point

	// Sprite Renderer to enable and disable if player is close or not
	SpriteRenderer sr;

	//object to spawn when dead
	public GameObject deadSpawner;

	// mask to avoid anything but tiles
	public LayerMask mask;

	BoxCollider2D currColl;

	void Start () {
		myTransform = transform;
		SpawnHorde ();
		sr = GetComponent<SpriteRenderer> ();
		sr.enabled = false;

		TurnOffColliderUnderneath ();
	}

	void Update(){
		if (spawning) {
			StartCoroutine(CountToSpawn());
		}
//		TurnOffColliderUnderneath ();
	}
	IEnumerator CountToSpawn(){
		// stop counting
		spawning = false;
		// wait
		yield return new WaitForSeconds (timeToSpawn);
		// spawn
		SpawnHorde ();
	}

	// need a way to turn off the resource tile under a Horde so it doens't interfere with our Raycast
	void TurnOffColliderUnderneath(){
		RaycastHit2D hit = Physics2D.Linecast (new Vector2 (myTransform.position.x, myTransform.position.y), Vector2.up, mask.value);
		if (hit.collider != null) {
			if (hit.collider.CompareTag("Tile") || hit.collider.CompareTag("Empty Tile") ){
				BoxCollider2D boxColl = hit.collider.gameObject.GetComponent<BoxCollider2D>();

				if (boxColl.enabled){
					boxColl.enabled = false;
				}
			}
		}
	}

	// need to check for collisions with a town tile in order to turn on my renderer and turn off colliders underneath
	void OnTriggerEnter2D(Collider2D coll){
		if (coll.gameObject.tag == "Map_Manager") {
			sr.enabled = true;
//			TurnOffColliderUnderneath();
		}
		if (coll.gameObject.tag == "Tile") {
			TurnOffColliderUnderneath();
		}
	}
	void OnTriggerExit2D(Collider2D coll){
		if (coll.gameObject.tag == "Map_Manager") {
			sr.enabled = false;
		}
	}



	public void SpawnHorde(){
		Vector3 hordePos = new Vector3 (myTransform.position.x, myTransform.position.y, 0);
		GameObject horde = Instantiate (hordeFab, hordePos, Quaternion.identity) as GameObject;
		horde.transform.parent = hordeHolder;
		Horde hordeScript = horde.GetComponent<Horde> ();
		spawning = true;
		// check what level this spawn is, and spawn the right kind of horde
		switch (myLevel) {
		case HordeLevel.Low:
			hordeScript.hordeMembers.Add(new Unit_Data(description: "Some description.", allegiance: Unit_Data.Allegiance.monster, quality: Unit_Data.Quality.low, fireRate: 1, sDamage: 2f, mDamage: 3f, lDamage: 5f));
			break;
		case HordeLevel.Medium:
			hordeScript.hordeMembers.Add(new Unit_Data(description: "Some description.", allegiance: Unit_Data.Allegiance.monster, quality: Unit_Data.Quality.medium, fireRate: 1, sDamage: 2f, mDamage: 3f, lDamage: 5f));
			break;
		case HordeLevel.High:
			hordeScript.hordeMembers.Add(new Unit_Data(description: "Some description.", allegiance: Unit_Data.Allegiance.monster, quality: Unit_Data.Quality.high, fireRate: 1, sDamage: 2f, mDamage: 3f, lDamage: 5f));
			break;
		case HordeLevel.Elite:
			hordeScript.hordeMembers.Add(new Unit_Data(description: "Some description.", allegiance: Unit_Data.Allegiance.monster, quality: Unit_Data.Quality.elite, fireRate: 1, sDamage: 2f, mDamage: 3f, lDamage: 5f));
			break;
		default:
			// default to low
			hordeScript.hordeMembers.Add(new Unit_Data(description: "Some description.", allegiance: Unit_Data.Allegiance.monster, quality: Unit_Data.Quality.low, fireRate: 1, sDamage: 2f, mDamage: 3f, lDamage: 5f));
			break;
		}
	}

	public void TakeDamage(float damage){
		myHitPoints = myHitPoints - damage;
		if (myHitPoints <= 0) {
			GameObject deadSpwn = Instantiate(deadSpawner, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Quaternion.identity) as GameObject;

			Destroy(this.gameObject);
		}
	}
}
