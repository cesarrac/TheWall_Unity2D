using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour {

	public Texture2D fogTex;
	public Color clearColor;
	public Color grayColor;
	public Color blackColor;

	public Color[] fogColors;

	public int fogWidth, fogHeight;

	public GameObject clearTile, grayTile, blackTile;

	public Transform fogHolder;

	// Use this for initialization
	void Start () {
		fogWidth = fogTex.width;
		fogHeight = fogTex.height;

		LoadFog ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LoadFog(){
		fogColors = new Color[fogWidth * fogHeight];
		fogColors = fogTex.GetPixels ();

		for (int y = 0; y < fogHeight; y++){
			for (int x = 0; x < fogWidth; x++) {
				if (fogColors[x + y * fogWidth] == clearColor){
					GameObject spwnClear = Instantiate(clearTile, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
					if (spwnClear != null){ spwnClear.transform.parent = fogHolder;}
				}
				if (fogColors[x + y * fogWidth] == grayColor){
					GameObject spwnGray = Instantiate(grayTile, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
					if (spwnGray != null){ spwnGray.transform.parent = fogHolder;}
				}
				if (fogColors[x + y * fogWidth] == blackColor){
					GameObject spwnBlack = Instantiate(blackTile, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
					if (spwnBlack != null){ spwnBlack.transform.parent = fogHolder;}
				}
			}
		}
	}
}
