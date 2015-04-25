﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitFactory : Building {
	public class UnitProduction {
		public float time;
		public UnitSettings unitSettings;

		public UnitProduction(UnitSettings unitSettings) {
			this.unitSettings = unitSettings;
			time = unitSettings.productionTime;
		}
	}

	private Vector3 spawnPoint;

	public Queue<UnitProduction> queue;

	private Queue<Unit> units;

	public Troop GetTroop() {
		List<Unit> result = new List<Unit>();
		for (int i = 0; i < 6 && units.Count > 0; ++i)
			result.Add(units.Dequeue());
		Troop troop = new Troop();
		troop.units = result.ToArray();
		return troop;
	}

	public bool Production(int id) {
		if (queue.Count >= 6)
			return false;
		var unitProduction = new UnitProduction(UnitsManager.instance.units[id]);
		queue.Enqueue(unitProduction);
		return true;
	}

	private void Awake() {
		units = new Queue<Unit>();
		queue = new Queue<UnitProduction>();
		cachedTransform = GetComponent<Transform>();
	}

	private void Update() {
		if (queue.Count > 0) {
			var unitProduction = queue.Peek();
			unitProduction.time -= Time.deltaTime;
			if (unitProduction.time <= 0.0f) {
				var unit = ((GameObject) Instantiate(unitProduction.unitSettings.prefab, Vector3.zero, Quaternion.identity)).GetComponent<Unit>();
				// TODO FIX IT
				unit.tPosition = tForward;
				unit.SetOwner(owner);
				unit.UpdatePosition(cachedTransform.position + cachedTransform.forward);
				units.Enqueue(unit);
				queue.Dequeue();
			}
		}
	}
}
