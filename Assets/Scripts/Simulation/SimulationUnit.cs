using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class SimulationUnit
{
    protected bool isPlayerUnit;

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

    public SimulationUnit(bool playerUnit)
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
        DoMovement(ref currentGrid);

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
    protected abstract void DoMovement(ref SimulationGrid currentGrid);
    protected abstract void DoAttack(ref SimulationGrid currentGrid);
    protected abstract void DoSpecial(ref SimulationGrid currentGrid);
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
}
