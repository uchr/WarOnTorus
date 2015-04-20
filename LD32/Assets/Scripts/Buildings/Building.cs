using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
	public int owner;

	public int hp = 15;
	
	public Vector3 tPosition;
	public Vector3 tForward;

	public void CortToTState() {
		tPosition = Map.instance.GetTFromCorе(transform.position);
		tForward = Map.instance.GetTFromCorе(transform.position + transform.forward * 1.5f);
	}

	private void Update() {
		if (hp <= 0)
			Destroy(gameObject);
	}

	public void SetOwner(int owner) {
		this.owner = owner;
		var renderers = GetComponentsInChildren<Renderer>();
		foreach (var r in renderers) {
			if (owner == 0)
				r.material = BalanceSettings.instance.blue;
			else
				r.material = BalanceSettings.instance.red;
		}
	}
}
