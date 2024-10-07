using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public static float TPS { get { return 2f; } }
    public static float TickDuration { get {  return 1f / TPS; } }

    [Header("TEMP VISUALISATION")]
    public TempSimUnitUIElement uiElementPrefab;
    public Transform playerUiContainer;
    public Transform enemyUiContainer;
    public TempUnitObject unitObjectPrefab;
    public Transform unitObjectContainer;

    private SimulationGrid grid;

    public EventHandler<bool> GameOver;

    public void StartSimulation(Dictionary<Vector2Int, UnitType> playerUnitsStartState, Dictionary<Vector2Int, UnitType> enemyUnitsStartState)
    {
        SimulationUnitBase[,] initialUnitGrid = new SimulationUnitBase[8, 8];

        foreach (KeyValuePair<Vector2Int, UnitType> unitPlacement in playerUnitsStartState)
            initialUnitGrid[unitPlacement.Key.x, unitPlacement.Key.y] = GetNewSimulationUnit(unitPlacement.Value, true);
        foreach (KeyValuePair<Vector2Int, UnitType> unitPlacement in enemyUnitsStartState)
            initialUnitGrid[unitPlacement.Key.x, unitPlacement.Key.y] = GetNewSimulationUnit(unitPlacement.Value, false);

        grid = new SimulationGrid(initialUnitGrid);
        StartCoroutine (DoSimulation());
    }

    private IEnumerator DoSimulation()
    {
        //UpdateTempVisualisations();
        yield return new WaitForSeconds(0.5f);

        float timer = 0;

        while (true)
        {
            // wait until time for tick
            timer += Time.deltaTime;
            if (timer < TickDuration)
            {
                yield return null;
                continue;
            }

            // reset timer
            timer = 0;

            // stop simulation if game over
            bool gameOver = DoTick();            

            UpdateUnitBars();

            //UpdateTempVisualisations();

            if (gameOver)
                break;
        }

        yield return null;
    }

    private void UpdateUnitBars()
    {
        Dictionary<Vector2Int, (float, float)> unitDatas = new Dictionary<Vector2Int, (float, float)>();
        Vector2Int gridDimensions = grid.GetGridDimensions();

        for (int i = 0; i < gridDimensions.x; i++)
        {
            for (int j = 0; j < gridDimensions.y; j++)
            {
                Vector2Int position = new Vector2Int(i, j);

                if (grid.TryGetUnitAt(position, out SimulationUnitBase unit))
                    unitDatas[position] = (unit.GetCurrentHpPortion(), unit.GetSpecialProgress());
            }
        }

        BoardUtils.UpdateUnitData(unitDatas);
    }

    private void UpdateTempVisualisations()
    {
        for (int i = playerUiContainer.childCount - 1; i >= 0; i--)
            DestroyImmediate(playerUiContainer.GetChild(i).gameObject);
        for (int i = enemyUiContainer.childCount - 1; i >= 0; i--)
            DestroyImmediate(enemyUiContainer.GetChild(i).gameObject);

        //for (int i = unitObjectContainer.childCount - 1; i >= 0; i--)
        //    DestroyImmediate(unitObjectContainer.GetChild(i).gameObject);

        List<SimulationUnitBase> sortedUnits = SimulationUtils.ShuffleUnits(grid.GetUnits());
        sortedUnits.Sort((SimulationUnitBase a, SimulationUnitBase b) => a.GetCurrentHpPortion() > b.GetCurrentHpPortion() ? -1 : 1);
        foreach (SimulationUnitBase unit in sortedUnits)
        {
            Transform targetContainer = unit.IsPlayerUnit() ? playerUiContainer : enemyUiContainer;
            Instantiate(uiElementPrefab, targetContainer).SetUnit(grid, unit);
            //Instantiate(unitObjectPrefab, unitObjectContainer).SetUnit(grid, unit);
        }
    }

    /// <summary>
    /// Executes 1 game tick worth of logic
    /// </summary>
    /// <returns>Returns true if game is over</returns>
    private bool DoTick()
    {
        List<SimulationUnitBase> units = SimulationUtils.ShuffleUnits(grid.GetUnits());

        // iterate over copy because we are potentially removing units
        foreach (SimulationUnitBase unit in units)
        {
            // skip unit if it has already been killed this tick
            if (unit.GetCurrentHp() <= 0)
                continue;

            unit.DoTick(ref grid);
        }

        // check remaining hp to see if game is over
        float playerHp = 0;
        float enemyHp = 0;
        foreach (SimulationUnitBase unit in units)
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

    public SimulationUnitBase GetNewSimulationUnit(UnitType unitType, bool player)
    {
        switch (unitType)
        {
            case UnitType.WorkerBee:
                return new WorkerBeeSimulationUnit(player);
            case UnitType.QueenBee:
                return new QueenBeeSimulationUnit(player);
            case UnitType.Beetle:
                return new BeetleSimulationUnit(player);
            case UnitType.Spider:
                return new SpiderSimulationUnit(player);
            case UnitType.Moth:
                return new MothSimulationUnit(player);

            // default to worker bee just in cases
            default:
                return new WorkerBeeSimulationUnit(player);
        }
    }

        private void OnGameOver(bool playerWon)
    {
        GameOver?.Invoke(this, playerWon);
    }
}
