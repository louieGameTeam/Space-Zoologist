using System.Collections;
using System.Collections.Generic;

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

    public float[] terrainComposition;
    public List<Animal> animals;
    public List<Population> populations;
    public List<FoodSource> foodSources;
    public byte id;
    public Dictionary<byte, float> previousArea = new Dictionary<byte, float>();
    public bool isEnclosed;

    /// <summary>
    /// This represents the all the (x,y) coordinates inside this enclosed area.
    /// Mainly use for accessing the CellGrid to pull info.
    /// </summary>
    /// <remarks>Using hash set for O(1)look up</remarks>
    public HashSet<Coordinate> coordinates;

    public EnclosedArea(byte id)
    {
        this.terrainComposition = new float[(int)TileType.TypesOfTiles];
        this.animals = new List<Animal>();
        this.coordinates = new HashSet<Coordinate>();
        this.populations = new List<Population>();
        this.foodSources = new List<FoodSource>();
        this.previousArea = new Dictionary<byte, float>();
        this.isEnclosed = true;
        this.id = id;
    }

    public bool IsInEnclosedArea(Coordinate coordinate)
    {
        return this.coordinates.Contains(coordinate);
    }

    public void AddCoordinate(Coordinate coordinate, int tileType, EnclosedArea prevArea = null)
    {
        if (GameManager.Instance.m_tileDataController.IsCellinGrid(coordinate.x, coordinate.y))
        {
            TileData tileData = GameManager.Instance.m_tileDataController.GetTileData(new UnityEngine.Vector3Int(coordinate.x, coordinate.y, 0));

            this.coordinates.Add(coordinate);

            if (tileData.Animal)
            {
                this.animals.Add(tileData.Animal.GetComponent<Animal>());

                Population population = tileData.Animal.GetComponent<Animal>().PopulationInfo;

                if (!this.populations.Contains(population))
                {
                    this.populations.Add(population);
                }
            }
            if (tileData.Food)
            {
                this.foodSources.Add(tileData.Food.GetComponent<FoodSource>());
            }

            // TODO: If an enclosure is not contained entirely within walls (IE if any tile touches an empty space), then set this.isEnclosed to false
            // NOTE: This code works, but will fail if the level is not entirely surrounded by walls and is not square-shaped
            if (tileType != (int)TileType.Wall && GameManager.Instance.m_gridSystem.IsCellOnGridEdge (coordinate.x, coordinate.y)) 
            {
                isEnclosed = false;
            }
        }

        this.terrainComposition[tileType]++;

        if (prevArea != null)
        {
            if (previousArea.ContainsKey(prevArea.id))
            {
                previousArea[prevArea.id]++;
            }
            else
            {
                previousArea.Add(prevArea.id, 1);
            }
        }
    }

    // Update the population/food list for this enclosed area
    public void Update()
    {
        this.populations = new List<Population>();
        this.foodSources = new List<FoodSource>();

        foreach (Coordinate coordinate in this.coordinates)
        {
            UnityEngine.Vector3Int coordinateVector = new UnityEngine.Vector3Int(coordinate.x, coordinate.y, 0);
            TileData coordinateTileData = GameManager.Instance.m_tileDataController.GetTileData(coordinateVector);

            if (coordinateTileData.Animal)
            {
                this.populations.Add(coordinateTileData.Animal.GetComponent<Animal>().PopulationInfo);
                continue;
            }
            if (coordinateTileData.Food)
            {
                this.foodSources.Add(coordinateTileData.Food.GetComponent<FoodSource>());
                continue;
            }
        }
    }
}
