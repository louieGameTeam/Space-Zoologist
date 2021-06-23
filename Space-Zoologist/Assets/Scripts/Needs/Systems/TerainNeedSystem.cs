using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Hadnles need value update for all  `Terrain` type needs
/// </summary>
public class TerrainNeedSystem : NeedSystem
{
    // For `Population` consumers
    private readonly ReservePartitionManager rpm = null;
    // For `FoodSource` consumers
    private TileSystem tileSystem = null;

    public TerrainNeedSystem(ReservePartitionManager rpm, TileSystem tileSystem, NeedType needType = NeedType.Terrain) : base(needType)
    {
        this.rpm = rpm;
        this.tileSystem = tileSystem;

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

            TileType type = tileSystem.GetGameTileAt(position).type;
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
            if (rpm.CanAccess(population, position) && tileSystem.GetGameTileAt(next) != null && tileSystem.GetGameTileAt(next).type == type)
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

        foreach (Population population in Consumers.OfType<Population>())
        {
            int[] terrainCountsByType = new int[(int)TileType.TypesOfTiles];
            terrainCountsByType = rpm.GetTypesOfTiles(population);
            foreach (var (count, index) in terrainCountsByType.WithIndex())
            {
                int numTiles = count;
                string needName = ((TileType)index).ToString();
                if (population.GetNeedValues().ContainsKey(needName))
                {
                    if (needName.Equals("Liquid"))
                    {
                        numTiles = rpm.GetLiquidComposition(population).Count;
                    }

                    population.UpdateNeed(needName, numTiles);
                }
            }
        }
        foreach (FoodSource foodSource in Consumers.OfType<FoodSource>())
        {
            int[] terrainCountsByType = new int[(int)TileType.TypesOfTiles];
            terrainCountsByType = tileSystem.CountOfTilesInRange(Vector3Int.FloorToInt(foodSource.GetPosition()), foodSource.Species.RootRadius);
            // Update need values
            foreach (var (count, index) in terrainCountsByType.WithIndex())
            {
                string needName = ((TileType)index).ToString();

                if (foodSource.GetNeedValues().ContainsKey(needName))
                {
                    foodSource.UpdateNeed(needName, count);
                }
            }
        }

        this.isDirty = false;
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