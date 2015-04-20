using UnityEngine;
using System.Collections;

public class VerticalTank : Unit {
	private enum State {
		Idle,
		AttackUnit,
		AttackBuilding
	};

	public GameObject bullet;

	public float time = 1.0f;
	public float timer = 1.0f;

	public float fireWidth;
	public float fireHeight;

	private State state = State.Idle;
	private Unit goalUnit;
	private Building goalBuilding;

	public override void Go(Vector3 goal) {
		state = State.Idle;
		Map.instance.SetPath(goal, this);
	}

	public override void AttackUnit(Unit unit) {
		state = State.AttackUnit;
		goalUnit = unit;
	}

	public override void AttackBuilding(Building building) {
		state = State.AttackBuilding;
		goalBuilding = building;
	}

	private void Update() {
		bool needGo = false;
		if (timer >= 0.0f)
			timer -= Time.deltaTime;
		if (state == State.AttackUnit) {
			var hits = Physics.CapsuleCastAll(cachedTransform.position, cachedTransform.position + cachedTransform.up * fireHeight, fireWidth, cachedTransform.up, 1 << 11);
			foreach (var hit in hits) {
				if (hit.transform.GetComponent<Unit>() == goalUnit && timer <= 0.0f) {
					var bt = ((GameObject) Instantiate(bullet, cachedTransform.position, Quaternion.identity)).transform;
					bt.up = (goalUnit.transform.position - cachedTransform.position).normalized;
					var b = bt.GetComponent<Bullet>();
					b.goal = goalUnit.transform.position;
					b.unit = goalUnit;
					timer = time;
				}
			}
		}
		if (state == State.AttackBuilding) {
			var hits = Physics.CapsuleCastAll(cachedTransform.position, cachedTransform.position + cachedTransform.up * fireHeight, fireWidth, cachedTransform.up, 1 << 10);
			foreach (var hit in hits) {
				if (hit.transform.GetComponent<Building>() == goalBuilding && timer <= 0.0f) {
					var bt = ((GameObject) Instantiate(bullet, cachedTransform.position, Quaternion.identity)).transform;
					bt.up = (goalBuilding.transform.position - cachedTransform.position).normalized;
					var b = bt.GetComponent<Bullet>();
					b.goal = goalBuilding.transform.position;
					b.building = goalBuilding;
					timer = time;
				}
			}
		}
		if (state == State.Idle) {
			needGo = true;
		}

		if (needGo && path != null && i < path.Length) {
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
}
