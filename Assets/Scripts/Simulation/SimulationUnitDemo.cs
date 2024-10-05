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
        attackTime = 2;
        specialTime = 4;
    }

    public override UnitType GetUnitType()
    {
        return UnitType.Basic;
    }

    protected override void DoAttack(SimulationGrid currentGrid)
    {
        Debug.Log($"Unit doing Attack [player={IsPlayerUnit()}]");
    }

    protected override void DoSpecial(SimulationGrid currentGrid)
    {
        Debug.Log($"Unit doing Special [player={IsPlayerUnit()}]");
    }
}
