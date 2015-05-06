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

	public float maxDistance = 3.0f;

	public int largePartitionFactor = 3;
	public int smallPartitionFactor = 3;

	private int width;
	private int height;

	private float largePartition;
	private float smallPartition;

	private float bR;
	private float sR;

	private TorusAStar torusAStar;

	public LayerMask layerMask;

	private List<ThreadForUnit> threds;

	public void CalculateGrid() {
		float phi = 0.0f, teta = 0.0f;
		for (int i = 0; i < width; ++i) {
			teta = 0.0f;
			Vector3 c = new Vector3(bR * Mathf.Cos(phi), bR * Mathf.Sin(phi), 0.0f);
			for(int j = 0; j < height; ++j) {
				Vector3 p = new Vector3((bR + sR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bR + sR * Mathf.Cos(teta)) * Mathf.Sin(phi), sR * Mathf.Sin(teta));
				if(Physics.Raycast(c, p - c, sR * 1.2f, layerMask.value))
					torusAStar.grid[i,j] = true;
				else
					torusAStar.grid[i,j] = false;
				teta += smallPartition;
			}
			phi += largePartition;
		}
	}

	public void SetPath(Vector3 to, Unit unit) {
		if (unit == null)
			return;

		GridPoint s = GetGridPoint(unit.tPosition);
		GridPoint g = GetGridPoint(to);

		if (torusAStar.Passable(g)) {
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
		t.thred = new Thread(new ParameterizedThreadStart(torusAStar.FindPath));
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
				Debug.Log("Path not found " + thred.arg.goal.x + " " + thred.arg.goal.y);
				thred.unit.UpdatePath(null);
			}
			else {
				// Calculate path
				List<Vector3> result = new List<Vector3>();
				foreach (var point in thred.arg.path) {
					var i = point.x;
					var j = point.y;
					result.Add(new Vector3(largePartition * i, smallPartition * j, 0.0f));
				}
				thred.unit.UpdatePath(result.ToArray());
			}
			threds.Remove(thred);
		}
	}
	
	private GridPoint GetGridPoint(Vector3 point) {
		if (point.x < 0.0f || point.x >= 2.0f * Mathf.PI || point.y < 0.0f || point.y >= 2.0f * Mathf.PI)
			Debug.LogError("Outside point " + point);

		int i = Mathf.RoundToInt(point.x / largePartition), j = Mathf.RoundToInt(point.y / smallPartition);

		return new GridPoint(i, j);
	}

	private void Awake() {
		width = torus.largePartition * largePartitionFactor;
		height = torus.smallPartition * largePartitionFactor;

		largePartition = 2.0f * Mathf.PI / width;
		smallPartition = 2.0f * Mathf.PI / height;

		bR = torus.bR;
		sR = torus.sR;

		torusAStar = new TorusAStar(width, height);

		CalculateGrid();

		threds = new List<ThreadForUnit>();
	}

	private void Update() {
		CheckThred();
		if (torusAStar != null) {
			float phi = 0.0f, teta = 0.0f;
			for (int i = 0; i < width; ++i) {
				teta = 0.0f;
				for(int j = 0; j < height; ++j) {
					Vector3 p1 = new Vector3((bR + sR * Mathf.Cos(teta)) * Mathf.Cos(phi), (bR + sR * Mathf.Cos(teta)) * Mathf.Sin(phi), sR * Mathf.Sin(teta));
					Vector3 p2 = new Vector3((bR + sR * Mathf.Cos(teta) * 1.5f) * Mathf.Cos(phi), (bR + sR * Mathf.Cos(teta) * 1.5f) * Mathf.Sin(phi), sR * Mathf.Sin(teta) * 1.5f);
					if(torusAStar.grid[i,j])
						Debug.DrawLine(p1, p2, Color.red);
					teta += smallPartition;
				}
				phi += largePartition;
			}
		}
	}
}