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

public class Grid {
	public int bigCoef;
	public int smallCoef;

	public int width;
	public int height;

	public bool[,] grid;

	public Grid(int width, int height, int bigCoef, int smallCoef) {
		this.width = width;
		this.height = height;
		this.bigCoef = bigCoef;
		this.smallCoef = smallCoef;
		grid = new bool[width, height];
	}

	public List<GridPoint> FindPath(GridPoint start, GridPoint goal) {
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
			var currentNode = openSet.OrderBy(node => node.estimateFullPathLength).First();
			if(currentNode.position.x == goal.x && currentNode.position.y == goal.y) {
				return GetPathForNode(currentNode);
			}
			openSet.Remove(currentNode);
			closedSet.Add(currentNode);
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
		return null;
	}

	private int GetHeuristicPathLength(GridPoint start, GridPoint goal) {
		return (int) Mathf.Sqrt((start.x - goal.x) * (start.x - goal.x) + (start.y - goal.y) * (start.y - goal.y));
	}

	private int GetDistanceBetweenNeighbours() {
		return 1;
	}

	private Collection<PathNode> GetNeighbours(PathNode pathNode, GridPoint goal) {
		var result = new Collection<PathNode>();

		GridPoint[] neighbourPoints = new GridPoint[8];
		neighbourPoints[0] = new GridPoint(pathNode.position.x + 1, pathNode.position.y);
		neighbourPoints[1] = new GridPoint(pathNode.position.x + 1, pathNode.position.y + 1);
		neighbourPoints[2] = new GridPoint(pathNode.position.x, pathNode.position.y + 1);
		neighbourPoints[3] = new GridPoint(pathNode.position.x - 1, pathNode.position.y + 1);
		neighbourPoints[4] = new GridPoint(pathNode.position.x - 1, pathNode.position.y);
		neighbourPoints[5] = new GridPoint(pathNode.position.x - 1, pathNode.position.y - 1);
		neighbourPoints[6] = new GridPoint(pathNode.position.x, pathNode.position.y - 1);
		neighbourPoints[7] = new GridPoint(pathNode.position.x + 1, pathNode.position.y - 1);

		foreach (var point in neighbourPoints) {
			if (point.x < 0 || point.x >= width)
				continue;
			if (point.y < 0 || point.y >= height)
				continue;
			if (grid[point.x, point.y])
				continue;
			var neighbourNode = new PathNode() {
				position = point,
				cameFrom = pathNode,
				pathLengthFromStart = pathNode.pathLengthFromStart + GetDistanceBetweenNeighbours(),
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
