using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour {
	/*private static AI _instance;
 
	public static AI instance
	{
		get
		{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<AI>();
			return _instance;
		}
	}

	public Vector3 tPosition;
	public Vector3 tLeftFactoryPosition;
	public Vector3 tRightFactoryPosition;

	public Vector3 leftArmyPosition;
	public Vector3 rightArmyPosition;
	
	public Factory leftFactory;
	public Factory rightFactory;

	public int maxArmy = 5;

	public float time = 2.0f;
	private float timer = 0.0f;

	public float productionTime = 1.5f;
	private float productionTimer = 0.0f;

	public float peaceTime = 30.0f;
	private float peaceTimer = 0.0f;

	private Torus torus;
	private CreatingObjects creatingObjects;

	private int army = 1;
	private List<Unit> leftArmy;
	private List<Unit> rightArmy;

	private void Awake() {
		torus = Torus.instance;
		creatingObjects = CreatingObjects.instance;

		tPosition = new Vector3(Mathf.PI, 0.0f, 0.0f);
		tLeftFactoryPosition = new Vector3(Mathf.PI - 0.1f, 0.0f, 0.0f);
		tRightFactoryPosition = new Vector3(Mathf.PI + 0.1f, 0.0f, 0.0f);

		leftArmyPosition = new Vector3(Mathf.PI / 2.0f, 0.0f, 0.0f);
		rightArmyPosition = new Vector3(3.0f * Mathf.PI / 2.0f, 0.0f, 0.0f);
		timer = time;
	}

	private void Update() {
		// Check factory
		RaycastHit hit;
		if (!Physics.Raycast(torus.GetCortPoint(tLeftFactoryPosition, 10.0f), -torus.GetCortPoint(tLeftFactoryPosition).normalized, out hit, 50.0f, 1 << 10)) {
			creatingObjects.CreateBuilding(0);
			creatingObjects.construct = false;
			creatingObjects.building.position = torus.GetCortPoint(tLeftFactoryPosition, 0.1f);
			creatingObjects.building.up = torus.GetNormalFromT(tLeftFactoryPosition);
			leftFactory = creatingObjects.building.GetComponent<Factory>();
			leftFactory.SetOwner(1);
			leftFactory.CortToTState();
			creatingObjects.building = null;
			Map.instance.CalculateGrid();
		}
		if (!Physics.Raycast(torus.GetCortPoint(tRightFactoryPosition, 10.0f), -torus.GetCortPoint(tRightFactoryPosition).normalized, out hit, 50.0f, 1 << 10)) {
			creatingObjects.CreateBuilding(0);
			creatingObjects.construct = false;
			creatingObjects.building.position = torus.GetCortPoint(tRightFactoryPosition, 0.1f);
			creatingObjects.building.up = torus.GetNormalFromT(tRightFactoryPosition);
			rightFactory = creatingObjects.building.GetComponent<Factory>();
			rightFactory.SetOwner(1);
			rightFactory.CortToTState();
			creatingObjects.building = null;
			Map.instance.CalculateGrid();
		}

		if (peaceTimer <= 0.0f) {
			if (timer < 0.0f) {
				++army;
				leftFactory.Production(0);
				rightFactory.Production(0);
				timer = time;
			}
			else
				timer -= Time.deltaTime;
			if (army >= 5) {
				productionTimer = productionTime;
				peaceTimer = peaceTime;
			}
		}
		else {
			if (productionTimer <= 0.0f && leftArmy == null) {
				leftArmy = new List<Unit>();
				rightArmy = new List<Unit>();
				var allUnits = GameObject.FindGameObjectsWithTag("Unit");
				foreach (var unit in allUnits) {
					var u = unit.GetComponent<Unit>();
					if (u != null)
						continue;
					if (u.owner == 0)
						continue;
					if (leftArmy.Count <= 5)
						leftArmy.Add(u);
					else
						rightArmy.Add(u);
				}

				foreach (var unit in leftArmy)
					Map.instance.SetPath(torus.GetCortPoint(leftArmyPosition), unit);

				foreach (var unit in rightArmy)
					Map.instance.SetPath(torus.GetCortPoint(rightArmyPosition), unit);

			}
			productionTimer -= Time.deltaTime;
			peaceTimer -= Time.deltaTime;
		}
	}*/
}
