using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public float tps = 2f;

    private SimulationGrid grid;

    public EventHandler<bool> GameOver;

    private void Start()
    {
        StartSimulation();
    }

    public void StartSimulation()
    {
        SimulationUnit[,] initialUnitGrid = new SimulationUnit[8, 8];

        // example units
        initialUnitGrid[3, 3] = new SimulationUnitDemo(true); // player demo
        initialUnitGrid[4, 3] = new SimulationUnitDemo(false); // enemy demo

        grid = new SimulationGrid(initialUnitGrid);
        StartCoroutine (DoSimulation());
    }

    private IEnumerator DoSimulation()
    {
        float tickTime = 1f / tps;
        float timer = 0;

        while (true)
        {
            // wait until time for tick
            timer += Time.deltaTime;
            if (timer < tickTime)
            {
                yield return null;
                continue;
            }

            Debug.Log(("Running Game Tick"));

            timer = 0;

            // stop simulation if game over
            if (DoTick())
                break;
        }

        Debug.Log("Game Over");
        yield return null;
    }

    /// <summary>
    /// Executes 1 game tick worth of logic
    /// </summary>
    /// <returns>Returns true if game is over</returns>
    private bool DoTick()
    {
        List<SimulationUnit> units = grid.GetUnits();

        // iterate over copy because we are potentially removing units
        foreach (SimulationUnit unit in new List<SimulationUnit>(units))
        {
            // skip unit if it has already been killed this tick
            if (!units.Contains(unit))
                continue;

            unit.DoTick(grid);
        }

        // check remaining hp to see if game is over
        int playerHp = 0;
        int enemyHp = 0;
        foreach (SimulationUnit unit in units)
        {
            if (unit.IsPlayerUnit())
                playerHp += unit.GetCurrentHp();
            else
                enemyHp += unit.GetCurrentHp();
        }

        if (enemyHp <= 0)
        {
            OnGameOver(true);
            return true;
        }
        else if (enemyHp <= 0)
        {
            OnGameOver(false);
            return true;
        }

        return false;
    }

    private void OnGameOver(bool playerWon)
    {
        if (GameOver != null)
            GameOver(this, playerWon);
    }
}
