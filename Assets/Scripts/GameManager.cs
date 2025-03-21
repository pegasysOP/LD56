using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int MaxLevels = 5;
    private int level = 0;

    private Dictionary<Vector2Int, UnitType>[] levelEnemyStartStates;
    private List<UnitType[]> levelUpgradeTypes;

    public Sprite[] unitSprites;
    public static Dictionary<UnitType, Sprite> UnitTypeToSprite;

    HudManager hudManager;
    BoardManager boardManager;
    AudioManager audioManager;

    private UnitInventory inventory;
    private GameObject currentDraggingUnit = null;
    private bool isPlaying = false;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InitializeManagers();
        SetupGameConfigurations();
        LoadLevel();
    }

    void SetupGameConfigurations()
    {
        SetupSpriteMap();
        SetupUpgradeConfigurations();

        InitializeInventory();
        SetupEnemyStartStates();

        hudManager.ShowInventoryPanel(InventoryUnitSelected, inventory.Units);

        boardManager.SavePlayerUnitStartPositions();
    }

    void InitializeManagers()
    {
        hudManager = FindFirstObjectByType<HudManager>();
        boardManager = FindFirstObjectByType<BoardManager>();
        audioManager = FindFirstObjectByType<AudioManager>();

        hudManager.HideUpgradePanel();
        hudManager.SetActiveStartButton(false);  
        boardManager.GameOver += OnGameOver;
    }

    void SetupSpriteMap()
    {
        UnitTypeToSprite = new Dictionary<UnitType, Sprite>
        {
            { UnitType.QueenBee, unitSprites[0] },
            { UnitType.Beetle, unitSprites[1] },
            { UnitType.Spider, unitSprites[2] },
            { UnitType.Moth, unitSprites[3] },
            { UnitType.WorkerBee, unitSprites[4] },
            { UnitType.FireAnt, unitSprites[5] }
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
        {
            levelEnemyStartStates[i] = GetEnemyUnitsForLevel(i);
        }
    }

    void InitializeInventory()
    {
        if (inventory == null)
        {
            inventory = new UnitInventory();
        }

        inventory.AddUnit(UnitType.WorkerBee);
        inventory.AddUnit(UnitType.WorkerBee);
    }

    private void InventoryUnitSelected(UnitType unitType)
    {
        if (currentDraggingUnit != null || inventory.IsInventoryEmpty())
            return;

        boardManager.SpawnNewPlayerUnit(unitType);
        inventory.PlaceUnit(unitType);

        hudManager.inventoryPanel.UpdateUnitQuantity(unitType, inventory.GetUnitCount(unitType));

        CheckIfAllUnitsPlaced();
    }

    void CheckIfAllUnitsPlaced()
    {
        if (inventory.IsInventoryEmpty() && !isPlaying)
        {
            hudManager.SetActiveStartButton(true);  
            isPlaying = true;
            boardManager.SavePlayerUnitStartPositions();
        }
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

    private void UpgradeUnitPicked(UnitType unitType)
    {
        inventory.AddUnit(unitType);
        hudManager.HideUpgradePanel();

        audioManager.PlayUpgradeButtonClip();
        hudManager.ShowInventoryPanel(InventoryUnitSelected, inventory.Units);

        boardManager.SetSelectionEnabled(true);
        isPlaying = false;
        hudManager.SetActiveStartButton(false);
    }

    void LoadLevel()
    {
        PrepareBoardForNextLevel();

        ShowUpgrades();
    }

    void ShowUpgrades()
    {
        // exclude first level
        if (level < 1 || level >= levelUpgradeTypes.Count)
            return;

        var upgrades = levelUpgradeTypes[level];
        hudManager.ShowUpgradePanel(UpgradeUnitPicked, upgrades[0], upgrades[1], upgrades[2]);
    }

    void PrepareBoardForNextLevel()
    {
        audioManager.PlayPlanningPhaseClip();
        boardManager.ClearBoardUnits();
        boardManager.LoadEnemyUnits(levelEnemyStartStates[level]);
        boardManager.LoadPlayerUnits();
        hudManager.SetActiveStartButton(false);  
    }

    public void StartLevel()
    {
        hudManager.HideInventoryPanel();

        boardManager.SavePlayerUnitStartPositions();
        boardManager.StartRound();

        audioManager.PlayRegularButtonClip();
        audioManager.PlaySimulationPhaseClip();
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
        audioManager.PlayRoundVictoryFanfareClip();
        StartCoroutine(WaitFor(6f, true));
    }

    void OnRoundLost()
    {
        audioManager.PlayFailureFanfareClip();
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
        SceneUtils.LoadCreditScene();
        AudioManager.Instance.PlayVictoryFanfareClip();
    }
}
