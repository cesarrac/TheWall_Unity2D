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
			BoxCollider2D coll = tileToSpawn.GetComponent<BoxCollider2D> ();
			coll.enabled = true;
			Destroy (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}
	}

	public void TileToDiscover(GameObject newTile, int mapPosX, int mapPosY, Transform tileHolder, ResourceGrid grid, PlayerControls selectedUnit, TileType.Types tileType){		// this is called by Resource grid with the proper tile obj
		tileToSpawn = Instantiate (newTile, transform.position, Quaternion.identity) as GameObject;
		tileToSpawn.transform.parent = tileHolder;
		// Give the Tile position relative to the grid map
		TileClick_Handler tc = tileToSpawn.GetComponent<TileClick_Handler> ();
		tc.mapPosX = mapPosX;
		tc.mapPosY = mapPosY;
		tc.resourceGrid = grid;
		tc.selectedUnit = selectedUnit;
		if (tileType == TileType.Types.empty) {
			tc.isWalkable = false;
		} else {
			tc.isWalkable = true;
		}
		// ADD this tile to the Grid's spawnedTiles array
		grid.spawnedTiles [mapPosX, mapPosY] = tileToSpawn;
	}
}
