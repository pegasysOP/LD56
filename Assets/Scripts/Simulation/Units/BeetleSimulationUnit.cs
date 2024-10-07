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

    protected override bool DoSpecial(ref SimulationGrid currentGrid)
    {
        // DO LAUNCHING ABILITY HERE:
        // launch target unit to back of grid (if possible)

        // Check for valid target
        if (!CanAttackCurrentTarget(ref currentGrid))
            return false;

        currentGrid.DoSpecial(this, currentTarget);


        AudioManager.Instance.PlayBeetleSpecialAttackClip();

        //Figure out the direction difference between the two units
        Vector2Int currentPos = currentGrid.GetGridCoordinates(this);
        Vector2Int targetPos = currentGrid.GetGridCoordinates(currentTarget);

        Vector2Int direction = targetPos - currentPos;
        float magnitude = direction.magnitude;

        direction = new Vector2Int(
            Mathf.RoundToInt(direction.x / magnitude),
            Mathf.RoundToInt(direction.y / magnitude)
        );

        // Track the grid dimensions for bounds checking
        Vector2Int gridDimensions = currentGrid.GetGridDimensions();
        Vector2Int furthestPos = targetPos;

        // 1. Move diagonally as far as possible (ignoring units along the way)
        for (int i = 1; i < gridDimensions.x; i++)
        {
            Vector2Int diagonalPos = targetPos + direction * i;  // Move both x and y

            // Stop at grid bounds but ignore units for now
            if (currentGrid.IsValidGridCoordinates(diagonalPos))
            {
                furthestPos = diagonalPos;  // Update to the furthest position in diagonal
            }
            else
            {
                break;  // Stop at grid bounds
            }
        }

        // 2. After diagonal movement, continue horizontally as far as possible (ignoring units)
        for (int i = 1; i < gridDimensions.x; i++)
        {
            Vector2Int horizontalPos = furthestPos + new Vector2Int(direction.x * i, 0);  // Move only along x-axis

            // Stop at grid bounds but ignore units for now
            if (currentGrid.IsValidGridCoordinates(horizontalPos))
            {
                furthestPos = horizontalPos;  // Update to the furthest position horizontally
            }
            else
            {
                break;  // Stop at grid bounds
            }
        }

        // 3. Now that we have the furthest position, check for obstacles and place the unit at the closest available spot
        Vector2Int finalPosition = furthestPos;

        // Check from the furthest position backward until we find an empty tile
        for (int i = 0; i <= furthestPos.magnitude; i++)
        {
            Vector2Int checkPos = furthestPos - direction * i;

            if (currentGrid.IsTileEmpty(checkPos))
            {
                finalPosition = checkPos;  // Find the first available empty tile
                break;
            }
        }

        // Only move the unit if the final position is different from its current position
        if (finalPosition != targetPos)
        {
            currentGrid.MoveUnit(targetPos, finalPosition);
        }
        else
        {
            Debug.Log("Tried to move to the same tile");
            return false;  // No movement occurred, return false if needed
        }

        // Apply damage and check if the target is destroyed
        if (currentTarget.TakeDamage(attack * 2))
        {
            currentGrid.RemoveUnit(currentTarget);
        }
        else
        {
            currentGrid.DamageUnit(currentTarget);
        }

        return true;
    }
}
