using System;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public class Node : IComparable<Node>
    {
        public Vector2Int Position;
        public int GCost; // movement from start
        public int HCost; // heuristic
        public int FCost => GCost + HCost;
        public Node Parent;

        public Node(Vector2Int position, int gCost, int hCost, Node parent = null)
        {
            Position = position;
            GCost = gCost;
            HCost = hCost;
            Parent = parent;
        }

        // for sorting the nodes
        public int CompareTo(Node other)
        {
            int compareResult = FCost.CompareTo(other.FCost);

            if (compareResult == 0)
            {
                // If FCost is the same, compare by position to avoid duplicates in SortedSet
                return Position.x == other.Position.x
                    ? Position.y.CompareTo(other.Position.y)
                    : Position.x.CompareTo(other.Position.x);
            }

            return compareResult;
        }
    }

    public static bool FindClosestTargetByPathfinding(SimulationGrid grid, SimulationUnitBase currentUnit, out SimulationUnitBase targetUnit, out Vector2Int moveLocation)
    {
        Vector2Int startPos = grid.GetGridCoordinates(currentUnit);

        // set up data
        Dictionary<Vector2Int, SimulationUnitBase> targets = new Dictionary<Vector2Int, SimulationUnitBase>();
        HashSet<Vector2Int> blockedCells = new HashSet<Vector2Int>();

        SimulationUnitBase[,] gridData = grid.GetGridData();
        for (int i = 0; i < gridData.GetLength(0); i++)
        {
            for (int j = 0; j < gridData.GetLength(1); j++)
            {
                SimulationUnitBase unit = gridData[i, j];
                if (unit == null)
                    continue;

                if (currentUnit.IsTargetingPlayer() == unit.IsPlayerUnit())
                    targets[new Vector2Int(i, j)] = unit;
                else
                    blockedCells.Add(new Vector2Int(i, j));
            }
        }

        // find shortest path
        targetUnit = null;
        moveLocation = default;
        List<Vector2Int> shortestPath = null;
        foreach (KeyValuePair<Vector2Int, SimulationUnitBase> targetLocation in targets)
        {
            // Ensure target selection is correct based on confusion status
            if (currentUnit.IsTargetingPlayer() == targetLocation.Value.IsPlayerUnit())
            {
                List<Vector2Int> path = AStarPathfind(grid, startPos, targetLocation.Key, blockedCells);
                if (path != null && path.Count > 0)  // Add check to ensure path has elements
                {
                    // 50% chance to pick if paths are the same length
                    if (shortestPath == null || path.Count < shortestPath.Count ||
                        (path.Count == shortestPath.Count && UnityEngine.Random.Range(0, 2) == 0))
                    {
                        shortestPath = path;
                        moveLocation = path[0];  // Safe to access path[0] now
                        targetUnit = targetLocation.Value;
                    }
                }
            }
        }

        // return falst if no path found
        return !(targetUnit == null);
    }

    // A* Pathfinding method that returns the full path
    private static List<Vector2Int> AStarPathfind(SimulationGrid grid, Vector2Int startPos, Vector2Int targetPos, HashSet<Vector2Int> blockedCells)
    {
        SortedSet<Node> nodes = new SortedSet<Node>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        Node startNode = new Node(startPos, 0, SimulationUtils.GetMoveDistance(startPos, targetPos));
        nodes.Add(startNode);

        while (nodes.Count > 0)
        {
            Node currentNode = nodes.Min;
            nodes.Remove(currentNode);

            // path found
            if (currentNode.Position == targetPos)
                return RetracePath(startNode, currentNode);

            closedSet.Add(currentNode.Position);

            foreach (Vector2Int neighbor in GetNeighbors(grid, currentNode.Position))
            {
                if (closedSet.Contains(neighbor) || blockedCells.Contains(neighbor))
                    continue;

                int newMovementCost = currentNode.GCost + 1;
                Node neighborNode = new Node(neighbor, newMovementCost, SimulationUtils.GetMoveDistance(neighbor, targetPos), currentNode);

                nodes.Add(neighborNode);
            }
        }

        // no path found
        return null;
    }

    private static List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private static List<Vector2Int> GetNeighbors(SimulationGrid grid, Vector2Int currentPosition)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),  // North
            new Vector2Int(0, -1), // South
            new Vector2Int(1, 0),  // East
            new Vector2Int(-1, 0), // West
            new Vector2Int(1, 1),  // North-East
            new Vector2Int(-1, 1), // North-West
            new Vector2Int(1, -1), // South-East
            new Vector2Int(-1, -1) // South-West
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPos = currentPosition + direction;

            if (grid.IsValidGridCoordinates(neighborPos))
                neighbors.Add(neighborPos);
        }

        return neighbors;
    }
}
