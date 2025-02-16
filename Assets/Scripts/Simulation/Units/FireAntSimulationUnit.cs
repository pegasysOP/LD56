using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAntSimulationUnit : SimulationUnitBase
{
    public FireAntSimulationUnit(bool playerUnit) : base(playerUnit)
    {
    }

    protected override void OnInit()
    {
        maxHp = 100;
        attack = 10;
        defence = 20;
        range = 1;
        attackTime = 5;
        specialTime = 17;
    }

    public override UnitType GetUnitType()
    {
        return UnitType.FireAnt;
    }

    protected override void DoAttack(ref SimulationGrid currentGrid)
    {
        DoSimpleAttack(ref currentGrid);
    }

    protected override bool DoMovement(ref SimulationGrid currentGrid)
    {
        return DoSimpleMovement(ref currentGrid);
    }

    protected override bool DoSpecial(ref SimulationGrid currentGrid)
    {
        //Check we have a valid target
        if (!CanAttackCurrentTarget(ref currentGrid))
        {
            return false;
        }

        currentGrid.DoSpecial(this, currentTarget);

        //Play fire ant special attack sound

        Vector2Int targetPos = currentGrid.GetGridCoordinates(currentTarget);

        //Get all the opposing sides units on the board
        Dictionary<SimulationUnitBase, int> units = currentGrid.GetUnitsInRange(currentGrid.GetGridCoordinates(currentTarget), 8, IsPlayerUnit(), !IsPlayerUnit());

        if (units.Count == 0)
        {
            return false;
        }

        //Now only set confused for those tht are on one of the four diagonal angles
        foreach (SimulationUnitBase unit in units.Keys)
        {
            Vector2Int unitPos = currentGrid.GetGridCoordinates(unit);
            int dx = Mathf.Abs(unitPos.x - targetPos.x);
            int dy = Mathf.Abs(unitPos.y - targetPos.y);

            //This is true for (2,2), (2,-2), (-2,2), (-2,-2) for example 
            if (dx == dy)
            {
                unit.SetBurnedCounter(10);
                unit.Debuff();
                //In BASE class: Set targeting and movement to be reversed and decrement confused timer
            }
        }
        return true;
    }
}
