using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Unit_StatusIndicator : MonoBehaviour {

	[SerializeField]
	private RectTransform healthBarRect;

	[SerializeField]
	private Canvas canvas;

	private GameObject _damageText;

	[HideInInspector]
	public ObjectPool objPool;

	void Start(){

		if (GetComponentInParent<Enemy_AttackHandler> () != null) {

			objPool = GetComponentInParent<Enemy_AttackHandler> ().objPool;

		} else {

			objPool = GameObject.FindGameObjectWithTag("Pool").GetComponent<ObjectPool>();

		}

		if (healthBarRect == null) {
			Debug.Log("STATUS INDICATOR: No health bar referenced!!");
		}


	}

	public void SetHealth(float _cur, float _max, float _damage = 0)
	{
		float _value = _cur / _max;

		if (_value < 0.4f) {
			healthBarRect.gameObject.GetComponent<Image> ().color = Color.red;

		} else if (_value < 0.6f) {
			healthBarRect.gameObject.GetComponent<Image> ().color = Color.yellow;
		} else {
			healthBarRect.gameObject.GetComponent<Image> ().color = Color.green;
		}

		healthBarRect.localScale = new Vector3 (_value, healthBarRect.localScale.y, healthBarRect.localScale.z);

		if (_damage > 0)
			CreateDamageText (_damage);
	}

	public void CreateDamageText(float _damage, string damageTypeID = "Damage")
	{
//		Debug.Log ("Creating Damage text for " + _damage + " " + damageTypeID);
		Vector2 min = new Vector2 (0.5f, 0.5f);
		Vector2 max = new Vector2 (0.5f, 0.5f);
		Vector2 size = new Vector2 (113, 137);
		Vector3 scale = new Vector3(0.6f, 1, 1);
		Vector3 _scaleCalc = canvas.transform.localScale - scale;

		_damageText = objPool.GetObjectForType ("Damage Text", true);

		if (_damageText != null) {

		

			RectTransform rectTransform = _damageText.GetComponent<RectTransform> ();
			rectTransform.SetParent (canvas.transform, true);
			rectTransform.anchorMax = min;
			rectTransform.anchorMin = max;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.sizeDelta = size;
			rectTransform.localScale = -_scaleCalc;


			switch (damageTypeID) {
			case "Damage":
				_damageText.GetComponent<Text>().color = Color.red;
				_damageText.GetComponent<Text>().text = _damage.ToString ();
				break;
			case "Attack":
				_damageText.GetComponent<Text>().color = Color.magenta;
				_damageText.GetComponent<Text>().text = "-" + _damage.ToString ();
				break;
			case "Defence":
				_damageText.GetComponent<Text>().color = Color.blue;
				_damageText.GetComponent<Text>().text = "-" + _damage.ToString ();
				break;
			case "Speed":
				_damageText.GetComponent<Text>().color = Color.cyan;
				_damageText.GetComponent<Text>().text = "-" + _damage.ToString ();
				break;
			default:
				_damageText.GetComponent<Text>().color = Color.red;
				_damageText.GetComponent<Text>().text = _damage.ToString ();
				break;
			}
//			_damageText.GetComponent<Text>().text = "-" + _damage.ToString();

			// since I know I'm going to give the Text object the Easy Pool script, I might as well
			// fill up its object pool variable here
			if (_damageText.GetComponent<EasyPool> () != null)
				_damageText.GetComponent<EasyPool> ().objPool = objPool;
		} else {
			Debug.Log ("STATUS INDICATOR: Could NOT find Damage Text in Pool!");
		}
	}


}
