using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// For translating between vector3Int to location on 2d grid array
/// </summary>
public class TilemapUtil : MonoBehaviour
{
    public static TilemapUtil ins;

    [Header("Should hold largest tilemap")]
    [SerializeField] public Tilemap largestMap = default;

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
        return grid.GetNode(location.x + (largestMap.origin.x * -1), location.y + (largestMap.origin.y * -1));
    }

    /// <summary>
    /// Translates node from 2d array back to world position.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="pathOffset"></param>
    /// <returns></returns>
    public Vector3 GridToWorld(Vector3 node, float pathOffset)
    {
        return new Vector3(node.x + pathOffset + largestMap.origin.x, node.y + pathOffset + largestMap.origin.y, 0);
    }

    public Vector3Int GridToWorld(Vector3 node)
    {
        return new Vector3Int((int)(node.x + largestMap.origin.x), (int)(node.y + largestMap.origin.y), 0);
    }

    public bool CanAccess(AnimalPathfinding.Node node, AnimalPathfinding.Grid grid)
    {
        return grid.GetNode(node.gridX, node.gridY).walkable;
    }

    // Using direction, starting location, and grid, determine if the next spot on the grid is accessible
    public bool DirectionAllowed(Direction direction, Vector3 startingLocation, AnimalPathfinding.Grid grid)
    {
        bool isAllowed = false;
        AnimalPathfinding.Node currentSpot = this.CellToGrid(this.WorldToCell(startingLocation), grid);
        if (currentSpot == null)
        {
            Debug.Log("Node outside of range");
            return false;
        }
        switch(direction)
        {
            case Direction.up:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX, currentSpot.gridY + 1);
                break;
            }
            case Direction.down:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX, currentSpot.gridY - 1);
                break;
            }
            case Direction.left:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX - 1, currentSpot.gridY);
                break;
            }
            case Direction.right:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX + 1, currentSpot.gridY);
                break;
            }
            case Direction.upRight:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX + 1, currentSpot.gridY + 1);
                break;
            }
            case Direction.upLeft:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX - 1, currentSpot.gridY + 1);
                break;
            }
            case Direction.downRight:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX + 1, currentSpot.gridY - 1);
                break;
            }
            case Direction.downLeft:
            {
                isAllowed = grid.IsAccessible(currentSpot.gridX - 1, currentSpot.gridY - 1);
                break;
            }
        }
        return isAllowed;
    }
}
