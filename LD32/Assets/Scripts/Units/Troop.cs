using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Troop {
	public Unit[] units;

	public bool Arrived() {
		foreach (var unit in units)
			if (!unit.isReached)
				return false;
		return true;
	}

	public Vector3 center {
		get {
			if (units.Length == 0)
				return Vector3.zero;
			Vector3 center = Vector3.zero;
			foreach (var unit in units)
				center += unit.tPosition;
			return center / units.Length;
		}
	}

	// DIRTY HACK
	// Resize when someone dies, adding a unit to the knowledge of the troop
	public int GetCount() {
		if (units == null) return 0;
		List<Unit> inAlive = new List<Unit>();
		foreach (var unit in units)
			if (unit != null)
				inAlive.Add(unit);
		units = inAlive.ToArray();
		return units.Length;
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
		float r = 0.08f;
		Vector3 t;
		var angle = (2.0f * Mathf.PI) / units.Length;
		for (int i = 0; i < units.Length; ++i) {
			t = new Vector3(r * Mathf.Cos(angle * i), r * Mathf.Sin(angle * i), 0.0f);
			units[i].AttackUnit(goalUnit, t);
		}
	}

	public void AttackBuilding(Building goalBuilding) {
		float r = 0.15f;
		Vector3 t;
		var angle = (2.0f * Mathf.PI) / units.Length;
		for (int i = 0; i < units.Length; ++i) {
			t = new Vector3(r * Mathf.Cos(angle * i), r * Mathf.Sin(angle * i), 0.0f);
			units[i].AttackBuilding(goalBuilding, t);
		}
	}

	public void Move(Vector3 goal) {
		float r = 0.09f;
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
