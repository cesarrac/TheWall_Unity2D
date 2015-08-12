using UnityEngine;
using System.Collections;

[System.Serializable]
public class Unit_Data {

	public string myName;
	public string myDescription;

	string[] monsterNames = new string[]{"Mort", "Sotted", "Grrax", "Lumped"};
	string[] humanNames = new string[]{"Tomas", "Fenix", "Dorean", "Tyrion"};

	public float hitPoints;
	public int attackRating;
	public int defenseRating;

	public Allegiance myAllegiance;
	public Quality myQuality;

	public Sprite mySprite;

	public int[] myStats = new int[3];

	public int rateOfFire;
	public float shortDamage, midDamage, longDamage;

	public float mySpeed;

	public enum Quality
	{
		elite,
		high,
		medium,
		low
	}
	public enum Allegiance{
		monster,
		captain
	}
	public Unit_Data(string description, Allegiance allegiance, Quality quality, int fireRate, float sDamage, float mDamage, float lDamage, float speed){
		myStats = initStats (quality);
		myName = GetName();
		myDescription = description;
		myAllegiance = allegiance;
		myQuality = quality;
		rateOfFire = fireRate;
		shortDamage = sDamage;
		midDamage = mDamage;
		longDamage = lDamage;

		mySprite = GetSprite ();

		hitPoints = myStats [0];
		attackRating = myStats [1];
		defenseRating = myStats [2];

		mySpeed = speed;
	}


	string GetName(){
		int namePick = Random.Range (0, 4);
		if (myName == null) {
			if (myAllegiance == Allegiance.monster) {
				myName = monsterNames [namePick];
			} else {
				myName = humanNames [namePick];
				
			}
		}

		return myName;
	}

	Sprite GetSprite(){

		mySprite = Resources.Load<Sprite>("Sprites/" + myAllegiance.ToString());
		return mySprite;
	}

	public int[] initStats(Quality quality){
		// Stats should be determined by QUALITY of Unit
		// Low , Medium, High, Elite
		// this would change the min and max of their stats (eg. Quality = elite; stats[0] = Random.Range(18, 32);
		// need to fill up HP[0], Attack Rating[1], and Defense Rating[2]
//		int[] stats = new int[3];
		switch (quality) 
		{
		case Quality.elite:
			myStats [0] = Random.Range (18, 42); 
			for (int x =1; x< myStats.Length; x++) {
				int randomStat = Random.Range(10, 20);
				myStats[x] = randomStat;
			}
			break;
		case Quality.high:
			myStats [0] = Random.Range (18, 25); 
			for (int x =1; x< myStats.Length; x++) {
				int randomStat = Random.Range(8, 12);
				myStats[x] = randomStat;
			}
			break;
		case Quality.medium:
			myStats [0] = Random.Range (10, 22); 
			for (int x =1; x< myStats.Length; x++) {
				int randomStat = Random.Range(4, 9);
				myStats[x] = randomStat;
			}
			break;
		case Quality.low:
			myStats [0] = Random.Range (9, 22); // added this to make sure HP is between 9-22
			for (int x =1; x< myStats.Length; x++) {
				int randomStat = Random.Range(3, 7);
				myStats[x] = randomStat;
			}
			break;
		default:
			myStats [0] = Random.Range (9, 22); // added this to make sure HP is between 9-22
			for (int x =1; x< myStats.Length; x++) {
				int randomStat = Random.Range(3, 7);
				myStats[x] = randomStat;
			}
			break;
		}
		
		
		
		return myStats;
	}
}
