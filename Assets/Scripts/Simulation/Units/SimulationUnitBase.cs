using System.Collections.Generic;
using UnityEngine;

public abstract class SimulationUnitBase
{
    // base stats
    protected int maxHp;
    protected int attack;
    protected int defence;
    protected int range;
    protected int attackTime;
    protected int specialTime;

    // active stats
    protected int currentHp;
    protected int attackCounter;
    protected int specialCounter;

    protected bool isPlayerUnit;
    protected SimulationUnitBase currentTarget;

    public SimulationUnitBase(bool playerUnit)
    {
        this.isPlayerUnit = playerUnit;

        Init();
    }

    private void Init()
    {
        OnInit();

        attackCounter = 0;
        specialCounter = 0;
        currentHp = maxHp;
    }

    /// <summary>
    /// Executes 1 game tick for this unit
    /// </summary>
    public void DoTick(ref SimulationGrid currentGrid)
    {
        // if a move was made don't keep charging attacks
        if (DoMovement(ref currentGrid))
            return;

        // special overrides attack
        specialCounter++;
        if (specialCounter > specialTime)
        {
            DoSpecial(ref currentGrid);
            specialCounter = 0;
            attackCounter = 0;
            return;
        }

        attackCounter++;
        if (attackCounter > attackTime)
        {
            DoAttack(ref currentGrid);
            attackCounter = 0;
        }
    }

    public virtual bool TakeDamage(int amount)
    {
        int realDamage = amount - defence;
        if (realDamage < 0)
            realDamage = 0;

        currentHp -= realDamage;
        if (currentHp < 0)
            currentHp = 0;

        return currentHp <= 0;
    }

    /// <summary>
    /// Should be used to set up any base stats
    /// </summary>
    protected abstract void OnInit();
    /// <summary>
    /// Returns true if a move was made
    /// </summary>
    /// <param name="currentGrid"></param>
    /// <returns></returns>
    protected abstract bool DoMovement(ref SimulationGrid currentGrid);
    protected abstract void DoAttack(ref SimulationGrid currentGrid);
    protected abstract void DoSpecial(ref SimulationGrid currentGrid);

    /// <summary>
    /// Checks if the current target is within the attack range and above 0 HP
    /// </summary>
    /// <param name="currentGrid"></param>
    /// <returns></returns>
    protected bool CanAttackCurrentTarget(ref SimulationGrid currentGrid)
    {
        if (currentTarget != null && currentTarget.GetCurrentHp() > 0)
        {
            Vector2Int currentPos = currentGrid.GetGridCoordinates(this);
            Vector2Int targetPos = currentGrid.GetGridCoordinates(currentTarget);

            if (currentGrid.IsValidGridCoordinates(targetPos) && SimulationUtils.GetMoveDistance(currentPos, targetPos) <= range)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Assigns a current target if there is one within range
    /// </summary>
    /// <param name="currentGrid"></param>
    /// <returns>False if no target was set</returns>
    protected bool AcquireTarget(ref SimulationGrid currentGrid)
    {
        // if our current target is both alive, and still in range then there no need to move
        if (CanAttackCurrentTarget(ref currentGrid))
            return true;

        // if there are opposing team units in range then there no need to move, instead select a target unit
        Vector2Int currentPos = currentGrid.GetGridCoordinates(this);
        Dictionary<SimulationUnitBase, int> unitsInRange = currentGrid.GetUnitsInRange(currentPos, range, isPlayerUnit, !isPlayerUnit);
        if (unitsInRange.Count > 0)
        {
            // get closest range only
            List<SimulationUnitBase> closestUnits = new List<SimulationUnitBase>();
            int minRange = int.MaxValue;
            foreach (KeyValuePair<SimulationUnitBase, int> unit in unitsInRange)
            {
                if (unit.Value == minRange)
                {
                    closestUnits.Add(unit.Key);
                }
                if (unit.Value < minRange)
                {
                    minRange = unit.Value;
                    closestUnits.Clear();
                    closestUnits.Add(unit.Key);
                }
            }

            if (closestUnits.Count > 0)
            {
                currentTarget = closestUnits[Random.Range(0, closestUnits.Count - 1)];
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Uses the pathfinding to pick the closest target and make 1 move towards it
    /// </summary>
    /// <param name="currentGrid"></param>
    /// <returns>False if no move could be found</returns>
    protected bool SimplePathFind(ref SimulationGrid currentGrid)
    {
        // find target and make 1 move towards it
        if (Pathfinding.FindClosestTargetByPathfinding(currentGrid, this, out SimulationUnitBase newTarget, out Vector2Int moveLocation))
        {
            currentTarget = newTarget;
            currentGrid.MoveUnit(currentGrid.GetGridCoordinates(this), moveLocation);
            return true;
        }
        else
        {
            Debug.LogWarning((IsPlayerUnit() ? "Player" : "Enemy") + $" {GetUnitType() + " " + currentGrid.GetGridCoordinates(this)} > PATHFINDING FAILED");
        }

        return false;
    }

    /// <summary>
    /// Checks if movement needs to be done to get a target, makes a move if so
    /// </summary>
    /// <param name="currentGrid"></param>
    /// <returns>Returns true if a move was made</returns>
    protected bool DoSimpleMovement(ref SimulationGrid currentGrid)
    {
        if (AcquireTarget(ref currentGrid))
            return false;

        return SimplePathFind(ref currentGrid);
    }

    protected void DoSimpleAttack(ref SimulationGrid currentGrid)
    {
        if (!CanAttackCurrentTarget(ref currentGrid))
            return;

        currentGrid.DoAttack(this, currentTarget);

        if (currentTarget.TakeDamage(attack))
            currentGrid.RemoveUnit(currentTarget);
        else
            currentGrid.DamageUnit(currentTarget);
    }

    #region Getters
    public abstract UnitType GetUnitType();
    /// <summary>
    /// Gets the portion (0-1) of hp left
    /// </summary>
    /// <returns></returns>
    public float GetCurrentHpPortion()
    {
        return (float)currentHp / maxHp;
    }

    /// <summary>
    /// Gets the portion (0-1) that special has charged up
    /// </summary>
    /// <returns></returns>
    public float GetSpecialProgress()
    {
        return (float)specialCounter / specialTime;
    }

    public int GetCurrentHp() { return currentHp; }

    public bool IsPlayerUnit() { return isPlayerUnit; }

    /// <summary>
    /// Get's the units current target
    /// </summary>
    /// <returns>Returns null if has no target</returns>
    public SimulationUnitBase GetCurrentTarget()
    {
        return currentTarget;
    }
    #endregion
}
