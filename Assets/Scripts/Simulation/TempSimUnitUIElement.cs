using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TempSimUnitUIElement : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI specialText;
    public Image background;

    public void SetUnit(SimulationUnit unit)
    {
        background.color = unit.IsPlayerUnit() ? new Color(0f, 0f, 1f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

        nameText.text = unit.GetUnitType().ToString();
        hpText.text = "HP: " + Mathf.CeilToInt(unit.GetCurrentHpPortion() * 100) + "%";
        specialText.text = "SP: " + Mathf.CeilToInt(unit.GetSpecialProgress() * 100) + "%";
    }
}
