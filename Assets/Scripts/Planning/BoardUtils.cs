using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
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
        Tile[,] board = BM.board;

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
        // out of the bounds of the board (should never happen as raycast would return null)
        if (x < 0 || x >= BM.width || y < 0 || y >= BM.height)
            return null;

        int boardX = Mathf.Clamp(Mathf.FloorToInt(x), 0, BM.width - 1);
        int boardY = Mathf.Clamp(Mathf.FloorToInt(y), 0, BM.height - 1);

        return BM.board[boardX, boardY];
    }

    public static bool PlaceUnit(Unit unit, float x, float y, BoardManager BM)
    {
        // Bounds check should include `>=` to avoid overflow
        if (x < 0 || y < 0 || x >= BM.width || y >= BM.height)  // Updated here
        {
            Debug.Log("Invalid coordinate");
            return false;
        }

        // Check if the target tile already has a unit
        if (BM.board[(int)x, (int)y].unit != null)
        {
            Debug.Log("There is already a unit here");
            return false;
        }

        // Place unit on the board
        Debug.Log(unit.gameObject.name + ": " + x + ", " + y);

        unit.transform.position = new Vector3(x, 0f, y); // Set unit position

        // Clear previous tile
        BM.board[(int)unit.previousPosition.x, (int)unit.previousPosition.z].unit = null;

        // Update the new position as the previous position
        unit.previousPosition = new Vector3(x, 0, y);

        // Assign unit to the new tile
        BM.board[(int)x, (int)y].unit = unit;

        return true;
    }
}
