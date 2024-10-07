public class WorkerBeeSimulationUnit : SimulationUnitBase
{
    public WorkerBeeSimulationUnit(bool playerUnit) : base(playerUnit)
    {
    }

    protected override void OnInit()
    {
        maxHp = 100;
        attack = 20;
        defence = 8;
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
        if (!CanAttackCurrentTarget(ref currentGrid))
            return;

        currentGrid.DoSpecial(this, currentTarget);

        if (currentTarget.TakeDamage(attack * 2))
            currentGrid.RemoveUnit(currentTarget);
        else
            currentGrid.DamageUnit(currentTarget);
    }
}
