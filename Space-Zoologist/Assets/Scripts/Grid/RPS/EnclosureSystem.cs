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

    public float[] ConvertAtmosphereComposition()
    {
        float[] composition = { GasX, GasY, GasZ };
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
public class EnclosedArea
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

            this.coordinates.Add(coordinate);

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
            if (cellData.ContainsMachine && cellData.Machine.GetComponent<AtmosphereMachine>() != null && oldComposition != null)
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
            if (this.gridSystem.CellGrid[coordinate.x, coordinate.y].ContainsAnimal)
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

    public List<EnclosedArea> EnclosedAreas;
    private List<EnclosedArea> internalEnclosedAreas;

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
        this.internalEnclosedAreas = new List<EnclosedArea>();
        this.EnclosedAreas = new List<EnclosedArea>();
        this.GlobalAtmosphere = this.LevelDataReference.LevelData.GlobalAtmosphere;
        // TODO Hard fix to reference issue
        this.TileSystem = FindObjectOfType<TileSystem>();
    }

    private void Start()
    {
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
            return this.internalEnclosedAreas[positionToEnclosedArea[position]].atmosphericComposition;
        }
        else
        {
            throw new System.Exception("Unable to find atmosphere at position (" + position.x + " , " + position.y + ")");
        }
    }

    public EnclosedArea GetEnclosedArea(Vector3Int worldPosition)
    {
        Vector3Int position = this.TileSystem.WorldToCell(worldPosition);

        return this.internalEnclosedAreas[positionToEnclosedArea[position]];
    }

    public void UpdateAtmosphereComposition(Vector3 worldPosition, AtmosphericComposition atmosphericComposition)
    {
        Vector3Int position = this.TileSystem.WorldToCell(worldPosition);
        if (positionToEnclosedArea.ContainsKey(position))
        {
            this.internalEnclosedAreas[positionToEnclosedArea[position]].UpdateAtmosphericComposition(atmosphericComposition);

            // Mark Atmosphere NS dirty
            this.needSystemManager.Systems[NeedType.Atmosphere].MarkAsDirty();
        }
        else
        {
            throw new System.Exception("Unable to find atmosphere at position (" + position.x + " , " + position.y + ")");
        }
    }

    /// <summary>
    /// This deletes enclosed areas that has nothing in it.
    /// To fix issues with creating enclosed area for areas outside of the border walls
    /// </summary>
    private void updatePublicEnlcosedAreas()
    {
        this.EnclosedAreas.Clear();

        foreach (EnclosedArea enclosedArea in this.internalEnclosedAreas)
        {
            if (enclosedArea.coordinates.Count != 0)
            {
                this.EnclosedAreas.Add(enclosedArea);
            }
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
                if (isUpdate && this.positionToEnclosedArea.ContainsKey(cur))
                {
                    enclosedArea.AddCoordinate(new EnclosedArea.Coordinate(cur.x, cur.y), (int)tile.type, this.internalEnclosedAreas[this.positionToEnclosedArea[cur]].atmosphericComposition);
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

        this.internalEnclosedAreas = newEnclosedAreas;
        this.updatePublicEnlcosedAreas();
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

        this.internalEnclosedAreas = newEnclosedAreas;
        this.updatePublicEnlcosedAreas();
    }
}