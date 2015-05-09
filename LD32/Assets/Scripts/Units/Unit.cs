using UnityEngine;
using System.Collections;
using System.Text;

public enum UnitState {
	Idle,
	AttackUnit,
	AttackBuilding
};

public abstract class Unit : MonoBehaviour {
	// Logic
	public UnitType unitType;
	public int owner = 0;
	public int maxhp = 5;
	public int hp = 5;
	public Troop troop;
	public string name;

	protected UnitState state = UnitState.Idle;

	// Movement
	public Vector3 tPosition;
	public float height = 1.0f;
	
	public float speed = 1.0f;
	protected float phiSpeed = 1.0f;
	protected float tetaSpeed = 1.0f;

	public bool isReached = false;

	protected int i = 0;
	public Vector3[] path;

	private int col = 0;

	// Attack
	public GameObject fireArea;
	
	public float time = 1.0f;
	protected float timer = 1.0f;

	protected Vector3 relativeAttackPosition;

	protected Vector3 tCurrent;
	protected Unit goalUnit;
	protected Building goalBuilding;

	protected Transform cachedTransform;

	public abstract void GoTo(Vector3 goal);
	public abstract void AttackUnit(Unit unit, Vector3 relativeAttackPosition);
	public abstract void AttackBuilding(Building building, Vector3 relativeAttackPosition);

	public void UpdatePath(Vector3[] path) {
		i = 1;
		this.path = path;
	}

	public void SetOwner(int owner) {
		this.owner = owner;
		var renderers = GetComponentsInChildren<Renderer>();
		foreach (var renderer in renderers) {
			// TODO DELETE IT
			if (renderer.gameObject.layer == 16) continue;
			if (owner == 0)
				renderer.material = BalanceSettings.instance.blue;
			else
				renderer.material = BalanceSettings.instance.red;
		}
	}

	public void UpdatePosition(Vector3 lookAt) {
		tPosition = Torus.instance.Repeat(tPosition);
		cachedTransform.position = Torus.instance.TorusToCartesian(tPosition);
		if (Vector3.Distance(cachedTransform.position, lookAt) > 0.2f)
			cachedTransform.LookAt(lookAt, Torus.instance.GetNormal2(tPosition));
	}

	private string RandomString(int size) {
		StringBuilder builder = new StringBuilder();
		char c;
		for (int i = 0; i < size; i++) {       
			builder.Append((char) Random.Range('A', 'Z'));
		}

		return builder.ToString();
	}

	protected string GetName() {
		return Random.Range(0, 10) + RandomString(4); 
	}

	private void Awake() {
		name = GetName();
		gameObject.name = name;

		cachedTransform = GetComponent<Transform>();
		cachedTransform.SetParent(GameObject.Find("Units").transform);

		tPosition = new Vector3(0.0f, 0.0f, height);

		var deltaPhi = Vector3.Distance(Torus.instance.TorusToCartesian(Vector3.zero), Torus.instance.TorusToCartesian(new Vector3(speed, 0.0f, 0.0f)));
		var deltaTeta = Vector3.Distance(Torus.instance.TorusToCartesian(Vector3.zero), Torus.instance.TorusToCartesian(new Vector3(0.0f, speed, 0.0f)));
		phiSpeed = speed;
		tetaSpeed = speed * (deltaPhi / deltaTeta);
	}

	protected void Move() {
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

			var t = (dir - tPosition).normalized;
			tPosition.x += t.x * phiSpeed * Time.deltaTime;
			tPosition.y += t.y * tetaSpeed * Time.deltaTime;

			dir.x -= tPosition.x;
			dir.y -= tPosition.y;

			// TODO FIX IT
			UpdatePosition(Torus.instance.TorusToCartesian(path[i] + new Vector3(0.0f, 0.0f, height)));

			if (Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y) < 0.08f) ++i;
			if (i == path.Length) {
				isReached = true;
				path = null;
			}
		}
	}

	protected virtual void Update() {
		col = 0;

		if (hp <= 0) {
			if (troop != null)
				troop.UnitKilled(this);
			Destroy(gameObject);
		}
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
			Vector3 prev = Torus.instance.TorusToCartesian(path[0]);
			foreach (var point in path) {
				var p = Torus.instance.TorusToCartesian(point);
				Gizmos.DrawLine(prev, p);
				prev = p;
			}
		}
	}
}
