using UnityEngine;
using System.Collections;

public class CreatingObjects : MonoBehaviour {
	private static CreatingObjects _instance;

	public static CreatingObjects instance
	{
		get
		{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<CreatingObjects>();
			return _instance;
		}
	}

	// Building
	public Vector3 up;
	public bool construct;
	public bool rotate;
	public bool checkMinerals;
	public Transform building;

	// Unit
	public Factory factory;

	private BalanceSettings bs;

	public void CreateBuilding(int id) {
		if (building != null) {
			Destroy(building.gameObject);
		}
		if (id == 2) 
			checkMinerals = true;
		else 
			checkMinerals = false;
		rotate = false;
		construct = true;
		building = ((GameObject) Instantiate(bs.buildings[id].prefab, Vector3.down * 100.0f, Quaternion.identity)).transform;
	}

	public void CreateUnit(int id) {
		factory.Production(id);
	}

	public void DestroyBuilding() {
		if (factory != null)
			Destroy(factory.gameObject);
	}

	private void Awake() {
		bs = BalanceSettings.instance;

		Vector3 tPosition = Vector3.zero;
		for (int i = 0; i < bs.maxMinerals; ++i) {
			tPosition.x = Random.Range(0.0f, 2.0f * Mathf.PI);
			tPosition.y = Random.Range(3.0f * Mathf.PI / 4.0f, 5.0f * Mathf.PI / 4.0f);
			var p = Torus.instance.GetCortPoint(tPosition, 0.2f);
			if (Physics.Raycast(Vector3.zero, p.normalized, 50.0f, 1 << 14)) {
				--i;
				continue;
			}
			var minerals = ((GameObject) Instantiate(bs.minerals, p, Quaternion.identity)).transform;
			minerals.up = Torus.instance.GetNormalFromT(tPosition);
		}
	}
}
