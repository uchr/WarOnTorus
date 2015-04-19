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
	public Transform building;

	// Unit
	public Factory factory;

	private BalanceSettings bs;

	public void CreateBuilding(int id) {
		if (building != null) {
			Destroy(building.gameObject);
		}
		rotate = false;
		construct = true;
		building = ((GameObject) Instantiate(bs.buildings[id].prefab, Vector3.down * 100.0f, Quaternion.identity)).transform;
	}

	public void CreateUnit(int id) {
		factory.Production(id);
	}

	private void Awake() {
		bs = BalanceSettings.instance;
	}
}
