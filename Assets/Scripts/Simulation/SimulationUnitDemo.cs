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
        return UnitType.Bee;
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

    }
}
