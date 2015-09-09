using UnityEngine;
using System.Collections;

public class Building_PositionHandler : MonoBehaviour {

	/// <summary>
	/// This is on a building that's about to be built (still following the mouse).
	/// Every time the mouse is on a different position we check what tile types are around this building.
	/// BUILDING RULES: (1) Building MUST be on an empty tile, (2) Building MUST have empty tiles 
	/// at position + 1 in all directions UNLESS, (3) its an extractor, then it MUST have ROCK 
	/// at any position +1 in all directions.
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

	public Building_UIHandler buildingUI;

	void Start () {
		sr = GetComponent<SpriteRenderer> ();
		halfColor = sr.color;
		trueColor = Color.green;
	}
	

	void Update () {
		if (followMouse) {
			FollowMouse();
		}
	}
	
	void FollowMouse(){
		m = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mX = Mathf.RoundToInt(m.x);
		mY = Mathf.RoundToInt(m.y);
		// Making sure that we are not trying to Build outside the BOUNDARIES of the GRID
		if (mX > resourceGrid.mapSizeX - 3){
			mX = resourceGrid.mapSizeX - 3;
		}
		if (mY > resourceGrid.mapSizeY -3){
			mY = resourceGrid.mapSizeY - 3;
		}
		if (mX < 3){
			mX = 3;
		}
		if (mY < 3){
			mY = 3;
		}
		// Move building with the mouse
		Vector3 followPos = new Vector3 (mX, mY);
		transform.position = followPos;


		if (CheckEmptyBeneath (mX, mY)) {
			
			if (tileType == TileData.Types.extractor) {
				if (CheckForResource (mX, mY + 1, true)) { // top
					sr.color = trueColor;
					canBuild = true;
				} else if (CheckForResource (mX, mY - 1, true)) { // bottom
					sr.color = trueColor;
					canBuild = true;
				} else if (CheckForResource (mX - 1, mY, true)) { // left
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckForResource (mX + 1, mY, true)) { // right
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckForResource (mX - 1, mY + 1, true)) { // top left
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckForResource (mX + 1, mY + 1, true)) { // top right
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckForResource (mX - 1, mY - 1, true)) { // bottom left
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckForResource (mX + 1, mY - 1, true)) { // bottom right
					canBuild = true;
					sr.color = trueColor;
				} else {				// NOT ON ROCK
					sr.color = halfColor;
					canBuild = false;
				}
			} else if (tileType == TileData.Types.desalt_s) { // THIS building is an EXTRACTOR so it needs to check for Rock
				if (CheckForResource (mX, mY + 1, false)) { // top
					sr.color = trueColor;
					canBuild = true;
				} else if (CheckForResource (mX, mY - 1, false)) { // bottom
					sr.color = trueColor;
					canBuild = true;
				} else if (CheckForResource (mX - 1, mY, false)) { // left
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckForResource (mX + 1, mY, false)) { // right
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckForResource (mX - 1, mY + 1, false)) { // top left
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckForResource (mX + 1, mY + 1, false)) { // top right
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckForResource (mX - 1, mY - 1, false)) { // bottom left
					canBuild = true;
					sr.color = trueColor;
				} else if (CheckForResource (mX + 1, mY - 1, false)) { // bottom right
					canBuild = true;
					sr.color = trueColor;
				} else {				// NOT ON Water
					sr.color = halfColor;
					canBuild = false;
				}
			} else{ // not an extractor or desalt pump
				if (CheckForEmptySides (mX, mY)) {
						// we are on a legal building position
					sr.color = trueColor;
					canBuild = true;
				} else {// we DO NOT have an empty tile on our sides or beneath
					sr.color = halfColor;
					canBuild = false;
				}
			}
		} else {				// NOT ON EMPTY
			sr.color = halfColor;
			canBuild = false;
		}


			// MAKE SURE WE HAVE ENOUGH ORE TO BUILD!
		if (resourceManager.ore >= currOreCost ) {
			canAfford = true;
		}
		// At this point we KNOW the mouse is NOT over a building or a rock, AND we have found rocks if extractor,
		if (Input.GetMouseButtonDown (0) && canBuild && canAfford) {			// So LEFT CLICK to BUILD!!

			// Subtract the cost
//			resourceManager.ChargeOreorWater("Ore", -currOreCost);
			resourceManager.ChangeResource("Ore", -currOreCost);

			// stop following and tell grid to swap this tile to this new building
			mapPosX = mX;
			mapPosY = mY;
			resourceGrid.SwapTileType (mX, mY, tileType);
			followMouse = false;

			// Right BEFORE pooling this object we tell Building UI that we are NOT currently building anymore
			this.buildingUI.currentlyBuilding = false;

			// Pool this object
			PoolObject (gameObject); // Pool this because resourceGrid just discovered the tile/building for us

		}
		if (Input.GetMouseButtonDown (1)) {
			// CANCEL THE BUILD
			PoolObject(gameObject);
		}
	}

	void PoolObject(GameObject objToPool){
		objPool.PoolObject (objToPool);
	}

	bool CheckForResource(int x, int y, bool trueIfOre){
		if (trueIfOre) {
			if (resourceGrid.tiles [x, y].tileType == TileData.Types.rock) {
				return true;
			} else {
				return false;
			}
		} else {
			if (resourceGrid.tiles [x, y].tileType == TileData.Types.water) {
				return true;
			} else {
				return false;
			}
		}
	}
	bool CheckForEmptySides(int x, int y){
		int top1 = y +1;
		int top2 = y +2;
		int bottom1 = y - 1;
		int bottom2 = y - 2;
		int left1 = x - 1;
		int left2 = x - 2;
		int right1 = x + 1;
		int right2 = x + 2;

		if (resourceGrid.tiles [x, top1].tileType == TileData.Types.empty && // top
			resourceGrid.tiles [x, top2].tileType == TileData.Types.empty &&
			resourceGrid.tiles [left1, top1].tileType == TileData.Types.empty && // top Left
			resourceGrid.tiles [left2, top2].tileType == TileData.Types.empty &&
			resourceGrid.tiles [right1, top1].tileType == TileData.Types.empty && // top Right
			resourceGrid.tiles [right2, top2].tileType == TileData.Types.empty &&
			resourceGrid.tiles [x, bottom1].tileType == TileData.Types.empty && // bottom
			resourceGrid.tiles [x, bottom2].tileType == TileData.Types.empty &&
			resourceGrid.tiles [left1, bottom1].tileType == TileData.Types.empty && // bottom left
			resourceGrid.tiles [left2, bottom2].tileType == TileData.Types.empty &&
			resourceGrid.tiles [right1, bottom1].tileType == TileData.Types.empty && // bottom right
			resourceGrid.tiles [right2, bottom2].tileType == TileData.Types.empty &&
			resourceGrid.tiles [left1, y].tileType == TileData.Types.empty && // left
			resourceGrid.tiles [left2, y].tileType == TileData.Types.empty &&
			resourceGrid.tiles [right1, y].tileType == TileData.Types.empty && // right
			resourceGrid.tiles [right2, y].tileType == TileData.Types.empty) {
			return true;
		} else {
			return false;
		}
	}

	bool CheckEmptyBeneath(int x, int y){
		if (resourceGrid.tiles [x, y].tileType == TileData.Types.empty) {
			return true;
		} else {
			return false;
		}
	}
}
