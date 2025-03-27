using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UnitData unitData;
    public HudManager hudManager;

    private int currentLevel = 0;

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
        unitData.Init();

        InitManagers();

        inventory = new UnitInventory();
        hudManager.ShowInventoryPanel(InventoryUnitSelected, inventory.Units);

        boardManager.SavePlayerUnitStartPositions(); // Sam - not sure why this is done at the start but too tired to check ¯\_(ツ)_/¯

        LoadLevel();
    }

    void InitManagers()
    {
        hudManager.Init();

        audioManager = FindFirstObjectByType<AudioManager>();
          
        boardManager = FindFirstObjectByType<BoardManager>();
        boardManager.GameOver += OnGameOver;
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
