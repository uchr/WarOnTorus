using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
	public Vector3 tPosition;
	public Vector3 tForward;

	public void CortToTState() {
		tPosition = Map.instance.GetTFromCorе(transform.position);
		tForward = Map.instance.GetTFromCorе(transform.position + transform.forward * 1.5f);
	}
}
