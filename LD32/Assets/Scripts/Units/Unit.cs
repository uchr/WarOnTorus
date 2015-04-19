using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	public float speed = 1.0f;
	public float height = 1.0f;

	private Torus torus;
	
	public Vector3 tPosition;

	public Vector3[] path;
	protected int i = 0;

	private Transform cachedTransform;

	public void UpdatePath(Vector3[] path) {
		i = 1;
		this.path = path;
	}

	public void UpdatePosition(Vector3 forward) {
		Vector3 c = new Vector3(torus.bigR * Mathf.Cos(tPosition.x), torus.bigR * Mathf.Sin(tPosition.x), 0.0f);
		cachedTransform.position = torus.GetCortPoint(tPosition, height);
		cachedTransform.LookAt(forward, (cachedTransform.position - c).normalized);
	}

	private void Awake() {
		torus = Torus.instance;
		cachedTransform = GetComponent<Transform>();

		tPosition = Vector3.zero;
	}

	private void Update() {
		if (path != null && i < path.Length) {
			Vector3 dir = Vector3.zero;

			if (tPosition.x <= Mathf.PI)
				dir.x = Mathf.Abs(path[i].x - 2 * Mathf.PI - tPosition.x) < Mathf.Abs(path[i].x - tPosition.x) ? path[i].x - 2 * Mathf.PI : path[i].x;
			else
				dir.x = Mathf.Abs(path[i].x + 2 * Mathf.PI - tPosition.x) < Mathf.Abs(path[i].x - tPosition.x) ? path[i].x + 2 * Mathf.PI : path[i].x;

			if (tPosition.y <= Mathf.PI)
				dir.y = Mathf.Abs(path[i].y - 2 * Mathf.PI - tPosition.y) < Mathf.Abs(path[i].y - tPosition.y) ? path[i].y - 2 * Mathf.PI : path[i].y;
			else
				dir.y = Mathf.Abs(path[i].y + 2 * Mathf.PI - tPosition.y) < Mathf.Abs(path[i].y - tPosition.y) ? path[i].y + 2 * Mathf.PI : path[i].y;

			tPosition += (dir - tPosition).normalized * speed * Time.deltaTime;

			dir.x -= tPosition.x;
			dir.y -= tPosition.y;
			tPosition.x %= 2 * Mathf.PI;
			tPosition.y %= 2 * Mathf.PI;

			UpdatePosition(torus.GetCortPoint(path[i], height));

			if ((Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y)) < 0.08f) ++i;
			if (i == path.Length) path = null;
		}
	}

	private void OnTriggerStay(Collider other) {
		var unit = other.GetComponent<Unit>();
		if (unit != null) {
			var dir = unit.tPosition - tPosition;
			tPosition -= (0.7f - dir.magnitude) * dir.normalized * BalanceSettings.instance.pushForce * Time.deltaTime;
		}
		var build = other.GetComponent<Building>();
		if (build != null) {
			var dir = build.tPosition - tPosition;
			tPosition -= (0.7f - dir.magnitude) * dir.normalized * BalanceSettings.instance.pushForce * Time.deltaTime;
		}
		UpdatePosition(cachedTransform.position + cachedTransform.forward);
	}

	private void OnDrawGizmos() {
		if (path != null && path.Length > 0) {
			Gizmos.color = Color.green;
			Vector3 prev = torus.GetCortPoint(path[0]);
			foreach (var point in path) {
				var p = torus.GetCortPoint(point);
				Gizmos.DrawLine(prev, p);
				prev = p;
			}
		}
	}
}
