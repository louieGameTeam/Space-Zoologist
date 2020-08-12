using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Consider adding support for a different grid type that keeps track of certain object locations
*/
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
    [SerializeField] private UnityEngine.Tilemaps.Tilemap TerrainTilemap = default;
    [SerializeField] private bool ShadePerimeter = false;

    private HashSet<Vector3Int> PopulationHomeLocations = default;

    private void Awake()
    {
        if (this.ShadePerimeter) this.ShadeOutsidePerimeter();
        this.PopulationHomeLocations = new HashSet<Vector3Int>();
        for (int x=0; x<GridWidth; x++)
        {
            this.PopulationHomeLocations.Add(new Vector3Int(x, 0, 0));
            this.PopulationHomeLocations.Add(new Vector3Int(x, GridHeight - 1, 0));
        }
        for (int y=0; y<GridHeight; y++)
        {
            this.PopulationHomeLocations.Add(new Vector3Int(0, y, 0));
            this.PopulationHomeLocations.Add(new Vector3Int(GridWidth - 1, y, 0));
        }
    }

    public bool CheckPopulationHomeLocations(Vector3 mousePosition)
    {
        return this.PopulationHomeLocations.Contains(this.Grid.WorldToCell(mousePosition));
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
        this.SetupBoundaires(tileGrid);
        this.SetupPopulationHomeLocation(this.PopulationHomeLocations, this.Grid.WorldToCell(population.gameObject.transform.position).x, this.Grid.WorldToCell(population.gameObject.transform.position).y);
        return new AnimalPathfinding.Grid(tileGrid, this.Grid);
    }

    private void SetupBoundaires(bool[,] tileGrid)
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

    private void SetupPopulationHomeLocation(HashSet<Vector3Int> populationHomeLocations, int x, int y)
    {
        for (int i=-1; i<=1; i++)
        {
            for (int j=-1; j<=1; j++)
            {
                Vector3Int loc = new Vector3Int(x + i, y + j, 0);
                //this.TerrainTilemap.SetColor(loc, Color.green);
                if (!populationHomeLocations.Contains(loc))
                {
                    populationHomeLocations.Add(loc);
                }
            }
        }
    }

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
        this.TerrainTilemap.SetColor(cellToShade, color);
    }
}
