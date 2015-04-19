using UnityEngine;
using System.Collections;

public class VerticalTank : Unit {
	public float speed = 0.7f;

	private Torus torus;

	private Transform cachedTransform;

	private void Awake() {
		torus = Torus.instance;
		cachedTransform = GetComponent<Transform>();
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
