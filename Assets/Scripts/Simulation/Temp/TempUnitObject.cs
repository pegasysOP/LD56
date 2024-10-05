using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TempUnitObject : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    public void SetUnit(SimulationGrid grid, SimulationUnit unit)
    {
        meshRenderer.material.color = unit.IsPlayerUnit() ? new Color(0f, 0f, 1f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

        Vector2Int unitLocation = grid.GetGridCoordinates(unit);
        transform.position = new Vector3(unitLocation.x + 0.5f, 0f, unitLocation.y + 0.5f);

        // look at target if possible
        SimulationUnit targetUnit = unit.GetCurrentTarget();
        if (targetUnit != null )
        {
            Vector2Int targetUnitLocation = grid.GetGridCoordinates(targetUnit);
            transform.LookAt(new Vector3(targetUnitLocation.x + 0.5f, 0f, targetUnitLocation.y + 0.5f));
        }
    }
}
