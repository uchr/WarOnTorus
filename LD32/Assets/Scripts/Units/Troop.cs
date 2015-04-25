using UnityEngine;
using System.Collections;

public class Troop {
	public Unit[] units;

	public bool InSitu() {
		foreach (var unit in units)
			if (!unit.isReached)
				return false;
		return true;
	}

	public void Select() {
		//foreach (var unit in units)
		//	unit.fireArea.SetActive(true);
	}

	public void Unselect() {
		//foreach (var unit in units)
		//	unit.fireArea.SetActive(false);
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
		float r = 0.1f;
		var t = goal;
		int i = 0;
		foreach (var unit in units) {
			unit.Move(t);
			++i;
			t = goal + new Vector3(r * Mathf.Cos(Mathf.PI / units.Length), r * Mathf.Sin(Mathf.PI / units.Length), 0.0f);
		}
	}
}
