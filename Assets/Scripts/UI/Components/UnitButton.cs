using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI unitNameText;
    public Image unitIcon;

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
        unitIcon.sprite = GameManager.UnitTypeToSprite[unitType];
    }

    private void OnButtonClick()
    {
        if (onClick != null)
        {
            onClick(unitType);
        }
    }

    public UnitType GetUnitType()
    {
        return unitType;
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
