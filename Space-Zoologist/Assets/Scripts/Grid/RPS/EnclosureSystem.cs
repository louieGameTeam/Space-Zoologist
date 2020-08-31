using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO refactor out temperature
public enum AtmosphereComponent { GasX, GasY, GasZ, Temperature };

/// <summary>
/// A class that represents the atmospheric composition of an area.
/// </summary>
[System.Serializable]
public class AtmosphericComposition
{
    [SerializeField] public float GasX = 0.0f;
    [SerializeField] public float GasY = 0.0f;
    [SerializeField] public float GasZ = 0.0f;
    [SerializeField] private float temperature = 0.0f;

    public float Temperature { get => temperature; }

    public AtmosphericComposition()
    {
        GasX = GasY = GasZ = temperature = 0;
    }

    public AtmosphericComposition(float _GasX, float _GasY, float _GasZ, float _temperature)
    {
        GasX = _GasX;
        GasY = _GasY;
        GasZ = _GasZ;
        temperature = _temperature;
    }

    public AtmosphericComposition(AtmosphericComposition from)
    {
        GasX = from.GasX; GasY = from.GasY; GasZ = from.GasZ; temperature = from.temperature;
    }

    public AtmosphericComposition Copy(AtmosphericComposition from)
    {
        GasX = from.GasX;
        GasY = from.GasY;
        GasZ = from.GasZ;
        temperature = from.temperature;
        return this;
    }

    public static AtmosphericComposition operator +(AtmosphericComposition lhs, AtmosphericComposition rhs)
    {
        return new AtmosphericComposition((lhs.GasX + rhs.GasX) / 2.0f, (lhs.GasY + rhs.GasY) / 2.0f,
            (lhs.GasZ + rhs.GasZ) / 2.0f, (lhs.temperature + rhs.temperature) / 2.0f);
    }

    public override string ToString()
    {
        return "GasX = " + GasX + " GasY = " + GasY + " GasZ = " + GasZ + " Temp = " + temperature;
    }

    /// <summary>
    /// Get the composition of the atmoshpere including temerature, in the order of AtmoshpereComponent enum
    /// </summary>
    /// <returns></returns>
    public float[] GetComposition()
    {
        float[] composition = { GasX, GasY, GasZ, temperature };
        return composition;
    }
}

/// <summary>
/// Represent all information about a enlcosed area and objects in it 
/// </summary>
/// <remarks>
/// Members of this struct could be expended for future needs
/// </remarks>
[System.Serializable]
public struct EnclosedArea
{
    // Data structure to hold a (x,y) coordinates
    public struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public AtmosphericComposition atmosphericComposition;
    public float[] terrainComposition;
    public List<Animal> animals;
    public List<Population> populations;
    public List<FoodSource> foodSources;
    public byte id;

    private readonly GridSystem gridSystem;

    /// <summary>
    /// This represents the all the (x,y) coordinates inside this enclosed area.
    /// Mainly use for accessing the CellGrid to pull info.
    /// </summary>
    /// <remarks>Using hash set for O(1)look up</remarks>
    public HashSet<Coordinate> coordinates;

    public EnclosedArea(AtmosphericComposition atmosphericComposition, GridSystem gridSystem, byte id)
    {
        this.atmosphericComposition = atmosphericComposition;
        this.terrainComposition = new float[(int)TileType.TypesOfTiles];
        this.gridSystem = gridSystem;
        this.animals = new List<Animal>();
        this.coordinates = new HashSet<Coordinate>();
        this.populations = new List<Population>();
        this.foodSources = new List<FoodSource>();
        this.id = id;
    }

    public void UpdateAtmosphericComposition(AtmosphericComposition atmosphericComposition)
    {
        this.atmosphericComposition = atmosphericComposition;
    }

    public bool IsInEnclosedArea(Coordinate coordinate)
    {
        return this.coordinates.Contains(coordinate);
    }

    public void AddCoordinate(Coordinate coordinate, int tileType, AtmosphericComposition oldComposition)
    {
        if (gridSystem.isCellinGrid(coordinate.x, coordinate.y))
        {
            GridSystem.CellData cellData = this.gridSystem.CellGrid[coordinate.x, coordinate.y];

            if (cellData.ContainsAnimal)
            {
                this.animals.Add(cellData.Animal.GetComponent<Animal>());

                Population population = cellData.Animal.GetComponent<Animal>().PopulationInfo;

                if (!this.populations.Contains(population))
                {
                    this.populations.Add(population);
                }
            }
            if (cellData.ContainsFood)
            {
                this.foodSources.Add(cellData.Food.GetComponent<FoodSource>());
            }

            if (cellData.ContainsMachine && cellData.Machine.GetComponent<AtmosphereMachine>() != null)
            {
                this.atmosphericComposition = oldComposition;
            }
        }

        this.terrainComposition[tileType]++;
    }

    // Update the population/food list for this 
    public void Update()
    {
        this.populations = new List<Population>();
        this.foodSources = new List<FoodSource>();

        foreach (Coordinate coordinate in this.coordinates)
        {
            if(this.gridSystem.CellGrid[coordinate.x, coordinate.y].ContainsAnimal)
            {
                this.populations.Add(this.gridSystem.CellGrid[coordinate.x, coordinate.y].Animal.GetComponent<Population>());
                continue;
            }
            if (this.gridSystem.CellGrid[coordinate.x, coordinate.y].ContainsFood)
            {
                this.foodSources.Add(this.gridSystem.CellGrid[coordinate.x, coordinate.y].Food.GetComponent<FoodSource>());
                continue;
            }
        }
    }
}

/// <summary>
/// This system finds and manages enclose areas
/// </summary>
public class EnclosureSystem : MonoBehaviour
{
    public Dictionary<Vector3Int, byte> positionToEnclosedArea { get; private set; }
    public List<AtmosphericComposition> Atmospheres { get; private set; }

    public List<EnclosedArea> enclosedAreas { get; private set; }

    [SerializeField] private LevelDataReference LevelDataReference = default;
    [SerializeField] private TileSystem TileSystem = default;
    [SerializeField] private NeedSystemManager needSystemManager = default;
    [SerializeField] private GridSystem gridSystem = default;

    // Have enclosed area been initialized?
    bool initialized = false;

    // The global atmosphere
    private AtmosphericComposition GlobalAtmosphere;

    /// <summary>
    /// Variable initialization on awake.
    /// </summary>
    private void Awake()
    {
        positionToEnclosedArea = new Dictionary<Vector3Int, byte>();
        Atmospheres = new List<AtmosphericComposition>();
        this.enclosedAreas = new List<EnclosedArea>();
    }

    private void Start()
    {
        GlobalAtmosphere = this.LevelDataReference.LevelData.GlobalAtmosphere;
        // TODO When this is called GridSystem might not be initlized,
        // ie, cannot read from CellData
        this.FindEnclosedAreas();
    }

    /// <summary>
    /// Gets the atmospheric composition at a given position.
    /// </summary>
    /// <param name="position">The position at which to get the atmopheric conditions</param>
    /// <returns></returns>
    public AtmosphericComposition GetAtmosphericComposition(Vector3 worldPosition)
    {
        Vector3Int position = this.TileSystem.WorldToCell(worldPosition);
        if (positionToEnclosedArea.ContainsKey(position))
        {
            return this.enclosedAreas[positionToEnclosedArea[position]].atmosphericComposition;
        }
        else
        {
            throw new System.Exception("Unable to find atmosphere at position (" + position.x + " , " + position.y + ")");
        }
    }

    public EnclosedArea GetEnclosedArea(Vector3Int worldPosition)
    {
        Vector3Int position = this.TileSystem.WorldToCell(worldPosition);

        return this.enclosedAreas[positionToEnclosedArea[position]];
    }

    public void UpdateAtmosphereComposition(Vector3 worldPosition, AtmosphericComposition atmosphericComposition)
    {
        Vector3Int position = this.TileSystem.WorldToCell(worldPosition);
        if (positionToEnclosedArea.ContainsKey(position))
        {
            this.enclosedAreas[positionToEnclosedArea[position]].UpdateAtmosphericComposition(atmosphericComposition);

            // Mark Atmosphere NS dirty
            this.needSystemManager.Systems[NeedType.Atmosphere].MarkAsDirty();
        }
        else
        {
            throw new System.Exception("Unable to find atmosphere at position (" + position.x + " , " + position.y + ")");
        }
    }

    /// <summary>
    /// Private function for determining if a position is within the area
    /// </summary>
    bool WithinRange(Vector3Int pos, int minx, int miny, int maxx, int maxy)
    {
        if (pos.x >= minx && pos.y >= miny && pos.x <= maxx && pos.y <= maxy)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Recursive flood fill. 
    /// </summary>
    /// <param name="cur">Start location</param>
    /// <param name="accessed">Accessed cells</param>
    /// <param name="unaccessible">Unaccessible cells</param>
    /// <param name="walls">wall cells</param>
    /// <param name="atmosphereCount">index of the enclosed area</param>
    private void FloodFill(Vector3Int cur, HashSet<Vector3Int> accessed, HashSet<Vector3Int> unaccessible, Stack<Vector3Int> walls, byte atmosphereCount, EnclosedArea enclosedArea, bool isUpdate)
    {
       
        if (accessed.Contains(cur) || unaccessible.Contains(cur))
        {
            // checked before, move on
            return;
        }

        // check if tilemap has tile
        TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(cur);
        if (tile != null)
        {
            if (tile.type != TileType.Wall)
            {
                // Mark the cell
                accessed.Add(cur);

                // Updating enclosed area
                if (isUpdate)
                {
                    enclosedArea.AddCoordinate(new EnclosedArea.Coordinate(cur.x, cur.y), (int)tile.type, this.enclosedAreas[this.positionToEnclosedArea[cur]].atmosphericComposition);
                }
                // Initial round
                else
                {
                    enclosedArea.AddCoordinate(new EnclosedArea.Coordinate(cur.x, cur.y), (int)tile.type, null);
                }

                this.positionToEnclosedArea[cur] = atmosphereCount;

                FloodFill(cur + Vector3Int.left, accessed, unaccessible, walls, atmosphereCount, enclosedArea, isUpdate);
                FloodFill(cur + Vector3Int.up, accessed, unaccessible, walls, atmosphereCount, enclosedArea, isUpdate);
                FloodFill(cur + Vector3Int.right, accessed, unaccessible, walls, atmosphereCount, enclosedArea, isUpdate);
                FloodFill(cur + Vector3Int.down, accessed, unaccessible, walls, atmosphereCount, enclosedArea, isUpdate);
            }
            else
            {
                walls.Push(cur);
                unaccessible.Add(cur);
            }
        }
        else
        {
            unaccessible.Add(cur);
        }
    }

    /// <summary>
    /// Call this to find all the enclosed areas and create a EnclosedArea data structure to hold its information.
    /// </summary>
    /// <remarks>
    /// This is using a flood fill (https://en.wikipedia.org/wiki/Flood_fill) to find enclosed areas.
    /// Assumptions: the reserve is bordered by walls
    /// </remarks>
    public void FindEnclosedAreas()
    {
        // This has to be inside the map
        Vector3Int startPos = this.TileSystem.WorldToCell(new Vector3(1,1,0));
        // tiles to-process
        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        // non-wall tiles
        HashSet<Vector3Int> accessed = new HashSet<Vector3Int>();
        // wall or null tiles
        HashSet<Vector3Int> unaccessible = new HashSet<Vector3Int>();
        // walls
        Stack<Vector3Int> walls = new Stack<Vector3Int>();

        List<EnclosedArea> newEnclosedAreas = new List<EnclosedArea>();

        // Initial flood fill
        byte atmosphereCount = 0;
        newEnclosedAreas.Add(new EnclosedArea(new AtmosphericComposition(this.GlobalAtmosphere), this.gridSystem, atmosphereCount));
        this.FloodFill(startPos, accessed, unaccessible, walls, atmosphereCount, newEnclosedAreas[atmosphereCount], false);


        while (walls.Count > 0)
        {
            atmosphereCount++;
            newEnclosedAreas.Add(new EnclosedArea(new AtmosphericComposition(this.GlobalAtmosphere), this.gridSystem, atmosphereCount));

            startPos = walls.Pop();

            this.FloodFill(startPos + Vector3Int.left, accessed, unaccessible, walls, atmosphereCount, newEnclosedAreas[atmosphereCount], false);
            this.FloodFill(startPos + Vector3Int.up, accessed, unaccessible, walls, atmosphereCount, newEnclosedAreas[atmosphereCount], false);
            this.FloodFill(startPos + Vector3Int.right, accessed, unaccessible, walls, atmosphereCount, newEnclosedAreas[atmosphereCount], false);
            this.FloodFill(startPos + Vector3Int.down, accessed, unaccessible, walls, atmosphereCount, newEnclosedAreas[atmosphereCount], false);
        }

        this.enclosedAreas = newEnclosedAreas;
    }

    public void UpdateEnclosedAreas()
    {
        Vector3Int startPos = this.TileSystem.WorldToCell(new Vector3(1, 1, 0));
        // tiles to-process
        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        // non-wall tiles
        HashSet<Vector3Int> accessed = new HashSet<Vector3Int>();
        // wall or null tiles
        HashSet<Vector3Int> unaccessible = new HashSet<Vector3Int>();
        // walls
        Stack<Vector3Int> walls = new Stack<Vector3Int>();

        List<EnclosedArea> newEnclosedAreas = new List<EnclosedArea>();

        // Initial flood fill
        byte atmosphereCount = 0;
        newEnclosedAreas.Add(new EnclosedArea(new AtmosphericComposition(this.GlobalAtmosphere), this.gridSystem, atmosphereCount));
        this.FloodFill(startPos, accessed, unaccessible, walls, atmosphereCount, newEnclosedAreas[atmosphereCount], true);


        while (walls.Count > 0)
        {
            atmosphereCount++;
            newEnclosedAreas.Add(new EnclosedArea(new AtmosphericComposition(this.GlobalAtmosphere), this.gridSystem, atmosphereCount));

            startPos = walls.Pop();

            this.FloodFill(startPos + Vector3Int.left, accessed, unaccessible, walls, atmosphereCount, newEnclosedAreas[atmosphereCount], true);
            this.FloodFill(startPos + Vector3Int.up, accessed, unaccessible, walls, atmosphereCount, newEnclosedAreas[atmosphereCount], true);
            this.FloodFill(startPos + Vector3Int.right, accessed, unaccessible, walls, atmosphereCount, newEnclosedAreas[atmosphereCount], true);
            this.FloodFill(startPos + Vector3Int.down, accessed, unaccessible, walls, atmosphereCount, newEnclosedAreas[atmosphereCount], true);
        }

        this.enclosedAreas = newEnclosedAreas;
    }

    /// <summary>
    /// Update the surrounding atmospheres of the position. Only used when adding or removing walls.
    /// </summary>
    /// <param name="positions">Positions where the walls are placed or removed.</param>
    //public void UpdateAtmosphere()
    //{
    //    Debug.Log("Atmosphere updated");
    //    // If not initialized or have more than , initialize instead
    //    if (!initialized || Atmospheres.Count >= 120)
    //    {
    //        FindEnclosedAreas();
    //        return;
    //    }

    //    // Step 1: Populate tiles outside with 0 and find walls

    //    // tiles to-process
    //    Stack<Vector3Int> stack = new Stack<Vector3Int>();

    //    // non-wall tiles
    //    HashSet<Vector3Int> accessed = new HashSet<Vector3Int>();

    //    // wall or null tiles
    //    HashSet<Vector3Int> unaccessible = new HashSet<Vector3Int>();

    //    // walls
    //    Stack<Vector3Int> walls = new Stack<Vector3Int>();

    //    // starting location, may be changed later for better performance
    //    Vector3Int cur = new Vector3Int(1, 1, 0);
    //    TerrainTile curTile = this.TileSystem.GetTerrainTileAtLocation(cur);
    //    if (curTile != null)
    //    {
    //        if (curTile.type == TileType.Wall)
    //        {
    //            walls.Push(cur);
    //        }
    //        else
    //        {
    //            stack.Push(cur);
    //        }
    //    }

    //    byte atmNum = (byte)Atmospheres.Count;
    //    List<byte> containedAtmosphere = new List<byte>();
    //    bool newAtmosphere = false;

    //    // iterate until no tile left in stack
    //    while (stack.Count > 0)
    //    {
    //        // next point
    //        cur = stack.Pop();

    //        if (accessed.Contains(cur) || unaccessible.Contains(cur))
    //        {
    //            // checked before, move on
    //            continue;
    //        }

    //        // check if tilemap has tile
    //        TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(cur);
    //        if (tile != null)
    //        {
    //            if (tile.type != TileType.Wall)
    //            {
    //                // save the Vector3Int since it is already checked
    //                accessed.Add(cur);

    //                // ignore global areas outside of range to reduce waste
    //                if (positionToEnclosedArea[cur] == 0)
    //                {
    //                    continue;
    //                }

    //                // save what used to be here
    //                if (!containedAtmosphere.Contains(positionToEnclosedArea[cur]))
    //                {
    //                    containedAtmosphere.Add(positionToEnclosedArea[cur]);
    //                }

    //                // save the atmosphere here as the current one
    //                positionToEnclosedArea[cur] = atmNum;

    //                // check all 4 tiles around, may be too expensive/awaiting optimization
    //                stack.Push(cur + Vector3Int.left);
    //                stack.Push(cur + Vector3Int.up);
    //                stack.Push(cur + Vector3Int.right);
    //                stack.Push(cur + Vector3Int.down);
    //            }
    //            else
    //            {
    //                positionToEnclosedArea[cur] = 255;
    //                unaccessible.Add(cur);

    //                walls.Push(cur);
    //            }
    //        }
    //        else
    //        {
    //            // save the Vector3Int since it is already checked
    //            unaccessible.Add(cur);
    //        }
    //    }

    //    if (newAtmosphere)
    //    {
    //        Atmospheres.Add(new AtmosphericComposition(Atmospheres[containedAtmosphere[0]]));
    //        for (int i = 1; i < containedAtmosphere.Count; i++)
    //        {
    //            Atmospheres[atmNum] += Atmospheres[containedAtmosphere[i]];
    //        }
    //    }

    //    // Step 2: Loop through walls and push every adjacent tile into the stack
    //    // and iterate through stack and assign atmosphere number
    //    atmNum = (byte)Atmospheres.Count;

    //    // iterate until no tile left in walls
    //    while (walls.Count > 0)
    //    {
    //        // next point
    //        cur = walls.Pop();

    //        // check all 4 tiles around, may be too expensive/awaiting optimization
    //        stack.Push(cur + Vector3Int.left);
    //        stack.Push(cur + Vector3Int.up);
    //        stack.Push(cur + Vector3Int.right);
    //        stack.Push(cur + Vector3Int.down);

    //        newAtmosphere = false;
    //        containedAtmosphere = new List<byte>();

    //        while (stack.Count > 0)
    //        {
    //            // next point
    //            cur = stack.Pop();

    //            if (accessed.Contains(cur) || unaccessible.Contains(cur))
    //            {
    //                // checked before, move on
    //                continue;
    //            }

    //            // check if tilemap has tile
    //            TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(cur);
    //            if (tile != null)
    //            {
    //                if (tile.type != TileType.Wall)
    //                {
    //                    // save the Vector3Int since it is already checked
    //                    accessed.Add(cur);

    //                    // ignore global areas outside of range to reduce waste
    //                    if (positionToEnclosedArea[cur] == 0)
    //                    {
    //                        continue;
    //                    }

    //                    // wasn't a wall and the atmosphere wasn't already included
    //                    if (positionToEnclosedArea[cur] != 255 && !containedAtmosphere.Contains(positionToEnclosedArea[cur]))
    //                    {
    //                        containedAtmosphere.Add(positionToEnclosedArea[cur]);
    //                    }

    //                    newAtmosphere = true;
    //                    positionToEnclosedArea[cur] = atmNum;

    //                    // check all 4 tiles around, may be too expensive/awaiting optimization
    //                    stack.Push(cur + Vector3Int.left);
    //                    stack.Push(cur + Vector3Int.up);
    //                    stack.Push(cur + Vector3Int.right);
    //                    stack.Push(cur + Vector3Int.down);
    //                }
    //                else
    //                {
    //                    // walls inside walls
    //                    positionToEnclosedArea[cur] = 255;
    //                    unaccessible.Add(cur);
    //                    walls.Push(cur);
    //                }
    //            }
    //            else
    //            {
    //                // save the Vector3Int since it is already checked
    //                unaccessible.Add(cur);
    //            }
    //        }

    //        // new atmosphere detected
    //        if (newAtmosphere)
    //        {
    //            atmNum++;
    //            AtmosphericComposition atmosphere;
    //            if (containedAtmosphere.Contains(0))
    //            {
    //                // if contains the global atmosphere, no other atmospheres matters
    //                atmosphere = Atmospheres[0];
    //            }
    //            else if (containedAtmosphere.Count > 0)
    //            {
    //                atmosphere = Atmospheres[containedAtmosphere[0]];
    //                for (int i = 1; i < containedAtmosphere.Count; i++)
    //                {
    //                    atmosphere += Atmospheres[containedAtmosphere[i]];
    //                }

    //            }
    //            else
    //            {
    //                // empty atmosphere if out of nowhere
    //                atmosphere = new AtmosphericComposition();
    //            }
    //            Atmospheres.Add(atmosphere);
    //        }
    //    }
    //}



    //public void FindEnclosedAreas()
    //{
    //    // temporary list of atmosphere
    //    List<AtmosphericComposition> newAtmospheres = new List<AtmosphericComposition>();

    //    if (!this.initialized)
    //    {
    //        newAtmospheres.Add(this.GlobalAtmosphere);

    //        // 
    //        this.enclosedAreas.Add(new EnclosedArea(this.GlobalAtmosphere, this.gridSystem));
    //    }
    //    else
    //    {
    //        newAtmospheres.Add(this.Atmospheres[positionToEnclosedArea[new Vector3Int(1, 1, 0)]]);
    //    }

    //    // Step 1: Populate tiles outside with 0 and find walls

    //    // tiles to-process
    //    Stack<Vector3Int> stack = new Stack<Vector3Int>();

    //    // non-wall tiles
    //    HashSet<Vector3Int> accessed = new HashSet<Vector3Int>();

    //    // wall or null tiles
    //    HashSet<Vector3Int> unaccessible = new HashSet<Vector3Int>();

    //    // walls
    //    Stack<Vector3Int> walls = new Stack<Vector3Int>();

    //    // outer most position
    //    Vector3Int cur = this.TileSystem.WorldToCell(new Vector3(1, 1, 0));
    //    stack.Push(cur);

    //    // iterate until no tile left in stack
    //    while (stack.Count > 0)
    //    {
    //        // next point
    //        cur = stack.Pop();

    //        if (accessed.Contains(cur) || unaccessible.Contains(cur))
    //        {
    //            // checked before, move on
    //            continue;
    //        }

    //        // check if tilemap has tile
    //        TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(cur);
    //        if (tile != null)
    //        {
    //            if (tile.type != TileType.Wall)
    //            {
    //                // save the Vector3Int since it is already checked
    //                accessed.Add(cur);

    //                positionToEnclosedArea[cur] = 0;

    //                // check all 4 tiles around, may be too expensive/awaiting optimization
    //                stack.Push(cur + Vector3Int.left);
    //                stack.Push(cur + Vector3Int.up);
    //                stack.Push(cur + Vector3Int.right);
    //                stack.Push(cur + Vector3Int.down);
    //            }
    //            else
    //            {
    //                walls.Push(cur);
    //                unaccessible.Add(cur);
    //                positionToEnclosedArea[cur] = 255;
    //            }
    //        }
    //        else
    //        {
    //            // save the Vector3Int since it is already checked
    //            unaccessible.Add(cur);
    //        }
    //    }


    //    // Step 2: Loop through walls and push every adjacent tile into the stack
    //    // and iterate through stack and assign atmosphere number
    //    byte atmNum = 1;
    //    // number of tiles in an atmosphere

    //    // iterate until no tile left in walls
    //    while (walls.Count > 0)
    //    {
    //        // next point
    //        cur = walls.Pop();

    //        // check all 4 tiles around, may be too expensive/awaiting optimization
    //        stack.Push(cur + Vector3Int.left);
    //        stack.Push(cur + Vector3Int.up);
    //        stack.Push(cur + Vector3Int.right);
    //        stack.Push(cur + Vector3Int.down);

    //        bool newAtmosphere = false;
    //        List<byte> containedAtmosphere = new List<byte>();

    //        while (stack.Count > 0)
    //        {
    //            // next point
    //            cur = stack.Pop();

    //            if (accessed.Contains(cur) || unaccessible.Contains(cur))
    //            {
    //                // checked before, move on
    //                continue;
    //            }

    //            // check if tilemap has tile
    //            TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(cur);
    //            if (tile != null)
    //            {
    //                if (tile.type != TileType.Wall)
    //                {
    //                    // save the Vector3Int since it is already checked
    //                    accessed.Add(cur);

    //                    if (positionToEnclosedArea.ContainsKey(cur) && positionToEnclosedArea[cur] != 255 && !containedAtmosphere.Contains(positionToEnclosedArea[cur]))
    //                    {
    //                        containedAtmosphere.Add(positionToEnclosedArea[cur]);
    //                    }

    //                    newAtmosphere = true;
    //                    positionToEnclosedArea[cur] = atmNum;

    //                    // check all 4 tiles around, may be too expensive/awaiting optimization
    //                    stack.Push(cur + Vector3Int.left);
    //                    stack.Push(cur + Vector3Int.up);
    //                    stack.Push(cur + Vector3Int.right);
    //                    stack.Push(cur + Vector3Int.down);
    //                }
    //                else
    //                {
    //                    // walls inside walls
    //                    walls.Push(cur);
    //                    unaccessible.Add(cur);
    //                    positionToEnclosedArea[cur] = 255;
    //                }
    //            }
    //            else
    //            {
    //                // save the Vector3Int since it is already checked
    //                unaccessible.Add(cur);
    //            }
    //        }

    //        // a new atmosphere was added
    //        if (newAtmosphere && !initialized)
    //        {
    //            atmNum++;
    //            newAtmospheres.Add(new AtmosphericComposition(Random.value, Random.value, Random.value, Random.value * 100));
    //        }
    //        else if (newAtmosphere && initialized)
    //        {
    //            atmNum++;
    //            AtmosphericComposition atmosphere;
    //            if (containedAtmosphere.Contains(0))
    //            {
    //                // if contains the global atmosphere, no other atmospheres matters
    //                atmosphere = Atmospheres[0];
    //            }
    //            else if (containedAtmosphere.Count > 0)
    //            {
    //                atmosphere = Atmospheres[containedAtmosphere[0]];
    //                for (int i = 1; i < containedAtmosphere.Count; i++)
    //                {
    //                    atmosphere += Atmospheres[containedAtmosphere[i]];
    //                }
    //            }
    //            else
    //            {
    //                // empty atmosphere if out of nowhere
    //                atmosphere = new AtmosphericComposition();
    //            }
    //            newAtmospheres.Add(atmosphere);
    //        }
    //    }

    //    Atmospheres = newAtmospheres;
    //    //print("Number of Atmospheres = " + Atmospheres.Count);
    //    //print("Detected Number of Atmospheres = " + atmNum);
    //    initialized = true;
    //}
}