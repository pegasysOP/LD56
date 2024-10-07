using System.Collections.Generic;

public class SpiderSimulationUnit : SimulationUnitBase
{
    public SpiderSimulationUnit(bool playerUnit) : base(playerUnit)
    {
    }

    protected override void OnInit()
    {
        maxHp = 60;
        attack = 24;
        defence = 6;
        range = 3;
        attackTime = 6;
        specialTime = 16;
    }

    public override UnitType GetUnitType()
    {
        return UnitType.Spider;
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
        // DO SPEED DEBUFF AOE ABILITY HERE:
        // shoot projectile at target - cause speed debuff to it and surrounding enemy units

        //Check we have a valid target
        if (!CanAttackCurrentTarget(ref currentGrid))
            return;

        //If so get all the units within a 3x3 radius of the target
        Dictionary<SimulationUnitBase, int> units = currentGrid.GetUnitsInRange(currentGrid.GetGridCoordinates(currentTarget), 1, IsPlayerUnit(), !IsPlayerUnit());

        //For each of those units slow down the tick rate.
        foreach(SimulationUnitBase unit in units.Keys)
        {
            unit.SetSlowCounter(10);
        }
    }
}
