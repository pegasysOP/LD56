using System.Collections.Generic;

public class QueenBeeSimulationUnit : SimulationUnitBase
{
    public QueenBeeSimulationUnit(bool playerUnit) : base(playerUnit)
    {
    }

    protected override void OnInit()
    {
        maxHp = 100;
        attack = 16;
        defence = 8;
        range = 2;
        attackTime = 4;
        specialTime = 20;
    }

    public override UnitType GetUnitType()
    {
        return UnitType.QueenBee;
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
        // DO HEALING ABILITY HERE:
        // heal all allied worker bees
        
        //Get all the units on the board
        List<SimulationUnitBase> units = currentGrid.GetUnits();

        AudioManager.Instance.PlayQueenBeeSpecialAttackClip();

        //Check the unit is a player unit
        foreach (SimulationUnitBase unit in units)
        {
            
            if(unit.IsPlayerUnit() == IsPlayerUnit() && unit.GetUnitType() == UnitType.WorkerBee && unit.GetCurrentHp() > 0)
            {
                unit.Heal(50);
            }
        }
    }
}
