using System.Collections.Generic;
using UnityEngine;

public class SimulationGrid
{
    private SimulationUnitBase[,] grid = new SimulationUnitBase[8, 8];

    public SimulationGrid (SimulationUnitBase[,] grid)
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
    public bool TryGetUnitAt(Vector2Int coordinates, out SimulationUnitBase unit)
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
    public void RemoveUnit(SimulationUnitBase unit)
    {
        Vector2Int gridCoordinates = GetGridCoordinates(unit);
        if (IsValidGridCoordinates(gridCoordinates))
        {
            RemoveUnitAt(gridCoordinates);
            BoardUtils.KillUnit(gridCoordinates);
            AudioManager.Instance.PlayDeathClip();
        }
    }

    /// <summary>
    /// Damages a unit on the grid (if it exists on the grid)
    /// </summary>
    /// <param name="unit"></param>
    public void DamageUnit(SimulationUnitBase unit, bool special = false)
    {
        Vector2Int gridCoordinates = GetGridCoordinates(unit);
        if (IsValidGridCoordinates(gridCoordinates))
        {
            BoardUtils.DamageUnit(gridCoordinates, special);
        }
    }

    /// <summary>
    /// Causes the attacker to do an attack animation towards the target (if both exist)
    /// </summary>
    /// <param name="unit"></param>
    public void DoAttack(SimulationUnitBase attacker, SimulationUnitBase target)
    {
        Vector2Int attackerCoordinates = GetGridCoordinates(attacker);
        Vector2Int targetCoordinates = GetGridCoordinates(target);
        if (IsValidGridCoordinates(attackerCoordinates) && IsValidGridCoordinates(targetCoordinates))
        {
            BoardUtils.DoAttack(attackerCoordinates, targetCoordinates);
        }
    }

    /// <summary>
    /// Causes the attacker to do an attack animation towards the target (if both exist)
    /// </summary>
    /// <param name="unit"></param>
    public void DoSpecial(SimulationUnitBase attacker, SimulationUnitBase target)
    {
        Vector2Int attackerCoordinates = GetGridCoordinates(attacker);
        Vector2Int targetCoordinates = GetGridCoordinates(target);
        if (IsValidGridCoordinates(attackerCoordinates) && IsValidGridCoordinates(targetCoordinates))
        {
            BoardUtils.DoSpecial(attackerCoordinates, targetCoordinates);
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
    /// Get's the list of alive units on the grid
    /// </summary>
    /// <returns></returns>
    public List<SimulationUnitBase> GetUnits()
    {
        List<SimulationUnitBase> units = new List<SimulationUnitBase>();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (TryGetUnitAt(new Vector2Int(i, j), out SimulationUnitBase unit))
                {
                    units.Add(unit);
                }
            }
        }

        return units;
    }

    /// <summary>
    /// Get's the grid coordinates ([0-7],[0-7]) of the specified unit
    /// </summary>
    /// <param name="unit"></param>
    /// <returns>Returns (-1,-1) if unit is not found</returns>
    public Vector2Int GetGridCoordinates(SimulationUnitBase unit)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (TryGetUnitAt(new Vector2Int(i, j), out SimulationUnitBase gridUnit))
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
    public Dictionary<SimulationUnitBase, int> GetUnitsInRange(Vector2Int center, int range, bool checkEnemy = true, bool checkPlayer = true)
    {
        Dictionary<SimulationUnitBase, int> units = new Dictionary<SimulationUnitBase, int>();

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
                Vector2Int targetPos = new Vector2Int(x, y);

                if (TryGetUnitAt(targetPos, out SimulationUnitBase unit))
                {
                    bool isPlayer = unit.IsPlayerUnit();

                    if (isPlayer && checkPlayer || !isPlayer && checkEnemy)
                        units[unit] = SimulationUtils.GetMoveDistance(center, targetPos);
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
        if (TryGetUnitAt(start, out SimulationUnitBase unit) && !TryGetUnitAt(end, out _))
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

    public Vector2Int GetGridDimensions()
    {
        return new Vector2Int(grid.GetLength(0), grid.GetLength(1));
    }

    public SimulationUnitBase[,] GetGridData()
    {
        return grid;
    }
}
