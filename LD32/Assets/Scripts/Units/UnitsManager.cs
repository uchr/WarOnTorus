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

	public int maxUnits = 10;
	public int currentUnits = 0;

	public UnitSettings[] units;

	public float updatePathRadius = 0.2f;

	public List<Unit> GetUnitsFromSelectionArea(Vector3 from, Vector3 to, int number) {
		var min = new Vector2(Mathf.Min(from.x, to.x), Mathf.Min(from.y, to.y));
		var max = new Vector2(Mathf.Max(from.x, to.x), Mathf.Max(from.y, to.y));
		var rect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

		var unitObjects = GameObject.FindGameObjectsWithTag("Unit");
		List<Unit> result = new List<Unit>();
		foreach (var unitObject in unitObjects) {
			if (result.Count == number) break;

			var p = Camera.main.WorldToScreenPoint(unitObject.transform.position);
			var unit = unitObject.GetComponent<Unit>();
			if (unit != null && unit.owner == 0 && rect.Contains(new Vector2(p.x, p.y)) && !Physics.Raycast(unitObject.transform.position, Camera.main.transform.position - unitObject.transform.position, 50.0f, 1 << 8))
				result.Add(unit);
		}

		return result;
	}
}
