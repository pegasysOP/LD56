using System;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public UpgradePanel upgradePanel;
    public Button startRoundButton;
    public Button unitInfoButton;
    public GameObject inventoryPanel;
    public GameObject instructionPanel;

    private void OnEnable()
    {
        startRoundButton.onClick.AddListener(OnStartRoundButtonClick);
        unitInfoButton.onClick.AddListener(OnUnitInfoButtonClick);

    }

    private void OnDisable()
    {
        startRoundButton.onClick.RemoveListener(OnStartRoundButtonClick);
        unitInfoButton.onClick.AddListener(OnUnitInfoButtonClick);
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

    public void SetActiveInventoryPanel(bool isActive)
    {
        inventoryPanel.gameObject.SetActive(isActive);
    }

    public void SetActiveStartButton(bool isActive)
    {
        startRoundButton.gameObject.SetActive(isActive);
    }

    public void ShowUpgradePanel(Action<UnitType> onUnitChosen, UnitType option1, UnitType option2, UnitType option3)
    {
        upgradePanel.SetUnitOptions(onUnitChosen, option1, option2, option3);
        upgradePanel.gameObject.SetActive(true);

        SetActiveInventoryPanel(false);
        SetActiveStartButton(false);
        BoardManager.Instance.SetSelectionEnabled(false);
    }

    public void HideUpgradePanel()
    {
        upgradePanel.gameObject.SetActive(false);
    }

    public void OnUnitInfoButtonClick()
    {
        if (instructionPanel.activeInHierarchy)
        {
            instructionPanel.SetActive(false); 
        }
        else
        {
            instructionPanel.SetActive(true);
        }
    }

    private void OnStartRoundButtonClick()
    {
        GameManager.Instance.StartLevel();
        startRoundButton.gameObject.SetActive(false);
    }
}
