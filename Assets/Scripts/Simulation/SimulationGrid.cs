using System.Collections.Generic;
using UnityEngine;

public class SimulationGrid
{
    private SimulationUnit[,] grid = new SimulationUnit[8, 8];

    public SimulationGrid (SimulationUnit[,] grid)
    {
        this.grid = grid;
    }

    /// <summary>
    /// Attempts to get the unit at the given coordinates (0 indexed)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="unit"></param>
    /// <returns>Returns false if no unit found</returns>
    public bool TryGetUnitAt(Vector2Int coordinates, out SimulationUnit unit)
    {
        if (grid[coordinates.x, coordinates.y] == null)
        {
            unit = null;
            return false;
        }

        unit = grid[coordinates.x, coordinates.y];
        return true;
    }

    /// <summary>
    /// Removes the unit at the given coordinates from the grid
    /// </summary>
    /// <param name="coordinates"></param>
    public void RemoveUnitAt(Vector2Int coordinates)
    {
        grid[coordinates.x, coordinates.y] = null;
    }

    /// <summary>
    /// Removes the unit from the grid (if it exists on the grid)
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnit(SimulationUnit unit)
    {
        Vector2Int gridCoordinates = GetGridCoordinates(unit);
        if (IsValidGridCoordinates(gridCoordinates))
        {
            RemoveUnitAt(gridCoordinates);
            BoardUtils.KillUnit(gridCoordinates);
        }
    }

    /// <summary>
    /// Damages a unit on the grid (if it exists on the grid)
    /// </summary>
    /// <param name="unit"></param>
    public void DamageUnit(SimulationUnit unit, bool special = false)
    {
        Vector2Int gridCoordinates = GetGridCoordinates(unit);
        if (IsValidGridCoordinates(gridCoordinates))
        {
            BoardUtils.DamageUnit(gridCoordinates, special);
        }
    }

    /// <summary>
    /// Checks if the space at the given coordinates is empty or not
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>True if tile empty</returns>
    public bool IsTileEmpty(Vector2Int coordinates)
    {
        return grid[coordinates.x, coordinates.y] == null;
    }

    /// <summary>
    /// Get's the list of alive units on the grid (shuffled)
    /// </summary>
    /// <returns></returns>
    public List<SimulationUnit> GetUnits()
    {
        List<SimulationUnit> units = new List<SimulationUnit>();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (TryGetUnitAt(new Vector2Int(i, j), out SimulationUnit unit))
                {
                    units.Add(unit);
                }
            }
        }

        // get random order
        return SimulationUtils.ShuffleUnits(units);
    }

    /// <summary>
    /// Get's the grid coordinates ([0-7],[0-7]) of the specified unit
    /// </summary>
    /// <param name="unit"></param>
    /// <returns>Returns (-1,-1) if unit is not found</returns>
    public Vector2Int GetGridCoordinates(SimulationUnit unit)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (TryGetUnitAt(new Vector2Int(i, j), out SimulationUnit gridUnit))
                {
                    if (unit == gridUnit)
                        return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// Checks if the provided <paramref name="coordinates"/> are within the bounds of the grid
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public bool IsValidGridCoordinates(Vector2Int coordinates)
    {
        if (coordinates.x < 0)
            return false;

        if (coordinates.x >= grid.GetLength(0))
            return false;

        if (coordinates.y < 0)
            return false;

        if (coordinates.y >= grid.GetLength(1))
            return false;

        return true;
    }

    /// <summary>
    /// Gets a list of all the units within <paramref name="range"/> of <paramref name="center"/>
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public List<SimulationUnit> GetUnitsInRange(Vector2Int center, int range, bool checkEnemy = true, bool checkPlayer = true)
    {
        List<SimulationUnit> units = new List<SimulationUnit>();

        int gridWidth = grid.GetLength(0);
        int gridHeight = grid.GetLength(1);

        // calculate safe coordinate space
        int xStart = Mathf.Clamp(center.x - range, 0, gridWidth - 1);
        int xEnd = Mathf.Clamp(center.x + range, 0, gridWidth - 1);
        int yStart = Mathf.Clamp(center.y - range, 0, gridHeight - 1);
        int yEnd = Mathf.Clamp(center.y + range, 0, gridHeight - 1);

        for (int x = xStart; x <= xEnd; x++)
        {
            for (int y = yStart; y <= yEnd; y++)
            {
                if (TryGetUnitAt(new Vector2Int(x, y), out SimulationUnit unit))
                {
                    bool isPlayer = unit.IsPlayerUnit();

                    if (isPlayer && checkPlayer || !isPlayer && checkEnemy)
                        units.Add(unit);
                }
            }
        }

        return units;
    }

    public void MoveUnit(Vector2Int start, Vector2Int end)
    {
        if (!IsValidGridCoordinates(start) || !IsValidGridCoordinates(end))
            return;

        // only work if unit exists in space and is moving into an empty space
        if (TryGetUnitAt(start, out SimulationUnit unit) && !TryGetUnitAt(end, out _))
        {
            grid[start.x, start.y] = null;
            grid[end.x, end.y] = unit;

            // update visuals
            BoardUtils.MoveUnit(start, end);

            Debug.Log($"Moving unit from {start} into {end}");
        }
        else
        {
            Debug.LogError($"Invalid movement from {start} into {end}");
        }
    }

    public SimulationUnit[,] GetGridData()
    {
        return grid;
    }
}
