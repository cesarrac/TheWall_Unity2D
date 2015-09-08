using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Enemy_SpawnIndicator : MonoBehaviour {
	/// <summary>
	/// Shows incoming waves, up to three different types of units, and their respective counts	/// </summary>

	public class SpawnIndicatorInfo
	{
		public int enemyTypeCount;

		public int memberCount;
		public string nameOne, nameTwo, nameThree;
		public Sprite spriteOne, spriteTwo, spriteThree;

		// for just One type of unit
		public void Init (int typeCount, string nOne, Sprite spOne)
		{
			enemyTypeCount = typeCount;
			nameOne = nOne;
			spriteOne = spOne;

		}

		// two types
		public void Init(int typeCount, string nOne, Sprite spOne, string nTwo, Sprite spTwo)
		{
			enemyTypeCount = typeCount;
			nameOne = nOne;
			spriteOne = spOne;
			nameTwo = nTwo;
			spriteTwo = spTwo;
		}

		// three types
		public void Init(int typeCount, string nOne, Sprite spOne, string nTwo, Sprite spTwo,  string nThree, Sprite spThree)
		{
			enemyTypeCount = typeCount;
			nameOne = nOne;
			spriteOne = spOne;
			nameTwo = nTwo;
			spriteTwo = spTwo;
			nameThree = nThree;
			spriteThree = spThree;
		}
		
	}


	[SerializeField]
	private Image imgOne, imgTwo, imgThree;

	void CreateIndicator(int unitTypeCount){

	}
}
