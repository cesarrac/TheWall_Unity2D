using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Building_ClickHandler : MonoBehaviour {

	public int mapPosX;
	public int mapPosY;
	public Building_UIHandler buildingUIhandler;
	public ResourceGrid resourceGrid;

	// UI Handler feeds this when this is a new building so it may Swap Tiles
	public TileData.Types tileType;

	// get the bounds of this collider to know where to place the options panel
	BoxCollider2D myCollider;
	float vertExtents;

	[SerializeField]
	private Canvas buildingCanvas;

	[SerializeField]
	private GameObject buildingPanel;

	[SerializeField]
	private Image buildingStatus;


	void Start () {

		if (buildingCanvas == null) {
			Debug.Log("CLICK HANDLER: Building Canvas not set!");
		} 
		if (buildingPanel == null) {
			Debug.Log("CLICK HANDLER: Building Panel not set!");
		}

		if (buildingStatus == null) {
			Debug.Log("CLICK HANDLER: Building Status Image not set!");
		}

		if (buildingCanvas != null) {
			buildingCanvas.worldCamera = Camera.main;
		}

					// IF THIS BUILDING is spawned by the UI Handler, it won't need to make this search
		if (buildingUIhandler == null) {
			buildingUIhandler = GameObject.FindGameObjectWithTag ("UI").GetComponent<Building_UIHandler> ();
		}

		myCollider = GetComponent<BoxCollider2D> ();
		vertExtents = myCollider.bounds.extents.y;
	}

	void OnMouseUpAsButton(){
		Debug.Log("You clicked on " + gameObject.name);
		ActivateBuildingUI ();
	}

	public void ActivateBuildingUI(){
		Vector3 offset = new Vector3 (transform.position.x, transform.position.y + vertExtents);
//		if (!buildingUIhandler.currentlyBuilding)
//			buildingUIhandler.CreateOptionsButtons (offset, CheckTileType(mapPosX, mapPosY), mapPosX, mapPosY, buildingPanel, buildingCanvas);

		if (!buildingPanel.gameObject.activeSelf) {
			buildingPanel.gameObject.SetActive(true);
		}

	}

	public void ClosePanel(){
		if (buildingPanel.gameObject.activeSelf) {
			buildingPanel.gameObject.SetActive(false);
		}
	}

	public void Sell(){
		if (resourceGrid != null) {
			resourceGrid.SwapTileType(mapPosX, mapPosY, TileData.Types.empty);
		}
	}

	TileData.Types CheckTileType(int x, int y){
		TileData.Types type = resourceGrid.tiles [x, y].tileType;
		return type;
	}

	public void ChangeBuildingStatus(int change){
		if (change == 0) {

			buildingStatus.color = Color.red;

		} else if (change == 1) {
		
			buildingStatus.color = Color.green;
		}
	}
}
