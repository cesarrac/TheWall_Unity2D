using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveUPandFade : MonoBehaviour {

	Color originalColor;

	Text text;

	public float duration = 0.5f;

	public float speed = 0.5f;

	void Start()
	{
		text = GetComponent<Text> ();
	}

	void Update () 
	{
	
		if (text == null) {
			text = GetComponent<Text> ();
		} else {

			// Move up each frame
			transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 0.5f), speed * Time.deltaTime);

			// Fade the alpha channel
			text.CrossFadeAlpha(0, duration, false);
		
		}


	
		
	}
}
