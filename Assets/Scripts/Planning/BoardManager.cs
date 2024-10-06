using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject unitPrefab;

    [Header("Layer Masks")]
    public LayerMask boardMask;
    public LayerMask unitMask;
    public LayerMask outOfBoundsMask;

    [Header("Simulation")]
    public Simulation simulation;

    [Header("Units")]
    public Transform unitContainer;
    public Dictionary<Vector2Int, Unit> ActiveUnits;


    private Tile[,] board;

    private readonly int height = 8;
    private readonly int width = 8;

    private bool isAttached = false;
    private GameObject selectedUnit = null;
    private Vector3 offset;

    private Dictionary<Vector2Int, Unit> playerUnitsStartState = new Dictionary<Vector2Int, Unit>();
    private Dictionary<Vector2Int, Unit> enemyUnitsStartState;



    public static BoardManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        board = new Tile[width, height];
        GenerateBoard();
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
                GameObject GO = Instantiate(tilePrefab, new Vector3(x + 0.5f, 0f, y + 0.5f), Quaternion.identity);
                board[x, y] = GO.GetComponent<Tile>();
                GO.name = "Tile" + x + ", " + y;
                GO.transform.parent = transform; // set parent to the manager to not clog hierarchy
            }
        }
    }

    void SpawnUnit(Vector2Int location)
    {
        GameObject GO = Instantiate(unitPrefab, unitContainer);
        Unit unit = GO.GetComponent<Unit>();
        GO.name = "Unit" + location.x + ", " + location.y;

        PlaceUnit(unit, location.x + 0.5f, location.y + 0.5f);
        board[location.x, location.y].unit = unit;
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
                selectedUnit = hit.transform.gameObject;

                Vector3 mouseWorldPosition = hit.point;

                // Store the current position as previousPosition before detaching
                Unit unitComponent = selectedUnit.GetComponent<Unit>();
                unitComponent.previousPosition = selectedUnit.transform.position;

                offset = selectedUnit.transform.position - mouseWorldPosition;

                //Get the tile this unit was attached to and remove it from that tile.
                Tile t = GetNearestTile(selectedUnit.transform.position.x, selectedUnit.transform.position.z);
                t.unit = null; // This tile no longer holds the unit
            }
        }

        if (isAttached && selectedUnit != null)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, boardMask | outOfBoundsMask))
            {
                //Update the unit position as we drag, with offset correction
                Vector3 boardPosition = hit.point;

                selectedUnit.transform.position = new Vector3(
                   boardPosition.x + offset.x,
                   selectedUnit.transform.position.y,
                   boardPosition.z + offset.z
                );
            }
        }

        if (Input.GetMouseButtonUp(0) && isAttached)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, boardMask))
            {
                Tile t = GetNearestTile(hit.point.x, hit.point.z);

                if (t.unit == null)
                {
                    bool successful = PlaceUnit(selectedUnit.GetComponent<Unit>(), t.transform.position.x, t.transform.position.z);
                    if (successful)
                    {
                        Debug.Log("Successfully placed unit.");
                        t.unit = selectedUnit.GetComponent<Unit>();
                    }
                    else
                    {
                        Debug.Log("Could not place unit on tile. Reverting to previous position.");
                        PlaceUnit(selectedUnit.GetComponent<Unit>(), selectedUnit.GetComponent<Unit>().previousPosition.x, selectedUnit.GetComponent<Unit>().previousPosition.z);
                    }
                }
                else
                {
                    Debug.Log("Tile is already occupied. Reverting to previous position.");
                    PlaceUnit(selectedUnit.GetComponent<Unit>(), selectedUnit.GetComponent<Unit>().previousPosition.x, selectedUnit.GetComponent<Unit>().previousPosition.z);
                }
            }
            else
            {
                PlaceUnit(selectedUnit.GetComponent<Unit>(), selectedUnit.GetComponent<Unit>().previousPosition.x, selectedUnit.GetComponent<Unit>().previousPosition.z);
            }
            isAttached = false;
            selectedUnit = null;
        }
    }

    public void SaveBoard()
    {
        playerUnitsStartState.Clear();
        //Save Units and positions
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(board[x, y].unit != null)
                {
                    playerUnitsStartState.Add(new Vector2Int(x, y), board[x, y].unit);
                }
            }
        }
    }

    public Unit GetUnitAtTile(int x, int y)
    {
        Tile t = GetNearestTile(x, y);
        if (t.unit != null)
            return t.unit;// board[x, y].unit;

        return null;
    }

    public Tile GetNearestTile(float x, float y)
    {
        // out of the bounds of the board (should never happen as raycast would return null)
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;

        int boardX = Mathf.Clamp(Mathf.FloorToInt(x), 0, width - 1);
        int boardY = Mathf.Clamp(Mathf.FloorToInt(y), 0, height - 1);

        return board[boardX, boardY];
    }

    public bool PlaceUnit(Unit unit, float x, float y)
    {
        if (x < 0 || y < 0 || x > width || y >= height)
        {
            Debug.Log("Invalid coordinate");
            return false;
        }

        // Check if the target tile already has a unit
        if (board[(int)x, (int)y].unit != null)
        {
            Debug.Log("There is already a unit here");
            return false;
        }

        unit.transform.position = new Vector3(x, 0f, y);
        board[(int)unit.previousPosition.x, (int)unit.previousPosition.z].unit = null;
        unit.previousPosition = new Vector3(x, 0, y);
        board[(int)x, (int)y].unit = unit;

        return true;
    }

    public void LoadEnemyUnits(Dictionary<Vector2Int, Unit> enemyUnitsStartState)
    {
        this.enemyUnitsStartState = enemyUnitsStartState;

        foreach (KeyValuePair<Vector2Int, Unit> unitLocation in enemyUnitsStartState)
            SpawnUnit(unitLocation.Key);
    }

    private void ClearBoardUnits()
    {
        for (int i = unitContainer.childCount - 1; i >= 0; i--)
            DestroyImmediate(unitContainer.GetChild(i).gameObject);
    }

    public void OnStartRoundPressed()
    {
        SaveBoard();

        if (playerUnitsStartState == null || playerUnitsStartState.Count < 1)
            return;

        if (enemyUnitsStartState == null || enemyUnitsStartState.Count < 1)
            return;

        ActiveUnits = new Dictionary<Vector2Int, Unit>();
        ActiveUnits.AddRange(playerUnitsStartState);
        ActiveUnits.AddRange(enemyUnitsStartState);

        simulation.StartSimulation(playerUnitsStartState, enemyUnitsStartState);
    }    
}