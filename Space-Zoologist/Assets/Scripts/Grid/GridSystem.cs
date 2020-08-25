using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Translates the tilemap into a 2d array for keeping track of object locations.
/// </summary>
public class GridSystem : MonoBehaviour
{
    public int GridWidth => LevelDataReference.LevelData.MapWidth;
    public int GridHeight => LevelDataReference.LevelData.MapHeight;
    [SerializeField] public Grid Grid = default;
    [SerializeField] private LevelDataReference LevelDataReference = default;
    [SerializeField] private ReservePartitionManager RPM = default;
    [SerializeField] private PopulationManager PopulationManager = default;
    [SerializeField] private List<Tilemap> AllTerrainTilemaps = default;
    // Food and home locations updated when added, animal locations updated when the store opens up.
    public CellData[,] CellGrid = default;
    public TileData TilemapData = default;

    private HashSet<Vector3Int> PopulationHomeLocations = default;

    private void Awake()
    {
        this.PopulationHomeLocations = new HashSet<Vector3Int>();
        this.CellGrid = new CellData[this.GridWidth, this.GridHeight];
        for (int i=0; i<this.GridWidth; i++)
        {
            for (int j=0; j<this.GridHeight; j++)
            {
                this.CellGrid[i, j] = new CellData(false);
            }
        }
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
            }
        }
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

    public bool IsWithinGridBouds(Vector3 mousePosition)
    {
        return (mousePosition.x < GridWidth - 1 && mousePosition.y < GridHeight - 1 &&
        mousePosition.x > 0 && mousePosition.y > 0);
    }

    // Will need to make the grid the size of the max tilemap size
    public AnimalPathfinding.Grid GetGridWithAccess(Population population)
    {
        Debug.Log("Setting up pathfinding grid");
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

    public void SetupPopulationHomeLocation(Vector3 populationLocation)
    {
        Vector3Int origin = this.Grid.WorldToCell(populationLocation);
        for (int i=-1; i<=1; i++)
        {
            for (int j=-1; j<=1; j++)
            {
                Vector3Int loc = new Vector3Int(origin.x + i, origin.y + j, 0);
                if (!this.PopulationHomeLocations.Contains(loc))
                {
                    this.PopulationHomeLocations.Add(loc);
                }
                this.CellGrid[loc.x, loc.y].HomeLocation = true;
            }
        }
    }

    // Showing how tiles can be counted
    private void CountAllTiles()
    {
        foreach(Tilemap tilemap in this.AllTerrainTilemaps)
        {
            Debug.Log("Tilemap name: " + tilemap.name);
            BoundsInt bounds = tilemap.cellBounds;
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                TerrainTile tile = tilemap.GetTile<TerrainTile>(pos);
                if (tile != null && pos.x < GridWidth - 1 && pos.y < GridHeight - 1 && pos.x > 0 && pos.y > 0)
                {
                    switch(tile.name)
                    {
                        case "Grass":
                            this.TilemapData.NumGrassTiles++;
                            break;
                        case "Dirt":
                            this.TilemapData.NumDirtTiles++;
                            break;
                        case "Sand":
                            this.TilemapData.NumSandTiles++;
                            break;
                        default:
                            break;
                    }
                }
            }
            Debug.Log("Num Grass Tiles: " + this.TilemapData.NumGrassTiles);
            Debug.Log("Num Dirt Tiles: " + this.TilemapData.NumDirtTiles);
            Debug.Log("Num Sand Tiles: " + this.TilemapData.NumSandTiles);
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
            this.ContainsItem = false;
            this.ContainsAnimal = false;
            this.Item = null;
            this.Animal = null;
            this.HomeLocation = false;
        }

        public bool ContainsItem { get; set; }
        public GameObject Item { get; set; }
        public bool ContainsAnimal { get; set; }
        public GameObject Animal { get; set; }
        public bool HomeLocation { get; set; }
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
