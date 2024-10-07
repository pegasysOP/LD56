using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothSimulationUnit : SimulationUnitBase
{
    public MothSimulationUnit(bool playerUnit) : base(playerUnit)
    {
    }

    protected override void OnInit()
    {
        maxHp = 120;
        attack = 16;
        defence = 10;
        range = 2;
        attackTime = 4;
        specialTime = 17;
    }

    public override UnitType GetUnitType()
    {
        return UnitType.Moth;
    }

    protected override bool DoMovement(ref SimulationGrid currentGrid)
    {
        return DoSimpleMovement(ref currentGrid);
    }
    protected override void DoAttack(ref SimulationGrid currentGrid)
    {
        AudioManager.Instance.PlayProjectileClip();
        DoSimpleAttack(ref currentGrid);
    }

    protected override bool DoSpecial(ref SimulationGrid currentGrid)
    {
        // DO CONFUSE DEBUFF AOE ABILITY HERE:
        // shoot projectile at target - cause confusion debuff to it and surrounding enemy units

        //Check we have a valid target
        if (!CanAttackCurrentTarget(ref currentGrid))
        {
            return false;
        }

        currentGrid.DoSpecial(this, currentTarget);

        AudioManager.Instance.PlayMothSpecialAttackClip();

        Vector2Int targetPos = currentGrid.GetGridCoordinates(currentTarget);

        //If so get all the units within the diagonals of that target

        //Get all the opposing sides units on the board
        Dictionary<SimulationUnitBase, int> units = currentGrid.GetUnitsInRange(currentGrid.GetGridCoordinates(currentTarget), 8, IsPlayerUnit(), !IsPlayerUnit());

        if(units.Count == 0)
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
            if(dx == dy)
            {
                unit.SetConfusionCounter(10);
                //In BASE class: Set targeting and movement to be reversed and decrement confused timer
            }
        }
        return true;
    }
}
