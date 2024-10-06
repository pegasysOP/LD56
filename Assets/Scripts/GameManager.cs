using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int level = 0;
    int finalLevel = 1;

    UIManager UM;
    BoardManager BM;
    AudioManager AM;
    // Start is called before the first frame update
    void Start()
    {
        UM = FindObjectOfType<UIManager>();
        BM = FindObjectOfType<BoardManager>();
        AM = FindObjectOfType<AudioManager>();
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
        //Get the current levels enemy configuration array 

        //Play the attack phase music 
        AM.PlaySimulationPhaseClip();
        //call onStartRoundPressed on boardManager and pass along the enemy configuration
    }

}
