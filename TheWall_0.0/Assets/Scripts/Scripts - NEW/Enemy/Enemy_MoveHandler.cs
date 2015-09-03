using UnityEngine;
using System.Collections.Generic;

public class Enemy_MoveHandler : MonoBehaviour {

	[System.Serializable]
	public class MovementStats {
		public float startMoveSpeed;

		private float _moveSpeed;

		public float curMoveSpeed{ get { return _moveSpeed; } set { _moveSpeed = Mathf.Clamp(value, 0, startMoveSpeed); } }

		public void InitMoveStats(){
			curMoveSpeed = startMoveSpeed;
		}
	}

	public MovementStats mStats = new MovementStats();

	public ResourceGrid resourceGrid;
	
	public int posX, targetPosX;
	public int posY, targetPosY;
	
	// This stores this unit's path
	public List<Node>currentPath = null;
	
	private Vector3 velocity = Vector3.zero;

	public bool isAttacking = false; // this turns true when this object enters a player unit's collider OR a blocking tile

	public Enemy_AttackHandler enemyAttkHandler;

//	public float movementSpeed = 1.0F;

	public bool moving;

	public int spwnPtIndex;
	public SpawnPoint_Handler spwnPtHandler;

	public float formationOffset;

	public float stoppingDistance;
	CircleCollider2D collider;
	public Vector3 destination;


	public bool movingBackToPath = false, movingToFormation = false;
	Vector2 formationPos;

	// THE BUDDY SYSTEM: each individual spawned enemy will know the Move Handler of the enemy before them (given to them by the SpawnHandler)
	public Enemy_MoveHandler myBuddy;

	public Animator anim;

	private Vector2 _capitalPosition;

	void Start () {

		mStats.InitMoveStats ();

		if (anim == null) {
			anim = GetComponentInChildren<Animator>();
		}

		posX = (int)transform.position.x;
		posY = (int)transform.position.y;
		enemyAttkHandler = GetComponentInChildren<Enemy_AttackHandler> ();

		GetFirstPath ();

		_capitalPosition = new Vector2 (resourceGrid.capitalSpawnX, resourceGrid.capitalSpawnY);
	}

	void GetFirstPath(){
		if (spwnPtHandler != null) {
			currentPath = new List<Node>();
			for (int x = 0; x < spwnPtHandler.path[spwnPtIndex].Count; x++){
				currentPath.Add(spwnPtHandler.path[spwnPtIndex][x]);
			}
			moving = true;
		}
	}

	public void GetPath(){
		posX = (int)transform.position.x;
		posY = (int)transform.position.y;
		int myPosIndexInPath = 0;
		if (spwnPtHandler != null) {
			currentPath = new List<Node>();
			for (int x = 0; x < spwnPtHandler.path[spwnPtIndex].Count; x++){
				// find my position on the path
				if(spwnPtHandler.path[spwnPtIndex][x].x == posX && spwnPtHandler.path[spwnPtIndex][x].y == posY){
					Debug.Log("Found my node at pathtoCapital" + x);
					myPosIndexInPath = x;
					break;
				}
			}
			if (myPosIndexInPath > 0){ // At this point Index should never be 0 if we found our position in the path
				for (int i = myPosIndexInPath; i < spwnPtHandler.path[spwnPtIndex].Count; i++){
					currentPath.Add(spwnPtHandler.path[spwnPtIndex][i]);
				}
				moving = true;
			}else{
				Debug.Log("Couldn't find my node :(");
			}


		}
	}
//	void MoveBackToPath(Vector2 _nodePos){
//		if (Vector2.Distance (transform.position, _nodePos) == 0) {
//			GetPath ();
//			movingBackToPath = false;
//		} else {
//
//			transform.position = Vector2.MoveTowards(transform.position, _nodePos ,movementSpeed * Time.deltaTime);
//		}
//	}


	void Update () {
		if (currentPath != null) {
			int currNode = 0;
			while (currNode < currentPath.Count -1) {
				Vector3 start = resourceGrid.TileCoordToWorldCoord (currentPath [currNode].x, currentPath [currNode].y);
				destination = resourceGrid.TileCoordToWorldCoord (currentPath [currNode + 1].x, currentPath [currNode + 1].y);
				;
				Debug.DrawLine (start, destination, Color.blue);
				currNode++;
			}
		
		} 

		if (moving && !isAttacking){
			// Have we moved close enough to the target tile that we can move to next tile in current path?
			if (Vector2.Distance (transform.position, resourceGrid.TileCoordToWorldCoord (posX, posY)) < (0.1)) {
				MoveToNextTile ();
			}
			transform.position = Vector2.MoveTowards(transform.position, 
			                                         resourceGrid.TileCoordToWorldCoord (posX, posY), 
			                                         mStats.curMoveSpeed * Time.deltaTime);
			// ANIMATION CONTROLS:
			if (posX > transform.position.x){
				anim.SetTrigger ("movingRight");
				anim.ResetTrigger("movingLeft");
			}else if (posX < transform.position.x){
				anim.SetTrigger ("movingLeft");
				anim.ResetTrigger("movingRight");
			}
		}


		// BUDDY SYSTEM CALLED ONLY IF MY BUDDY IS NOT NULL
		if (myBuddy != null) {
			BuddySystem();
		}
	}
	public void MoveToNextTile(){
		if (currentPath == null) {
			return;
		}
		// Remove the old first node from the path
		currentPath.RemoveAt (0);
		
		// Check if the next tile is a UNWAKABLE tile, if it is clear path
		if (resourceGrid.UnitCanEnterTile (currentPath [1].x, currentPath [1].y) == false) {
			moving = false;
			Debug.Log ("Path is blocked! at x" + currentPath [1].x + ", y" + currentPath [1].y);
			if (CheckForTileAttack (currentPath [1].x, currentPath [1].y)) {
				Debug.Log("Attacking Tile!");
				// here I would tell the Attack script to start its attack on the tile
				// But if it's the Capital, not just any tile, then it needs to do a special attack
				if (currentPath[1].x == _capitalPosition.x && currentPath[1].y == _capitalPosition.y)
				{
					// we are at the capital! Do special!
					enemyAttkHandler.SpecialAttack(currentPath[1].x, currentPath[1].y);
				}
				else
				{
					targetPosX = currentPath [1].x;
					targetPosY = currentPath [1].y;
					enemyAttkHandler.targetTilePosX = currentPath [1].x;
					enemyAttkHandler.targetTilePosY = currentPath [1].y;
					enemyAttkHandler.resourceGrid = resourceGrid;
					enemyAttkHandler.canAttackTile = true;
					isAttacking = true;
				}
			
			} 

		} else {
			if (isAttacking){ // at this point if this is true it means this unit is engaging a Player Unit
				currentPath = null;
				moving = false;
				return;
			}
		} 	
		// Move to the next Node position in path
		posX = currentPath [1].x;
		posY = currentPath [1].y;

		// We are on the tile that is our DESTINATION, 
		// CLEAR PATH
		if (currentPath.Count == 1) {
			currentPath = null;
		}
	}

	bool CheckForTileAttack(int x, int y){
		if (resourceGrid.tiles [x, y].tileType != TileData.Types.empty && resourceGrid.tiles [x, y].tileType != TileData.Types.rock) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Checks if MY BUDDY has:
	/// -Reached the destination OR
	/// - is Attacking
	/// </summary>
	void BuddySystem(){
//		if (!destinationReached) {
//			if (myBuddy.destinationReached == true){
//				destinationReached = true;
////				moving = false;
////				Debug.Log ("stopping.");
//				//TODO: Instead of stopping here, when they reach their destination, I want them to surround the building
//			}
//		}
		if (!isAttacking) {
			if (myBuddy.isAttacking == true){
				if (CheckForTileAttack(myBuddy.targetPosX, myBuddy.targetPosY)){
					isAttacking = true;
					moving = false;
					targetPosX = myBuddy.targetPosX;
					targetPosY = myBuddy.targetPosY;
					enemyAttkHandler.targetTilePosX = myBuddy.targetPosX;
					enemyAttkHandler.targetTilePosY = myBuddy.targetPosY;
					enemyAttkHandler.resourceGrid = resourceGrid;
					enemyAttkHandler.canAttackTile = true;
					Debug.Log ("Also attacking tile!");
				}
			}
		}
	}
}
