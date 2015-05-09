using UnityEngine;
using System.Collections;

public enum BuildingType {
	UnitFactory,
	MineralsProcessing,
	ScinceCenter,
	Shild
}

[System.Serializable]
public class BuildingSettings {
	public GameObject prefab;
	public int cost;
	public float productionTime;
}

public class BuildingsManager : MonoBehaviour {
	private static BuildingsManager _instance;

	public static BuildingsManager instance {
		get {
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<BuildingsManager>();
			return _instance;
		}
	}

	public int maxMinerals = 15;

	public BuildingSettings[] buildings;

	public GameObject mineral;

	public float recalculateTime = 0.05f;
	private float recalculateTimer;
	private bool recalculate;

	public void Sell() {
		UserControls.instance.building.Sell();
	}

	public void Repair() {
		UserControls.instance.building.Repair();
	}

	public void Recalculate() {
		recalculateTimer = recalculateTime;
		recalculate = true;
	}

	public Building CreateBuilding(int id) {
		var building = ((GameObject) Instantiate(buildings[id].prefab, Vector3.zero, Quaternion.identity)).GetComponent<Building>();
		return building;
	}

	private void Awake() {
		var minerals = new GameObject("Minerals");
		Vector3 tPosition;
		for (int i = 0; i < maxMinerals; ++i) {
			tPosition.x = Random.Range(0.0f, 2.0f * Mathf.PI);
			tPosition.y = Random.Range(3.0f * Mathf.PI / 4.0f, 5.0f * Mathf.PI / 4.0f);
			tPosition.z = 0.2f;
			var p = Torus.instance.TorusToCartesian(tPosition);
			if (Physics.Raycast(Vector3.zero, p.normalized, 50.0f, 1 << 14)) {
				--i;
				continue;
			}
			var t = ((GameObject) Instantiate(mineral, p, Quaternion.identity)).transform;
			t.SetParent(minerals.transform);
			t.up = Torus.instance.GetNormal2(tPosition);
		}
	}

	private void Update() {
		if (recalculateTimer > 0.0f) recalculateTimer -= Time.deltaTime;
		if (recalculate && recalculateTimer <= 0.0f) {
			Map.instance.CalculateGrid();
			recalculate = false;
		}
	}
}
