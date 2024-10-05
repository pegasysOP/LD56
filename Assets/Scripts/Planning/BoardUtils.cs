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
}
