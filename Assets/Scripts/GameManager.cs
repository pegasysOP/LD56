using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const int MaxLevels = 5;
    private int level = 0;

    private Dictionary<Vector2Int, UnitType>[] levelEnemyStartStates;
    private List<UnitType[]> levelUpgradeTypes;

    public Sprite[] unitSprites;
    public static Dictionary<UnitType, Sprite> UnitTypeToSprite;

    public Button[] buttons;

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
        SetupEnemyStartStates();
        InitializeInventory();
        SetupInventoryUI();
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

    void SetupInventoryUI()
    {
        var unitTypes = new List<UnitType>(UnitTypeToSprite.Keys);

        for (int i = 0; i < buttons.Length; i++)
        {
            UnitType unitType = unitTypes[i];
            Sprite unitSprite = UnitTypeToSprite[unitType];

            Image childImage = buttons[i].transform.Find("Unit Icon").GetComponent<Image>();
            if (childImage != null)
            {
                childImage.sprite = unitSprite;
            }

            SetInventoryUnitColour(unitType, childImage);

            TextMeshProUGUI amountText = buttons[i].transform.Find("Unit Amount Text").GetComponent<TextMeshProUGUI>();
            amountText.text = inventory.Units[unitType].ToString();

            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = hudManager.GetUnitNameText(unitType);
            }
        }
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

    public void HandleUnitSelection(int index)
    {
        UnitType selectedUnit = (UnitType)index;
        if (currentDraggingUnit != null || inventory.isInventoryEmpty()) return;

        if (inventory.CanPlaceUnit(selectedUnit))
        {
            boardManager.SpawnNewPlayerUnit(selectedUnit);
            inventory.PlaceUnit(selectedUnit);
            CheckIfAllUnitsPlaced();

            TextMeshProUGUI amountText = buttons[index].transform.Find("Unit Amount Text").GetComponent<TextMeshProUGUI>();
            amountText.text = inventory.Units[selectedUnit].ToString();

            Image childImage = buttons[GetButtonForUnitType(selectedUnit)].transform.Find("Unit Icon").GetComponent<Image>();
            SetInventoryUnitColour(selectedUnit, childImage);
        }
    }

    void CheckIfAllUnitsPlaced()
    {
        if (inventory.isInventoryEmpty() && !isPlaying)
        {
            hudManager.SetActiveStartButton(true);  
            isPlaying = true;
            boardManager.SavePlayerUnitStartPositions();
        }
    }

    void SetInventoryUnitColour(UnitType unitType, Image image)
    {
        if (inventory.GetUnitCount(unitType) == 0)
        {
            image.color = new Color(0.5f, 0.5f, 0.5f, 1f);  // Darken the sprite (greyed out)
        }
        else
        {
            image.color = Color.white;  // Restore to normal color
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

    int GetButtonForUnitType(UnitType type)
    {
        if (type == UnitType.QueenBee) return 0;
        else if (type == UnitType.Beetle) return 1;
        else if (type == UnitType.Spider) return 2;
        else if (type == UnitType.Moth) return 3;
        else if (type == UnitType.WorkerBee) return 4;
        else return 5;
    }

    void PickUpgradeUnit(UnitType unitType)
    {
        inventory.AddUnit(unitType);
        hudManager.HideUpgradePanel();

        audioManager.PlayUpgradeButtonClip();
        hudManager.SetActiveInventoryPanel(true);

        TextMeshProUGUI amountText = buttons[GetButtonForUnitType(unitType)].transform.Find("Unit Amount Text").GetComponent<TextMeshProUGUI>();
        amountText.text = inventory.Units[unitType].ToString();
        
        Image image = buttons[GetButtonForUnitType(unitType)].transform.Find("Unit Icon").GetComponent<Image>();
        SetInventoryUnitColour(unitType, image);

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
        hudManager.ShowUpgradePanel(PickUpgradeUnit, upgrades[0], upgrades[1], upgrades[2]);
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
        hudManager.SetActiveInventoryPanel(false);
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
