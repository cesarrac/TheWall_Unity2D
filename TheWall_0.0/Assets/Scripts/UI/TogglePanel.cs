using UnityEngine;
using System.Collections;

public class TogglePanel : MonoBehaviour {

	public void TogglePanelButton (GameObject panel) {
		panel.SetActive (!panel.activeSelf);
	}
}
