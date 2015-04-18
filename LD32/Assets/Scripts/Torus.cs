using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Torus : MonoBehaviour {
	public float bigR = 10.0f;
	public float smallR = 4.0f;
	public int bigSteps = 50;
	public int smallSteps = 40;

	public LayerMask layerMask;

	private Grid grid;

	public void CalculateGrid() {
		grid = new Grid(bigSteps * 3, smallSteps * 3, 3, 3);

		float bigStep = 2.0f * Mathf.PI / (bigSteps * grid.bigCoef);
		float smallStep = 2.0f * Mathf.PI / (smallSteps * grid.smallCoef);
		float phi = 0.0f, teta = 0.0f;
		for (int i = 0; i < grid.width; ++i) {
			teta = 0.0f;
			Vector3 c = new Vector3(bigR * Mathf.Cos(phi), bigR * Mathf.Sin(phi), 0.0f);
			for(int j = 0; j < grid.height; ++j) {
				Vector3 p = new Vector3((bigR + smallR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta)) * Mathf.Sin(phi), smallR * Mathf.Sin(teta));
				if(Physics.Raycast(c, p - c, smallR * 2.0f, layerMask.value)) {
					grid.grid[i,j] = true;
				}
				else {
					grid.grid[i,j] = false;
				}
				teta += smallStep;
			}
			phi += bigStep;
		}
	}

	public Vector3 GetNormal(Vector3 point) {
		float phi = GetPhi(point);
		return (point - new Vector3(bigR * Mathf.Cos(phi), bigR * Mathf.Sin(phi), 0.0f));
	}

	private float GetPhi(Vector3 point) {
		return Mathf.Atan2(point.y, point.x);
	}

	//HACK!
	public GridPoint GetGridPoint(Vector3 point) {
		float eps = 0.5f;
		float bigStep = 2.0f * Mathf.PI / (bigSteps * 3.0f);
		float smallStep = 2.0f * Mathf.PI / (smallSteps * 3.0f);
		float phi = 0.0f, teta = 0.0f;
		for (int i = 0; i < bigSteps * 3; ++i) {
			teta = 0.0f;
			for(int j = 0; j < smallSteps * 3; ++j) {
				Vector3 p = new Vector3((bigR + smallR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta)) * Mathf.Sin(phi), smallR * Mathf.Sin(teta));
				if ((p - point).magnitude < eps) 
					return new GridPoint(i + 1, j + 1);
				teta += smallStep;
			}
			phi += bigStep;
		}

		return new GridPoint(0, 0);
	}


	public GridPoint[] GetPath(Vector3 from, Vector3 to) {
		GridPoint s = GetGridPoint(from), e = GetGridPoint(to);

		var r = new GridPoint[1];
		return r;
	}
	private void Awake() {
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		//Повторяющиейся вершины в 2*PI для uv
		float bigStep = 2 * Mathf.PI / bigSteps;
		float smallStep = 2 * Mathf.PI / smallSteps;
		float phi = 0.0f, teta = 0.0f;
		for (int i = 0; i <= bigSteps; ++i) {
			teta = 0.0f;
			Vector3 c = new Vector3(bigR * Mathf.Cos(phi), bigR * Mathf.Sin(phi), 0.0f);
			for(int j = 0; j <= smallSteps; ++j) {
				Vector3 p = new Vector3((bigR + smallR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta)) * Mathf.Sin(phi), smallR * Mathf.Sin(teta));
				vertices.Add(p);
				normals.Add(p - c);
				uv.Add(new Vector2((float) i / (bigSteps), (float) j / (smallSteps)));
				teta += smallStep;
			}
			phi += bigStep;
		}

		for (int i = 0; i < bigSteps; ++i) {
			for(int j = 0; j < smallSteps; ++j) {
				triangles.Add(j + (smallSteps + 1) * i);
				triangles.Add(j + (smallSteps + 1) * (i + 1));
				triangles.Add((j + 1) + (smallSteps + 1) * i);

				triangles.Add(j + (smallSteps + 1) * (i + 1));
				triangles.Add((j + 1) + (smallSteps + 1) * (i + 1));
				triangles.Add((j + 1) + (smallSteps + 1) * i);
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
	
	private void OnDrawGizmos() {
		if (gameObject.GetComponent<MeshFilter>().sharedMesh != null && grid != null) {
			Gizmos.color = Color.red;

			float bigStep = 2.0f * Mathf.PI / (bigSteps * grid.bigCoef);
			float smallStep = 2.0f * Mathf.PI / (smallSteps * grid.smallCoef);
			float phi = 0.0f, teta = 0.0f;
			for (int i = 0; i < grid.width; ++i) {
				teta = 0.0f;
				Vector3 c = new Vector3(bigR * Mathf.Cos(phi), bigR * Mathf.Sin(phi), 0.0f);
				for(int j = 0; j < grid.height; ++j) {
					Vector3 p1 = new Vector3((bigR + smallR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta)) * Mathf.Sin(phi), smallR * Mathf.Sin(teta));
					Vector3 p2 = new Vector3((bigR + smallR * Mathf.Cos(teta) * 2.0f) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta) * 2.0f) * Mathf.Sin(phi), smallR * Mathf.Sin(teta) * 2.0f);
					if(grid.grid[i,j]) {
						Gizmos.DrawLine(p1, p2);
					}
					teta += smallStep;
				}
				phi += bigStep;
			}
		}
	}
}
