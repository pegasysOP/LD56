using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    Tile[,] board;

    private int height = 8;
    private int width = 8;
    // Start is called before the first frame update
    void Start()
    {
        board = new Tile[width,height];
        for(int x=0; x < width; x++)
        {
            for( int y=0; y < height; y++)
            {
                board[x, y] = new Tile(new Vector2(x, y));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
