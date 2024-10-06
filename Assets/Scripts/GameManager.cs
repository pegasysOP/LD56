using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    int level = 0;
    int finalLevel = 1;

    private Dictionary<Vector2Int, UnitType>[] levelEnemyStartStates;

    UnitType[] upgradeTypes;
    List<UnitType[]> levelUpgradeTypes;

    public Sprite[] unitSprites;

    public static Dictionary<UnitType, Sprite> UnitTypeToSprite;

    //public EventHandler<bool> GameOver;

    UIManager UM;
    BoardManager BM;
    AudioManager AM;
    // Start is called before the first frame update
    void Start()
    {
        upgradeTypes = new UnitType[3];
        levelUpgradeTypes = new List<UnitType[]>();
        UM = FindObjectOfType<UIManager>();
        BM = FindObjectOfType<BoardManager>();
        AM = FindObjectOfType<AudioManager>();

        SetSpriteMap();

        UM.SetActiveUpgradePanel(false);

        //Spawn starting player units 
        BM.SpawnNewPlayerUnit(UnitType.Bee);
        BM.SpawnNewPlayerUnit(UnitType.Spider);
        BM.SpawnNewPlayerUnit(UnitType.Stag);
        BM.SpawnNewPlayerUnit(UnitType.Queen);
        BM.SpawnNewPlayerUnit(UnitType.Moth);
        BM.SpawnNewPlayerUnit(UnitType.Bee);

        BM.SavePlayerUnitStartPositions();

        PopulateEnemyStartStates();
        BM.GameOver += OnGameOver;
        LoadLevel();
    }

    void SetSpriteMap()
    {
        UnitTypeToSprite = new Dictionary<UnitType, Sprite>();
        UnitTypeToSprite[UnitType.Queen] = unitSprites[0];
        UnitTypeToSprite[UnitType.Stag] = unitSprites[1];
        UnitTypeToSprite[UnitType.Spider] = unitSprites[2];
        UnitTypeToSprite[UnitType.Moth] = unitSprites[3];
        UnitTypeToSprite[UnitType.Bee] = unitSprites[4];
    }

    void setUpgradeUnits()
    {
        upgradeTypes = new UnitType[3];
        upgradeTypes[0] = UnitType.Spider;
        upgradeTypes[1] = UnitType.Moth;
        upgradeTypes[2] = UnitType.Stag;

        levelUpgradeTypes.Add(upgradeTypes);

        upgradeTypes = new UnitType[3];
        upgradeTypes[0] = UnitType.Queen;
        upgradeTypes[1] = UnitType.Stag;
        upgradeTypes[2] = UnitType.Bee;

        levelUpgradeTypes.Add(upgradeTypes);

        UM.Unit1Button.GetComponentInChildren<TextMeshProUGUI>().text = levelUpgradeTypes[level][0].ToString(); 
        UM.Unit2Button.GetComponentInChildren<TextMeshProUGUI>().text = levelUpgradeTypes[level][1].ToString();
        UM.Unit3Button.GetComponentInChildren<TextMeshProUGUI>().text = levelUpgradeTypes[level][2].ToString();

        UM.Image1.sprite = UnitTypeToSprite[levelUpgradeTypes[level][0]];
        UM.Image2.sprite = UnitTypeToSprite[levelUpgradeTypes[level][1]];
        UM.Image3.sprite = UnitTypeToSprite[levelUpgradeTypes[level][2]];
    }

    public void PickUpgradeUnit(Button buttonPressed)
    {
        
        if (buttonPressed.name == UM.Unit1Button.name)
        {
            Debug.Log("Upgrade picked 1");
            BM.SpawnNewPlayerUnit(levelUpgradeTypes[level][0]);
        }
        if (buttonPressed.name == UM.Unit2Button.name)
        {
            Debug.Log("Upgrade picked 2");
            BM.SpawnNewPlayerUnit(levelUpgradeTypes[level][1]);
        }
        if (buttonPressed.name == UM.Unit3Button.name)
        {
            Debug.Log("Upgrade picked 3");
            BM.SpawnNewPlayerUnit(levelUpgradeTypes[level][2]);
        }

        UM.SetActiveUpgradePanel(false);
    }

    void PopulateEnemyStartStates()
    {
        levelEnemyStartStates = new Dictionary<Vector2Int, UnitType>[finalLevel + 1];

        Dictionary<Vector2Int, UnitType> FirstLevelEnemyUnits = new Dictionary<Vector2Int, UnitType>();

        // example enemy units
        FirstLevelEnemyUnits[new Vector2Int(7, 6)] = UnitType.Bee;
        FirstLevelEnemyUnits[new Vector2Int(7, 4)] = UnitType.Moth;
        FirstLevelEnemyUnits[new Vector2Int(7, 1)] = UnitType.Queen;

        levelEnemyStartStates[0] = FirstLevelEnemyUnits;

        Dictionary<Vector2Int, UnitType> SecondLevelEnemyUnits = new Dictionary<Vector2Int, UnitType>();

        // example enemy units
        SecondLevelEnemyUnits[new Vector2Int(6, 3)] = UnitType.Bee;
        SecondLevelEnemyUnits[new Vector2Int(6, 2)] = UnitType.Stag;
        SecondLevelEnemyUnits[new Vector2Int(6, 1)] = UnitType.Spider;

        levelEnemyStartStates[1] = SecondLevelEnemyUnits;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void onRoundWon()
    {
        //Play victory fanfare

        //Load UI for victory. Showing health, score and next level buttons 

        //When next level is pressed go to planning phase for next level. Load new units 
        getNextLevel();
    }

    void onRoundLost()
    {
        //Play defeat fanfare 

        //Load UI for loss. Showing health, score, retry and exit buttons

        //When retry is pressed we go back to the planning phase and you can place units again
        //Get board from saved board and use that to reset the board. Then pass in the same levels enemy dict

        //When exit is pressed return to main menu 
        LoadLevel();
    }

    void onRoundOver(bool playerWon)
    {
        if (playerWon)
        {
            onRoundWon();
        }
        else
        {
            onRoundLost();
        }
    }

    void onGameWon()
    {
        Debug.Log("You won!");
        UM.LoadMenuScene();
    }

    void getNextLevel()
    {
        level++;

        if(level > finalLevel)
        {
            onGameWon();
        }
        else
        {
            LoadLevel();
        }   
    }

    public void LoadLevel()
    {
        //Display upgrade panel 
        if (level != 0)
        {
            UM.SetActiveUpgradePanel(true);
        }
        setUpgradeUnits();
        //Load into planning phase 
        AM.PlayPlanningPhaseClip();

        //Reset the board
        BM.ClearBoardUnits();

        //Generate enemy positions for current level 
        BM.LoadEnemyUnits(levelEnemyStartStates[level]);

        //Spawn players from current save state
        BM.LoadPlayerUnits();
    }

    public void StartLevel()
    {
        //Save the player positions
        BM.SavePlayerUnitStartPositions();
        Debug.Log("Start Level"); 
        BM.StartRound();

        //Play the attack phase music 
        AM.PlaySimulationPhaseClip();
        //call onStartRoundPressed on boardManager and pass along the enemy configuration   
    }

    private void OnGameOver(object sender, bool playerWon)
    {
        onRoundOver(playerWon);
    }
}
