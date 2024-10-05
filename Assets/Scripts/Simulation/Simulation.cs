using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public float tps = 3f;

    [Header("TEMP VISUALISATION")]
    public TempSimUnitUIElement uiElementPrefab;
    public Transform playerUiContainer;
    public Transform enemyUiContainer;
    public TempUnitObject unitObjectPrefab;
    public Transform unitObjectContainer;

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
        initialUnitGrid[0, 7] = new SimulationUnitDemo(true); // player demos
        initialUnitGrid[0, 4] = new SimulationUnitDemo(true);
        initialUnitGrid[0, 0] = new SimulationUnitDemo(true);
        initialUnitGrid[7, 6] = new SimulationUnitDemo(false); // enemy demos
        initialUnitGrid[7, 4] = new SimulationUnitDemo(false);
        initialUnitGrid[7, 1] = new SimulationUnitDemo(false);

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

            UpdateTempVisualisations();

            if (gameOver)
                break;
        }

        yield return null;
    }

    private void UpdateTempVisualisations()
    {
        for (int i = playerUiContainer.childCount - 1; i >= 0; i--)
            DestroyImmediate(playerUiContainer.GetChild(i).gameObject);
        for (int i = enemyUiContainer.childCount - 1; i >= 0; i--)
            DestroyImmediate(enemyUiContainer.GetChild(i).gameObject);

        for (int i = unitObjectContainer.childCount - 1; i >= 0; i--)
            DestroyImmediate(unitObjectContainer.GetChild(i).gameObject);

        List<SimulationUnit> sortedUnits = grid.GetUnits();
        sortedUnits.Sort((SimulationUnit a, SimulationUnit b) => a.GetCurrentHpPortion() > b.GetCurrentHpPortion() ? -1 : 1);
        foreach (SimulationUnit unit in sortedUnits)
        {
            Transform targetContainer = unit.IsPlayerUnit() ? playerUiContainer : enemyUiContainer;
            Instantiate(uiElementPrefab, targetContainer).SetUnit(grid, unit);
            Instantiate(unitObjectPrefab, unitObjectContainer).SetUnit(grid, unit);
        }
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
