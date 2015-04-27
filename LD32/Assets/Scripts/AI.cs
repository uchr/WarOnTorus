using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour {
	private static AI _instance;
 
	public static AI instance
	{
		get
		{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<AI>();
			return _instance;
		}
	}

	public float timeBeforeAttack = 10.0f;
	public float timerBeforeAttackLeft = 0.0f;
	public float timerBeforeAttackRight = 0.0f;

	private Vector3 tPosition;
	private Vector3 tLeftFactoryPosition;
	private Vector3 tRightFactoryPosition;
	private Vector3 tLeftFactoryPositionForward;
	private Vector3 tRightFactoryPositionForward;
	private Vector3 tLeftTroopPosition;
	private Vector3 tRightTroopPosition;

	private UnitFactory leftFactory;
	private UnitFactory rightFactory;

	public Troop leftTroop;
	public Troop rightTroop;

	private Unit leftGoalUnit;
	private Unit rightGoalUnit;

	private Building leftGoalBuilding;
	private Building rightGoalBuilding;

	private bool isWarLeft = false;
	private bool isWarRight = false;

	private float maxDistance = 10.0f;

	private void Production(int n) {
		for (int i = 0; i < n; ++i) {
			leftFactory.Production(0);
			rightFactory.Production(0);
		}
	}

	private void Awake() {
		// Init positions
		tPosition = new Vector3(Mathf.PI, 0.0f, 0.0f);
		tLeftFactoryPosition = new Vector3(Mathf.PI - 0.1f, 0.0f, 0.0f);
		tRightFactoryPosition = new Vector3(Mathf.PI + 0.1f, 0.0f, 0.0f);
		tLeftFactoryPositionForward = new Vector3(Mathf.PI - 0.12f, 0.0f, 0.0f);
		tRightFactoryPositionForward = new Vector3(Mathf.PI + 0.12f, 0.0f, 0.0f);
		tLeftTroopPosition = new Vector3(Mathf.PI / 2.0f, 0.0f, 0.0f);
		tRightTroopPosition = new Vector3(3.0f * Mathf.PI / 2.0f, 0.0f, 0.0f);

		// Init troop
		leftTroop = new Troop();
		rightTroop = new Troop();

		// Create factorys
		leftFactory = (UnitFactory) BuildingsManager.instance.CreateBuilding(0);
		// TODO FIX IT
		leftFactory.cachedTransform.position = Torus.instance.TorusToCartesian(tLeftFactoryPosition);
		leftFactory.cachedTransform.LookAt(Torus.instance.TorusToCartesian(tLeftFactoryPositionForward), Torus.instance.GetNormal2(tLeftFactoryPosition));
		var colliders = leftFactory.GetComponentsInChildren<SphereCollider>();
		foreach (var c in colliders)
			c.enabled = true;
		leftFactory.Init(1);

		rightFactory = (UnitFactory) BuildingsManager.instance.CreateBuilding(0);
		rightFactory.cachedTransform.position = Torus.instance.TorusToCartesian(tRightFactoryPosition);
		rightFactory.cachedTransform.LookAt(Torus.instance.TorusToCartesian(tRightFactoryPositionForward), Torus.instance.GetNormal2(tRightFactoryPosition));
		colliders = rightFactory.GetComponentsInChildren<SphereCollider>();
		foreach (var c in colliders)
			c.enabled = true;
		rightFactory.Init(1);

		Map.instance.CalculateGrid();

		Production(5);
	}

	// TODO FIX FOR VERY LONG DISTANCE !!!
	private Unit GetEnemy(Vector3 troopPoint) {
		Unit goal = null;
		var units = GameObject.FindGameObjectsWithTag("Unit");
		List<Unit> enemies = new List<Unit>();
		foreach (var unitObject in units) {
			var unit = unitObject.GetComponent<Unit>();
			if (unit != null && unit.owner == 0)
				enemies.Add(unit);
		}

		if (enemies.Count > 0) {
			float distance = 0;
			foreach (var enemy in enemies) {
				if (goal == null) {
					goal = enemy;
					distance = Torus.instance.Distance(troopPoint, goal.tPosition);
					continue;
				}
				if (Torus.instance.Distance(troopPoint, enemy.tPosition) < distance) {
					goal = enemy;
					distance = Torus.instance.Distance(troopPoint, goal.tPosition);
				}
			}
		}
		if (goal == null || Torus.instance.Distance(troopPoint, goal.tPosition) >= maxDistance)
			return null;
		return goal;
	}

	// TODO FIX FOR VERY LONG DISTANCE !!!
	private Building GetBuilding(Vector3 troopPoint) {
		Building goal = null;
		var buildings = GameObject.FindGameObjectsWithTag("Building");
		List<Building> enemyBuildings = new List<Building>();
		foreach (var buildingObject in buildings) {
			var building = buildingObject.GetComponent<Building>();
			if (building != null && building.owner == 0)
				enemyBuildings.Add(building);
		}

		if (enemyBuildings.Count > 0) {
			float distance = 0;
			foreach (var enemyBuilding in enemyBuildings) {
				if (goal == null) {
					goal = enemyBuilding;
					distance = Torus.instance.Distance(troopPoint, goal.tPosition);
					continue;
				}
				if (Torus.instance.Distance(troopPoint, enemyBuilding.tPosition) < distance) {
					goal = enemyBuilding;
					distance = Torus.instance.Distance(troopPoint, goal.tPosition);
				}
			}
		}
		if (goal == null || Torus.instance.Distance(troopPoint, goal.tPosition) >= maxDistance)
			return null;
		return goal;
	}

	private void Update() {
		if (timerBeforeAttackLeft > 0.0f) timerBeforeAttackLeft -= Time.deltaTime;
		if (!isWarLeft && leftFactory.queue.Count == 0) {
			timerBeforeAttackLeft = timeBeforeAttack;
			leftTroop = leftFactory.GetTroop();

			leftTroop.Move(tLeftTroopPosition);
			isWarLeft = true;
		}

		if (isWarLeft && timerBeforeAttackLeft <= 0.0f) {
			if (leftTroop.InSitu() && leftGoalUnit == null && leftGoalBuilding == null) {
				leftGoalUnit = GetEnemy(leftTroop.center);
				if (leftGoalUnit != null)
					leftTroop.AttackUnit(leftGoalUnit);
				else {
					leftGoalBuilding = GetBuilding(leftTroop.center);
					if (leftGoalBuilding != null)
						leftTroop.AttackBuilding(leftGoalBuilding);
				}
			}
		}
		if (isWarLeft && leftTroop.GetCount() == 0) {
			isWarLeft = false;
			leftGoalBuilding = null;
			leftGoalUnit = null;
			for (int i = 0; i < 5; ++i) {
				leftFactory.Production(0);
			}
		}
	}
}
