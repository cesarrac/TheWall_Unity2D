using UnityEngine;
using System.Collections;

public class DiscoverTile : MonoBehaviour {
	/// <summary>
	/// This script is called by Resource Grid when it needs to instantiate a tile.
	/// It gets the actual tile GameObject from Resource grid, spawns a grey tile and starts to dissolve it.
	/// The time it will take to disappear will be determined by mining time. (Which can be upgraded later)
	/// </summary>


	bool fading;

	SpriteRenderer sr;

	GameObject tileToSpawn;

	ResourceGrid resourceGrid;

	public ObjectPool objPool;

	void Start () {
		fading = true;
		sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (fading) {
			StartCoroutine(FadeOut());
		}
	}

	IEnumerator FadeOut(){
		// stop from calling again
		fading = false;
		//wait
		yield return new WaitForSeconds (0.6f);
		//fade a bit
		Fade ();
		
	}

	void Fade(){
		sr.color = new Color (sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.6f);
		if (sr.color.a <= 0) {
			Die ();
		} else {
			fading = true;
		}
	}
	void Die(){
		// before dying make sure to turn on the tileToSpawn's box collider so the player can interact with it
		if (tileToSpawn != null) {
//			BoxCollider2D coll = tileToSpawn.GetComponent<BoxCollider2D> ();
//			coll.enabled = true;
			Destroy (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}
	}

	public void TileToDiscover(string newTileName, int mapPosX, int mapPosY, Transform tileHolder, ResourceGrid grid,  TileData.Types tileType, GameObject playerCapital){		// this is called by Resource grid with the proper tile obj
		tileToSpawn = objPool.GetObjectForType (newTileName, false);
		if (tileToSpawn != null) {
			tileToSpawn.transform.position = transform.position;
			tileToSpawn.transform.parent = tileHolder;
			// Give the Tile position relative to the grid map
//		TileClick_Handler tc = tileToSpawn.GetComponent<TileClick_Handler> ();
//		tc.mapPosX = mapPosX;
//		tc.mapPosY = mapPosY;
//		tc.resourceGrid = grid;
//		tc.playerUnit = selectedUnit;

//		// IF TILE IS NOT A ROCK OR EMPTY, IT'S A BUILDING,
			// so it will have a Building Click Handler that needs its pos X and pos Y
			if (tileType != TileData.Types.rock && tileType != TileData.Types.empty) {
				Building_ClickHandler bClickHandler = tileToSpawn.GetComponent<Building_ClickHandler> ();
				bClickHandler.mapPosX = mapPosX;
				bClickHandler.mapPosY = mapPosY;
				bClickHandler.resourceGrid = grid;
				bClickHandler.objPool = objPool;

			} 
			if (tileType == TileData.Types.extractor) {
				// IF IT'S AN EXTRACTOR it will ALSO need the extractor variables
				Extractor extra = tileToSpawn.GetComponent<Extractor> ();
				extra.mapPosX = mapPosX;
				extra.mapPosY = mapPosY;
				extra.resourceGrid = grid;
				extra.playerResources = playerCapital.GetComponent<Player_ResourceManager> ();
			} 

	

			// ADD this tile to the Grid's spawnedTiles array
			grid.spawnedTiles [mapPosX, mapPosY] = tileToSpawn;
		}
	}
}
