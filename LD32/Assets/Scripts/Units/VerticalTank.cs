using UnityEngine;
using System.Collections;

public class VerticalTank : Unit {
	public float fireWidth;
	public float fireHeight;
	
	public GameObject bullet;

	public override void GoTo(Vector3 goal) {
		state = UnitState.Idle;
		Map.instance.SetPath(goal, this);
	}

	public override void AttackUnit(Unit unit, Vector3 attackPosition) {
		state = UnitState.AttackUnit;
		goalUnit = unit;
	}

	public override void AttackBuilding(Building building, Vector3 relativeAttackPosition) {
		state = UnitState.AttackBuilding;
		goalBuilding = building;
	}

	protected override void Update() {
		base.Update();

		if (timer >= 0.0f)
			timer -= Time.deltaTime;

		if (state == UnitState.AttackUnit) {
			if (goalUnit == null) {
				state = UnitState.Idle;
				path = null;
				return;
			}

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

		if (state == UnitState.AttackBuilding) {
			if (goalBuilding == null) {
				state = UnitState.Idle;
				path = null;
				return;
			}

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

		if (state == UnitState.Idle) {
			Move();
		}
	}
}
