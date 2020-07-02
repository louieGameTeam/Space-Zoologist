using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// For translating between vector3Int to location on 2d grid array
public class MapToGridUtil : MonoBehaviour
{
    public static MapToGridUtil ins;

    [SerializeField] public Tilemap map = default;

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
    /// Converts a world position to cell position using the reference tilemap.
    /// </summary>
    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return map.WorldToCell(worldPos);
    }

    public AnimalPathfinding.Node CellToGrid(Vector3Int location, AnimalPathfinding.Grid grid)
    {
        return grid.GetNode(location.x + (map.origin.x * -1), location.y + (map.origin.y * -1));
    }

    public Vector3 GridToCell(AnimalPathfinding.Node node, float pathOffset)
    {
        return new Vector3(node.gridX + pathOffset + MapToGridUtil.ins.map.origin.x, node.gridY + pathOffset + MapToGridUtil.ins.map.origin.y, 0);
    }
}
