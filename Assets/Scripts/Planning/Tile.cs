using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile: MonoBehaviour
{
    public Unit unit;
    
    public Tile(Vector2 tilePosition)
    {
        unit = null; 
    }
}
