using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UserControls : MonoBehaviour {
	public Torus torus;
	public Transform tangentSpace;
	public BuildingsConstruction buildingsConstruction;

	public HorTank horTank;

	public Transform pointer;
	public GameObject building;

	private void Update() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(!EventSystem.current.IsPointerOverGameObject()) {
			if (Physics.Raycast(ray, out hit, 50.0f, 1 << 8)) {
				// Movement
				pointer.position = hit.point;
				pointer.up = torus.GetNormal(hit.point);
				if (buildingsConstruction.construct && !buildingsConstruction.rotate) {
					buildingsConstruction.building.transform.position = pointer.position;
					buildingsConstruction.building.transform.rotation = pointer.rotation;
				}

				// Right Button
				if (Input.GetMouseButtonDown(0)) {
					if (buildingsConstruction.construct) {
						buildingsConstruction.rotate = true;
						tangentSpace.position = hit.point;
						tangentSpace.up = torus.GetNormal(hit.point);
					}
				}

				if (Input.GetMouseButton(0)){
					if (buildingsConstruction.construct && buildingsConstruction.rotate) {
						RaycastHit tangentHit;
						Physics.Raycast(ray, out tangentHit, 50.0f, 1 << 9);
						buildingsConstruction.building.transform.LookAt(tangentHit.point, tangentSpace.up);
					}
				}

				if (Input.GetMouseButtonUp(0)) {
					if (buildingsConstruction.construct) {
						buildingsConstruction.construct = false;
						buildingsConstruction.building = null;
					}
					Map.instance.CalculateGrid();
				}

				//Left button
				if (Input.GetMouseButtonUp(1)) {
					Map.instance.SetPath(horTank.transform.position, pointer.position, horTank);
				}
			} else {
				// Movement
				pointer.position = Vector3.down * 100.0f;
				if (buildingsConstruction.construct && !buildingsConstruction.rotate)
					buildingsConstruction.building.transform.position = pointer.position;

				if (Input.GetMouseButtonUp(0)) {
					if (buildingsConstruction.construct) {
						buildingsConstruction.construct = false;
						Destroy(buildingsConstruction.building);
					}
				}
			}
		}
	}
}
