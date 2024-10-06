public class WorkerBeeSimulationUnit : SimulationUnitBase
{
    public WorkerBeeSimulationUnit(bool playerUnit) : base(playerUnit)
    {
    }

    protected override void OnInit()
    {
        maxHp = 100;
        attack = 10;
        defence = 4;
        range = 1;
        attackTime = 3;
        specialTime = 12;
    }

    public override UnitType GetUnitType()
    {
        return UnitType.WorkerBee;
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
