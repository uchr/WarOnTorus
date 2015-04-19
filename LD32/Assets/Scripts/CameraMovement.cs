using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	public float bigR = 10.0f;
	public float smallR = 6.0f;
	public float step = 0.1f;

	private float phi = 0.0f;
	private float teta = 0.0f;
	private Transform cachedTransform;

	private void UpdatePosition() {
		Vector3 c = new Vector3(bigR * Mathf.Cos(phi), bigR * Mathf.Sin(phi), 0.0f);
		Vector3 u = new Vector3(bigR * Mathf.Cos(phi + Mathf.PI / 2.0f), bigR * Mathf.Sin(phi + Mathf.PI / 2.0f), 0.0f);
		cachedTransform.position = new Vector3((bigR + smallR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta)) * Mathf.Sin(phi), smallR * Mathf.Sin(teta));
		transform.LookAt(c, u);
	}

	private void Awake() {
		cachedTransform = GetComponent<Transform>();
		UpdatePosition();
	}

	private void Update() {
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
			phi += step;
			phi %= Mathf.PI * 2.0f;
			UpdatePosition();
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
			phi -= step;
			phi %= Mathf.PI * 2.0f;
			UpdatePosition();
		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
			teta += step;
			teta %= Mathf.PI * 2.0f;
			UpdatePosition();
		}
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
			teta -= step;
			teta %= Mathf.PI * 2.0f;
			UpdatePosition();
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			smallR -= step * 3.0f;
			UpdatePosition();
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			smallR += step * 3.0f;
			UpdatePosition();
		}
	}
}
