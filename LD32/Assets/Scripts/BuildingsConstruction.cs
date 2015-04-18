using UnityEngine;
using System.Collections;

public class BuildingsConstruction : MonoBehaviour {
	public bool construct;
	public bool rotate;

	public GameObject building;
	public Vector3 up;

	public GameObject[] buildings;

	public void Construct(int id) {
		if (building != null) {
			Destroy(building);
		}
		rotate = false;
		construct = true;
		building = (GameObject) Instantiate(buildings[id], Vector3.down * 100.0f, Quaternion.identity);
	}
}
