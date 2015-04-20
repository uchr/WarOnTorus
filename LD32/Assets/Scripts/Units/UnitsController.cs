using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitsController : MonoBehaviour {
	public List<Unit> units;

	private static UnitsController _instance;

	public static UnitsController instance
	{
		get
		{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<UnitsController>();
			return _instance;
		}
	}

	public void SelectUnits(Vector3 from, Vector3 to) {
		var min = new Vector2(Mathf.Min(from.x, to.x), Mathf.Min(from.y, to.y));
		var max = new Vector2(Mathf.Max(from.x, to.x), Mathf.Max(from.y, to.y));
		var rect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

		var allUnits = GameObject.FindGameObjectsWithTag("Unit");
		List<Unit> inField = new List<Unit>();
		foreach (var unit in allUnits) {
			var p = Camera.main.WorldToScreenPoint(unit.transform.position);
			if (rect.Contains(new Vector2(p.x, p.y)) && !Physics.Raycast(unit.transform.position, Camera.main.transform.position - unit.transform.position, 50.0f, 1 << 8))
				inField.Add(unit.GetComponent<Unit>());
		}
		if (inField.Count > 0) {
			units = new List<Unit>();
			foreach (var unit in inField) {
				var u = unit.GetComponent<Unit>();
				u.fireArea.SetActive(true);
				units.Add(u);
			}
		}
		else {
			units = null;
		}
	}

	public void UnselectUnits() {
		if (units == null) 
			return;
		foreach (var unit in units){
			unit.fireArea.SetActive(false);
		}
		units = null;
	}
}
