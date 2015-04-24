using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UnitType {
	HorizontalTank,
	VerticalTank
}

[System.Serializable]
public class UnitSettings {
	public GameObject prefab;
	public int cost;
	public float productionTime;
}

public class UnitsManager : MonoBehaviour {
	private static UnitsManager _instance;

	public static UnitsManager instance {
		get {
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<UnitsManager>();
			return _instance;
		}
	}

	public UnitSettings[] units;

	public Troop GetTroopFromSelectionArea(Vector3 from, Vector3 to) {
		var min = new Vector2(Mathf.Min(from.x, to.x), Mathf.Min(from.y, to.y));
		var max = new Vector2(Mathf.Max(from.x, to.x), Mathf.Max(from.y, to.y));
		var rect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

		var units = GameObject.FindGameObjectsWithTag("Unit");
		List<Unit> result = new List<Unit>();
		foreach (var unit in units) {
			var p = Camera.main.WorldToScreenPoint(unit.transform.position);
			if (unit.GetComponent<Unit>().owner == 0 && result.Count <= 6 && rect.Contains(new Vector2(p.x, p.y)) && !Physics.Raycast(unit.transform.position, Camera.main.transform.position - unit.transform.position, 50.0f, 1 << 8))
				result.Add(unit.GetComponent<Unit>());
		}


		if (result.Count > 0) {
			Troop troop = new Troop();
			troop.units = result.ToArray();
			troop.Select();
			return troop;
		}
		return null;
	}
}
