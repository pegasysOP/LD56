using System;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    public UnitSelectionButton button1;
    public UnitSelectionButton button2;
    public UnitSelectionButton button3;

    public void SetUnitOptions(Action<UnitType> onUnitChosen, UnitType option1, UnitType option2, UnitType option3)
    {
        button1.Init(option1, onUnitChosen);
        button2.Init(option2, onUnitChosen);
        button3.Init(option3, onUnitChosen);
    }
}
