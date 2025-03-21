using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    public UnitQuantityButton unitButtonPrefab;
    public Transform buttonContainer;

    private List<UnitQuantityButton> buttonComponents = new List<UnitQuantityButton>();

    public void Init(Action<UnitType> onUnitClicked, Dictionary<UnitType, int> inventoryContents, bool showEmpty = false)
    {
        ClearButtons();

        foreach ((UnitType unitType, int quantity) in inventoryContents)
        {
            if (!showEmpty && quantity < 1)
                continue;

            UnitQuantityButton unitButton = Instantiate(unitButtonPrefab, buttonContainer);
            unitButton.Init(unitType, quantity, onUnitClicked);
            buttonComponents.Add(unitButton);
        }
    }

    public void UpdateUnitQuantity(UnitType unitType, int newQuantity)
    {
        foreach (UnitQuantityButton unitButton in buttonComponents)
        {
            if (unitButton.GetUnitType() != unitType)
                continue;

            unitButton.SetQuantity(newQuantity);
        }
    }

    private void ClearButtons()
    {
        foreach (UnitQuantityButton button in buttonComponents)
            Destroy(button.gameObject);

        buttonComponents.Clear();
    }
}
