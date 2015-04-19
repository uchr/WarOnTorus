using UnityEngine;
using System.Collections;

public class HorizontalTank : Unit {
	public float speed = 1.0f;

	private Torus torus;

	private float phi = 0.0f;
	private float teta = 0.0f;
	private Transform cachedTransform;

	//private void UpdatePosition() {
	//	Vector3 c = new Vector3(torus.bigR * Mathf.Cos(phi), torus.bigR * Mathf.Sin(phi), 0.0f);
	//	cachedTransform.position = new Vector3((torus.bigR + (cachedTransform.localScale.y + torus.smallR) * Mathf.Cos(teta)) * Mathf.Cos(phi), (torus.bigR + (cachedTransform.localScale.y + torus.smallR) * Mathf.Cos(teta)) * Mathf.Sin(phi), (cachedTransform.localScale.y + torus.smallR) * Mathf.Sin(teta));
	//	cachedTransform.up = (cachedTransform.position - c).normalized;
	//}

	private void Awake() {
		torus = Torus.instance;
		cachedTransform = GetComponent<Transform>();
		//UpdatePosition();
	}

	private void Update() {
		if (path != null && i < path.Length) {
			var to = path[i] + Torus.instance.GetNormal(path[i]) * 0.1f;
			cachedTransform.position = Vector3.Lerp(cachedTransform.position, to, Time.deltaTime * speed);
			cachedTransform.LookAt(to, torus.GetNormal(cachedTransform.position));
			if (Vector3.Distance(cachedTransform.position, to) < 1.0f && i + 1 < path.Length)
				++i;
		}
	}
}
