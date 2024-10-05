using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUtils : MonoBehaviour
{
    BoardManager BM;
    // Start is called before the first frame update
    void Start()
    {
        BM = FindObjectOfType<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Unit GetUnitAtTile(int x, int y, BoardManager BM)
    {
        Tile[,] board = BM.GetBoard();

        Tile t = GetTileAt(x, y, BM);

        if (t.unit != null)
        {
            return board[x,y].unit;
        }
        return null; 
    }

    public static Tile GetTileAt(float x, float y, BoardManager BM)
    {
        Tile t = GetNearestTile(x, y, BM);
        return t;
    }

    public static Tile GetNearestTile(float x, float y, BoardManager BM)
    {
        for (int i = 0; i < BM.width; i++)
        {
            for (int j = 0; j < BM.height; j++)
            {
                if (Mathf.Abs(x - i) > 1 || Mathf.Abs(y - j) > 1)
                {
                    //The closest tile will be at most 1 unit away. So this can't be the closest.
                    continue;
                }
                return BM.board[i, j];
            }
        }
        //TODO: How should we handle trying to place outside the board. Currently this will just place at the closest

        //We didn't find any tile that is closest. So return null
        return null;
    }

    public static bool PlaceUnit(Unit unit, float x, float y, BoardManager BM)
    {
        Debug.Log("Utils: " + x + ", " + y);
        //Tile t = GetNearestTile(x, y, BM);

        //If tile has a unit on then don't place unit on the same tile
        //if(t.unit != null)
        //{
        //    return false;
        //}

        //If tile is out of bounds then don't place unit on tile
        if(x < 0 || y < 0 || x > 8 || y > 8)
        {
            Debug.Log("Invalid coordinate");
            return false;
        }
        //else
        //{

        //Otherwise place unit on time
            Debug.Log(unit.gameObject.name + ": " + x + ", " + y );
           // t.unit = unit;
            unit.GetComponentInParent<Transform>().transform.position = new Vector3(x, 0.11f, y + 0.5f);
        //unit.gameObject.GetComponentInChildren<Transform>().position = new Vector3(0, 0.11f, 0);

        //unit.gameObject.GetComponentInChildren<Transform>().rotation = Quaternion.Euler(new Vector3(80, 0, 0));
            BM.board[(int)x,(int)y].unit = unit;
        
            return true;
        //}
    }
}
