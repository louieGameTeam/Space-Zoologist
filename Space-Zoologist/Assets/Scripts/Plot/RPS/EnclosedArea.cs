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
        if (gridSystem.IsCellinGrid(coordinate.x, coordinate.y))
        {
            GridSystem.TileData tileData = this.gridSystem.GetTileData(new UnityEngine.Vector3Int(coordinate.x, coordinate.y, 0));

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

        }

        this.terrainComposition[tileType]++;
    }

    // Update the population/food list for this enclosed area
    public void Update()
    {
        this.populations = new List<Population>();
        this.foodSources = new List<FoodSource>();

        foreach (Coordinate coordinate in this.coordinates)
        {
            UnityEngine.Vector3Int coordinateVector = new UnityEngine.Vector3Int(coordinate.x, coordinate.y, 0);

            if (this.gridSystem.GetTileData(coordinateVector).Animal)
            {
                this.populations.Add(this.gridSystem.GetTileData(coordinateVector).Animal.GetComponent<Animal>().PopulationInfo);
                continue;
            }
            if (this.gridSystem.GetTileData(coordinateVector).Food)
            {
                this.foodSources.Add(this.gridSystem.GetTileData(coordinateVector).Food.GetComponent<FoodSource>());
                continue;
            }
        }
    }
}
