using System;
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

    UIManager UM;
    BoardManager BM;
    AudioManager AM;

    private UnitInventory inventory;
    private GameObject currentDraggingUnit = null;
    private bool isPlaying = false;

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
        BM.SavePlayerUnitStartPositions();
    }

    void InitializeManagers()
    {
        UM = FindObjectOfType<UIManager>();
        BM = FindObjectOfType<BoardManager>();
        AM = FindObjectOfType<AudioManager>();

        UM.SetActiveUpgradePanel(false);
        UM.SetActiveStartButton(false);  
        BM.GameOver += OnGameOver;
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

            TextMeshProUGUI amountText = buttons[i].transform.Find("Unit Amount Text").GetComponent<TextMeshProUGUI>();
            amountText.text = inventory.Units[unitType].ToString();

            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = unitType.ToString();
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
            BM.SpawnNewPlayerUnit(selectedUnit);
            inventory.PlaceUnit(selectedUnit);
            CheckIfAllUnitsPlaced();

            TextMeshProUGUI amountText = buttons[index].transform.Find("Unit Amount Text").GetComponent<TextMeshProUGUI>();
            amountText.text = inventory.Units[selectedUnit].ToString();
        }
    }

    void CheckIfAllUnitsPlaced()
    {
        if (inventory.isInventoryEmpty() && !isPlaying)
        {
            UM.SetActiveStartButton(true);  
            isPlaying = true;
            BM.SavePlayerUnitStartPositions();
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
        else return 4;
    }

    void PickUpgradeUnit(object sender, UnitType unitType)
    {
        UM.upgradePanel.UnitChosen -= PickUpgradeUnit;
        inventory.AddUnit(unitType);
        AM.PlayUpgradeButtonClip();
        UM.SetActiveUpgradePanel(false);
        UM.SetActiveInventoryPanel(true);
        TextMeshProUGUI amountText = buttons[GetButtonForUnitType(unitType)].transform.Find("Unit Amount Text").GetComponent<TextMeshProUGUI>();
        amountText.text = inventory.Units[unitType].ToString();
        BM.setSelectionEnabled(true);
        isPlaying = false;
        UM.SetActiveStartButton(false);
    }

    void LoadLevel()
    {
        SetUpgradeUnitsForLevel();
        PrepareBoardForNextLevel();

        if (level != 0)
        {
            UM.upgradePanel.UnitChosen += PickUpgradeUnit;
            UM.ShowUpgradePanel();
        }
            
    }

    void SetUpgradeUnitsForLevel()
    {
        if (level < levelUpgradeTypes.Count)
        {
            var upgrades = levelUpgradeTypes[level];
            UM.upgradePanel.SetUnitOptions(upgrades[0], upgrades[1], upgrades[2]);
        }
    }

    void PrepareBoardForNextLevel()
    {
        AM.PlayPlanningPhaseClip();
        BM.ClearBoardUnits();
        BM.LoadEnemyUnits(levelEnemyStartStates[level]);
        BM.LoadPlayerUnits();
        UM.SetActiveStartButton(false);  
    }

    public void StartLevel()
    {
        UM.StartRoundButton.SetActive(false);
        UM.SetActiveInventoryPanel(false);
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
