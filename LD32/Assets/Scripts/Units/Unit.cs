using UnityEngine;
using System.Collections;

public abstract class Unit : MonoBehaviour {
	public int owner = 0;

	public int hp = 5;

	public float speed = 1.0f;
	public float height = 1.0f;

	protected Torus torus;
	
	public Vector3 tPosition;

	public GameObject fireArea;

	public Vector3[] path;
	protected int i = 0;

	protected Transform cachedTransform;

	public abstract void Go(Vector3 goal);
	public abstract void AttackUnit(Unit unit);
	public abstract void AttackBuilding(Building building);

	public void UpdatePath(Vector3[] path) {
		i = 1;
		this.path = path;
	}

	public void SetOwner(int owner) {
		this.owner = owner;
		var renderers = GetComponentsInChildren<Renderer>();
		foreach (var r in renderers) {
			if (r.gameObject.layer == 16) {
				Debug.Log("Layer");
				continue;
			}
			if (owner == 0)
				r.material = BalanceSettings.instance.blue;
			else
				r.material = BalanceSettings.instance.red;
		}
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
		if (hp <= 0)
			Destroy(gameObject);
	}

	private void OnTriggerStay(Collider other) {
		var unit = other.GetComponent<Unit>();
		if (unit != null) {
			var dir = unit.tPosition - tPosition;
			tPosition -= (0.4f - dir.magnitude) * dir.normalized * BalanceSettings.instance.pushForce * Time.deltaTime;
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
