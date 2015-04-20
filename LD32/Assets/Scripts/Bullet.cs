using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Vector3 goal;

	public Building building;
	public Unit unit;

	public float speed;

	private Transform cachedTransform;

	private void Awake() {
		cachedTransform = GetComponent<Transform>();
	}
	
	private void Update () {
		cachedTransform.position += cachedTransform.up * speed * Time.deltaTime;
		if (Vector3.Distance(cachedTransform.position, goal) < 0.1f) {
			if (building != null) {
				//Debug.Log(building.hp);
				building.hp -= 1;
			}
			if (unit != null) {
				//Debug.Log(unit.hp);
				unit.hp -= 1;
			}
			Destroy(gameObject);
		}
	}
}
