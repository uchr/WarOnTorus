using UnityEngine;
using System.Collections;

public class HorizontalTank : Unit {
	public float fireLength;

	public GameObject bullet;

	public override void GoTo(Vector3 goal) {
		state = UnitState.Idle;
		Map.instance.SetPath(goal, this);
		isReached = false;
	}

	public override void AttackUnit(Unit unit, Vector3 relativeAttackPosition) {
		this.relativeAttackPosition = relativeAttackPosition;
		state = UnitState.AttackUnit;
		goalUnit = unit;
		tCurrent = Torus.instance.Repeat(goalUnit.tPosition + relativeAttackPosition);
		Map.instance.SetPath(tCurrent, this);
	}

	public override void AttackBuilding(Building building, Vector3 relativeAttackPosition) {
		this.relativeAttackPosition = relativeAttackPosition;
		state = UnitState.AttackBuilding;
		goalBuilding = building;
		Map.instance.SetPath(Torus.instance.Repeat(goalBuilding.tPosition + relativeAttackPosition), this);
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

			if (Torus.instance.Distance(goalUnit.tPosition, tPosition) < fireLength) {
				if (timer <= 0.0f) {
					UpdatePosition(Torus.instance.TorusToCartesian(goalUnit.tPosition));
					var bt = ((GameObject) Instantiate(bullet, cachedTransform.position, Quaternion.identity)).transform;
					bt.up = (goalUnit.transform.position - cachedTransform.position).normalized;
					var b = bt.GetComponent<Bullet>();
					b.goal = goalUnit.transform.position;
					b.unit = goalUnit;
					timer = time;
				}
			}
			else {
				Move();
			}

			var t = Torus.instance.Repeat(goalUnit.tPosition + relativeAttackPosition);
			if (path == null || Torus.instance.Distance(t, tCurrent) > UnitsManager.instance.updatePathRadius) {
				tCurrent = Torus.instance.Repeat(goalUnit.tPosition + relativeAttackPosition);
				Map.instance.SetPath(tCurrent, this);
			}
		}

		if (state == UnitState.AttackBuilding) {
			if (goalBuilding == null) {
				state = UnitState.Idle;
				path = null;
				return;
			}

			if (Torus.instance.Distance(goalBuilding.tPosition, tPosition) <= fireLength) {
				if (timer <= 0.0f) {
					UpdatePosition(Torus.instance.TorusToCartesian(goalBuilding.tPosition));
					var bt = ((GameObject) Instantiate(bullet, cachedTransform.position, Quaternion.identity)).transform;
					bt.up = (goalBuilding.transform.position - cachedTransform.position).normalized;
					var b = bt.GetComponent<Bullet>();
					b.goal = goalBuilding.transform.position;
					b.building = goalBuilding;
					timer = time;
				}
			}
			else {
				Move();
			}
		}

		if (state == UnitState.Idle) {
			Move();
		}
	}
}