using UnityEngine;
using System.Collections;

[System.Serializable]
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

	// TODO FIX CHOICE IMPASSABLE AREAS IN ATTACKUNIT/ATTACKBUILDING/MOVE
	public void AttackUnit(Unit goalUnit) {
		float r = 0.07f;
		Vector3 t;
		var angle = (2.0f * Mathf.PI) / units.Length;
		for (int i = 0; i < units.Length; ++i) {
			t = new Vector3(r * Mathf.Cos(angle * i), r * Mathf.Sin(angle * i), 0.0f);
			units[i].AttackUnit(goalUnit, t);
		}
	}

	public void AttackBuilding(Building goalBuilding) {
		float r = 0.12f;
		Vector3 t;
		var angle = (2.0f * Mathf.PI) / units.Length;
		for (int i = 0; i < units.Length; ++i) {
			t = new Vector3(r * Mathf.Cos(angle * i), r * Mathf.Sin(angle * i), 0.0f);
			units[i].AttackBuilding(goalBuilding, t);
		}
	}

	public void Move(Vector3 goal) {
		float r = 0.07f;
		var t = goal;
		units[0].Move(t);
		var angle = (2.0f * Mathf.PI) / (units.Length - 1);
		for (int i = 1; i < units.Length; ++i) {
			t = goal + new Vector3(r * Mathf.Cos(angle * (i - 1)), r * Mathf.Sin(angle * (i - 1)), 0.0f);
			t = Torus.instance.Repeat(t);
			units[i].Move(t);
		}
	}
}
