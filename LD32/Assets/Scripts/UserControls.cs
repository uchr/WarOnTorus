using UnityEngine;
using System.Collections;

public class UserControls : MonoBehaviour {
	public Torus torus;

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

				var gridPoint = torus.GetGridPoint(pointer.position);
				float bigStep = 2.0f * Mathf.PI / (torus.bigSteps * 3.0f);
				float smallStep = 2.0f * Mathf.PI / (torus.smallSteps * 3.0f);
				var i = gridPoint.x;
				var j = gridPoint.y;
				var c = new Vector3(torus.bigR * Mathf.Cos(bigStep * i), torus.bigR * Mathf.Sin(bigStep * i), 0.0f);
				var p = new Vector3((torus.bigR + torus.smallR * Mathf.Cos(smallStep * j)) * Mathf.Cos(bigStep * i), (torus.bigR + torus.smallR * Mathf.Cos(smallStep * j)) * Mathf.Sin(bigStep * i), torus.smallR * Mathf.Sin(smallStep * j));
				Debug.DrawLine(c, c + 2.0f * (p - c), Color.green);
				pointer.up = torus.GetNormal(hit.point);
			}
			if (Input.GetMouseButtonUp(0)) {
				Debug.Log(torus.GetGridPoint(pointer.position).x + " " + torus.GetGridPoint(pointer.position).y);
			}
		} else {
			if (Input.GetMouseButtonUp(0)) {
				Destroy(currentBuilder);
			}
		}
	}
}
