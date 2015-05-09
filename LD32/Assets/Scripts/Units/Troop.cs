using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class Troop {
	public List<Unit> units;

	public Vector3 center {
		get {
			if (units.Count == 0)
				return Vector3.zero;
			
			Vector3 center = Vector3.zero;
			foreach (var unit in units)
				center += unit.tPosition;
			return center / units.Count;
		}
	}

	public int count {
		get {
			return units.Count;
		}
	}

	public Troop() {
		units = new List<Unit>();
	}

	public bool Arrived() {
		foreach (var unit in units)
			if (!unit.isReached)
				return false;
		return true;
	}

	public void ChangeTo(List<Unit> newUnits) {
		foreach (var unit in units) 
			unit.troop = null;

		if (newUnits == null) {
			units = new List<Unit>();
			return;
		}

		units = newUnits;
		foreach (var unit in units) 
			unit.troop = this;
	}

	public void UnitKilled(Unit unit) {
		units.Remove(unit);
	}

	// TODO FIX CHOICE IMPASSABLE AREAS IN ATTACKUNIT/ATTACKBUILDING/MOVE
	public void AttackUnit(Unit goalUnit) {
		float r = 0.08f;
		Vector3 t;
		var angle = (2.0f * Mathf.PI) / units.Count;
		for (int i = 0; i < units.Count; ++i) {
			t = new Vector3(r * Mathf.Cos(angle * i), r * Mathf.Sin(angle * i), 0.0f);
			units[i].AttackUnit(goalUnit, t);
		}
	}

	public void AttackBuilding(Building goalBuilding) {
		float r = 0.15f;
		Vector3 t;
		var angle = (2.0f * Mathf.PI) / units.Count;
		for (int i = 0; i < units.Count; ++i) {
			t = new Vector3(r * Mathf.Cos(angle * i), r * Mathf.Sin(angle * i), 0.0f);
			units[i].AttackBuilding(goalBuilding, t);
		}
	}

	public void Move(Vector3 goal) {
		float r = 0.09f;
		var t = goal;
		units[0].GoTo(t);
		var angle = (2.0f * Mathf.PI) / (units.Count - 1);
		for (int i = 1; i < units.Count; ++i) {
			t = goal + new Vector3(r * Mathf.Cos(angle * (i - 1)), r * Mathf.Sin(angle * (i - 1)), 0.0f);
			t = Torus.instance.Repeat(t);
			units[i].GoTo(t);
		}
	}

	public string GetDescription() {
		StringBuilder builder = new StringBuilder();

		for (int i = 0; i < units.Count; ++i) {
			builder.Append(i + 1);
			builder.Append('.');
			builder.Append(' ');
			builder.Append(units[i].name);
			builder.Append(' ');
			builder.Append(units[i].hp);
			builder.Append('/');
			builder.Append(units[i].maxHP);
			builder.Append('\n');
		}
		
        return builder.ToString();
	}
}
