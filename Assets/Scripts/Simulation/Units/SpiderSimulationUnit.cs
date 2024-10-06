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
    }
}
