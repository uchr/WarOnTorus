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

	public float bigR = 10.0f;
	public float smallR = 4.0f;
	
	public int bigTilings = 50;
	public int smallTilings = 40;

	public Vector3 GetCortPoint(Vector3 point, float height = 0.0f) {
		return new Vector3((bigR + (height + smallR) * Mathf.Cos(point.y)) * Mathf.Cos(point.x), (bigR + (height + smallR) * Mathf.Cos(point.y)) * Mathf.Sin(point.x), (height + smallR) * Mathf.Sin(point.y));
	}

	public Vector3 GetNormal(Vector3 point) {
		float phi = Mathf.Atan2(point.y, point.x);
		return (point - new Vector3(bigR * Mathf.Cos(phi), bigR * Mathf.Sin(phi), 0.0f)).normalized;
	}

	public Vector3 GetNormalFromT(Vector3 tPoint) {
		var p = GetCortPoint(tPoint);
		var c =  new Vector3(bigR * Mathf.Cos(tPoint.x), bigR * Mathf.Sin(tPoint.x), 0.0f);
		return (p - c).normalized;
	}

	public Vector3 GetNormal2(Vector3 point) {
		return new Vector3(Mathf.Cos(point.x) * Mathf.Cos(point.y), Mathf.Sin(point.x) * Mathf.Cos(point.y), Mathf.Sin(point.y));
	}

	public Vector3 CartesianToTorus(Vector3 point) {
		Vector3 rightPoint = GetCortPoint(new Vector3(0.0f, 0.0f, 0.0f));
		Vector3 leftPoint = GetCortPoint(new Vector3(Mathf.PI, 0.0f, 0.0f));
		Vector3 rightUpInflectionPoint = GetCortPoint(new Vector3(Mathf.PI / 4.0f, 0.0f, 0.0f));
		float distanceToPoint = point.magnitude;
		float distanceToOutside = GetCortPoint(new Vector3(0.0f, Mathf.PI / 2.0f, 0.0f)).magnitude;
		float distanceToInflection = Vector3.Distance(rightPoint, rightUpInflectionPoint);

		float phi = 0.0f, teta = 0.0f;

		float sqrt = Mathf.Sqrt(smallR * smallR - point.z * point.z);
		if (distanceToOutside <= distanceToPoint) {
			if (Vector3.Distance(rightPoint, point) <= distanceToInflection || Vector3.Distance(leftPoint, point) <= distanceToInflection) {
				phi = Mathf.Asin(Mathf.Clamp(point.y / (bigR + sqrt), -1.0f, 1.0f));
				if (point.x <= 0.0f) phi = 3.0f * Mathf.PI - phi;
			}
			else {
				phi = Mathf.Acos(Mathf.Clamp(point.x / (bigR + sqrt), -1.0f, 1.0f));
				if (point.y <= 0.0f) phi = 2.0f * Mathf.PI - phi;
			}
		}
		else {
			if (Vector3.Distance(rightPoint, point) <= distanceToInflection || Vector3.Distance(leftPoint, point) <= distanceToInflection) {
				phi = Mathf.Asin(Mathf.Clamp(point.y / (bigR - sqrt), -1.0f, 1.0f));
				if (point.x <= 0.0f) phi = 3.0f * Mathf.PI - phi;
			}
			else {
				phi = Mathf.Acos(Mathf.Clamp(point.x / (bigR - sqrt), -1.0f, 1.0f));
				if (point.y <= 0.0f) phi = 2.0f * Mathf.PI - phi;
			}
		}
		
		teta = Mathf.Asin(point.z / smallR);
		if (distanceToOutside > distanceToPoint) teta = 3.0f * Mathf.PI - teta;
		phi = Mathf.Repeat(phi, 2.0f * Mathf.PI);
		teta = Mathf.Repeat(teta, 2.0f * Mathf.PI);
		//var t = GetCortPoint(new Vector3(phi, teta, 0.0f));
		//Debug.DrawLine(t, GetCortPoint(new Vector3(phi, teta, 0.0f), 10.0f), Color.green);
		//Debug.Log("Length: " + Vector3.Distance(t, point));
		return new Vector3(phi, teta, 0.0f);
	}

	private void Awake() {
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		//Повторяющиейся вершины в 2*PI для uv
		float bigStep = 2 * Mathf.PI / bigTilings;
		float smallStep = 2 * Mathf.PI / smallTilings;
		float phi = 0.0f, teta = 0.0f;
		for (int i = 0; i <= bigTilings; ++i) {
			teta = 0.0f;
			Vector3 c = new Vector3(bigR * Mathf.Cos(phi), bigR * Mathf.Sin(phi), 0.0f);
			for(int j = 0; j <= smallTilings; ++j) {
				Vector3 p = new Vector3((bigR + smallR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta)) * Mathf.Sin(phi), smallR * Mathf.Sin(teta));
				vertices.Add(p);
				normals.Add(p - c);
				uv.Add(new Vector2((float) i / (bigTilings), (float) j / (smallTilings)));
				teta += smallStep;
			}
			phi += bigStep;
		}

		for (int i = 0; i < bigTilings; ++i) {
			for(int j = 0; j < smallTilings; ++j) {
				triangles.Add(j + (smallTilings + 1) * i);
				triangles.Add(j + (smallTilings + 1) * (i + 1));
				triangles.Add((j + 1) + (smallTilings + 1) * i);

				triangles.Add(j + (smallTilings + 1) * (i + 1));
				triangles.Add((j + 1) + (smallTilings + 1) * (i + 1));
				triangles.Add((j + 1) + (smallTilings + 1) * i);
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.normals  = normals.ToArray();
		mesh.uv = uv.ToArray();

		var meshFilter = gameObject.GetComponent<MeshFilter>();
		var meshCollider = gameObject.GetComponent<MeshCollider>();
		meshFilter.sharedMesh = mesh;
		meshCollider.sharedMesh = mesh;
	}
}
