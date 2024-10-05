using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    Tile[,] board;

    private readonly int height = 8;
    private readonly int width = 8;
    // Start is called before the first frame update
    void Start()
    {
        board = new Tile[width,height];
        GenerateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                board[x, y] = new Tile(new Vector2(x, y));
            }
        }
    }

    void PlaceUnit(int x, int y)
    {
        Unit unit = new();
        //TODO: Bounds checking
        board[x,y].unit = unit;
    }

    public Tile[,] GetBoard()
    {
        return board;
    }
}
