using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class GridPoint {
	public int x;
	public int y;

	public GridPoint(int x, int y) {
		this.x = x;
		this.y = y;
	}
}

public class TorusAStar {
	public class PathNode {
		// Координаты точки
		public GridPoint position;
		// Длина пути от старта - G
		public int pathLengthFromStart;
		// Узел от которо пришли
		public PathNode cameFrom;
		// Примерное расстояние до цели - H
		public int heuristicEstimatePathLength;
		// Ожидаемое расстояние до цели - F
		public int estimateFullPathLength {
			get {
				return pathLengthFromStart + heuristicEstimatePathLength;
			}
		}
	}

	public int width;
	public int height;

	public bool[,] grid;

	public TorusAStar(int width, int height) {
		this.width = width;
		this.height = height;

		grid = new bool[width, height];
	}

	public bool Passable(GridPoint p) {
		if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height)
			return false;
		return grid[p.x, p.y];
	}

	public void FindPath(object arg) {
		GridPoint start = ((Arg) arg).start;
		GridPoint goal = ((Arg) arg).goal;
		var closedSet = new Collection<PathNode>();
		var openSet = new Collection<PathNode>();

		PathNode startNode = new PathNode() {
			position = start,
			cameFrom = null,
			pathLengthFromStart = 0,
			heuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
		};

		openSet.Add(startNode);
		while (openSet.Count > 0) {
			if (((Arg) arg).suicide)
				return;
			var currentNode = openSet.OrderBy(node => node.estimateFullPathLength).First();
			if(currentNode.position.x == goal.x && currentNode.position.y == goal.y) {
				((Arg) arg).path = GetPathForNode(currentNode);
				return;
			}
			openSet.Remove(currentNode);
			closedSet.Add(currentNode);
			// HACK!
			if (currentNode.pathLengthFromStart > 70) {
				return;
			}
			foreach (var neighbourNode in GetNeighbours(currentNode, goal)) {
				if (closedSet.Count(node => (node.position.x == neighbourNode.position.x && node.position.y == neighbourNode.position.y)) > 0)
					continue;
				var openNode = openSet.FirstOrDefault(node => (node.position.x == neighbourNode.position.x && node.position.y == neighbourNode.position.y));
				if (openNode == null)
					openSet.Add(neighbourNode);
				else {
					if (openNode.pathLengthFromStart > neighbourNode.pathLengthFromStart) {
						openNode.cameFrom = currentNode;
						openNode.pathLengthFromStart = neighbourNode.pathLengthFromStart;
					}
				}
			}
		}
		return;
	}

	private int GetHeuristicPathLength(GridPoint start, GridPoint goal) {
		float tMin;
		int distanceX = start.x - goal.x, distanceY = start.y - goal.y;
		int gluingX = 0, gluingY = 0;
		float min = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);
		
		// X gluing
		gluingX = (start.x < (width / 2)) ? distanceX + width : distanceX - width;
		tMin = Mathf.Sqrt(gluingX * gluingX + distanceY * distanceY);
		min = tMin < min ? tMin : min;
		
		// Y gluing
		gluingY = (start.y < (height / 2)) ? distanceY + height : distanceY - height;
		tMin = Mathf.Sqrt(distanceX * distanceX + gluingY * gluingY);
		min = tMin < min ? tMin : min;
		
		// XY gluing
		tMin = Mathf.Sqrt(gluingX * gluingX + gluingY * gluingY);
		min = tMin < min ? tMin : min;
		return Mathf.RoundToInt(min);
	}

	private Collection<PathNode> GetNeighbours(PathNode pathNode, GridPoint goal) {
		var result = new Collection<PathNode>();

		GridPoint[] neighbourPoints = new GridPoint[4];
		neighbourPoints[0] = new GridPoint(pathNode.position.x + 1, pathNode.position.y);
		neighbourPoints[1] = new GridPoint(pathNode.position.x, pathNode.position.y + 1);
		neighbourPoints[2] = new GridPoint(pathNode.position.x - 1, pathNode.position.y);
		neighbourPoints[3] = new GridPoint(pathNode.position.x, pathNode.position.y - 1);

		foreach (var point in neighbourPoints) {
			if (point.x < 0)
				point.x = width - 1;
			if (point.x >= width)
				point.x = 0;

			if (point.y < 0)
				point.y = height - 1;
			if (point.y >= height)
				point.y = 0;

			if (grid[point.x, point.y])
				continue;

			var neighbourNode = new PathNode() {
				position = point,
				cameFrom = pathNode,
				pathLengthFromStart = pathNode.pathLengthFromStart + 1,
				heuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
			};
			result.Add(neighbourNode);
		}
		return result;
	}

	private List<GridPoint> GetPathForNode(PathNode pathNode) {
		var result = new List<GridPoint>();
		var currentNode = pathNode;
		while (currentNode != null) {
			result.Add(currentNode.position);
			currentNode = currentNode.cameFrom;
		}
		result.Reverse();
		return result;
	}
}
