using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UnitFactory : Building {
	public class UnitProduction {
		public float timer;
		public UnitSettings unitSettings;
		public UnitType unitType;

		public UnitProduction(int id) {
			unitSettings = UnitsManager.instance.units[id];
			timer = unitSettings.productionTime;
			unitType = (UnitType) id;
		}
	}

	private Vector3 spawnPoint;

	public Queue<UnitProduction> queue;

	private Queue<Unit> units;

	public List<Unit> GetUnits(int number) {
		List<Unit> result = new List<Unit>();
		for (int i = 0; i < number && units.Count > 0; ++i)
			result.Add(units.Dequeue());
		return result;
	}

	public bool Production(int id) {
		if (queue.Count >= 6)
			return false;
		var unitProduction = new UnitProduction(id);
		queue.Enqueue(unitProduction);
		return true;
	}

	public string GetDescription() {
		StringBuilder builder = new StringBuilder();

		var q = queue.ToArray();
		for (int i = 0; i < queue.Count; ++i) {
			builder.Append(i + 1);
			builder.Append('.');
			builder.Append(' ');
			switch (q[i].unitType) {
				case UnitType.HorizontalTank:
					builder.Append("HorizontalTank");
					break;
				case UnitType.VerticalTank:
					builder.Append("VerticalTank");
					break;
			}
			builder.Append(' ');
			builder.Append(100 - (int) (100.0f * q[i].timer / q[i].unitSettings.productionTime));
			builder.Append('%');
			builder.Append('\n');
		}
		
        return builder.ToString();
	}

	private void Awake() {
		units = new Queue<Unit>();
		queue = new Queue<UnitProduction>();
		cachedTransform = GetComponent<Transform>();
	}

	protected override void Update() {
		base.Update();
		if (queue.Count > 0) {
			var unitProduction = queue.Peek();
			unitProduction.timer -= Time.deltaTime;
			if (unitProduction.timer <= 0.0f) {
				var unit = ((GameObject) Instantiate(unitProduction.unitSettings.prefab, Vector3.zero, Quaternion.identity)).GetComponent<Unit>();
				// TODO FIX IT
				unit.unitType = unitProduction.unitType;
				unit.tPosition = tForward;
				unit.SetOwner(owner);
				unit.UpdatePosition(cachedTransform.position + cachedTransform.forward);
				units.Enqueue(unit);
				queue.Dequeue();
			}
		}
	}
}
