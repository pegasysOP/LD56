using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public float tps = 5f;
    public TempSimUnitUIElement uiElementPrefab;
    public Transform playerUiContainer;
    public Transform enemyUiContainer;

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
        initialUnitGrid[3, 4] = new SimulationUnitDemo(true);
        initialUnitGrid[3, 5] = new SimulationUnitDemo(true);

        initialUnitGrid[4, 3] = new SimulationUnitDemo(false); // enemy demo
        initialUnitGrid[4, 4] = new SimulationUnitDemo(false);
        initialUnitGrid[4, 5] = new SimulationUnitDemo(false);

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
            bool gameOver = DoTick();            

            // UPDATE UI ELEMENTS (TEMP)
            for (int i = playerUiContainer.childCount - 1; i >= 0; i--)
                DestroyImmediate(playerUiContainer.GetChild(i).gameObject);
            for (int i = enemyUiContainer.childCount - 1; i >= 0; i--)
                DestroyImmediate(enemyUiContainer.GetChild(i).gameObject);

            List<SimulationUnit> sortedUnits = grid.GetUnits();
            sortedUnits.Sort((SimulationUnit a, SimulationUnit b) => a.GetCurrentHpPortion() > b.GetCurrentHpPortion() ? -1 : 1);
            foreach (SimulationUnit unit in sortedUnits)
            {
                Transform targetContainer = unit.IsPlayerUnit() ? playerUiContainer : enemyUiContainer;
                Instantiate(uiElementPrefab, targetContainer).SetUnit(grid, unit);
            }

            if (gameOver)
                break;
        }

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
        foreach (SimulationUnit unit in units)
        {
            // skip unit if it has already been killed this tick
            if (unit.GetCurrentHp() <= 0)
                continue;

            unit.DoTick(ref grid);
        }

        // check remaining hp to see if game is over
        float playerHp = 0;
        float enemyHp = 0;
        foreach (SimulationUnit unit in units)
        {
            if (unit.IsPlayerUnit())
                playerHp += unit.GetCurrentHpPortion();
            else
                enemyHp += unit.GetCurrentHpPortion();
        }

        if (enemyHp <= 0)
        {
            OnGameOver(true);
            return true;
        }
        else if (playerHp <= 0)
        {
            OnGameOver(false);
            return true;
        }

        return false;
    }

    private void OnGameOver(bool playerWon)
    {
        Debug.Log($"GAME OVER - Player won: {playerWon}");

        if (GameOver != null)
            GameOver(this, playerWon);
    }
}
