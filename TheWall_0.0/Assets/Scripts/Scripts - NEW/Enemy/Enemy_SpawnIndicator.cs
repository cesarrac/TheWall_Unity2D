using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Enemy_SpawnIndicator : MonoBehaviour {
	/// <summary>
	/// Shows incoming waves, up to three different types of units, and their respective counts	/// </summary>

	public class SpawnIndicatorInfo
	{

		public int enemyTypeCount;
		public Sprite enemySprite;

		public void Init(Sprite sprite, int count)
		{
			enemyTypeCount = count;
			enemySprite = sprite;
		}
		
	}

	// the three possible indicators for three different unit types
	public SpawnIndicatorInfo indicator1 = new SpawnIndicatorInfo ();
	public SpawnIndicatorInfo indicator2 = new SpawnIndicatorInfo ();
	public SpawnIndicatorInfo indicator3 = new SpawnIndicatorInfo();

	// the positions that Images & Text need to be placed in
	private Vector3 _imgPosLeft = new Vector3(-99, 52, 0.0F);
	private Vector3 _imgPosCenter = new Vector3(0, 52, 0.0F);
	private Vector3 _imgPosRight = new Vector3(99, 52, 0.0F);

	private Vector3 _txtPosLeft = new Vector3(-99, -15, 0.0F);
	private Vector3 _txtPosCenter = new Vector3(0, -15, 0.0F);
	private Vector3 _txtPosRight = new Vector3(99, -15, 0.0F);

	[SerializeField]
	private Image imgOne, imgTwo, imgThree;

	[SerializeField]
	private Text textOne, textTwo, textThree;

	[SerializeField]
	private Text x1, x2, x3; // the X that goes before the count

	[HideInInspector]
	public Enemy_WaveSpawner enemyWaveSpawner;




	public void InitOneTypeIndicator(Sprite sprite, int count)
	{
		indicator1.Init (sprite, count);
	}
	public void InitTwoTypeIndicator(Sprite sprite, int count, Sprite sprite2, int count2)
	{
		indicator2.Init (sprite, count);
		indicator3.Init (sprite2, count2);
	}
	public void InitThreeTypeIndicator(Sprite sprite, int count, Sprite sprite2, int count2, Sprite sprite3, int count3)
	{
		indicator1.Init (sprite, count);
		indicator2.Init (sprite2, count2);
		indicator3.Init (sprite3, count3);
	}

	public void SetIndicator1()
	{
		imgOne.sprite = indicator1.enemySprite;
		textOne.text = indicator1.enemyTypeCount.ToString();

		x2.gameObject.SetActive (false);
		imgTwo.gameObject.SetActive (false);
		textTwo.gameObject.SetActive (false);
		x3.gameObject.SetActive (false);
		imgThree.gameObject.SetActive (false);
		textThree.gameObject.SetActive (false);
	}

	public void SetIndicator2()
	{
		// using slots 2 and 3 to keep a balanced composition

		x1.gameObject.SetActive (false);
		imgOne.gameObject.SetActive (false);
		textOne.gameObject.SetActive (false);

		imgTwo.sprite = indicator2.enemySprite;
		textTwo.text = indicator2.enemyTypeCount.ToString();

		imgThree.sprite = indicator3.enemySprite;
		textThree.text = indicator3.enemyTypeCount.ToString();

	}

	public void SetIndicator3()
	{
		
		imgOne.sprite = indicator1.enemySprite;
		textOne.text = indicator1.enemyTypeCount.ToString();
		
		imgTwo.sprite = indicator2.enemySprite;
		textTwo.text = indicator2.enemyTypeCount.ToString();
		
		imgThree.sprite = indicator3.enemySprite;
		textThree.text = indicator3.enemyTypeCount.ToString();
		
	}

	public void ForceSpawn(){
		enemyWaveSpawner.ForceStartAttack ();
	}
}
