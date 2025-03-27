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

        unitNameText.text = UnitData.GetUnitNameText(unitType);
        unitIcon.sprite = UnitData.GetUnitSprite(unitType);
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
}
