using UnityEngine;
using System.Collections;

public class Man : MonoBehaviour {
	public Torus torus;

	public Vector3[] path;

	private float phi = 0.0f;
	private float teta = 0.0f;
	private Transform cachedTransform;

	private void UpdatePosition() {
		Vector3 c = new Vector3(torus.bigR * Mathf.Cos(phi), torus.bigR * Mathf.Sin(phi), 0.0f);
		cachedTransform.position = new Vector3((torus.bigR + (cachedTransform.localScale.y + torus.smallR) * Mathf.Cos(teta)) * Mathf.Cos(phi), (torus.bigR + (cachedTransform.localScale.y + torus.smallR) * Mathf.Cos(teta)) * Mathf.Sin(phi), (cachedTransform.localScale.y + torus.smallR) * Mathf.Sin(teta));
		cachedTransform.up = (cachedTransform.position - c).normalized;
	}

	private void Awake() {
		cachedTransform = GetComponent<Transform>();
		UpdatePosition();
	}

	private void Update() {
	}

	private void OnDrawGizmos() {
		if (path != null) {
			Gizmos.color = Color.green;
			Vector3 prev = transform.position;
			foreach (var point in path) {
				Gizmos.DrawLine(prev, point);
				prev = point;
			}
		}
	}
}
