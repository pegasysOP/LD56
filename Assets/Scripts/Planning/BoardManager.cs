using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public Dictionary<Vector2Int, Unit> ActiveUnits = new Dictionary<Vector2Int, Unit>();

    private Tile[,] board;

    private readonly int height = 8;
    private readonly int width = 8;

    private bool isAttached = false;
    private Unit selectedUnit = null;
    //private Vector3 offset;

    private Dictionary<Vector2Int, UnitType> playerUnitsStartState = new Dictionary<Vector2Int, UnitType>();
    private Dictionary<Vector2Int, UnitType> enemyUnitsStartState = new Dictionary<Vector2Int, UnitType>();

    public static BoardManager Instance;

    public EventHandler<bool> GameOver;

    private bool selectionEnabled = true;

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
                tile.IsPlayerSpace = (x < 3);
                tile.name = "Tile [" + x + "," + y + "]";
            }
        }
    }

    public Unit SpawnUnit(Vector2Int location, UnitType unitType, bool player)
    {
        Unit unit = Instantiate(unitPrefab, unitContainer);
        unit.name = "Unit" + location.x + ", " + location.y;

        unit.transform.position = new Vector3(location.x + 0.5f, 0f, location.y + 0.5f);
        //PlaceUnit(unit, location.x + 0.5f, location.y + 0.5f); // Sam - was setting the tiles unit to null for some reason
        board[location.x, location.y].currentUnit = unit;

        return unit;
    }

    void DragUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (selectionEnabled && Input.GetMouseButtonDown(0))
        {
            //Check that we are clicking on a unit and not just the board
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, unitMask))
            {
                //Get the tile this unit was attached to and remove it from that tile.
                Tile tile = GetNearestTile(hit.transform.position.x, hit.transform.position.z);
                if (tile.IsPlayerSpace) // only move if the space is in the player selection
                {
                    tile.currentUnit = null; // This tile no longer holds the unit

                    isAttached = true;
                    selectedUnit = hit.transform.GetComponent<Unit>();
                    selectedUnit.previousPosition = selectedUnit.transform.position;

                    //offset = selectedUnit.transform.position - hit.point;

                    ShowTileIndicators(true);
                }
            }
        }

        if (isAttached && selectedUnit != null)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, boardMask | outOfBoundsMask))
            {
                //Update the unit position as we drag, with offset correction
                Vector3 boardPosition = hit.point;

                selectedUnit.transform.position = new Vector3(
                   boardPosition.x /*+ offset.x*/,
                   selectedUnit.transform.position.y,
                   boardPosition.z /*+ offset.z*/
                );
            }
        }

        if (Input.GetMouseButtonUp(0) && isAttached)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, boardMask))
            {
                Tile newTile = GetNearestTile(hit.point.x, hit.point.z);

                if (newTile != null && newTile.IsPlayerSpace)
                {
                    if (newTile.currentUnit == null) // just place
                    {
                        if (PlaceUnit(selectedUnit, newTile.transform.position.x, newTile.transform.position.z))
                        {
                            Debug.Log("Successfully placed unit.");
                            newTile.currentUnit = selectedUnit;
                        }
                        else
                        {
                            Debug.Log("Could not place unit on tile. Reverting to previous position.");
                            PlaceUnit(selectedUnit, selectedUnit.previousPosition.x, selectedUnit.previousPosition.z);
                        }
                    }
                    else // otherwise swap
                    {
                        Unit otherUnit = newTile.currentUnit;
                        Tile originalTile = GetNearestTile(selectedUnit.previousPosition.x, selectedUnit.previousPosition.z);

                        //newTile.currentUnit = null;
                        //originalTile.currentUnit = null;

                        //if (PlaceUnit(selectedUnit, newTile.transform.position.x, newTile.transform.position.z))
                        //    Debug.LogError("PLACED UNIT 1");
                        //if (PlaceUnit(otherUnit, originalTile.transform.position.x, originalTile.transform.position.z))
                        //    Debug.LogError("PLACED UNIT 2");

                        selectedUnit.transform.position = newTile.transform.position;
                        selectedUnit.previousPosition = newTile.transform.position;

                        otherUnit.transform.position = originalTile.transform.position;
                        otherUnit.previousPosition = originalTile.transform.position;

                        newTile.currentUnit = selectedUnit;
                        originalTile.currentUnit = otherUnit;
                    }
                }
                else
                {
                    Debug.Log("Tile is already occupied. Reverting to previous position.");
                    PlaceUnit(selectedUnit, selectedUnit.previousPosition.x, selectedUnit.previousPosition.z);
                }
            }
            else
            {
                PlaceUnit(selectedUnit, selectedUnit.previousPosition.x, selectedUnit.previousPosition.z);
            }

            isAttached = false;
            selectedUnit = null;

            ShowTileIndicators(false);
        }
    }

    public void SavePlayerUnitStartPositions()
    {
        playerUnitsStartState.Clear();

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(board[x, y].currentUnit != null)
                {
                    playerUnitsStartState[new Vector2Int(x, y)] = board[x, y].currentUnit.unitType;
                    ActiveUnits[new Vector2Int(x, y)] = board[x, y].currentUnit; // TODO: Possibly need to remove this once proper player unit flow is established
                }
            }
        }
    }

    public Unit GetUnitAtTile(int x, int y)
    {
        Tile t = GetNearestTile(x, y);
        if (t.currentUnit != null)
            return t.currentUnit;// board[x, y].unit;

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

        Tile tile = GetNearestTile(x, y);
        if (tile == null)
            return false;

        // Check if the target tile already has a unit
        if (tile.currentUnit != null)
        {
            Debug.Log("There is already a unit here");
            return false;
        }

        unit.transform.position = new Vector3(x, 0f, y);
        GetNearestTile(unit.previousPosition.x, unit.previousPosition.z).currentUnit = null;
        unit.previousPosition = new Vector3(x, 0f, y);
        tile.currentUnit = unit;

        return true;
    }

    public void ClearBoardUnits()
    {
        ActiveUnits.Clear();

        for (int i = unitContainer.childCount - 1; i >= 0; i--)
            DestroyImmediate(unitContainer.GetChild(i).gameObject);
    }

    public void LoadPlayerUnits()
    {
        foreach (KeyValuePair<Vector2Int, UnitType> unitLocation in playerUnitsStartState)
            ActiveUnits[unitLocation.Key] = SpawnUnit(unitLocation.Key, unitLocation.Value, true);

        selectionEnabled = true;
    }

    public void LoadEnemyUnits(Dictionary<Vector2Int, UnitType> enemyUnitsStartState)
    {
        this.enemyUnitsStartState.Clear();

        foreach (KeyValuePair<Vector2Int, UnitType> unitLocation in enemyUnitsStartState)
        {
            this.enemyUnitsStartState[unitLocation.Key] = unitLocation.Value;
            ActiveUnits[unitLocation.Key] = SpawnUnit(unitLocation.Key, unitLocation.Value, false);
        }
    }    

    public void StartRound()
    {
        if (playerUnitsStartState == null || playerUnitsStartState.Count < 1)
            return;

        if (enemyUnitsStartState == null || enemyUnitsStartState.Count < 1)
            return;

        selectionEnabled = false;

        simulation.GameOver += OnGameOver;
        simulation.StartSimulation(playerUnitsStartState, enemyUnitsStartState);
    }

    private void OnGameOver(object sender, bool won)
    {
        simulation.GameOver -= OnGameOver;

        GameOver?.Invoke(sender, won);
    }

    private void ShowTileIndicators(bool enabled)
    {
        foreach (Tile tile in board)
            tile.ShowIndicator(enabled);
    }
}