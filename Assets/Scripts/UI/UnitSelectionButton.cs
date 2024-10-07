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
        unitNameText.text = unitType.ToString();
        unitImage.sprite = GameManager.UnitTypeToSprite[unitType];
    }
}
