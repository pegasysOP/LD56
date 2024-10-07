using System;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    public UnitSelectionButton button1;
    public UnitSelectionButton button2;
    public UnitSelectionButton button3;

    public EventHandler<UnitType> UnitChosen;

    private UnitType unitType1;
    private UnitType unitType2;
    private UnitType unitType3;

    public void SetUnitOptions(UnitType unitType1, UnitType unitType2, UnitType unitType3)
    {
        this.unitType1 = unitType1;
        this.unitType2 = unitType2;
        this.unitType3 = unitType3;

        button1.SetUnit(unitType1);
        button2.SetUnit(unitType2);
        button3.SetUnit(unitType3);

        button1.button.onClick.AddListener(() => OnUnitButtonClick(this.unitType1));
        button2.button.onClick.AddListener(() => OnUnitButtonClick(this.unitType2));
        button3.button.onClick.AddListener(() => OnUnitButtonClick(this.unitType3));
    }

    public void OnUnitButtonClick(UnitType unitType)
    {
        UnitChosen?.Invoke(this, unitType);
    }
}
