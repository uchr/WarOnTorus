using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AICycle {
	// Settings
	public Vector3 factoryPosition;
	public Vector3 waitingPosition;
	public int tanksNumber = 5;
	public float waitTime = 1.0f;

	public AIAction action;

	[System.NonSerialized]
	public bool isGoal = false;
	[System.NonSerialized]
	public Building goalBuilding;
	[System.NonSerialized]
	public Unit goalUnit;

	[System.NonSerialized]
	public UnitFactory factory;
	[System.NonSerialized]
	public Troop troop;

	[System.NonSerialized]
	public float timer = 0.0f;

	public AICycle() {
		troop = new Troop();
	}

	public void CreateFactory() {
		factory = (UnitFactory) BuildingsManager.instance.CreateBuilding(0);
		var colliders = factory.GetComponentsInChildren<SphereCollider>();
		foreach (var c in colliders)
			c.enabled = true;
		factory.Init(1, factoryPosition);		
	}

	public void CreateTroop() {
		for (int i = 0; i < tanksNumber; ++i)
			factory.Production(0);
	}

	public void MoveTroop() {
		troop.Move(waitingPosition);
	}

	public void AttackTroop() {
		Unit goalUnit = GetEnemy(troop.center);
		if (goalUnit != null) {
			this.goalUnit = goalUnit;
			troop.AttackUnit(goalUnit);
			isGoal = true;
			return;
		}

		Building goalBuilding = GetBuilding(troop.center);
		if (goalBuilding != null) {
			this.goalBuilding = goalBuilding;
			troop.AttackBuilding(goalBuilding);
			isGoal = true;
		}
	}

	public void ClearGoals() {
		goalBuilding = null;
		goalUnit = null;
		isGoal = false;
	}

	public void CheckGoal() {
		if (goalBuilding == null && goalUnit == null) 
			isGoal = false;
	}

	private Unit GetEnemy(Vector3 troopPosition) {
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
					distance = Torus.instance.Distance(troopPosition, goal.tPosition);
					continue;
				}
				if (Torus.instance.Distance(troopPosition, enemy.tPosition) < distance) {
					goal = enemy;
					distance = Torus.instance.Distance(troopPosition, goal.tPosition);
				}
			}
		}

		if (goal == null || Torus.instance.Distance(troopPosition, goal.tPosition) >= Map.instance.maxDistance)
			return null;

		return goal;
	}

	private Building GetBuilding(Vector3 troopPosition) {
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
					distance = Torus.instance.Distance(troopPosition, goal.tPosition);
					continue;
				}
				if (Torus.instance.Distance(troopPosition, enemyBuilding.tPosition) < distance) {
					goal = enemyBuilding;
					distance = Torus.instance.Distance(troopPosition, goal.tPosition);
				}
			}
		}
	
		if (goal == null || Torus.instance.Distance(troopPosition, goal.tPosition) >= Map.instance.maxDistance)
			return null;
	
		return goal;
	}
}
