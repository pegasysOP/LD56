using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int MaxLevels = 5;
    private int level = 0;

    private Dictionary<Vector2Int, UnitType>[] levelEnemyStartStates;
    private List<UnitType[]> levelUpgradeTypes;

    public Sprite[] unitSprites;
    public static Dictionary<UnitType, Sprite> UnitTypeToSprite;

    UIManager UM;
    BoardManager BM;
    AudioManager AM;

    private UnitInventory inventory;
    private GameObject currentDraggingUnit = null;
    private bool isPlaying = false;

    void Start()
    {
        InitializeManagers();
        SetupSpriteMap();
        SetupUpgradeConfigurations();
        SetupEnemyStartStates();
        InitialiseInventory();
        BM.SavePlayerUnitStartPositions();

        BM.GameOver += OnGameOver;
        LoadLevel();
    }

    void InitialiseInventory()
    {
        if (inventory == null)
            inventory = new UnitInventory();
        inventory.AddUnit(UnitType.WorkerBee);
        inventory.AddUnit(UnitType.WorkerBee);
    }

    void HandleUnitSelection()
    {
        if (currentDraggingUnit != null) return;
        if (inventory.isInventoryEmpty()) return;
        
        if (Input.GetKeyDown(KeyCode.Alpha1) && inventory.CanPlaceUnit(UnitType.QueenBee))
        {
            BM.SpawnNewPlayerUnit(UnitType.QueenBee);
            inventory.PlaceUnit(UnitType.QueenBee);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && inventory.CanPlaceUnit(UnitType.Beetle))
        {
            BM.SpawnNewPlayerUnit(UnitType.Beetle);
            inventory.PlaceUnit(UnitType.Beetle);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && inventory.CanPlaceUnit(UnitType.Spider))
        {
            BM.SpawnNewPlayerUnit(UnitType.Spider);
            inventory.PlaceUnit(UnitType.Spider);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && inventory.CanPlaceUnit(UnitType.Moth))
        {
            BM.SpawnNewPlayerUnit(UnitType.Moth);
            inventory.PlaceUnit(UnitType.Moth);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && inventory.CanPlaceUnit(UnitType.WorkerBee))
        {
            BM.SpawnNewPlayerUnit(UnitType.WorkerBee);
            inventory.PlaceUnit(UnitType.WorkerBee);
        }
        CheckIfAllUnitsPlaced();  // Check after every unit placement
    }

    // New method to check if all units are placed
    void CheckIfAllUnitsPlaced()
    {
        // Check if the inventory is empty (i.e. all units have been placed)
        if (inventory.isInventoryEmpty() && !isPlaying)
        {
            UM.SetActiveStartButton(true);  // Show the start button
            isPlaying = true;
            BM.SavePlayerUnitStartPositions();
        }
    }

    void Update()
    {
        HandleUnitSelection();
    }

    void InitializeManagers()
    {
        UM = FindObjectOfType<UIManager>();
        BM = FindObjectOfType<BoardManager>();
        AM = FindObjectOfType<AudioManager>();

        // Ensure the start button is hidden at the start of the game/level
        UM.SetActiveUpgradePanel(false);
        UM.SetActiveStartButton(false);  // Start button is hidden initially
    }

    void SetupSpriteMap()
    {
        UnitTypeToSprite = new Dictionary<UnitType, Sprite>
        {
            { UnitType.QueenBee, unitSprites[0] },
            { UnitType.Beetle, unitSprites[1] },
            { UnitType.Spider, unitSprites[2] },
            { UnitType.Moth, unitSprites[3] },
            { UnitType.WorkerBee, unitSprites[4] }
        };
    }

    void SetupUpgradeConfigurations()
    {
        levelUpgradeTypes = new List<UnitType[]>
        {
            new UnitType[] { UnitType.Spider, UnitType.Moth, UnitType.Beetle },
            new UnitType[] { UnitType.QueenBee, UnitType.Beetle, UnitType.WorkerBee },
            new UnitType[] { UnitType.Moth, UnitType.Spider, UnitType.Beetle },
            new UnitType[] { UnitType.QueenBee, UnitType.Moth, UnitType.Spider },
            new UnitType[] { UnitType.Beetle, UnitType.QueenBee, UnitType.WorkerBee }
        };
    }

    void SetupEnemyStartStates()
    {
        levelEnemyStartStates = new Dictionary<Vector2Int, UnitType>[MaxLevels];

        for (int i = 0; i < MaxLevels; i++)
            levelEnemyStartStates[i] = GetEnemyUnitsForLevel(i);
    }

    Dictionary<Vector2Int, UnitType> GetEnemyUnitsForLevel(int level)
    {
        var enemyUnits = new Dictionary<Vector2Int, UnitType>();

        switch (level)
        {
            case 0:
                enemyUnits.Add(new Vector2Int(7, 6), UnitType.WorkerBee);
                break;

            case 1:
                enemyUnits.Add(new Vector2Int(6, 3), UnitType.WorkerBee);
                enemyUnits.Add(new Vector2Int(7, 4), UnitType.Beetle);
                break;

            case 2:
                enemyUnits.Add(new Vector2Int(6, 2), UnitType.QueenBee);
                enemyUnits.Add(new Vector2Int(7, 2), UnitType.Spider);
                enemyUnits.Add(new Vector2Int(7, 3), UnitType.Moth);
                break;

            case 3:
                enemyUnits.Add(new Vector2Int(6, 2), UnitType.WorkerBee);
                enemyUnits.Add(new Vector2Int(6, 4), UnitType.Beetle);
                enemyUnits.Add(new Vector2Int(7, 2), UnitType.Moth);
                enemyUnits.Add(new Vector2Int(7, 4), UnitType.QueenBee);
                break;

            case 4:
                enemyUnits.Add(new Vector2Int(6, 3), UnitType.QueenBee);
                enemyUnits.Add(new Vector2Int(5, 3), UnitType.WorkerBee);
                enemyUnits.Add(new Vector2Int(6, 2), UnitType.WorkerBee);
                enemyUnits.Add(new Vector2Int(6, 4), UnitType.WorkerBee);
                enemyUnits.Add(new Vector2Int(7, 3), UnitType.WorkerBee);
                enemyUnits.Add(new Vector2Int(7, 5), UnitType.WorkerBee);
                break;
        }
        return enemyUnits;
    }

    void PickUpgradeUnit(object sender, UnitType unitType)
    {
        UM.upgradePanel.UnitChosen -= PickUpgradeUnit;
        inventory.AddUnit(unitType);
        AM.PlayUpgradeButtonClip();
        UM.SetActiveUpgradePanel(false);
        BM.setSelectionEnabled(true);

        // After upgrading a unit, wait for the player to place units before showing start button
        isPlaying = false;
        UM.SetActiveStartButton(false);  // Hide Start Button until units are placed
    }

    void LoadLevel()
    {
        SetUpgradeUnitsForLevel();
        PrepareBoardForNextLevel();

        if (level != 0)
            ShowUpgradePanel();
        //else
            //UM.StartRoundButton.SetActive(true);  // First level, no upgrade, start immediately
    }

    void SetUpgradeUnitsForLevel()
    {
        if (level < levelUpgradeTypes.Count)
            UM.upgradePanel.SetUnitOptions(levelUpgradeTypes[level][0], levelUpgradeTypes[level][1], levelUpgradeTypes[level][2]);
    }

    void PrepareBoardForNextLevel()
    {
        AM.PlayPlanningPhaseClip();
        BM.ClearBoardUnits();
        BM.LoadEnemyUnits(levelEnemyStartStates[level]);  // Load enemy units at start of level
        BM.LoadPlayerUnits();  // Load player units
        UM.SetActiveStartButton(false);  // Hide Start Button initially
    }

    void ShowUpgradePanel()
    {
        UM.SetActiveUpgradePanel(true);
        UM.upgradePanel.UnitChosen += PickUpgradeUnit;
        UM.SetActiveStartButton(false);  // Hide Start Button during upgrade panel
        BM.setSelectionEnabled(false);
    }

    public void StartLevel()
    {
        UM.StartRoundButton.SetActive(false);  // Hide Start Button once round begins
        BM.SavePlayerUnitStartPositions();
        BM.StartRound();
        AM.PlayRegularButtonClip();
        AM.PlaySimulationPhaseClip();
    }

    private void OnGameOver(object sender, bool playerWon)
    {
        OnRoundOver(playerWon);
    }

    void OnRoundOver(bool playerWon)
    {
        if (playerWon)
            OnRoundWon();
        else
            OnRoundLost();
    }

    void OnRoundWon()
    {
        AM.PlayRoundVictoryFanfareClip();
        StartCoroutine(WaitFor(6f, true));
    }

    void OnRoundLost()
    {
        AM.PlayFailureFanfareClip();
        StartCoroutine(WaitFor(10f, false));
    }

    IEnumerator WaitFor(float seconds, bool playerWon)
    {
        yield return new WaitForSeconds(seconds);
        if (playerWon)
            GetNextLevel();
        else
            LoadLevel();
    }

    void GetNextLevel()
    {
        level++;
        if (level >= MaxLevels)
            OnGameWon();
        else
            LoadLevel();
    }

    void OnGameWon()
    {
        UM.StartRoundButton.SetActive(false);
        UM.LoadCreditScene();
    }
}