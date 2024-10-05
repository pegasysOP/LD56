using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    Tile[,] board;

    private readonly int height = 8;
    private readonly int width = 8;

    private bool isDragging = false;
    private bool isAttached = false;
    GameObject unitHit = null;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        board = new Tile[width,height];
        GenerateBoard();
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

        if (Input.GetMouseButtonUp(0))
        {
            isAttached = false;   
            unitHit = null;        
        }


        //If we hit an object then attatch it to the mouse until we stop dragging

        //Find the nearest tile using util class and "plop" the unit in that tile assuming no enemy there
    }
}
