using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Enemy_ForceSpawn : MonoBehaviour {

	public class SpawnIndicatorInfo
	{

		private string _name;
		public string name { get {return _name;} set {_name = value;}}
		private int _count;
		public int count { get { return _count;} set { _count = value;}}
		private Sprite _sprite;
		public Sprite sprite {get { return _sprite; } set { _sprite = value; }}

		public void Init (string enemyName, int enemyCount, Sprite enemySprite)
		{
			name = enemyName;
			count = enemyCount;
			sprite = enemySprite;

		}

	}

	public SpawnIndicatorInfo spwnIndicator = new SpawnIndicatorInfo();

	[HideInInspector]
	public Enemy_WaveSpawner enemyWaveSpawner;

	[SerializeField]
	private Image img;
	[SerializeField]
	private Text txt_name, txt_count;

	
	public void SetIndicator(){
		img.sprite = spwnIndicator.sprite;
		txt_name.text = spwnIndicator.name;
		txt_count.text = spwnIndicator.count.ToString ();

	}

	public void ForceSpawn(){
		enemyWaveSpawner.ForceStartAttack ();
	}
}
