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

        if (board[x,y].unit != null)
        {
            return board[x,y].unit;
        }
        return null; 
    }

    public static Tile GetTileAt(int x, int y, BoardManager BM)
    {
        GetNearestTile(x, y);
        return null;
    }

    public static Tile GetNearestTile(int x, int y)
    {
        return null;
    }
}
