using TMPro;

public class UnitDisplayComponent : UnitButton
{
    public TextMeshProUGUI descriptionText;

    public void Init(UnitType unitType)
    {
        base.Init(unitType, null);

        descriptionText.text = UnitData.GetUnitDescriptionText(unitType);
    }
}
