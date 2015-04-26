using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
	public int owner;

	public BuildingType buildingType;

	public int hp = 15;
	
	public Vector3 tPosition;
	public Vector3 tForward;

	public Transform cachedTransform;

	public void Init(int owner) {
		tPosition = Torus.instance.CartesianToTorus(cachedTransform.position);
		// TODO FIX
		tForward = Torus.instance.CartesianToTorus(transform.position + transform.forward * 2.0f);

		this.owner = owner;
		var renderers = GetComponentsInChildren<Renderer>();
		foreach (var r in renderers) {
			if (owner == 0)
				r.material = BalanceSettings.instance.blue;
			else
				r.material = BalanceSettings.instance.red;
		}
	}

	public bool PossibleToBuild() {
		// TODO ADD IT
		return true;
	}

	private void Awake() {
		cachedTransform = GetComponent<Transform>();
	}

	protected virtual void Update() {
		// TODO ADD ANIMATION
		if (hp <= 0) {
			Destroy(gameObject);
			BuildingsManager.instance.Recalculate();
		}
	}
}
