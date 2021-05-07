using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Setup function to give back home locations of given population
/// <summary>
/// Translates the tilemap into a 2d array for keeping track of object locations.
/// </summary>
/// PlaceableArea transparency can be increased or decreased when adding it
public class GridSystem : MonoBehaviour
{
    public int GridWidth => LevelDataReference.LevelData.MapWidth;
    public int GridHeight => LevelDataReference.LevelData.MapHeight;
    [HideInInspector]
    public PlacementValidation PlacementValidation = default;
    [SerializeField] public Grid Grid = default;
    [SerializeField] public ReferenceData ReferenceData = default;
    [SerializeField] private LevelDataReference LevelDataReference = default;
    [SerializeField] private ReservePartitionManager RPM = default;
    [SerializeField] private TileSystem TileSystem = default;
    [SerializeField] private PopulationManager PopulationManager = default;
    [SerializeField] private Tilemap TilePlacementValidation = default;
    [SerializeField] private GameTile Tile = default;
    // Food and home locations updated when added, animal locations updated when the store opens up.
    public CellData[,] CellGrid = default;
    public TileData TilemapData = default;
    private HashSet<Vector3Int> PopulationHomeLocations = new HashSet<Vector3Int>();

    private void Awake()
    {
        this.CellGrid = new CellData[this.GridWidth, this.GridHeight];
        for (int i=0; i<this.GridWidth; i++)
        {
            for (int j=0; j<this.GridHeight; j++)
            {
                this.CellGrid[i, j] = new CellData(false);
            }
        }
    }

    private void Start()
    {
        this.PlacementValidation = this.gameObject.GetComponent<PlacementValidation>();
        this.PlacementValidation.Initialize(this, this.TileSystem, this.LevelDataReference, this.ReferenceData);
    }

    public bool isCellinGrid(int x, int y)
    {
        if (x < 0 || x >= this.GridWidth || y < 0 || y >= this.GridHeight)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Called when the store is opened
    /// </summary>
    public void UpdateAnimalCellGrid()
    {
        // Reset previous locations
        for (int i=0; i<this.CellGrid.GetLength(0); i++)
        {
            for (int j=0; j<this.CellGrid.GetLength(1); j++)
            {
                this.CellGrid[i, j].ContainsAnimal = false;
                this.CellGrid[i, j].HomeLocation = false;
            }
        }
        // Could be broken up for better efficiency since iterating through population twice
        this.PopulationHomeLocations = this.RecalculateHomeLocation();
        // Update locations and grab reference to animal GameObject (for future use)
        foreach (Population population in this.PopulationManager.Populations)
        {
            foreach (GameObject animal in population.AnimalPopulation)
            {
                Vector3Int animalLocation = this.Grid.WorldToCell(animal.transform.position);
                this.CellGrid[animalLocation.x, animalLocation.y].ContainsAnimal = true;
                this.CellGrid[animalLocation.x, animalLocation.y].Animal = animal;
            }
        }
    }

    public Vector3Int[,] GetHomeLocations(Population population)
    {
        Vector3Int[,] homeLocations = new Vector3Int[3,3];
        Vector3Int origin = this.Grid.WorldToCell(population.transform.position);
        int x = 0;
        int y = 0;
        for (int i=-1; i<=1; i++)
        {
            for (int j=-1; j<=1; j++)
            {
                Vector3Int loc = new Vector3Int(origin.x + i, origin.y + j, 0);
                homeLocations[x, y] = loc;
                y++;
            }
            x++;
            y = 0;
        }
        return homeLocations;
    }

    public bool IsWithinGridBounds(Vector3 mousePosition)
    {
        return (mousePosition.x < GridWidth - 1 && mousePosition.y < GridHeight - 1 &&
        mousePosition.x > 0 && mousePosition.y > 0);
    }

    // Will need to make the grid the size of the max tilemap size
    public AnimalPathfinding.Grid GetGridWithAccess(Population population)
    {
        // Debug.Log("Setting up pathfinding grid");
        bool[,] tileGrid = new bool[GridWidth, GridHeight];
        for (int x=0; x<GridWidth; x++)
        {
            for (int y=0; y<GridHeight; y++)
            {
                if (RPM.CanAccess(population, new Vector3Int(x, y, 0)))
                {
                    tileGrid[x, y] = true;
                }
                else
                {
                    tileGrid[x, y] = false;
                }
            }
        }
        // Setup boundaries for movement
        this.SetupMovementBoundaires(tileGrid);
        return new AnimalPathfinding.Grid(tileGrid, this.Grid);
    }

    // iterate through CellData, if contains item of interest and locations accessible, calculate distance and keep track of closest item location
    public Vector3Int FindClosestItem(Population population, GameObject animal, ItemType item)
    {
        Vector3Int itemLocation = new Vector3Int(-1, -1, -1);
        float closestDistance = 10000f;
        float localDistance = 0f;
        for (int x=0; x<this.CellGrid.GetLength(0); x++)
        {
            for (int y=0; y<this.CellGrid.GetLength(1); y++)
            {
                if (this.CellGrid[x, y].ContainsFood && item.Equals(ItemType.Food) && population.Grid.IsAccessible(x, y))
                {
                    localDistance = this.CalculateDistance(animal.transform.position.x, animal.transform.position.y, x, y);
                    if (localDistance < closestDistance)
                    {
                        closestDistance = localDistance;
                        itemLocation = new Vector3Int(x, y, 0);
                    }
                }
                else if (this.CellGrid[x, y].ContainsMachine && item.Equals(ItemType.Machine) && population.Grid.IsAccessible(x, y))
                {
                    localDistance = this.CalculateDistance(animal.transform.position.x, animal.transform.position.y, x, y);
                    if (localDistance < closestDistance)
                    {
                        closestDistance = localDistance;
                        itemLocation = new Vector3Int(x, y, 0);
                    }
                }
                else if (item.Equals(ItemType.Terrain))
                {
                    // if contains liquid tile, check neighbors accessibility
                    GameTile tile = this.TileSystem.GetGameTileAt(new Vector3Int(x, y, 0));
                    if (tile != null && tile.type == TileType.Liquid)
                    {
                        if (population.Grid.IsAccessible(x + 1, y))
                        {
                            localDistance = this.CalculateDistance(animal.transform.position.x, animal.transform.position.y, x, y);
                            if (localDistance < closestDistance)
                            {
                                closestDistance = localDistance;
                                itemLocation = new Vector3Int(x + 1, y, 0);
                            }
                        }
                        if (population.Grid.IsAccessible(x - 1, y))
                        {
                            localDistance = this.CalculateDistance(animal.transform.position.x, animal.transform.position.y, x, y);
                            if (localDistance < closestDistance)
                            {
                                closestDistance = localDistance;
                                itemLocation = new Vector3Int(x - 1, y, 0);
                            }
                        }
                        if (population.Grid.IsAccessible(x, y + 1))
                        {
                            localDistance = this.CalculateDistance(animal.transform.position.x, animal.transform.position.y, x, y);
                            if (localDistance < closestDistance)
                            {
                                closestDistance = localDistance;
                                itemLocation = new Vector3Int(x, y + 1, 0);
                            }
                        }
                        if (population.Grid.IsAccessible(x, y - 1))
                        {
                            localDistance = this.CalculateDistance(animal.transform.position.x, animal.transform.position.y, x, y);
                            if (localDistance < closestDistance)
                            {
                                closestDistance = localDistance;
                                itemLocation = new Vector3Int(x, y - 1, 0);
                            }
                        }
                    }
                }
            }
        }
        return itemLocation;
    }

    private float CalculateDistance(float x1, float y1, float x2, float y2)
    {
        return Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2));
    }

    private void SetupMovementBoundaires(bool[,] tileGrid)
    {
        for (int x=0; x<GridWidth; x++)
        {
            tileGrid[x, 0] = false;
            tileGrid[x, GridHeight - 1] = false;
        }
        for (int y=0; y<GridHeight; y++)
        {
            tileGrid[0, y] = false;
            tileGrid[GridWidth - 1, y] = false;
        }
    }

    // Resets and recalculates everytime in case a population dies
    public void HighlightHomeLocations()
    {
        this.PopulationHomeLocations = this.RecalculateHomeLocation();
        foreach (Vector3Int location in this.PopulationHomeLocations)
        {
            this.TilePlacementValidation.SetTile(location, this.Tile);
        }
    }

    private HashSet<Vector3Int> RecalculateHomeLocation()
    {
        HashSet<Vector3Int> homeLocations = new HashSet<Vector3Int>();
        foreach (Population population in this.PopulationManager.Populations)
        {
            Vector3Int origin = this.Grid.WorldToCell(population.transform.position);
            for (int i=-1; i<=1; i++)
            {
                for (int j=-1; j<=1; j++)
                {
                    Vector3Int loc = new Vector3Int(origin.x + i, origin.y + j, 0);
                    if (!homeLocations.Contains(loc))
                    {
                        homeLocations.Add(loc);
                        this.CellGrid[origin.x + i, origin.y + j].HomeLocation = true;
                    }
                }
            }
        }
        return homeLocations;
    }

    public void UnhighlightHomeLocations()
    {
        foreach (Vector3Int location in this.PopulationHomeLocations)
        {
            this.TilePlacementValidation.SetTile(location, null);
        }
    }

    public void AddFood(Vector3Int gridPosition, int size, GameObject foodSource) {
        int radius = size / 2;
        Vector3Int pos;
        if (size % 2 == 1)
        {
            // Check if the whole object is in bounds
            for (int x = -1 * radius; x <= radius; x++)
            {
                for (int y = -1 * radius; y <= radius; y++)
                {
                    pos = gridPosition;
                    pos.x += x;
                    pos.y += y;
                    CellGrid[pos.x, pos.y].ContainsFood = true;
                    CellGrid[pos.x, pos.y].Food = foodSource;
                }
            }
        }
        else {
            // Check if the whole object is in bounds
            for (int x = -1 * radius + 1; x <= radius; x++)
            {
                for (int y = -1 * radius + 1; y <= radius; y++)
                {
                    pos = gridPosition;
                    pos.x += x;
                    pos.y += y;
                    CellGrid[pos.x, pos.y].ContainsFood = true;
                    CellGrid[pos.x, pos.y].Food = foodSource;
                }
            }
        }
    }

    // Remove the food source on this tile, a little inefficient
    public void RemoveFood(Vector3Int gridPosition)
    {
        Vector3Int pos = gridPosition;
        if (!CellGrid[pos.x, pos.y].ContainsFood) return;

        GameObject foodSource = CellGrid[pos.x, pos.y].Food;
        int size = foodSource.GetComponent<FoodSource>().Species.Size;

        for (int dx = -size; dx < size; dx++) {
            for (int dy = -size; dy < size; dy++) {
                CellData cell = CellGrid[pos.x + dx, pos.y + dy];
                if (cell.ContainsFood && ReferenceEquals(cell.Food, foodSource))
                {
                    CellGrid[pos.x + dx, pos.y + dy].ContainsFood = false;
                    CellGrid[pos.x + dx, pos.y + dy].Food = null;
                }
            }
        }
    }



    /// <summary>
    /// Not guaranteed to work, use RemoveFood(Vector3Int gridPosition) instead!
    /// Need to be very sure of what you're doing. This gridPosition must match up EXACTLY with the one called in AddFood() for this to work.
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <param name="size"></param>
    public void RemoveFood(Vector3Int gridPosition, int size)
    {
        int radius = size / 2;
        Vector3Int pos;
        if (size % 2 == 1)
        {
            // Check if the whole object is in bounds
            for (int x = -1 * radius; x <= radius; x++)
            {
                for (int y = -1 * radius; y <= radius; y++)
                {
                    pos = gridPosition;
                    pos.x += x;
                    pos.y += y;
                    CellGrid[pos.x, pos.y].ContainsFood = false;
                    CellGrid[pos.x, pos.y].Food = null;
                }
            }

        }
        else
        {
            // Check if the whole object is in bounds
            for (int x = -1 * (radius - 1); x <= radius; x++)
            {
                for (int y = -1 * (radius - 1); y <= radius; y++)
                {
                    pos = gridPosition;
                    pos.x += x;
                    pos.y += y;
                    CellGrid[pos.x, pos.y].ContainsFood = false;
                    CellGrid[pos.x, pos.y].Food = null;
                }
            }
        }
    }


    // Showing how tiles can be shaded
    // We'll likely need a better version of this in the future for determing if we're setting up levels correctly
    private void ShadeOutsidePerimeter()
    {
        for (int x=-1; x<this.GridWidth + 1; x++)
        {
            this.ShadeSquare(x, -1, Color.red);
            this.ShadeSquare(x, this.GridHeight, Color.red);
        }
        for (int y=-1; y<this.GridHeight + 1; y++)
        {
            this.ShadeSquare(-1, y, Color.red);
            this.ShadeSquare(this.GridWidth, y, Color.red);
        }
    }

    public void ShadeSquare(int x, int y, Color color)
    {
        Vector3Int cellToShade = new Vector3Int(x, y, 0);
    }

    public struct CellData
    {
        public CellData(bool start)
        {
            this.ContainsFood = false;
            this.ContainsAnimal = false;
            this.Food = null;
            this.Animal = null;
            this.Machine = null;
            this.ContainsMachine = false;
            this.HomeLocation = false;
            this.OutOfBounds = false;
        }

        public bool ContainsMachine { get; set; }
        public GameObject Machine { get; set; }
        public bool ContainsFood { get; set; }
        public GameObject Food { get; set; }
        public bool ContainsAnimal { get; set; }
        public GameObject Animal { get; set; }
        public bool HomeLocation { get; set; }
        public bool OutOfBounds { get; set; }
    }

    public struct TileData
    {
        public TileData(int n)
        {
            this.NumDirtTiles = 0;
            this.NumGrassTiles = 0;
            this.NumSandTiles = 0;
        }

        public int NumGrassTiles { get; set; }
        public int NumDirtTiles { get; set; }
        public int NumSandTiles { get; set; }
    }
}
