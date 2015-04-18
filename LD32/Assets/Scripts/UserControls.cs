using UnityEngine;
using System.Collections;

public class UserControls : MonoBehaviour {
	public Torus torus;

	public HorTank horTank;

	public Transform pointer;
	public GameObject building;

	private GameObject currentBuilder;

	private void Update() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 50.0f, 1 << 8)) {
			pointer.position = hit.point;

			// Right Button
			if (Input.GetMouseButtonDown(0)) {
				currentBuilder = (GameObject) Instantiate(building, pointer.position, pointer.rotation);
			}
			if (Input.GetMouseButton(0)){
				currentBuilder.transform.position = pointer.position;
				currentBuilder.transform.rotation = pointer.rotation;

				pointer.up = torus.GetNormal(hit.point);
			}
			if (Input.GetMouseButtonUp(0)) {
				Map.instance.CalculateGrid();
				currentBuilder = null;
			}

			//Left button
			if (Input.GetMouseButtonUp(1)) {
				Map.instance.SetPath(horTank.transform.position, pointer.position, horTank);
			}
		} else {
			if (Input.GetMouseButtonUp(0)) {
				Destroy(currentBuilder);
			}
		}
	}
}
