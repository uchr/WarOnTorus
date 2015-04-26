using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Torus : MonoBehaviour {
	private static Torus _instance;
 
	public static Torus instance {
		get	{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<Torus>();
			return _instance;
		}
	}

	public float bR = 10.0f;
	public float sR = 4.0f;
	
	public int largePartition = 50;
	public int smallPartition = 40;

	public float Distance(Vector3 p1, Vector3 p2) {
		Vector3 t = Vector3.zero;
		if (p1.x <= Mathf.PI)
			t.x = Mathf.Abs(p2.x - 2 * Mathf.PI - p1.x) < Mathf.Abs(p2.x - p1.x) ? p2.x - 2 * Mathf.PI : p2.x;
		else
			t.x = Mathf.Abs(p2.x + 2 * Mathf.PI - p1.x) < Mathf.Abs(p2.x - p1.x) ? p2.x + 2 * Mathf.PI : p2.x;

		if (p1.y <= Mathf.PI)
			t.y = Mathf.Abs(p2.y - 2 * Mathf.PI - p1.y) < Mathf.Abs(p2.y - p1.y) ? p2.y - 2 * Mathf.PI : p2.y;
		else
			t.y = Mathf.Abs(p2.y + 2 * Mathf.PI - p1.y) < Mathf.Abs(p2.y - p1.y) ? p2.y + 2 * Mathf.PI : p2.y;
		t -= p1;
		return Mathf.Sqrt(t.x * t.x + t.y * t.y);
	}

	public Vector3 GetNormal2(Vector3 point) {
		return (new Vector3(Mathf.Cos(point.x) * Mathf.Cos(point.y), Mathf.Sin(point.x) * Mathf.Cos(point.y), Mathf.Sin(point.y))).normalized;
	}

	public Vector3 GetNormalFromCartesian(Vector3 point) {
		var t = CartesianToTorus(point);
		return GetNormal2(t);
	}

	public Vector3 TorusToCartesian(Vector3 point) {
		return new Vector3((bR + (point.z + sR) * Mathf.Cos(point.y)) * Mathf.Cos(point.x), (bR + (point.z + sR) * Mathf.Cos(point.y)) * Mathf.Sin(point.x), (point.z + sR) * Mathf.Sin(point.y));
	}

	public Vector3 Repeat(Vector3 point) {
		var t = point;
		t.x = Mathf.Repeat(t.x, 2.0f * Mathf.PI - 0.05f);
		t.y = Mathf.Repeat(t.y, 2.0f * Mathf.PI - 0.05f);
		return t;
	}

	public Vector3 CartesianToTorus(Vector3 point) {
		Vector3 rightPoint = TorusToCartesian(new Vector3(0.0f, 0.0f, 0.0f));
		Vector3 leftPoint = TorusToCartesian(new Vector3(Mathf.PI, 0.0f, 0.0f));
		Vector3 rightUpInflectionPoint = TorusToCartesian(new Vector3(Mathf.PI / 4.0f, 0.0f, 0.0f));
		float distanceToPoint = point.magnitude;
		float distanceToOutside = TorusToCartesian(new Vector3(0.0f, Mathf.PI / 2.0f, 0.0f)).magnitude;
		float distanceToInflection = Vector3.Distance(rightPoint, rightUpInflectionPoint);

		float phi = 0.0f, teta = 0.0f;

		float sqrt = Mathf.Sqrt(sR * sR - point.z * point.z);
		if (distanceToOutside <= distanceToPoint) {
			if (Vector3.Distance(rightPoint, point) <= distanceToInflection || Vector3.Distance(leftPoint, point) <= distanceToInflection) {
				phi = Mathf.Asin(Mathf.Clamp(point.y / (bR + sqrt), -1.0f, 1.0f));
				if (point.x <= 0.0f) phi = 3.0f * Mathf.PI - phi;
			}
			else {
				phi = Mathf.Acos(Mathf.Clamp(point.x / (bR + sqrt), -1.0f, 1.0f));
				if (point.y <= 0.0f) phi = 2.0f * Mathf.PI - phi;
			}
		}
		else {
			if (Vector3.Distance(rightPoint, point) <= distanceToInflection || Vector3.Distance(leftPoint, point) <= distanceToInflection) {
				phi = Mathf.Asin(Mathf.Clamp(point.y / (bR - sqrt), -1.0f, 1.0f));
				if (point.x <= 0.0f) phi = 3.0f * Mathf.PI - phi;
			}
			else {
				phi = Mathf.Acos(Mathf.Clamp(point.x / (bR - sqrt), -1.0f, 1.0f));
				if (point.y <= 0.0f) phi = 2.0f * Mathf.PI - phi;
			}
		}
		
		teta = Mathf.Asin(point.z / sR);
		if (distanceToOutside > distanceToPoint) teta = 3.0f * Mathf.PI - teta;

		phi = Mathf.Repeat(phi, 2.0f * Mathf.PI);
		teta = Mathf.Repeat(teta, 2.0f * Mathf.PI);
		return new Vector3(phi, teta, 0.0f);
	}

	private void Awake() {
		var mesh = Primitives.GetTorus(bR, sR, largePartition, smallPartition);
		var meshFilter = gameObject.GetComponent<MeshFilter>();
		var meshCollider = gameObject.GetComponent<MeshCollider>();
		meshFilter.sharedMesh = mesh;
		meshCollider.sharedMesh = mesh;
	}
}
