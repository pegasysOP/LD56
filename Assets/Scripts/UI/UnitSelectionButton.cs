using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI unitNameText;
    public Image unitImage;

    public void SetUnit(UnitType unitType)
    {
        unitNameText.text = GetUnitNameText(unitType);
        unitImage.sprite = GameManager.UnitTypeToSprite[unitType];
    }

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
