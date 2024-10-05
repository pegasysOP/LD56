using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public bool TryGetUnitAt(int x, int y, out SimulationUnit unit)
    {
        if (grid[x, y] == null)
        {
            unit = null;
            return false;
        }

        unit = grid[x, y];
        return true;
    }

    /// <summary>
    /// Checks if the space at the given coordinates is empty or not
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>True if tile empty</returns>
    public bool IsTileEmpty(int x, int y)
    {
        return grid[x, y] == null;
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
                if (TryGetUnitAt(i, j, out SimulationUnit unit))
                {
                    units.Add(unit);
                }
            }
        }

        // get random order
        return SimulationUtils.ShuffleUnits(units);
    }
}
