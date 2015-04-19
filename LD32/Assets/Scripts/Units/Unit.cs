using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	public Vector3[] path;
	protected int i = 0;

	public void UpdatePath(Vector3[] path) {
		i = 1;
		this.path = path;
	}

	private void OnDrawGizmos() {
		if (path != null) {
			Gizmos.color = Color.green;
			Vector3 prev = transform.position;
			foreach (var point in path) {
				Gizmos.DrawLine(prev, point);
				prev = point;
			}
		}
	}
}
