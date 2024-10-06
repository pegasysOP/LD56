using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleSimulationUnit : SimulationUnitBase
{
    public BeetleSimulationUnit(bool playerUnit) : base(playerUnit)
    {
    }

    protected override void OnInit()
    {
        maxHp = 160;
        attack = 8;
        defence = 6;
        range = 1;
        attackTime = 5;
        specialTime = 15;
    }

    public override UnitType GetUnitType()
    {
        return UnitType.Stag;
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
        // DO LAUNCHING ABILITY HERE:
        // launch target unit to back of grid (if possible)
    }
}
