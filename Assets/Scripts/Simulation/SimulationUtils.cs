using System.Collections.Generic;
using UnityEngine;

public static class SimulationUtils
{
    public static List<SimulationUnit> ShuffleUnits<SimulationUnit>(List<SimulationUnit> list)
    {
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }

        return list;
    }

    public static int GetManhattenDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
