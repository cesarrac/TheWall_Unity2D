using UnityEngine;
using System.Collections;

public class Player_MoveHandler : MonoBehaviour {

	public float speed = 1f;

	public ResourceGrid resourceGrid;

	int mapX, mapY;

	void Start () {
		mapX = resourceGrid.mapSizeX;
		mapY = resourceGrid.mapSizeY;
	}
	

	void Update () {
	
		Move ();
	}

	void Move()
	{
		float _inputX = Input.GetAxis ("Horizontal");
		float _inputY = Input.GetAxis ("Vertical");
		float inputX = Mathf.Clamp (_inputX,(float) -mapX, (float) mapX);
		float inputY = Mathf.Clamp (_inputY,(float) -mapY, (float) mapY);

		Vector3 move = new Vector3 (inputX, inputY, 0.0f);

		Vector3 newMovePos = new Vector3 (transform.position.x + inputX, transform.position.y + inputY, 0.0f);

//		if (CheckWalkabale (newMovePos)) {
//
//			transform.position += move * speed * Time.deltaTime;
//
//		}
		transform.position += move * speed * Time.deltaTime;


	}

	bool CheckWalkabale(Vector3 pos){
		int posX = Mathf.RoundToInt (pos.x);
		int posY = Mathf.RoundToInt (pos.y);

		if (resourceGrid.UnitCanEnterTile (posX, posY)) {
			return true;
		} else {
			return false;
		}

	}
}
