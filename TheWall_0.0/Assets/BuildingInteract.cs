using UnityEngine;
using System.Collections;

public class BuildingInteract : MonoBehaviour {

	public LayerMask mask;
	void Update(){
		if (Input.GetMouseButtonDown (0) && Input.GetKey(KeyCode.LeftShift)) {
			Raycast ();
		}
	}
	void Raycast(){
		var hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f, mask.value);
		if (hit.collider != null) {
			if (hit.collider.gameObject == gameObject){
				GameObject spawnedDrones = GetComponentInParent<DroneTower>().spawnedDrones;
				if (spawnedDrones != null) {
					DroneController dC = spawnedDrones.GetComponent<DroneController> ();
					if (!dC.attacking && !dC.selectingSpawnPoint) {
						dC.selectingSpawnPoint = true;
					}
				}
			}
		}
	}
}