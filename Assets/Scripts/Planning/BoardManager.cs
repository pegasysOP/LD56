using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    [Header("Layer Masks")]
    public LayerMask boardMask;
    public LayerMask unitMask;
    public LayerMask outOfBoundsMask;

    [Header("Simulation")]
    public Simulation simulation;

    [Header("Tiles")]
    public Tile tilePrefab;
    public Transform tileContainer;

    [Header("Units")]
    public Unit unitPrefab;
    public Transform unitContainer;
    public Dictionary<Vector2Int, Unit> ActiveUnits;


    private Tile[,] board;

    private readonly int height = 8;
    private readonly int width = 8;

    private bool isAttached = false;
    private GameObject selectedUnit = null;
    private Vector3 offset;

    private Dictionary<Vector2Int, Unit> playerUnitsStartState = new Dictionary<Vector2Int, Unit>();
    private Dictionary<Vector2Int, Unit> enemyUnitsStartState = new Dictionary<Vector2Int, Unit>();

    public static BoardManager Instance;

    private void Awake()
    {
        if (Instance == null)
        { 
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            return;
        }

        board = new Tile[width, height];
        GenerateBoard();
    }

    private void Start()
    {
        SpawnUnit(new Vector2Int(0, 0));
        SpawnUnit(new Vector2Int(0, 3));

        SavePlayerUnitStartPositions();
    }

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
                Tile tile = Instantiate(tilePrefab, new Vector3(x + 0.5f, 0f, y + 0.5f), Quaternion.identity, tileContainer);
                board[x, y] = tile;
                tile.name = "Tile [" + x + "," + y + "]";
            }
        }
    }

    private Unit SpawnUnit(Vector2Int location)
    {
        Unit unit = Instantiate(unitPrefab, unitContainer);
        unit.name = "Unit" + location.x + ", " + location.y;

        unit.transform.position = new Vector3(location.x + 0.5f, 0f, location.y + 0.5f);
        //PlaceUnit(unit, location.x + 0.5f, location.y + 0.5f); // Sam - was setting the tiles unit to null for some reason
        board[location.x, location.y].unit = unit;

        return unit;
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

    public void SavePlayerUnitStartPositions()
    {
        playerUnitsStartState.Clear();

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(board[x, y].unit != null)
                    playerUnitsStartState.Add(new Vector2Int(x, y), board[x, y].unit);
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
        if (x < 0 || y < 0 || x > width || y >= height) // TODO: ADD BACK VALIDATION TO DRAGGING
        {
            Debug.Log("Invalid coordinate");
            return false;
        }

        Tile tile = GetNearestTile(x, y);
        if (tile == null)
            return false;

        // Check if the target tile already has a unit
        if (tile.unit != null)
        {
            Debug.Log("There is already a unit here");
            return false;
        }

        unit.transform.position = new Vector3(x, 0f, y);
        GetNearestTile(unit.previousPosition.x, unit.previousPosition.z).unit = null;
        unit.previousPosition = new Vector3(x, 0, y);
        tile.unit = unit;

        return true;
    }

    public void LoadPlayerUnits()
    {
        foreach (KeyValuePair<Vector2Int, Unit> unitLocation in playerUnitsStartState)
            SpawnUnit(unitLocation.Key);
    }

    public void LoadEnemyUnits(Dictionary<Vector2Int, Unit> enemyUnitsStartState)
    {
        this.enemyUnitsStartState.Clear();

        foreach (KeyValuePair<Vector2Int, Unit> unitLocation in enemyUnitsStartState)
            this.enemyUnitsStartState.Add(unitLocation.Key, SpawnUnit(unitLocation.Key));
    }

    public void ClearBoardUnits()
    {
        if (ActiveUnits != null)
            ActiveUnits.Clear();

        for (int i = unitContainer.childCount - 1; i >= 0; i--)
            DestroyImmediate(unitContainer.GetChild(i).gameObject);
    }

    public void StartRound()
    {
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