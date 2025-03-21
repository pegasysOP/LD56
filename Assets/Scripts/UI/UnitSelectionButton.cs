using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI unitNameText;
    public Image unitImage;

    private UnitType unitType;
    private Action<UnitType> onClick;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    public void Init(UnitType unitType, Action<UnitType> onClick)
    {
        this.unitType = unitType;
        this.onClick = onClick;

        unitNameText.text = GetUnitNameText(unitType);
        unitImage.sprite = GameManager.UnitTypeToSprite[unitType];
    }

    private void OnButtonClick()
    {
        if (onClick != null)
        {
            onClick(unitType);

            // clear after to avoid duping
            onClick = null;
        }
    }

    // TODO: This should be included in the big data file
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
}
