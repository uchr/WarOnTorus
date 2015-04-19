using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Factory : MonoBehaviour {
	private BalanceSettings bs;
	private class UnitProduction {
		public float time;
		public int unitID;
	}

	private Transform cachedTransform;
	private Queue<UnitProduction> queue;

	public bool Production(int id) {
		if (queue.Count >= 6)
			return false;
		queue.Enqueue(new UnitProduction(){time = bs.units[id].productionTime, unitID = id});
		return true;
	}

	private void Awake() {
		queue = new Queue<UnitProduction>();
		cachedTransform = GetComponent<Transform>();
		bs = BalanceSettings.instance;
	}

	private void Update() {
		if (queue.Count > 0) {
			var unitProduction = queue.Peek();
			unitProduction.time -= Time.deltaTime;
			if (unitProduction.time <= 0.0f) {
				var unit = ((GameObject) Instantiate(bs.units[unitProduction.unitID].prefab, cachedTransform.position + cachedTransform.forward * 1.0f, Quaternion.identity)).transform;
				unit.up = Torus.instance.GetNormal(unit.position);
				queue.Dequeue();
			}
		}
	}
}
