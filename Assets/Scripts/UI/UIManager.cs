using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public UpgradePanel upgradePanel;
    public GameObject StartRoundButton;
    public GameObject InventoryPanel;
    public GameObject InstructionPanel;

    private BoardManager BM;

    // Start is called before the first frame update
    void Start()
    {
        BM = FindObjectOfType<BoardManager>(); 

        if(SceneManager.GetActiveScene().name == SceneUtils.MENU_SCENE)
        {
            //   AudioManager.Instance.PlayMenuClip();
        }
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
        SceneUtils.LoadGameScene();
    }

    public void LoadMenuScene()
    {
        AudioManager.Instance.PlayMenuClip();
        AudioManager.Instance.PlayRegularButtonClip();
        SceneUtils.LoadMenuScene();
    }

    public void LoadCreditScene()
    {

        AudioManager.Instance.PlayVictoryFanfareClip();
        SceneUtils.LoadCreditScene();
    }

    public void LoadInstructionScene()
    {
        AudioManager.Instance.PlayMenuClip();
        AudioManager.Instance.PlayRegularButtonClip();
        SceneUtils.LoadInstructionScene();
    }

    public void QuitButtonPressed()
    {
        SceneUtils.QuitApplication();
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

    public void SetInformationPanelActive()
    {
        if (InstructionPanel.activeInHierarchy)
        {
            InstructionPanel.SetActive(false);
            
        }
        else
        {
            InstructionPanel.SetActive(true);
            //upgradePanel.gameObject.SetActive(false);
            //StartRoundButton.SetActive(false);
        }
    }
}
