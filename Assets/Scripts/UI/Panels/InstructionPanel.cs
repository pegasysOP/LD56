using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPanel : MonoBehaviour
{
    public UnitDisplayComponent unitComponentPrefab;
    public Transform row1;
    public Transform row2;

    public Transform effectsComponent;

    private bool initialized = false;

    public void Init()
    {
        if (initialized)
            return;

        SpawnUnitComponents();
        effectsComponent.SetAsLastSibling();

        initialized = true;
    }

    private void SpawnUnitComponents()
    {
        // 1 extra for effects description
        int rowWidth = (UnitData.GetUnitCount() / 2) + 1;

        for (int i = 0; i < UnitData.GetUnitCount(); i++)
        {
            UnitType unitType = (UnitType)i;
            Transform row = i < rowWidth ? row1 : row2;
            SpawnUnitComponent((UnitType)i, row);
        }
    }

    private void SpawnUnitComponent(UnitType unitType, Transform parent)
    {
        UnitDisplayComponent unitComponent = Instantiate(unitComponentPrefab, parent);
        unitComponent.Init(unitType);
    }
}
