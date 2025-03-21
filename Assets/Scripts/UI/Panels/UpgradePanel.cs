using System;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    public UnitButton button1;
    public UnitButton button2;
    public UnitButton button3;

    public void SetUnitOptions(Action<UnitType> onUnitChosen, UnitType option1, UnitType option2, UnitType option3)
    {
        button1.Init(option1, onUnitChosen);
        button2.Init(option2, onUnitChosen);
        button3.Init(option3, onUnitChosen);
    }
}
