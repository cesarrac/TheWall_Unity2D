using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Unit_StatusIndicator : MonoBehaviour {

	[SerializeField]
	private RectTransform healthBarRect;

	[SerializeField]
	private Text damageText;

//	private IEnumerator _coroutine;

	void Start(){
		if (healthBarRect == null) {
			Debug.Log("STATUS INDICATOR: No health bar referenced!!");
		}

//		_coroutine = EmptyText (0.2f);
//		StartCoroutine (_coroutine);
	}

	public void SetHealth(float _cur, float _max, float _ammt = 0)
	{
		float _value = _cur / _max;

		if (_value < _max * 0.2f) {
			healthBarRect.gameObject.GetComponent<Image> ().color = Color.red;

		} else if (_value < _max * 0.5f) {
			healthBarRect.gameObject.GetComponent<Image> ().color = Color.yellow;
		} else {
			healthBarRect.gameObject.GetComponent<Image> ().color = Color.green;
		}

		healthBarRect.localScale = new Vector3 (_value, healthBarRect.localScale.y, healthBarRect.localScale.z);

		if (_ammt > 0) {
			damageText.text = _ammt.ToString ();
		} else {
			damageText.text = string.Empty;
		}
		
	}

//	IEnumerator EmptyText(float time){
//
//		yield return new WaitForSeconds (time);
//		Debug.Log ("STATUS INDICATOR: setting text to empty!");
//		if (damageText.gameObject.activeSelf) {
//			damageText.gameObject.SetActive(false);
//		}
////		damageText.text = string.Empty;
////		StopCoroutine (_coroutine);
//	}
}
