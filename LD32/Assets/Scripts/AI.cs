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
	public float timerBeforeAttack = 0.0f;

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

	private bool isWar = false;

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

	private void Update() {
		if (timerBeforeAttack > 0.0f) timerBeforeAttack -= Time.deltaTime;
		if (!isWar && leftFactory.queue.Count == 0 && rightFactory.queue.Count == 0) {
			timerBeforeAttack = timeBeforeAttack;
			leftTroop = leftFactory.GetTroop();
			rightTroop = rightFactory.GetTroop();

			leftTroop.Move(tLeftTroopPosition);
			rightTroop.Move(tRightTroopPosition);
			isWar = true;
		}

		if (isWar && timerBeforeAttack <= 0.0f) {
			if (leftTroop.InSitu() && leftGoalUnit == null) {
				// TODO FIX FOR VERY LONG DISTANCE
				var units = GameObject.FindGameObjectsWithTag("Unit");
				List<Unit> enemies = new List<Unit>();
				foreach (var unit in units) {
					var u = unit.GetComponent<Unit>();
					if (u != null && u.owner == 0)
						enemies.Add(u);
				}
				if (enemies.Count > 0) {
					Unit goal = null;
					float distance = 0;
					foreach (var enemy in enemies) {
						if (goal == null) {
							goal = enemy;
							distance = Torus.instance.Distance(tPosition, goal.tPosition);
							continue;
						}
						if (Torus.instance.Distance(tPosition, enemy.tPosition) < distance) {
							goal = enemy;
							distance = Torus.instance.Distance(tPosition, goal.tPosition);
						}
					}
					leftGoalUnit = goal;
					leftTroop.AttackUnit(leftGoalUnit);
				}
			}
		}

	}
}
