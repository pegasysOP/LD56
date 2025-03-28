using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public UpgradePanel upgradePanel;
    public InventoryPanel inventoryPanel;
    public InstructionPanel instructionPanel;

    public Button startRoundButton;
    public Button unitInfoButton;


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
    
    public void Init()
    {
        HideUpgradePanel();
        SetActiveStartButton(false);
    }

    public void SetActiveStartButton(bool isActive)
    {
        startRoundButton.gameObject.SetActive(isActive);
    }

    public void ShowInventoryPanel(Action<UnitType> onUnitClicked, Dictionary<UnitType, int> inventoryContents)
    {
        inventoryPanel.Init(onUnitClicked, inventoryContents);
        inventoryPanel.gameObject.SetActive(true);
    }

    public void HideInventoryPanel()
    {
        inventoryPanel.gameObject.SetActive(false);
    }

    public void ShowUpgradePanel(Action<UnitType> onUnitChosen, UnitType option1, UnitType option2, UnitType option3)
    {
        upgradePanel.SetUnitOptions(onUnitChosen, option1, option2, option3);
        upgradePanel.gameObject.SetActive(true);

        HideInventoryPanel();
        SetActiveStartButton(false);
        BoardManager.Instance.SetSelectionEnabled(false);
    }

    public void HideUpgradePanel()
    {
        upgradePanel.gameObject.SetActive(false);
    }

    public void OnUnitInfoButtonClick()
    {
        if (instructionPanel.gameObject.activeInHierarchy)
        {
            instructionPanel.gameObject.SetActive(false); 
        }
        else
        {
            instructionPanel.gameObject.SetActive(true);
            instructionPanel.Init();
        }
    }

    private void OnStartRoundButtonClick()
    {
        GameManager.Instance.StartLevel();
        startRoundButton.gameObject.SetActive(false);
    }
}
