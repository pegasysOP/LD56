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
            BoardManager.Instance.ActiveUnits.Add(end, movingUnit);
        }

    }
}
