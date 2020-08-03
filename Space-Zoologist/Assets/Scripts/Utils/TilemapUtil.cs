using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Translating between world position and grid cell location and highlights the boundaries
/// </summary>
public class TilemapUtil : MonoBehaviour
{
    public static TilemapUtil ins;
    public int MaxWidth { get => LevelDataReference.LevelData.MapWidth; }
    public int MaxHeight { get => LevelDataReference.LevelData.MapHeight; }
    [SerializeField] private LevelDataReference LevelDataReference = default;
    [SerializeField] private TilePlacementController TilePlacementController = default;
    [Header("Shows shaded perimeter when true")]
    [SerializeField] private bool DesigningLevel = true;

    [Header("Used for WorldToCell function")]
    [SerializeField] public Tilemap referenceTilemap = default;

    private void Awake()
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

    private void Start()
    {
        if (this.DesigningLevel) this.ShadeOutsidePerimeter();
    }

    /// <summary>
    /// Converts a world position to cell position using a reference tilemap.
    /// </summary>
    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return referenceTilemap.WorldToCell(worldPos);
    }

    /// <summary>
    /// Indicates where the perimeter for placing items is at
    /// </summary>
    private void ShadeOutsidePerimeter()
    {
        for (int x=-1; x<this.MaxWidth + 1; x++)
        {
            this.ShadeSquare(x, -1, Color.red);
            this.ShadeSquare(x, this.MaxHeight, Color.red);
        }
        for (int y=-1; y<this.MaxHeight + 1; y++)
        {
            this.ShadeSquare(-1, y, Color.red);
            this.ShadeSquare(this.MaxWidth, y, Color.red);
        }
    }

    private void ShadeSquare(int x, int y, Color color)
    {
        Vector3Int cellToShade = new Vector3Int(x, y, 0);
        referenceTilemap.SetColor(cellToShade, color);
    }
}
