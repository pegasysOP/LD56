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

    public static bool FindClosestTargetByPathfinding(SimulationGrid grid, SimulationUnit currentUnit, out SimulationUnit targetUnit, out Vector2Int moveLocation)
    {
        Vector2Int startPos = grid.GetGridCoordinates(currentUnit);

        // set up data
        Dictionary<Vector2Int, SimulationUnit> targets = new Dictionary<Vector2Int, SimulationUnit>();
        HashSet<Vector2Int> blockedCells = new HashSet<Vector2Int>();

        SimulationUnit[,] gridData = grid.GetGridData();
        for (int i = 0; i < gridData.GetLength(0); i++)
        {
            for (int j = 0; j < gridData.GetLength(1); j++)
            {
                SimulationUnit unit = gridData[i, j];
                if (unit == null)
                    continue;

                if (currentUnit.IsPlayerUnit() == unit.IsPlayerUnit())
                    blockedCells.Add(new Vector2Int(i, j));
                else
                    targets.Add(new Vector2Int(i, j), unit);
            }
        }

        // find shortest path
        targetUnit = null;
        moveLocation = default;
        List<Vector2Int> shortestPath = null;
        foreach (KeyValuePair<Vector2Int, SimulationUnit> targetLocation in targets)
        {
            List<Vector2Int> path = AStarPathfind(grid, startPos, targetLocation.Key, blockedCells);
            if (path == null)
                continue;

            // 50% chance to pick if they are the same length for at least a little randomness
            if (shortestPath == null || path.Count < shortestPath.Count || (path.Count == shortestPath.Count && UnityEngine.Random.Range(0, 1) < 0.5f))
            {
                shortestPath = path;
                moveLocation = path[0];
                targetUnit = targetLocation.Value;
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

        Node startNode = new Node(startPos, 0, SimulationUtils.GetManhattenDistance(startPos, targetPos));
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
                Node neighborNode = new Node(neighbor, newMovementCost, SimulationUtils.GetManhattenDistance(neighbor, targetPos), currentNode);

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
