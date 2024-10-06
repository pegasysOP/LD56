public class QueenBeeSimulationUnit : SimulationUnitBase
{
    public QueenBeeSimulationUnit(bool playerUnit) : base(playerUnit)
    {
    }

    protected override void OnInit()
    {
        maxHp = 100;
        attack = 8;
        defence = 4;
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
    }
}
