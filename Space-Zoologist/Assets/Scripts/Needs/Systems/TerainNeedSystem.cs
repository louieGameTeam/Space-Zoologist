using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Hadnles need value update for all  `Terrain` type needs
/// </summary>
public class TerrainNeedSystem : NeedSystem
{
    //The order of terrain from least number of population preferences to most.
    private readonly TileType[] terrainOrder = new TileType[] {TileType.Sand, TileType.Liquid, TileType.Dirt, TileType.Swamp, TileType.Stone, TileType.Grass};
    // For `Population` consumers
    private readonly ReservePartitionManager rpm = null;
    // For `FoodSource` consumers
    private GridSystem gridSystem = null;

    public TerrainNeedSystem(ReservePartitionManager rpm, GridSystem gridSystem, NeedType needType = NeedType.Terrain) : base(needType)
    {
        this.rpm = rpm;
        this.gridSystem = gridSystem;

        EventManager.Instance.SubscribeToEvent(EventType.TerrainChange, () =>
        {
            this.isDirty = true;
        });
    }

    // TODO: refactor update system to use this
    public void calculateConnectedTerrain()
    {
        foreach (Population population in Consumers)
        {
            CountConnectedTerrain(population);
        }
    }

    /// <summary>
    /// Count number of connected terrain of each type for terrain need
    /// </summary>
    /// <param name="population"></param>
    /// <returns></returns>
    public Dictionary<TileType, List<int>> CountConnectedTerrain(Population population)
    {
        List<Vector3Int> accessiblePositions = rpm.GetLocationsWithAccess(population);

        HashSet<Vector3Int> closed = new HashSet<Vector3Int>();

        Dictionary<TileType, List<int>> ConnectedTilesByType = new Dictionary<TileType, List<int>>();


        foreach (Vector3Int position in accessiblePositions)
        {
            if (closed.Contains(position)) continue;

            TileType type = gridSystem.GetGameTileAt(position).type;
            if (!ConnectedTilesByType.ContainsKey(type))
            {
                ConnectedTilesByType.Add(type, new List<int>());
            }
            ConnectedTilesByType[type].Add(DFS(population, position, ref closed, type));
        }

        // For debugging
        foreach (KeyValuePair<TileType, List<int>> pair in ConnectedTilesByType)
        {
            string output = population.name + " ";
            output += pair.Key + ": {";
            for (int i = 0; i < pair.Value.Count; i++)
            {
                output += pair.Value[i];
                if (i != pair.Value.Count - 1)
                    output += ", ";
            }
            output += "}";
            Debug.Log(output);
        }

        return ConnectedTilesByType;
    }

    static int[] rowNbr = { -1, 0, 0, 1 };
    static int[] colNbr = { 0, -1, 1, 0 };
    private int DFS(Population population, Vector3Int position, ref HashSet<Vector3Int> closed, TileType type)
    {

        int total = 1;

        closed.Add(position);

        for (int i = 0; i < rowNbr.Length; i++)
        {
            Vector3Int next = position;
            next += new Vector3Int(colNbr[i], rowNbr[i], 0);
            if (closed.Contains(next)) continue;
            if (rpm.CanAccess(population, position) && gridSystem.GetGameTileAt(next) != null && gridSystem.GetGameTileAt(next).type == type)
            {
                total += DFS(population, next, ref closed, type);
            }
        }
        return total;
    }

    /// <summary>
    /// Get the terrian conposition of the consumers and update the need values
    /// </summary>
    public override void UpdateSystem()
    {

        // Unmark dirty when there is not consumer then exit
        if (this.Consumers.Count == 0)
        {
            this.isDirty = false;
            return;
        }
        
        //All tiles in the grid that can be accessed by some population, organized by tiletype
        Dictionary<TileType, HashSet<Vector3Int>> accessibleTilesByTileType = new Dictionary<TileType, HashSet<Vector3Int>>();
        //All populations currently in the level, in order of dominance
        SortedSet<Population> populationsByDominance = new SortedSet<Population>(Consumers.OfType<Population>(), new DominanceComparer());
        //Number of tiles needed by a given population
        Dictionary<Population, int> tilesNeeded = new Dictionary<Population, int>();
        //Number of tiles allocated to a given population
        Dictionary<Population, int> tilesAllocated = new Dictionary<Population, int>();

        foreach (Population population in populationsByDominance)
        {
            tilesNeeded[population] = population.Count * population.species.TerrainTilesRequired;

            foreach(Vector3Int position in rpm.GetLocationsWithAccess(population))
            {
                TileType type = gridSystem.GetTileData(position).currentTile.type;

                if(!accessibleTilesByTileType.ContainsKey(type))
                    accessibleTilesByTileType.Add(type, new HashSet<Vector3Int>());

                accessibleTilesByTileType[type].Add(position);
            }
        }

        foreach (TileType tile in terrainOrder)
        {
            int tileContribution = tile == TileType.Grass ? 2 : 1;

            foreach(Population population in populationsByDominance)
            {
                if (population.Needs.ContainsKey(tile.ToString()) && tilesAllocated[population] < tilesNeeded[population])
                {
                    foreach(Vector3Int position in accessibleTilesByTileType[tile])
                    {
                        if(rpm.CanAccess(population, position))
                        {
                            accessibleTilesByTileType[tile].Remove(position);
                            tilesAllocated[population] += tileContribution;
                            if(tilesAllocated[population] >= tilesNeeded[population])
                                break;
                        }
                    }
                }
            }
        }

        // I am uncertain what this code accomplishes
        // if (needName.Equals("Liquid"))
        // {
        //     numTiles = rpm.GetLiquidComposition(population).Count;
        //     Debug.Log("terrain count" + count + ", numtiles: " + numTiles);
        // }
        foreach (TileType tile in terrainOrder)
        {
            foreach(Population population in populationsByDominance)
            {
                population.UpdateNeed(tile.ToString(), tilesAllocated[population]);
                Debug.Log("Tiles allocated to " + population.Species.SpeciesName + ": " + tilesAllocated[population]);
            }
        }

        foreach (FoodSource foodSource in Consumers.OfType<FoodSource>())
        {
            int[] terrainCountsByType = new int[(int)TileType.TypesOfTiles];
            terrainCountsByType = gridSystem.CountOfTilesInArea(gridSystem.WorldToCell(foodSource.GetPosition()), foodSource.Species.Size, foodSource.Species.RootArea);
            // Update need values
            foreach (var (count, index) in terrainCountsByType.WithIndex())
            {
                string needName = ((TileType)index).ToString();

                if (foodSource.GetNeedValues().ContainsKey(needName))
                {
                    if (needName.Equals("Liquid"))
                    {
                        int liquidCount = gridSystem.CountOfTilesInRange(gridSystem.WorldToCell(foodSource.GetPosition()), foodSource.Species.RootRadius)[index];
                        //Debug.Log(foodSource.name + " updated " + needName + " with value: " + liquidCount);
                        foodSource.UpdateNeed(needName, liquidCount);
                        continue;
                    }
                    //Debug.Log(foodSource.name + " updated " + needName + " with value: " + count);
                    foodSource.UpdateNeed(needName, count);
                }
            }
        }

        this.isDirty = false;
    }


    private class DominanceComparer : IComparer<Population>
    {
        public int Compare(Population x, Population y)
        {
            return y.Dominance - x.Dominance;
        }
    }
}

/// <summary>
/// To use .WithIndex
/// </summary>
public static class ForeachExtension
{
    public static  IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
    {
        return self.Select((item, index) => (item, index));
    }
}