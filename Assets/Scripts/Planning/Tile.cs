using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector2 tilePosition;
    public Unit unit;
    
    public Tile(Vector2 tilePosition)
    {
        this.tilePosition = tilePosition;
        unit = null; 
    }


}
