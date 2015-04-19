using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	public Vector3[] path;
	protected int i = 0;

	public void UpdatePath(Vector3[] path) {
		i = 1;
		this.path = path;
	}
}
