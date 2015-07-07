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

	public int[] myStats;

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
	public Unit_Data(string description, Allegiance allegiance, Quality quality){
		myStats = initStats (quality);
		myName = GetName();
		myDescription = description;
		myAllegiance = allegiance;
		myQuality = quality;

		mySprite = GetSprite ();

		hitPoints = myStats [0];
		attackRating = myStats [1];
		defenseRating = myStats [2];
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
		int[] stats = new int[3];
		switch (quality) 
		{
		case Quality.elite:
			stats [0] = Random.Range (18, 42); 
			for (int x =1; x< stats.Length; x++) {
				int randomStat = Random.Range(10, 20);
				stats[x] = randomStat;
			}
			break;
		case Quality.high:
			stats [0] = Random.Range (18, 25); 
			for (int x =1; x< stats.Length; x++) {
				int randomStat = Random.Range(10, 20);
				stats[x] = randomStat;
			}
			break;
		case Quality.medium:
			stats [0] = Random.Range (10, 22); 
			for (int x =1; x< stats.Length; x++) {
				int randomStat = Random.Range(10, 20);
				stats[x] = randomStat;
			}
			break;
		case Quality.low:
			stats [0] = Random.Range (9, 22); // added this to make sure HP is between 9-22
			for (int x =1; x< stats.Length; x++) {
				int randomStat = Random.Range(2, 11);
				stats[x] = randomStat;
			}
			break;
		default:
			stats [0] = Random.Range (9, 22); // added this to make sure HP is between 9-22
			for (int x =1; x< stats.Length; x++) {
				int randomStat = Random.Range(2, 11);
				stats[x] = randomStat;
			}
			break;
		}
		
		
		
		return stats;
	}
}
