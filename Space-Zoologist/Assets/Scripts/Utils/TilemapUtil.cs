using UnityEngine;
using UnityEngine.Tilemaps;

// TODO refactor/possibly remove now that the tilemap translates directly to the grid
/// <summary>
/// For translating between vector3Int to location on 2d grid array
/// </summary>
public class TilemapUtil : MonoBehaviour
{
    public static TilemapUtil ins;

    [Header("Should hold largest tilemap")]
    [SerializeField] public Tilemap largestMap = default;
    [SerializeField] public int MaxWidth = default;
    [SerializeField] public int MaxHeight = default;
    public void Awake()
    {
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }
    }

    /// <summary>
    /// Converts a world position to cell position using the largest reference tilemap.
    /// </summary>
    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return largestMap.WorldToCell(worldPos);
    }

    /// <summary>
    /// Translates cell location to 2d array location using the largest map as reference.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public AnimalPathfinding.Node CellToGrid(Vector3Int location, AnimalPathfinding.Grid grid)
    {
        return grid.GetNode(location.x, location.y);
    }

    /// <summary>
    /// Translates node from 2d array back to world position.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="pathOffset"></param>
    /// <returns></returns>
    public Vector3 GridToWorld(Vector3 node, float pathOffset)
    {
        return new Vector3(node.x + pathOffset, node.y + pathOffset, 0);
    }

    public Vector3Int GridToWorld(Vector3 node)
    {
        return new Vector3Int((int)(node.x), (int)(node.y), 0);
    }

    public bool CanAccess(AnimalPathfinding.Node node, AnimalPathfinding.Grid grid)
    {
        return grid.GetNode(node.gridX, node.gridY).walkable;
    }
}
