using UnityEngine;

public class BeetleSimulationUnit : SimulationUnitBase
{
    public BeetleSimulationUnit(bool playerUnit) : base(playerUnit)
    {
    }

    protected override void OnInit()
    {
        maxHp = 160;
        attack = 16;
        defence = 12;
        range = 1;
        attackTime = 5;
        specialTime = 15;
    }

    public override UnitType GetUnitType()
    {
        return UnitType.Beetle;
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

        // Check for valid target
        if (!CanAttackCurrentTarget(ref currentGrid))
            return;

        AudioManager.Instance.PlayBeetleSpecialAttackClip();

        //Figure out the direction difference between the two units
        Vector2Int currentPos = currentGrid.GetGridCoordinates(this);
        Vector2Int targetPos = currentGrid.GetGridCoordinates(currentTarget);

        Vector2Int direction = targetPos - currentPos;

        float magnitude = direction.magnitude;

        direction = new Vector2Int(
            Mathf.FloorToInt((float)direction.x / (float)magnitude),
            Mathf.FloorToInt((float)direction.y / (float)magnitude)
        );

        Vector2Int moveLocation = targetPos;

        // Track the grid dimensions for bounds checking
        Vector2Int gridDimensions = currentGrid.GetGridDimensions();

        // Move in both y and x direction based on the normalized direction
        for (int i = 0; i < 8; i++)
        {
            // First, check vertical movement
            Vector2Int verticalPos = targetPos + new Vector2Int(0, direction.y * (i + 1));

            // Check if within bounds
            if (currentGrid.IsValidGridCoordinates(verticalPos))
            {
                // Check if the tile is empty
                if (currentGrid.IsTileEmpty(verticalPos))
                {
                    moveLocation = verticalPos;  // Update to this valid empty location
                }
            }

            // Then, check horizontal movement
            Vector2Int horizontalPos = targetPos + new Vector2Int(direction.x * (i + 1), 0);

            // Check if within bounds
            if (currentGrid.IsValidGridCoordinates(horizontalPos))
            {
                // Check if the tile is empty
                if (currentGrid.IsTileEmpty(horizontalPos))
                {
                    moveLocation = horizontalPos;  // Update to this valid empty location
                }
            }
        }

        // Only move the unit if the final moveLocation is different from its current position
        if (moveLocation != targetPos)
        {
            currentGrid.MoveUnit(targetPos, moveLocation);
        }
        else
        {
            Debug.Log("Tried to move to the same tile");
        }
        if (currentTarget.TakeDamage(attack * 2))
            currentGrid.RemoveUnit(currentTarget);
        else
            currentGrid.DamageUnit(currentTarget);
    }
}
