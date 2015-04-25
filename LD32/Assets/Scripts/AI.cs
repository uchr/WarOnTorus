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

	public Vector3 tPosition;
	public Vector3 tLeftFactoryPosition;
	public Vector3 tRightFactoryPosition;
	public Vector3 tLeftFactoryPositionForward;
	public Vector3 tRightFactoryPositionForward;

	public Vector3 tLeftTroopPosition;
	public Vector3 tRightTroopPosition;

	private UnitFactory leftFactory;
	private UnitFactory rightFactory;

	private Troop leftTroop;
	private Troop rightTroop;

	private bool isWar = false;

	private void Production(int n) {
		for (int i = 0; i < n; ++i) {
			leftFactory.Production(0);
			rightFactory.Production(0);
		}
	}

	private void Awake() {
		// Init base position
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

		Production(6);
	}

	private void Update() {
		if(!isWar && leftFactory.queue.Count == 0 && rightFactory.queue.Count == 0) {
			
			leftTroop = leftFactory.GetTroop();
			rightTroop = rightFactory.GetTroop();

			leftTroop.Move(tLeftTroopPosition);
			rightTroop.Move(tRightTroopPosition);
			isWar = true;
		}
	}
}
