using System.Collections.Generic;

public class UnitInventory
{
    public Dictionary<UnitType, int> Units { get; private set; }

    public UnitInventory()
    {
        Units = new Dictionary<UnitType, int>
        {
            { UnitType.WorkerBee, 0 }, 
            { UnitType.Beetle, 0 },
            { UnitType.Moth, 0 },
            { UnitType.Spider, 0 },
            { UnitType.QueenBee, 0 }
        };
    }

    public void AddUnit(UnitType unitType)
    {
        if (Units.ContainsKey(unitType))
        {
            Units[unitType]++;
        }
    }

    public bool CanPlaceUnit(UnitType unitType)
    {
        return Units.ContainsKey(unitType) && Units[unitType] > 0;
    }

    public void PlaceUnit(UnitType unitType)
    {
        if (CanPlaceUnit(unitType))
        {
            Units[unitType]--;
        }
    }

    public int GetUnitCount(UnitType unitType)
    {
        return Units.ContainsKey(unitType) ? Units[unitType] : 0;
    }
}