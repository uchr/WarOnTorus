using UnityEngine;
using System.Collections;

public class HorTank : Unit {
	public float speed = 10.0f;

	private Torus torus;

	private float phi = 0.0f;
	private float teta = 0.0f;
	private Transform cachedTransform;

	private void UpdatePosition() {
		Vector3 c = new Vector3(torus.bigR * Mathf.Cos(phi), torus.bigR * Mathf.Sin(phi), 0.0f);
		cachedTransform.position = new Vector3((torus.bigR + (cachedTransform.localScale.y + torus.smallR) * Mathf.Cos(teta)) * Mathf.Cos(phi), (torus.bigR + (cachedTransform.localScale.y + torus.smallR) * Mathf.Cos(teta)) * Mathf.Sin(phi), (cachedTransform.localScale.y + torus.smallR) * Mathf.Sin(teta));
		cachedTransform.up = (cachedTransform.position - c).normalized;
	}

	private void Awake() {
		torus = Torus.instance;
		cachedTransform = GetComponent<Transform>();
		UpdatePosition();
	}

	private void Update() {
		if (path != null && i < path.Length) {
			cachedTransform.position = Vector3.Lerp(cachedTransform.position, path[i], Time.deltaTime * speed);
			cachedTransform.up = torus.GetNormal(cachedTransform.position);
			if (Vector3.Distance(cachedTransform.position, path[i]) < 1.0f && i + 1 < path.Length)
				++i;
		}
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
