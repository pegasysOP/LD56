using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UnitData unitData;

    private int currentLevel = 0;

    private HudManager hudManager;
    private BoardManager boardManager;
    private AudioManager audioManager;

    private UnitInventory inventory;
    private GameObject currentDraggingUnit = null;
    private bool isPlaying = false;

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
        unitData.Init();

        InitializeInventory();

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
        if (currentLevel < 1 || currentLevel >= UnitData.GetLevelCount())
            return;

        UnitType[] options = UnitData.GetLevelUpgrades(currentLevel);
        hudManager.ShowUpgradePanel(UpgradeUnitPicked, options[0], options[1], options[2]);
    }

    void PrepareBoardForNextLevel()
    {
        audioManager.PlayPlanningPhaseClip();
        boardManager.ClearBoardUnits();
        boardManager.LoadEnemyUnits(UnitData.GetLevelEnemies(currentLevel));
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
        currentLevel++;
        if (currentLevel >= UnitData.GetLevelCount())
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
