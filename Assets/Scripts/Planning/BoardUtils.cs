using System.Collections.Generic;
using UnityEngine;

public static class BoardUtils
{
    public static Vector3 BoardToRealCoordinates(Vector2Int boardCoordinates)
    {
        Tile tile = BoardManager.Instance.GetNearestTile(boardCoordinates.x, boardCoordinates.y);
        if (tile == null)
            return new Vector3(-1f, -1f, -1f);

        return tile.transform.position;
    }

    /// <summary>
    /// Moves the unit on the tile <paramref name="start"/> to the tile <paramref name="end"/> if it exists
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public static void MoveUnit(Vector2Int start, Vector2Int end)
    {
        if (BoardManager.Instance.ActiveUnits == null)
            return;

        if (BoardManager.Instance.ActiveUnits.TryGetValue(start, out Unit movingUnit))
        {
            movingUnit.MoveTo(BoardToRealCoordinates(end));

            // update active units dictionary
            BoardManager.Instance.ActiveUnits.Remove(start);
            BoardManager.Instance.ActiveUnits[end] = movingUnit;
        }
        else
        {
            Debug.LogWarning($"Failed to find physical unit at {start}");
        }
    }

    public static void KillUnit(Vector2Int unitLocation)
    {
        if (BoardManager.Instance.ActiveUnits == null)
            return;

        if (BoardManager.Instance.ActiveUnits.TryGetValue(unitLocation, out Unit movingUnit))
        {
            BoardManager.Instance.GetNearestTile(unitLocation.x, unitLocation.y).currentUnit = null;
            BoardManager.Instance.ActiveUnits.Remove(unitLocation);
            movingUnit.Die();
        }
        else
        {
            Debug.LogWarning($"Failed to find physical unit at {unitLocation}");
        }
    }

    public static void DamageUnit(Vector2Int unitLocation, bool special)
    {
        if (BoardManager.Instance.ActiveUnits == null)
            return;

        if (BoardManager.Instance.ActiveUnits.TryGetValue(unitLocation, out Unit damagedUnit))
        {
            damagedUnit.TakeDamage(special);
        }
        else
        {
            Debug.LogWarning($"Failed to find physical unit at {unitLocation}");
        }
    }

    public static void DoAttack(Vector2Int attacker, Vector2Int target)
    {
        if (BoardManager.Instance.ActiveUnits == null)
            return;

        if (BoardManager.Instance.ActiveUnits.TryGetValue(attacker, out Unit attackingUnit))
            attackingUnit.DoAttack(BoardToRealCoordinates(target), SimulationUtils.GetMoveDistance(attacker, target));
        else
            Debug.LogWarning($"Failed to find physical unit at {attacker}");
    }

    public static void DoSpecial(Vector2Int attacker, Vector2Int target)
    {
        if (BoardManager.Instance.ActiveUnits == null)
            return;

        if (BoardManager.Instance.ActiveUnits.TryGetValue(attacker, out Unit attackingUnit))
            attackingUnit.DoSpecial(BoardToRealCoordinates(target), SimulationUtils.GetMoveDistance(attacker, target));
        else
            Debug.LogWarning($"Failed to find physical unit at {attacker}");
    }

    /// <summary>
    /// Updates the UI data for the units from their bar data (health 0-1, special 0-1)
    /// </summary>
    /// <param name="unitData"></param>
    public static void UpdateUnitData(Dictionary<Vector2Int, (float, float)> unitDatas)
    {
        foreach (KeyValuePair<Vector2Int, (float, float)> unitData in unitDatas)
        {
            // protection in case units get destroyed while running this loop (i.e. round ends)
            if (BoardManager.Instance.ActiveUnits.TryGetValue(unitData.Key, out Unit unit))
                unit.UpdateData(unitData.Value.Item1, unitData.Value.Item2);
        }
    }
}