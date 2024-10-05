using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Tile[,] board;
    public List<Unit> units;

    public readonly int height = 8;
    public readonly int width = 8;

    private bool isDragging = false;
    private bool isAttached = false;
    GameObject unitHit = null;
    private Vector3 offset;
    public GameObject tileGO;
    public GameObject unitGO;

    // Start is called before the first frame update
    void Start()
    {
        board = new Tile[width,height];
        //Create the game objects for each tile. Currently each tile has no unit attatched. 
        GenerateBoard();

        //Place two units on the board
        PlaceUnit(2, 2);
        PlaceUnit(7, 7);
    }

    // Update is called once per frame
    void Update()
    {
        DragUnit();
    }


    void GenerateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {     
                GameObject GO = Instantiate(tileGO, new Vector3(x, 0.1f, y), Quaternion.identity);
                board[x, y] = GO.GetComponent<Tile>();
                GO.name = "Tile" + x + ", " + y;  
            }
        }
    }

    void PlaceUnit(int x, int y)
    {
        GameObject GO = Instantiate(unitGO);
        GO.name = "Unit" + x + ", " + y;
        //Tile t = BoardUtils.GetNearestTile(x, y, this);

        //Assign the unit to the tile it is currently occupying 
        board[x, y].unit = GO.GetComponent<Unit>();
        units.Add(GO.GetComponent<Unit>());
        Debug.Log(x + ", " + y);
        bool successful = BoardUtils.PlaceUnit(board[x,y].unit, x, y, this);
        if (successful)
        {
            //Use previousPosition later on to return the unit to it's previous position if the move was unsuccessful 
            board[x,y].unit.previousPosition = board[x,y].unit.transform.position;
        } 
    }

    public Tile[,] GetBoard()
    {
        return board;
    }

    public void DragUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Input.GetMouseButtonDown(0))
        {
            //Check that we are clicking on a unit and not just the board
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
            {
                if (hit.transform.tag == "Unit")
                {
                    isAttached = true;                   
                    unitHit = hit.transform.gameObject;     

                    Vector3 mouseWorldPosition = hit.point;
                    offset = unitHit.transform.position - mouseWorldPosition;

                    //Get the tile this unit was attatched to and remove it from that tile. 
                }
            }
        }

        //If we have a unit attatched then find out where the board is and the required offsets for it to track the mouse
        if (isAttached && unitHit != null)
        {
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
            {
                //Get the board position
                Vector3 boardPosition = hit.point;

                unitHit.transform.position = new Vector3(
                   boardPosition.x + offset.x, 
                   unitHit.transform.position.y, 
                   boardPosition.z + offset.z
                );
            }
        }

        if (Input.GetMouseButtonUp(0) && isAttached)
        {
            //Find the closest tile to the mouse and attach the unit to that tile. 
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Tile t = BoardUtils.GetNearestTile(mousePos.x, mousePos.z, this);
            if (t.unit == null)
            {
                bool successful = BoardUtils.PlaceUnit(unitHit.GetComponent<Unit>(), t.transform.position.x, t.transform.position.z, this);
                if (successful)
                {
                    //If this tile is valid then place the unit on that tile
                    t.unit = unitHit.GetComponent<Unit>();
                    Tile nearest = BoardUtils.GetNearestTile(unitHit.GetComponent<Unit>().previousPosition.x, unitHit.GetComponent<Unit>().previousPosition.z, this);
                    nearest.unit = unitHit.GetComponent<Unit>();
                    BoardUtils.PlaceUnit(unitHit.GetComponent<Unit>(), nearest.transform.position.x, nearest.transform.position.z, this);
                    board[(int)nearest.unit.previousPosition.x, (int)nearest.unit.previousPosition.z].unit = null;
                    
                }
                else
                {
                    //If this tile isn't valid then place unit back on previous tile
                    BoardUtils.PlaceUnit(unitHit.GetComponent<Unit>(), unitHit.GetComponent<Unit>().previousPosition.x, unitHit.GetComponent<Unit>().previousPosition.z, this);
                }
                
            }
            isAttached = false;
            unitHit = null;

        }
    }
}
