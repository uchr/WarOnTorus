using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Torus : MonoBehaviour {
	private static Torus _instance;
 
	public static Torus instance
	{
		get
		{
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
		float phi = GetPhi(point);
		return (point - new Vector3(bigR * Mathf.Cos(phi), bigR * Mathf.Sin(phi), 0.0f)).normalized;
	}

	public Vector3 GetNormalFromT(Vector3 tPoint) {
		var p = GetCortPoint(tPoint);
		var c =  new Vector3(bigR * Mathf.Cos(tPoint.x), bigR * Mathf.Sin(tPoint.x), 0.0f);
		return (p - c).normalized;
	}

	private float GetPhi(Vector3 point) {
		return Mathf.Atan2(point.y, point.x);
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
