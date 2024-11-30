using System;
using System.Collections.Generic;
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
    public Dictionary<Vector2Int, Unit> ActiveUnits = new Dictionary<Vector2Int, Unit>();

    // == Private Variables ==
    private Tile[,] board;
    private AudioManager audioManager;
    private readonly int height = 8;
    private readonly int width = 8;

    private bool isAttached = false;
    private Unit selectedUnit = null;

    private Dictionary<Vector2Int, UnitType> playerUnitsStartState = new Dictionary<Vector2Int, UnitType>();
    private Dictionary<Vector2Int, UnitType> enemyUnitsStartState = new Dictionary<Vector2Int, UnitType>();

    public static BoardManager Instance;
    public EventHandler<bool> GameOver;
    private bool selectionEnabled = true;

    // == MonoBehaviour Methods ==
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
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        HandleUnitDragging();
    }

    // == Board Setup Methods ==

    /// <summary>
    /// Generates the game board by instantiating tile prefabs.
    /// </summary>
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

    // == Unit Management Methods ==

    /// <summary>
    /// Spawns a new player unit in the next available slot on the board.
    /// </summary>
    public Unit SpawnNewPlayerUnit(UnitType unitType)
    {
        // spawn in next free slot
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y].currentUnit == null)
                    return SpawnUnit(new Vector2Int(x, y), unitType, true);
            }
        }

        Debug.LogError("No free slots found for spawning player unit");
        return null;
    }

    /// <summary>
    /// Spawns a new player unit at the specified position.
    /// </summary>
    public Unit SpawnNewPlayerUnit(UnitType unitType, Vector2Int position)
    {
        if (IsValidTile(position) && IsTileEmpty(position))
        {
            return SpawnUnit(position, unitType, true);
        }
        Debug.Log("Invalid position to place unit");
        return null;
    }

    /// <summary>
    /// Spawns a unit of the specified type at a given location.
    /// </summary>
    private Unit SpawnUnit(Vector2Int location, UnitType unitType, bool player)
    {
        Unit unit = Instantiate(unitPrefab, unitContainer);
        unit.name = "Unit" + location.x + ", " + location.y;
        unit.Init(unitType, player);

        unit.transform.position = new Vector3(location.x + 0.5f, 0f, location.y + 0.5f);
        board[location.x, location.y].currentUnit = unit;

        return unit;
    }

    /// <summary>
    /// Removes all units from the board.
    /// </summary>
    public void ClearBoardUnits()
    {
        ActiveUnits.Clear();

        for (int i = unitContainer.childCount - 1; i >= 0; i--)
            DestroyImmediate(unitContainer.GetChild(i).gameObject);
    }

    // == Drag and Drop Unit Management ==

    /// <summary>
    /// Handles unit dragging behavior, including placing and swapping units.
    /// </summary>
    void HandleUnitDragging()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (selectionEnabled && Input.GetMouseButtonDown(0))
        {
            //Check that we are clicking on a unit and not just the board
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, unitMask))
            {
                //Get the tile this unit was attached to and remove it from that tile.
                Tile tile = GetNearestTile(hit.transform.position.x, hit.transform.position.z);
                if (tile.IsPlayerSpace)
                {
                    tile.currentUnit = null;

                    isAttached = true;
                    selectedUnit = hit.transform.GetComponent<Unit>();
                    selectedUnit.previousPosition = selectedUnit.transform.position;

                    ShowTileIndicators(true);

                    audioManager.PlayPickUpClip();
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
                            audioManager.PlayPutDownClip();
                        }
                        else
                        {
                            Debug.Log("Could not place unit on tile. Reverting to previous position.");
                            PlaceUnit(selectedUnit, selectedUnit.previousPosition.x, selectedUnit.previousPosition.z);
                            audioManager.PlayInvalidPlacementClip();
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
                        audioManager.PlayPutDownClip();
                    }
                }
                else
                {
                    Debug.Log("Tile is already occupied. Reverting to previous position.");
                    PlaceUnit(selectedUnit, selectedUnit.previousPosition.x, selectedUnit.previousPosition.z);
                    audioManager.PlayInvalidPlacementClip();
                }
            }
            else
            {
                PlaceUnit(selectedUnit, selectedUnit.previousPosition.x, selectedUnit.previousPosition.z);
                audioManager.PlayInvalidPlacementClip();
            }

            isAttached = false;
            selectedUnit = null;

            ShowTileIndicators(false);
        }
    }

    // == Tile Management Methods ==

    /// <summary>
    /// Checks if the given tile position is valid within the player area.
    /// </summary>
    public bool IsValidTile(Vector2Int tilePosition)
    {
        return tilePosition.x >= 0 && tilePosition.x < 3 && tilePosition.y >= 0 && tilePosition.y < height;
    }

    /// <summary>
    /// Checks if the given tile position is empty (no unit present).
    /// </summary>
    public bool IsTileEmpty(Vector2Int tilePosition)
    {
        Tile tile = board[tilePosition.x, tilePosition.y];
        return tile.currentUnit == null;
    }

    /// <summary>
    /// Finds the nearest tile to the given world coordinates.
    /// </summary>
    public Tile GetNearestTile(float x, float y)
    {
        // out of the bounds of the board (should never happen as raycast would return null)
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;

        int boardX = Mathf.Clamp(Mathf.FloorToInt(x), 0, width - 1);
        int boardY = Mathf.Clamp(Mathf.FloorToInt(y), 0, height - 1);

        return board[boardX, boardY];
    }


    /// <summary>
    /// Gets the unit on the tile specified if it is present
    /// </summary>
    public Unit GetUnitAtTile(int x, int y)
    {
        Tile t = GetNearestTile(x, y);
        if (t.currentUnit != null)
            return t.currentUnit;// board[x, y].unit;

        return null;
    }

    /// <summary>
    /// Places a unit on a specific tile and updates its position.
    /// </summary>
    public bool PlaceUnit(Unit unit, float x, float y)
    {
        if (x < 0 || y < 0 || x > width || y >= height)
        {
            Debug.Log("Invalid coordinate");
            return false;
        }

        Tile tile = GetNearestTile(x, y);
        if (tile == null)
        {
            return false;
        }

        // Check if the target tile already has a unit
        if (tile.currentUnit != null)
        {
            //TODO: Do we want invalid clip here. If they should be swapped
            Debug.Log("There is already a unit here");
            return false;
        }

        unit.transform.position = new Vector3(x, 0f, y);
        GetNearestTile(unit.previousPosition.x, unit.previousPosition.z).currentUnit = null;
        unit.previousPosition = new Vector3(x, 0f, y);
        tile.currentUnit = unit;

        return true;
    }

    // == Gameplay Event Methods ==

    /// <summary>
    /// Saves the starting positions of all player units.
    /// </summary>
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

    /// <summary>
    /// Starts the round by initiating the simulation.
    /// </summary>
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

    /// <summary>
    /// Handles the end of the simulation
    /// </summary>
    private void OnGameOver(object sender, bool won)
    {
        simulation.GameOver -= OnGameOver;

        foreach (Unit unit in ActiveUnits.Values)
        {
            unit.DoJump(true);
        }

        GameOver?.Invoke(sender, won);
    }

    /// <summary>
    /// Spanws the Player Units in the saved positions from the start of the level
    /// </summary>
    public void LoadPlayerUnits()
    {
        foreach (KeyValuePair<Vector2Int, UnitType> unitLocation in playerUnitsStartState)
            ActiveUnits[unitLocation.Key] = SpawnUnit(unitLocation.Key, unitLocation.Value, true);

        selectionEnabled = true;
    }

    /// <summary>
    /// Spanws the Player Units in the positions for each level
    /// </summary>
    public void LoadEnemyUnits(Dictionary<Vector2Int, UnitType> enemyUnitsStartState)
    {
        this.enemyUnitsStartState.Clear();

        foreach (KeyValuePair<Vector2Int, UnitType> unitLocation in enemyUnitsStartState)
        {
            this.enemyUnitsStartState[unitLocation.Key] = unitLocation.Value;
            ActiveUnits[unitLocation.Key] = SpawnUnit(unitLocation.Key, unitLocation.Value, false);
        }
    }


    // == User Interface Methods ==

    /// <summary>
    /// Shows or hides tile indicators.
    /// </summary>
    private void ShowTileIndicators(bool enabled)
    {
        foreach (Tile tile in board)
            tile.ShowIndicator(enabled);
    }

    /// <summary>
    /// Enables or disables the selection of units
    /// </summary>
    public void SetSelectionEnabled(bool shouldEnable)
    {
        selectionEnabled = shouldEnable;
    }
}