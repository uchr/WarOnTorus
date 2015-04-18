using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Map : MonoBehaviour {
	private class ThreadForUnit {
		Thread thred;
		Unit unit;
	}

	private static Map _instance;
 
    public static Map instance
    {
        get
        {
            if(_instance == null)
                _instance = GameObject.FindObjectOfType<Map>();
            return _instance;
        }
    }

	public Torus torus;

	public int bigTilingsFactor = 3;
	public int smallTilingsFactor = 3;

	private int width;
	private int height;

	private float bigTilings;
	private float smallTilings;

	private float bigR;
	private float smallR;

	private Grid grid;

	public LayerMask layerMask;

	private List<ThreadForUnit> threds;

	//HACK!
	public GridPoint GetGridPoint(Vector3 point) {
		const float eps = 0.3f;
		float phi = 0.0f, teta = 0.0f;
		for (int i = 0; i < width; ++i) {
			teta = 0.0f;
			for(int j = 0; j < height; ++j) {
				Vector3 p = new Vector3((bigR + smallR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta)) * Mathf.Sin(phi), smallR * Mathf.Sin(teta));
				if (Vector3.Distance(p, point) < eps) 
					return new GridPoint(i, j);
				teta += smallTilings;
			}
			phi += bigTilings;
		}

		return null;
	}

	public void CalculateGrid() {
		float phi = 0.0f, teta = 0.0f;
		for (int i = 0; i < width; ++i) {
			teta = 0.0f;
			Vector3 c = new Vector3(bigR * Mathf.Cos(phi), bigR * Mathf.Sin(phi), 0.0f);
			for(int j = 0; j < height; ++j) {
				Vector3 p = new Vector3((bigR + smallR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta)) * Mathf.Sin(phi), smallR * Mathf.Sin(teta));
				if(Physics.Raycast(c, p - c, smallR * 2.0f, layerMask.value))
					grid.grid[i,j] = true;
				else
					grid.grid[i,j] = false;
				teta += smallTilings;
			}
			phi += bigTilings;
		}
	}

	public void SetPath(Vector3 from, Vector3 to, Unit unit) {
		GridPoint start = GetGridPoint(from);
		GridPoint goal = GetGridPoint(to);

		if (grid.grid[goal.x, goal.y])
			return null;

		var path = grid.FindPath(start, goal);
		if (path == null)
			return null;

		List<Vector3> result = new List<Vector3>();
		foreach (var point in path) {
			var i = point.x;
			var j = point.y;
			result.Add(new Vector3((bigR + smallR * Mathf.Cos(smallTilings * j)) * Mathf.Cos(bigTilings * i), (bigR + smallR * Mathf.Cos(smallTilings * j)) * Mathf.Sin(bigTilings * i), smallR * Mathf.Sin(smallTilings * j)));
		}
		return result.ToArray();
	}

	public 

	private void Awake() {
		width = torus.bigTilings * bigTilingsFactor;
		height = torus.smallTilings * bigTilingsFactor;

		bigTilings = 2.0f * Mathf.PI / width;
		smallTilings = 2.0f * Mathf.PI / height;

		bigR = torus.bigR;
		smallR = torus.smallR;
		grid = new Grid(width, height);
		CalculateGrid();
	}

	private void Update() {
		if (grid != null) {

			float phi = 0.0f, teta = 0.0f;
			for (int i = 0; i < width; ++i) {
				teta = 0.0f;
				for(int j = 0; j < height; ++j) {
					Vector3 p1 = new Vector3((bigR + smallR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta)) * Mathf.Sin(phi), smallR * Mathf.Sin(teta));
					Vector3 p2 = new Vector3((bigR + smallR * Mathf.Cos(teta) * 1.5f) * Mathf.Cos(phi), (bigR + smallR * Mathf.Cos(teta) * 1.5f) * Mathf.Sin(phi), smallR * Mathf.Sin(teta) * 1.5f);
					if(grid.grid[i,j])
						Debug.DrawLine(p1, p2, Color.red);
					teta += smallTilings;
				}
				phi += bigTilings;
			}
		}
	}
}


//var gridPoint = torus.GetGridPoint(pointer.position);
//float bigStep = 2.0f * Mathf.PI / (torus.bigSteps * 3.0f);
//float smallStep = 2.0f * Mathf.PI / (torus.smallSteps * 3.0f);
//var i = gridPoint.x;
//var j = gridPoint.y;
//var c = new Vector3(torus.bigR * Mathf.Cos(bigStep * i), torus.bigR * Mathf.Sin(bigStep * i), 0.0f);
//var p = new Vector3((torus.bigR + torus.smallR * Mathf.Cos(smallStep * j)) * Mathf.Cos(bigStep * i), (torus.bigR + torus.smallR * Mathf.Cos(smallStep * j)) * Mathf.Sin(bigStep * i), torus.smallR * Mathf.Sin(smallStep * j));
//Debug.DrawLine(c, c + 2.0f * (p - c), Color.green);