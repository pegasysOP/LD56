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
        // if our current target is both alive, and still in range then there no need to move
        if (CanAttackCurrentTarget(currentGrid))
            return;

        // if there are opposing team units in range then there no need to move, instead select a target unit
        Vector2Int currentPos = currentGrid.GetGridCoordinates(this);
        List<SimulationUnit> unitsInRange = currentGrid.GetUnitsInRange(currentPos, range, isPlayerUnit, !isPlayerUnit);
        if (unitsInRange.Count > 0)
        {
            currentTarget = unitsInRange[Random.Range(0, unitsInRange.Count - 1)];
            return;
        }
                
        if (Pathfinding.FindClosestTargetByPathfinding(currentGrid, this, out SimulationUnit newTarget, out Vector2Int moveLocation))
        {
            currentTarget = newTarget;

            currentGrid.MoveUnit(currentPos, moveLocation);


            Debug.Log((IsPlayerUnit() ? "Player" : "Enemy") + $" {GetUnitType() + " " + currentGrid.GetGridCoordinates(this)} > PATHFINDING TO: {moveLocation}");
        }
        else
        {
            Debug.LogWarning((IsPlayerUnit() ? "Player" : "Enemy") + $" {GetUnitType() + " " + currentGrid.GetGridCoordinates(this)} > PATHFINDING FAILED");
        }
    }
    protected override void DoAttack(ref SimulationGrid currentGrid)
    {
        if (!CanAttackCurrentTarget(currentGrid))
            return;

        currentGrid.DoAttack(this, currentTarget);

        if (currentTarget.TakeDamage(attack))
            currentGrid.RemoveUnit(currentTarget);
        else
            currentGrid.DamageUnit(currentTarget);

        Debug.Log((IsPlayerUnit() ? "Player" : "Enemy") + $" {GetUnitType() + " " + currentGrid.GetGridCoordinates(this)} > ATTACK");
    }

    protected override void DoSpecial(ref SimulationGrid currentGrid)
    {
        // TODO: Have different logic for the special or make code reusable, currently just double damage

        if (!CanAttackCurrentTarget(currentGrid))
            return;

        currentGrid.DoAttack(this, currentTarget);

        if (currentTarget.TakeDamage(attack * 2))
            currentGrid.RemoveUnit(currentTarget);
        else
            currentGrid.DamageUnit(currentTarget, true);

        Debug.Log((IsPlayerUnit() ? "Player" : "Enemy") + $" {GetUnitType() + " " + currentGrid.GetGridCoordinates(this)} > SPECIAL");
    }

    protected bool CanAttackCurrentTarget(SimulationGrid currentGrid)
    {
        if (currentTarget != null && currentTarget.GetCurrentHp() > 0)
        {
            Vector2Int currentPos = currentGrid.GetGridCoordinates(this);
            Vector2Int targetPos = currentGrid.GetGridCoordinates(currentTarget);
            if (currentGrid.IsValidGridCoordinates(targetPos) && SimulationUtils.GetMoveDistance(currentPos, targetPos) <= range)
                return true;
        }

        return false;
    }
}
