using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationUnitDemo : SimulationUnit
{
    public SimulationUnitDemo(bool playerUnit) : base(playerUnit)
    {
    }

    protected override void OnInit()
    {
        maxHp = 50;
        attack = 5;
        defence = 2;
        range = 1;
        attackTime = 3;
        specialTime = 12;
    }

    public override UnitType GetUnitType()
    {
        return UnitType.Basic;
    }

    protected override void DoMovement(ref SimulationGrid currentGrid)
    {
        Vector2Int currentPos = currentGrid.GetGridCoordinates(this);
        if (!currentGrid.IsValidGridCoordinates(currentPos))
            return;

        // if there are opposing team units in range, no need to move
        List<SimulationUnit> unitsInRange = currentGrid.GetUnitsInRange(currentPos, range, isPlayerUnit, !isPlayerUnit);
        if (unitsInRange.Count > 0)
            return;

        // TODO: DEFAULT PATHFINDING + DECIDE ON MOVEMENT AMOUNT (PROBABLY JUST 1 ADJACENT TILE)

        Debug.Log((IsPlayerUnit() ? "Player" : "Enemy") + $" {GetUnitType()} > MOVE");
    }
    protected override void DoAttack(ref SimulationGrid currentGrid)
    {
        Vector2Int currentPos = currentGrid.GetGridCoordinates(this);
        if (!currentGrid.IsValidGridCoordinates(currentPos))
            return;

        List<SimulationUnit> unitsInRange = currentGrid.GetUnitsInRange(currentPos, range, isPlayerUnit, !isPlayerUnit);
        if (unitsInRange.Count < 1)
            return;

        SimulationUnit targetUnit = unitsInRange[Random.Range(0, unitsInRange.Count - 1)];
        if (targetUnit.TakeDamage(attack))
            currentGrid.RemoveUnit(targetUnit);

        Debug.Log((IsPlayerUnit() ? "Player" : "Enemy") + $" {GetUnitType()} > ATTACK");
    }

    protected override void DoSpecial(ref SimulationGrid currentGrid)
    {
        // TODO: Have different logic for the special or make code reusable
        
        Vector2Int currentPos = currentGrid.GetGridCoordinates(this);
        if (!currentGrid.IsValidGridCoordinates(currentPos))
            return;

        List<SimulationUnit> unitsInRange = currentGrid.GetUnitsInRange(currentPos, range, isPlayerUnit, !isPlayerUnit);
        if (unitsInRange.Count < 1)
            return;

        SimulationUnit targetUnit = unitsInRange[Random.Range(0, unitsInRange.Count - 1)];
        targetUnit.TakeDamage(attack);

        Debug.Log((IsPlayerUnit() ? "Player" : "Enemy") + $" {GetUnitType()} > SPECIAL");
    }
}
