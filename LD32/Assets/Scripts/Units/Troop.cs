using UnityEngine;
using System.Collections;

public class Troop {
	public Unit[] units;

	public void Select() {
		foreach (var unit in units)
			unit.fireArea.SetActive(true);
	}

	public void Unselect() {
		foreach (var unit in units)
			unit.fireArea.SetActive(false);
	}

	public void AttackUnit(Unit goal) {
		foreach (var unit in units) {
			unit.AttackUnit(goal);
		}
	}

	public void AttackBuilding(Building goal) {
		foreach (var unit in units) {
			unit.AttackBuilding(goal);
		}
	}

	public void Move(Vector3 goal) {
		foreach (var unit in units) {
			unit.Move(goal);
		}
	}
}
