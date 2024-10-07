using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile: MonoBehaviour
{
    public bool IsPlayerSpace = false;
    public Unit currentUnit = null;

    public SpriteRenderer indicatorRenderer;
    public Color emptyColor;
    public Color occupiedColor;

    public void ShowIndicator(bool enabled)
    {
        // only show for player spaces
        if (!IsPlayerSpace)
        {
            indicatorRenderer.gameObject.SetActive(false);
            return;
        }

        indicatorRenderer.gameObject.SetActive(enabled);
        indicatorRenderer.color = (currentUnit == null) ? emptyColor : occupiedColor;
    }
}
