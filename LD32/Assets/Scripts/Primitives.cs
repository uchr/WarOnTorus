using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Primitives {
	public static Mesh GetTorus(float R, float r, int largePartition, int smallPartition) {
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		float largeStep = 2 * Mathf.PI / largePartition;
		float smallStep = 2 * Mathf.PI / smallPartition;
		float phi = 0.0f, teta = 0.0f;
		for (int i = 0; i <= largePartition; ++i) {
			teta = 0.0f;
			Vector3 c = new Vector3(R * Mathf.Cos(phi), R * Mathf.Sin(phi), 0.0f);
			for(int j = 0; j <= smallPartition; ++j) {
				Vector3 p = new Vector3((R + r * Mathf.Cos(teta)) * Mathf.Cos(phi), (R + r * Mathf.Cos(teta)) * Mathf.Sin(phi), r * Mathf.Sin(teta));
				vertices.Add(p);
				normals.Add(p - c);
				uv.Add(new Vector2((float) i / (largePartition), (float) j / (smallPartition)));
				teta += smallStep;
			}
			phi += largeStep;
		}

		for (int i = 0; i < largePartition; ++i) {
			for(int j = 0; j < smallPartition; ++j) {
				triangles.Add(j + (smallPartition + 1) * i);
				triangles.Add(j + (smallPartition + 1) * (i + 1));
				triangles.Add((j + 1) + (smallPartition + 1) * i);

				triangles.Add(j + (smallPartition + 1) * (i + 1));
				triangles.Add((j + 1) + (smallPartition + 1) * (i + 1));
				triangles.Add((j + 1) + (smallPartition + 1) * i);
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.normals  = normals.ToArray();
		mesh.uv = uv.ToArray();
		return mesh;
	}
}
