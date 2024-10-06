using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    int level = 0;
    int finalLevel = 1;

    private Dictionary<Vector2Int, UnitType>[] levelEnemyStartStates;

    UnitType[] upgradeTypes;

    public Sprite[] unitSprites;

    public Dictionary<UnitType, Sprite> UnitTypeToSprite;

    //public EventHandler<bool> GameOver;

    UIManager UM;
    BoardManager BM;
    AudioManager AM;
    // Start is called before the first frame update
    void Start()
    {
        UM = FindObjectOfType<UIManager>();
        BM = FindObjectOfType<BoardManager>();
        AM = FindObjectOfType<AudioManager>();

        SetSpriteMap();

        UM.SetActiveUpgradePanel(false);

        //Spawn player units 
        BM.SpawnUnit(new Vector2Int(0, 0), UnitType.Basic, true);
        BM.SpawnUnit(new Vector2Int(1, 1), UnitType.Basic, true);
        BM.SpawnUnit(new Vector2Int(1, 0), UnitType.Basic, true);
        BM.SpawnUnit(new Vector2Int(2, 1), UnitType.Basic, true);
        BM.SpawnUnit(new Vector2Int(0, 1), UnitType.Basic, true);
        BM.SpawnUnit(new Vector2Int(1, 2), UnitType.Basic, true);

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
        UnitTypeToSprite[UnitType.Basic] = unitSprites[4];
    }

    void setUpgradeUnits()
    {
        UnitType[] upgradeTypes = new UnitType[3];
        upgradeTypes[0] = UnitType.Spider;
        upgradeTypes[1] = UnitType.Moth;
        upgradeTypes[2] = UnitType.Stag;

        UM.Unit1Button.GetComponentInChildren<TextMeshProUGUI>().text = upgradeTypes[0].ToString();
        UM.Unit2Button.GetComponentInChildren<TextMeshProUGUI>().text = upgradeTypes[1].ToString();
        UM.Unit3Button.GetComponentInChildren<TextMeshProUGUI>().text = upgradeTypes[2].ToString();

        UM.Image1.sprite = UnitTypeToSprite[upgradeTypes[0]];
        UM.Image2.sprite = UnitTypeToSprite[upgradeTypes[1]];
        UM.Image3.sprite = UnitTypeToSprite[upgradeTypes[2]];
    }

    void PopulateEnemyStartStates()
    {
        levelEnemyStartStates = new Dictionary<Vector2Int, UnitType>[finalLevel + 1];

        Dictionary<Vector2Int, UnitType> FirstLevelEnemyUnits = new Dictionary<Vector2Int, UnitType>();

        // example enemy units
        FirstLevelEnemyUnits[new Vector2Int(7, 6)] = UnitType.Basic;
        FirstLevelEnemyUnits[new Vector2Int(7, 4)] = UnitType.Basic;
        FirstLevelEnemyUnits[new Vector2Int(7, 1)] = UnitType.Basic;

        levelEnemyStartStates[0] = FirstLevelEnemyUnits;

        Dictionary<Vector2Int, UnitType> SecondLevelEnemyUnits = new Dictionary<Vector2Int, UnitType>();

        // example enemy units
        SecondLevelEnemyUnits[new Vector2Int(6, 3)] = UnitType.Basic;
        SecondLevelEnemyUnits[new Vector2Int(6, 2)] = UnitType.Basic;
        SecondLevelEnemyUnits[new Vector2Int(6, 1)] = UnitType.Basic;

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
        UM.SetActiveUpgradePanel(true);
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

    public void PickUpgradeUnit()
    {

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
