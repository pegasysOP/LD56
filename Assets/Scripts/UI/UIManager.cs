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
    public GameObject InventoryPanel;

    private BoardManager BM;

    // Start is called before the first frame update
    void Start()
    {
        BM = FindObjectOfType<BoardManager>(); 

        if(SceneManager.GetActiveScene().name == MENU_SCENE)
        {
            //   AudioManager.Instance.PlayMenuClip();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetUnitNameText(UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Beetle:
                return "Beetle";
            case UnitType.Moth:
                return "Moth";
            case UnitType.QueenBee:
                return "Queen Bee";
            case UnitType.Spider:
                return "Spider";
            case UnitType.WorkerBee:
                return "Worker Bee";
            case UnitType.FireAnt:
                return "Fire Ant";
            default:
                return "x";
        }
    }

    public void LoadGameScene()
    {
        AudioManager.Instance.PlayRegularButtonClip();
        SceneManager.LoadScene(GAME_SCENE);
    }

    public void LoadMenuScene()
    {
        AudioManager.Instance.PlayMenuClip();
        AudioManager.Instance.PlayRegularButtonClip();
        SceneManager.LoadScene(MENU_SCENE);
    }

    public void LoadCreditScene()
    {

        AudioManager.Instance.PlayVictoryFanfareClip();
        SceneManager.LoadScene(CREDIT_SCENE);
    }

    public void LoadInstructionScene()
    {
        AudioManager.Instance.PlayMenuClip();
        AudioManager.Instance.PlayRegularButtonClip();
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

    public void SetActiveInventoryPanel(bool isActive)
    {
        InventoryPanel.gameObject.SetActive(isActive);
    }

    public void SetActiveStartButton(bool isActive)
    {
        StartRoundButton.gameObject.SetActive(isActive);
    }

    public void ShowUpgradePanel()
    {
        SetActiveUpgradePanel(true);
        SetActiveInventoryPanel(false);
        SetActiveStartButton(false);
        BM.setSelectionEnabled(false);
    }
}
