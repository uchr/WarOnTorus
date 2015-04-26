using UnityEngine;
using System.Collections;

public abstract class Unit : MonoBehaviour {
	public int owner = 0;

	public UnitType unitType;

	public int hp = 5;

	public float speed = 1.0f;
	protected float phiSpeed = 1.0f;
	protected float tetaSpeed = 1.0f;
	public float height = 1.0f;

	protected Torus torus;
	
	public Vector3 tPosition;

	public GameObject fireArea;

	public bool isReached = false;

	protected Vector3 relativeAttackPosition;

	public Vector3[] path;
	protected int i = 0;

	protected Transform cachedTransform;

	public abstract void AttackUnit(Unit unit, Vector3 relativeAttackPosition);
	public abstract void AttackBuilding(Building building, Vector3 relativeAttackPosition);
	public abstract void Move(Vector3 goal);

	public void UpdatePath(Vector3[] path) {
		i = 1;
		this.path = path;
	}

	private int col = 0;

	public void SetOwner(int owner) {
		this.owner = owner;
		var renderers = GetComponentsInChildren<Renderer>();
		foreach (var r in renderers) {
			// TODO DELETE IT
			if (r.gameObject.layer == 16) continue;
			if (owner == 0)
				r.material = BalanceSettings.instance.blue;
			else
				r.material = BalanceSettings.instance.red;
		}
	}

	public void UpdatePosition(Vector3 lookAt) {
		tPosition = Torus.instance.Repeat(tPosition);
		cachedTransform.position = torus.TorusToCartesian(tPosition);
		if (Vector3.Distance(cachedTransform.position, lookAt) > 0.2f)
			cachedTransform.LookAt(lookAt, torus.GetNormal2(tPosition));
	}

	private void Awake() {
		torus = Torus.instance;
		cachedTransform = GetComponent<Transform>();

		tPosition = new Vector3(0.0f, 0.0f, height);

		var deltaPhi = Vector3.Distance(torus.TorusToCartesian(Vector3.zero), torus.TorusToCartesian(new Vector3(speed, 0.0f, 0.0f)));
		var deltaTeta = Vector3.Distance(torus.TorusToCartesian(Vector3.zero), torus.TorusToCartesian(new Vector3(0.0f, speed, 0.0f)));
		phiSpeed = speed;
		tetaSpeed = speed * (deltaPhi / deltaTeta);
	}

	protected virtual void Update() {
		col = 0;
	}

	private void OnTriggerStay(Collider other) {
		if (col > 6) return;
		++col;

		var unit = other.GetComponent<Unit>();
		if (unit != null) {
			var dir = unit.tPosition - tPosition + Vector3.right * Random.Range(-0.02f, 0.03f) + Vector3.up * Random.Range(-0.02f, 0.03f);
			tPosition -= (0.4f - dir.magnitude) * dir.normalized * BalanceSettings.instance.pushForce * Time.fixedDeltaTime;
		}
		var build = other.GetComponent<Building>();
		if (build != null) {
			var dir = build.tPosition - tPosition + Vector3.right * Random.Range(-0.02f, 0.03f) + Vector3.up * Random.Range(-0.02f, 0.03f);
			tPosition -= (0.7f - dir.magnitude) * dir.normalized * BalanceSettings.instance.pushForce * Time.fixedDeltaTime;
		}
		UpdatePosition(cachedTransform.position + cachedTransform.forward);
	}

	private void OnDrawGizmos() {
		if (path != null && path.Length > 0) {
			Gizmos.color = Color.green;
			Vector3 prev = torus.TorusToCartesian(path[0]);
			foreach (var point in path) {
				var p = torus.TorusToCartesian(point);
				Gizmos.DrawLine(prev, p);
				prev = p;
			}
		}
	}
}
