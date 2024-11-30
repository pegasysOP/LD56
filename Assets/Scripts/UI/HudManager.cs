using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public UpgradePanel upgradePanel;
    public Button startRoundButton;
    public GameObject inventoryPanel;
    public GameObject instructionPanel;

    private void OnEnable()
    {
        startRoundButton.onClick.AddListener(OnStartRoundButtonClick);
    }

    private void OnDisable()
    {
        startRoundButton.onClick.RemoveListener(OnStartRoundButtonClick);
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
        inventoryPanel.gameObject.SetActive(isActive);
    }

    public void SetActiveStartButton(bool isActive)
    {
        startRoundButton.gameObject.SetActive(isActive);
    }

    public void ShowUpgradePanel()
    {
        SetActiveUpgradePanel(true);
        SetActiveInventoryPanel(false);
        SetActiveStartButton(false);
        BoardManager.Instance.SetSelectionEnabled(false);
    }

    public void SetInformationPanelActive()
    {
        if (instructionPanel.activeInHierarchy)
        {
            instructionPanel.SetActive(false);
            
        }
        else
        {
            instructionPanel.SetActive(true);
            //upgradePanel.gameObject.SetActive(false);
            //StartRoundButton.SetActive(false);
        }
    }

    private void OnStartRoundButtonClick()
    {
        GameManager.Instance.StartLevel();
        startRoundButton.gameObject.SetActive(false);
    }
}
