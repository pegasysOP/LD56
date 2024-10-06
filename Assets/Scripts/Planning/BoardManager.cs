using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public LayerMask boardMask;
    public LayerMask unitMask;
    public LayerMask outOfBoundsMask;

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

    public Tile[,] savedBoard;

    public Dictionary<Vector3, Unit> PositionToUnitDictionary = new Dictionary<Vector3, Unit>();

    // Start is called before the first frame update
    void Start()
    {
        board = new Tile[width, height];
        //Create the game objects for each tile. Currently each tile has no unit attatched. 
        GenerateBoard();

        //Place two units on the board
        SpawnUnit(1, 3);
        SpawnUnit(1, 1);

        SaveBoard();
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
                GameObject GO = Instantiate(tileGO, new Vector3(x + 0.5f, 0f, y + 0.5f), Quaternion.identity);
                board[x, y] = GO.GetComponent<Tile>();
                GO.name = "Tile" + x + ", " + y;
                GO.transform.parent = transform; // set parent to the manager to not clog hierarchy
            }
        }
    }

    void SpawnUnit(int x, int y)
    {

        GameObject GO = Instantiate(unitGO);
        Unit unit = GO.GetComponent<Unit>();
        GO.name = "Unit" + x + ", " + y;

        units.Add(unit);
        BoardUtils.PlaceUnit(unit, x + 0.5f, y + 0.5f, this);
        board[x, y].unit = unit;

    }

    void DragUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            //Check that we are clicking on a unit and not just the board
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, unitMask))
            {
                isAttached = true;
                unitHit = hit.transform.gameObject;

                Vector3 mouseWorldPosition = hit.point;

                // Store the current position as previousPosition before detaching
                Unit unitComponent = unitHit.GetComponent<Unit>();
                unitComponent.previousPosition = unitHit.transform.position;

                offset = unitHit.transform.position - mouseWorldPosition;

                //Get the tile this unit was attached to and remove it from that tile.
                Tile t = BoardUtils.GetNearestTile(unitHit.transform.position.x, unitHit.transform.position.z, this);
                t.unit = null; // This tile no longer holds the unit
            }
        }

        if (isAttached && unitHit != null)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, boardMask | outOfBoundsMask))
            {
                //Update the unit position as we drag, with offset correction
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
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, boardMask))
            {
                Tile t = BoardUtils.GetNearestTile(hit.point.x, hit.point.z, this);

                if (t.unit == null)
                {
                    bool successful = BoardUtils.PlaceUnit(unitHit.GetComponent<Unit>(), t.transform.position.x, t.transform.position.z, this);
                    if (successful)
                    {
                        Debug.Log("Successfully placed unit.");
                        t.unit = unitHit.GetComponent<Unit>();
                    }
                    else
                    {
                        Debug.Log("Could not place unit on tile. Reverting to previous position.");
                        BoardUtils.PlaceUnit(unitHit.GetComponent<Unit>(), unitHit.GetComponent<Unit>().previousPosition.x, unitHit.GetComponent<Unit>().previousPosition.z, this);
                    }
                }
                else
                {
                    Debug.Log("Tile is already occupied. Reverting to previous position.");
                    BoardUtils.PlaceUnit(unitHit.GetComponent<Unit>(), unitHit.GetComponent<Unit>().previousPosition.x, unitHit.GetComponent<Unit>().previousPosition.z, this);
                }
            }
            else
            {
                BoardUtils.PlaceUnit(unitHit.GetComponent<Unit>(), unitHit.GetComponent<Unit>().previousPosition.x, unitHit.GetComponent<Unit>().previousPosition.z, this);
            }
            isAttached = false;
            unitHit = null;
        }
    }

    public void SaveBoard()
    {
        savedBoard = board;

        PositionToUnitDictionary.Clear();
        //Save Units and positions
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(board[x, y].unit != null)
                {
                    PositionToUnitDictionary.Add(new Vector3(x, 0, y), board[x, y].unit);
                }
            }
        }

    }
}