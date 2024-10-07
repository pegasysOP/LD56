using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    int level = 0;
    int finalLevel = 4;

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
        AudioManager.Instance.PlayRegularButtonClip();
        upgradeTypes = new UnitType[3];
        levelUpgradeTypes = new List<UnitType[]>();
        UM = FindObjectOfType<UIManager>();
        BM = FindObjectOfType<BoardManager>();
        AM = FindObjectOfType<AudioManager>();

        SetSpriteMap();

        UM.SetActiveUpgradePanel(false);

        //Spawn starting player units 
        BM.SpawnNewPlayerUnit(UnitType.WorkerBee);
        BM.SpawnNewPlayerUnit(UnitType.Moth);

        BM.SavePlayerUnitStartPositions();

        PopulateEnemyStartStates();
        BM.GameOver += OnGameOver;
        LoadLevel();
    }

    void SetSpriteMap()
    {
        UnitTypeToSprite = new Dictionary<UnitType, Sprite>();
        UnitTypeToSprite[UnitType.QueenBee] = unitSprites[0];
        UnitTypeToSprite[UnitType.Beetle] = unitSprites[1];
        UnitTypeToSprite[UnitType.Spider] = unitSprites[2];
        UnitTypeToSprite[UnitType.Moth] = unitSprites[3];
        UnitTypeToSprite[UnitType.WorkerBee] = unitSprites[4];
    }

    void setUpgradeUnits()
    {
        upgradeTypes = new UnitType[3];
        upgradeTypes[0] = UnitType.Spider;
        upgradeTypes[1] = UnitType.Moth;
        upgradeTypes[2] = UnitType.Beetle;

        levelUpgradeTypes.Add(upgradeTypes);

        upgradeTypes = new UnitType[3];
        upgradeTypes[0] = UnitType.QueenBee;
        upgradeTypes[1] = UnitType.Beetle;
        upgradeTypes[2] = UnitType.WorkerBee;

        levelUpgradeTypes.Add(upgradeTypes);

        upgradeTypes = new UnitType[3];
        upgradeTypes[0] = UnitType.Moth;
        upgradeTypes[1] = UnitType.Spider;
        upgradeTypes[2] = UnitType.Beetle;

        levelUpgradeTypes.Add(upgradeTypes);

        upgradeTypes = new UnitType[3];
        upgradeTypes[0] = UnitType.QueenBee;
        upgradeTypes[1] = UnitType.Moth;
        upgradeTypes[2] = UnitType.Spider;

        levelUpgradeTypes.Add(upgradeTypes);

        upgradeTypes = new UnitType[3];
        upgradeTypes[0] = UnitType.Beetle;
        upgradeTypes[1] = UnitType.QueenBee;
        upgradeTypes[2] = UnitType.WorkerBee;

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

        AudioManager.Instance.PlayUpgradeButtonClip();
        UM.SetActiveUpgradePanel(false);
        UM.StartRoundButton.SetActive(true);
        BM.setSelectionEnabled(true);
    }

    void PopulateEnemyStartStates()
    {
        levelEnemyStartStates = new Dictionary<Vector2Int, UnitType>[finalLevel + 1];

        Dictionary<Vector2Int, UnitType> FirstLevelEnemyUnits = new Dictionary<Vector2Int, UnitType>();

        // example enemy units
        FirstLevelEnemyUnits[new Vector2Int(7, 6)] = UnitType.WorkerBee;

        levelEnemyStartStates[0] = FirstLevelEnemyUnits;

        Dictionary<Vector2Int, UnitType> SecondLevelEnemyUnits = new Dictionary<Vector2Int, UnitType>();

        // example enemy units
        SecondLevelEnemyUnits[new Vector2Int(6, 3)] = UnitType.WorkerBee;
        SecondLevelEnemyUnits[new Vector2Int(6, 2)] = UnitType.Beetle;

        levelEnemyStartStates[1] = SecondLevelEnemyUnits;

        Dictionary<Vector2Int, UnitType> ThirdLevelEnemyUnits = new Dictionary<Vector2Int, UnitType>();

        // example enemy units
        ThirdLevelEnemyUnits[new Vector2Int(6, 3)] = UnitType.QueenBee;
        ThirdLevelEnemyUnits[new Vector2Int(6, 1)] = UnitType.Spider;

        levelEnemyStartStates[2] = ThirdLevelEnemyUnits;

        Dictionary<Vector2Int, UnitType> FourthLevelEnemyUnits = new Dictionary<Vector2Int, UnitType>();

        // example enemy units
        FourthLevelEnemyUnits[new Vector2Int(6, 3)] = UnitType.WorkerBee;
        FourthLevelEnemyUnits[new Vector2Int(6, 2)] = UnitType.Beetle;
        FourthLevelEnemyUnits[new Vector2Int(6, 1)] = UnitType.Moth;

        levelEnemyStartStates[3] = FourthLevelEnemyUnits;

        Dictionary<Vector2Int, UnitType> FifthLevelEnemyUnits = new Dictionary<Vector2Int, UnitType>();

        // example enemy units
        FifthLevelEnemyUnits[new Vector2Int(6, 3)] = UnitType.WorkerBee;
        FifthLevelEnemyUnits[new Vector2Int(6, 2)] = UnitType.Beetle;
        FifthLevelEnemyUnits[new Vector2Int(6, 1)] = UnitType.Spider;

        levelEnemyStartStates[4] = FifthLevelEnemyUnits;

    }

    //Now this is epic
    void CrazyMode()
    {
        BM.SpawnNewPlayerUnit(UnitType.QueenBee);
        BM.SpawnNewPlayerUnit(UnitType.Beetle);
        BM.SpawnNewPlayerUnit(UnitType.Spider);
        BM.SpawnNewPlayerUnit(UnitType.Moth);
        BM.SpawnNewPlayerUnit(UnitType.QueenBee);
        BM.SpawnNewPlayerUnit(UnitType.Beetle);
        BM.SpawnNewPlayerUnit(UnitType.Spider);
        BM.SpawnNewPlayerUnit(UnitType.Moth);
        BM.SpawnNewPlayerUnit(UnitType.QueenBee);
        BM.SpawnNewPlayerUnit(UnitType.Beetle);
        BM.SpawnNewPlayerUnit(UnitType.Spider);
        BM.SpawnNewPlayerUnit(UnitType.Moth);
        BM.SpawnNewPlayerUnit(UnitType.WorkerBee);
        BM.SpawnNewPlayerUnit(UnitType.QueenBee);
        BM.SpawnNewPlayerUnit(UnitType.Beetle);   
        BM.SavePlayerUnitStartPositions();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void onRoundWon()
    {
        //Play victory fanfare
        AM.PlayRoundVictoryFanfareClip();

        //Wait 5 seconds then show continue button
        StartCoroutine(WaitFor(6f, true));

        //Load UI for victory. Showing health, score and next level buttons 

        
    }

    void onRoundLost()
    {
        //Play defeat fanfare 
        AM.PlayFailureFanfareClip();

        //Wait 5 seconds then show continue button
        StartCoroutine(WaitFor(10f, false));
        //Load UI for loss. Showing health, score, retry and exit buttons

        //When retry is pressed we go back to the planning phase and you can place units again
        //Get board from saved board and use that to reset the board. Then pass in the same levels enemy dict

        //When exit is pressed return to main menu 
        
    }

    IEnumerator WaitFor(float seconds, bool playerWon)
    {
        Debug.Log("Waiting");
        // Wait for 5 seconds
        yield return new WaitForSeconds(seconds);
        //When next level is pressed go to planning phase for next level. Load new units 
        if (playerWon)
        {
            getNextLevel();
        }
        else
        {
            LoadLevel();
        }
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
        UM.StartRoundButton.SetActive(false);
        Debug.Log("You won!");
        UM.LoadCreditScene();
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
            if(level == finalLevel)
            {
                CrazyMode();
            }
        }   
    }

    public void LoadLevel()
    {
        setUpgradeUnits();
        //Load into planning phase 
        AM.PlayPlanningPhaseClip();

        //Reset the board
        BM.ClearBoardUnits();

        //Generate enemy positions for current level 
        BM.LoadEnemyUnits(levelEnemyStartStates[level]);

        //Spawn players from current save state
        BM.LoadPlayerUnits();

        //Display upgrade panel 
        if (level != 0)
        {
            UM.SetActiveUpgradePanel(true);
            //Disable the next round button
            UM.StartRoundButton.SetActive(false);
            BM.setSelectionEnabled(false);
        }
        else
        {
            UM.StartRoundButton.SetActive(true);
        }
    }

    public void StartLevel()
    {
        //Disable the start level button
        UM.StartRoundButton.SetActive(false);
        //Save the player positions
        BM.SavePlayerUnitStartPositions();
        Debug.Log("Start Level"); 
        BM.StartRound();

        AudioManager.Instance.PlayRegularButtonClip();

        //Play the attack phase music 
        AM.PlaySimulationPhaseClip();
        //call onStartRoundPressed on boardManager and pass along the enemy configuration   
    }

    private void OnGameOver(object sender, bool playerWon)
    {
        onRoundOver(playerWon);
    }
}
