using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

public class Arg {
	public GridPoint start;
	public GridPoint goal;
	public List<GridPoint> path;
	public bool suicide;
}

public class Map : MonoBehaviour {
	private class ThreadForUnit {
		public Thread thred;
		public Unit unit;
		public Arg arg;
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
		const float eps = 0.5f;
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

	public Vector3 GetTFromCorе(Vector3 point) {
		var p = GetGridPoint(point);
		return new Vector3(p.x * bigTilings, p.y * smallTilings, 0.0f);
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

	public void SetPath(Vector3 to, Unit unit) {
		if (unit == null)
			return;

		GridPoint s = GetGridPoint(unit.transform.position);
		GridPoint g = GetGridPoint(to);

		if (grid == null) {
			unit.UpdatePath(null);
			return;
		}

		if (grid.grid == null || grid.grid[g.x, g.y]) {
			unit.UpdatePath(null);
			return;
		}

		foreach (var thred in threds) {
			if (thred.unit == unit)
				thred.arg.suicide = true;
			//threds.Remove(thred);
		}

		var t = new ThreadForUnit();
		t.unit = unit;
		t.arg = new Arg() {
			start = s,
			goal = g,
			suicide = false,
			path = null
		};
		t.thred = new Thread(new ParameterizedThreadStart(grid.FindPath));
		t.thred.Start(t.arg);
		threds.Add(t);
	}

	public void CheckThred() {
		//HACK!
		List<ThreadForUnit> deads = new List<ThreadForUnit>(); 
		foreach (var thred in threds) {
			if (!thred.thred.IsAlive)
				deads.Add(thred);
		}
		foreach (var thred in deads) {
			if (thred.arg.path == null) {
				thred.unit.UpdatePath(null);
			}
			else {
				// Calculate path
				List<Vector3> result = new List<Vector3>();
				foreach (var point in thred.arg.path) {
					var i = point.x;
					var j = point.y;
					result.Add(new Vector3(bigTilings * i, smallTilings * j, 0.0f));
				}
				thred.unit.UpdatePath(result.ToArray());
			}
			threds.Remove(thred);
		}
	}

	private void Awake() {
		width = torus.bigTilings * bigTilingsFactor;
		height = torus.smallTilings * bigTilingsFactor;

		bigTilings = 2.0f * Mathf.PI / width;
		smallTilings = 2.0f * Mathf.PI / height;

		bigR = torus.bigR;
		smallR = torus.smallR;

		grid = new Grid(width, height);

		CalculateGrid();

		threds = new List<ThreadForUnit>();
	}

	private void Update() {
		CheckThred();
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