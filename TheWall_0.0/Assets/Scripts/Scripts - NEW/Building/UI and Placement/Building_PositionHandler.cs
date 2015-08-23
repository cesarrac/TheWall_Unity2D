using UnityEngine;
using System.Collections;

public class Building_PositionHandler : MonoBehaviour {

	/// <summary>
	/// This is on a building that's about to be built (still following the mouse).
	/// Every time the mouse is on a different position we check what tile types are around this building,
	/// if they match this building's gathering resource then the sprite Alpha changes to full and player
	/// can click to build.
	/// </summary>

	// the UI_Handler will feed it the Resource Grid, 
	
	public int mapPosX;
	public int mapPosY;

	public ResourceGrid resourceGrid;
	public bool followMouse;

	public TileData.Types tileType;

	SpriteRenderer sr; // to handle the alpha change
	Color halfColor, trueColor;

	Vector3 m, lastM;
	int mX;
	int mY;

	bool canBuild, canAfford; // turns true when building is in proper position

	public Player_ResourceManager resourceManager;
	public int currOreCost; // this will charge the resources manager with the cost sent from UI handler

	public ObjectPool objPool;

	public FoodCost_Manager foodCostMan;

	void Start () {
		sr = GetComponent<SpriteRenderer> ();
		halfColor = sr.color;
		trueColor = new Color (halfColor.r, halfColor.g, halfColor.b, 255);
		if (foodCostMan == null) {
			foodCostMan = GameObject.FindGameObjectWithTag("Capital").GetComponent<FoodCost_Manager>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (followMouse) {
			FollowMouse();
		}
	}
	
	void FollowMouse(){
		m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mX = Mathf.RoundToInt(m.x);
		mY = Mathf.RoundToInt(m.y);
		// Making sure that we are not trying to path outside the BOUNDARIES of the GRID
		if (mX > resourceGrid.mapSizeX - 1){
			mX = resourceGrid.mapSizeX - 1;
		}
		if (mY > resourceGrid.mapSizeY -1){
			mY = resourceGrid.mapSizeY - 1;
		}
		if (mX < 0){
			mX = 0;
		}
		if (mY < 0){
			mY = 0;
		}
		// Move building with the mouse
		Vector3 followPos = new Vector3 (mX, mY);
		transform.position = followPos;

		// IF THIS BUILDING IS AN EXTRACTOR, check the tiles around the building
		if (tileType == TileData.Types.extractor) {
			if (CheckForEmpty (mX, mY)) { // making it so you DONT perform the rock check if you are not on an empty tile
				if (CheckTileType (mX, mY + 1)) { // top
					sr.color = trueColor;
					canBuild = true;
				} else if (CheckTileType (mX, mY - 1)) { // bottom
					sr.color = trueColor;
					canBuild = true;
				} else if (CheckTileType (mX - 1, mY)) { // left
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckTileType (mX + 1, mY)) { // right
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckTileType (mX - 1, mY + 1)) { // top left
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckTileType (mX + 1, mY + 1)) { // top right
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckTileType (mX - 1, mY - 1)) { // bottom left
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckTileType (mX + 1, mY - 1)) { // bottom right
					canBuild = true;
					sr.color = trueColor;
				} else {
					sr.color = halfColor;
					canBuild = false;
				}
			} else { // we are NOT on an empty tile
				sr.color = halfColor;
				canBuild = false;
			}
		} else {
			// THIS TILE IS NOT AN EXTRACTOR, so just check for empty
			if (CheckForEmpty (mX, mY)){
				canBuild = true;
			}
		}
			// MAKE SURE WE HAVE ENOUGH ORE TO BUILD!
		if (resourceManager.ore >= currOreCost ) {
			canAfford = true;
		}
		// At this point we KNOW the mouse is NOT over a building or a rock, AND we have found rocks if extractor,
		if (Input.GetMouseButtonDown (0) && canBuild && canAfford) {			// So LEFT CLICK to BUILD!!
			// Subtract the cost
			resourceManager.ChangeResource("Ore", -currOreCost);
			mapPosX = mX;
			mapPosY = mY;
			// stop following and tell grid to swap this tile to this new building
			resourceGrid.SwapTileType (mX, mY, tileType);
			followMouse = false;
			PoolObject (gameObject); // Pool this because resourceGrid just discovered it for us

		}
	}

	void PoolObject(GameObject objToPool){
		objPool.PoolObject (objToPool);
	}

	bool CheckTileType(int x, int y){
		if (x < resourceGrid.mapSizeX && y < resourceGrid.mapSizeY && x > 0 && y > 0) {
			if (resourceGrid.tiles [x, y].tileType == TileData.Types.rock) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}
	bool CheckForEmpty(int x, int y){
		if (resourceGrid.tiles [x, y].tileType == TileData.Types.empty) {
			return true;
		} else {
			return false;
		}
	}
}
