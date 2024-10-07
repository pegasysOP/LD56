using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public const string GAME_SCENE = "Lewis Test";
    public const string MENU_SCENE = "Menu";
    public const string CREDIT_SCENE = "Credits";
    public const string INSTRUCTION_SCENE = "Instructions";

    public UpgradePanel upgradePanel;
    public GameObject StartRoundButton;

    private BoardManager BM;

    // Start is called before the first frame update
    void Start()
    {
        BM = FindObjectOfType<BoardManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(MENU_SCENE);
    }

    public void LoadCreditScene()
    {
        SceneManager.LoadScene(CREDIT_SCENE);
    }

    public void LoadInstructionScene()
    {
        SceneManager.LoadScene(INSTRUCTION_SCENE);
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }

    public void PickUnitPressed(UnitType type)
    {
        //Spawn a unit in the next avaialble space based on UnitType. Maybe this should be in BM
        upgradePanel.gameObject.SetActive(false);
    }

    public void SetActiveUpgradePanel(bool isActive)
    {
        upgradePanel.gameObject.SetActive(isActive);
    }
}