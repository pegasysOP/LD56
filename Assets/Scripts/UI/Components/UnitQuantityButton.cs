using System;
using TMPro;
using UnityEngine;

public class UnitQuantityButton : UnitButton
{
    public TextMeshProUGUI quantityText;

    public void Init(UnitType unitType, int quantity, Action<UnitType> onClick)
    {
        base.Init(unitType, onClick);
        SetQuantity(quantity);
    }

    public void SetQuantity(int quantity)
    {
        quantityText.text = quantity.ToString();

        // darken if empty
        if (quantity < 1)
        {
            unitIcon.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            button.interactable = false;
        }
        else
        {
            unitIcon.color = Color.white;
        }
    }
}
