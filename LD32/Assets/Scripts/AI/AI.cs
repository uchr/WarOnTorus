using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AIAction {
	None,
	CreateFactory,
	CreateTroop,
	MoveTroop,
	AttackTroop
}

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

	public int resourceNumber = 2000;

	public AICycle[] cycles;

	private void ChangeState(AICycle cycle) {
		switch (cycle.action) {
			case AIAction.None:
				cycle.action = AIAction.CreateFactory;
				cycle.CreateFactory();
				break;
			case AIAction.CreateFactory:
				cycle.action = AIAction.CreateTroop;
				cycle.CreateTroop();
				break;
			case AIAction.CreateTroop:
				if (cycle.factory.queue.Count == 0) {
					cycle.troop.ChangeTo(cycle.factory.GetUnits(cycle.tanksNumber));
					cycle.action = AIAction.MoveTroop;
					cycle.MoveTroop();
					cycle.timer = cycle.waitTime;
				}
				break;
			case AIAction.MoveTroop:
				if (cycle.troop.Arrived()) {
					if (cycle.timer <= 0.0f) {
						cycle.action = AIAction.AttackTroop;
						cycle.AttackTroop();
					}
					else {
						cycle.timer -= Time.deltaTime;
					}
				}
				break;
			case AIAction.AttackTroop:
				cycle.CheckGoal();
				if (cycle.troop.count == 0) {
					if (cycle.factory == null) {
						cycle.action = AIAction.CreateFactory;
						cycle.CreateFactory();
					}
					else {
						cycle.action = AIAction.CreateTroop;
						cycle.CreateTroop();
					}
					cycle.ClearGoals();
				}
				else if (cycle.isGoal == false) {
					cycle.action = AIAction.AttackTroop;
					cycle.AttackTroop();
				}
				break;
		}
	}

	private void Update() {
		foreach (var cycle in cycles)
			ChangeState(cycle);
	}
}