using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int level = 0;
    int finalLevel = 1;

    private Dictionary<Vector2Int, Unit>[] levelEnemyStartStates;

    UIManager UM;
    BoardManager BM;
    AudioManager AM;
    // Start is called before the first frame update
    void Start()
    {
        UM = FindObjectOfType<UIManager>();
        BM = FindObjectOfType<BoardManager>();
        AM = FindObjectOfType<AudioManager>();

        PopulateEnemyStartStates();
        BM.LoadEnemyUnits(levelEnemyStartStates[level]);
    }

    void PopulateEnemyStartStates()
    {
        levelEnemyStartStates = new Dictionary<Vector2Int, Unit>[finalLevel + 1];

        Dictionary<Vector2Int, Unit> FirstLevelEnemyUnits = new Dictionary<Vector2Int, Unit>();

        // example enemy units
        FirstLevelEnemyUnits[new Vector2Int(7, 6)] = new Unit();
        FirstLevelEnemyUnits[new Vector2Int(7, 4)] = new Unit();
        FirstLevelEnemyUnits[new Vector2Int(7, 1)] = new Unit();

        levelEnemyStartStates[0] = FirstLevelEnemyUnits;

        Dictionary<Vector2Int, Unit> SecondLevelEnemyUnits = new Dictionary<Vector2Int, Unit>();

        // example enemy units
        SecondLevelEnemyUnits[new Vector2Int(6, 3)] = new Unit();
        SecondLevelEnemyUnits[new Vector2Int(6, 2)] = new Unit();
        SecondLevelEnemyUnits[new Vector2Int(6, 1)] = new Unit();

        levelEnemyStartStates[1] = SecondLevelEnemyUnits;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onRoundWon();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            startLevel();
        }
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
    }

    void onGameWon()
    {
        Debug.Log("You won!");
        UM.LoadMenuScene();
    }

    void getNextLevel()
    {
        Debug.Log("Getting next level");
        level++;
        if(level > finalLevel)
        {
            onGameWon();
        }
        //Get dictioary of next level. 

        //Pass that to the simulation
    }

    void startLevel()
    {
        Debug.Log("Start Level"); 

        //Play the attack phase music 
        AM.PlaySimulationPhaseClip();
        //call onStartRoundPressed on boardManager and pass along the enemy configuration   
    }
}
