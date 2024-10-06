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
        attack = 8;
        defence = 5;
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
        DoSimpleAttack(ref currentGrid);
    }

    protected override void DoSpecial(ref SimulationGrid currentGrid)
    {
        // DO CONFUSE DEBUFF AOE ABILITY HERE:
        // shoot projectile at target - cause confusion debuff to it and surrounding enemy units
    }
}
