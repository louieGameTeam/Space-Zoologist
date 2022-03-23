using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A runtime instance of a food source
/// </summary>
public class FoodSource : MonoBehaviour
{
    public FoodSourceSpecies Species => species;
    public float FoodOutput
    {
        get
        {
            // Check if terrain and water ratings are above zero
            if (Rating.TerrainRating > 0.001f && Rating.WaterRating > 0.001f)
            {
                float boost = (Rating.TerrainRating + Rating.WaterRating) / 2;
                return Mathf.Ceil(species.BaseOutput * boost);
            }
            else return 0f;
        }
    }
    public Vector2 Position { get; private set; } = Vector2.zero;
    public NeedAvailability Availability => GameManager.Instance.Needs.Availability.GetAvailability(this);
    public NeedRating Rating => GameManager.Instance.Needs.Ratings.GetRating(this);

    // For runtime instances of a food source
    [Expandable][SerializeField] private FoodSourceSpecies species = default;

    public bool isUnderConstruction = false;

    private int[] accessibleTerrian = new int[(int)TileType.TypesOfTiles];
    private bool hasAccessibilityChanged = default;
    private bool hasAccessibilityChecked = default;

    // To figure out if the output has changed, in order to invoke vent
    private float prevOutput = 0;

    private void Awake()
    {
        if (species)
        {
            InitializeFoodSource(species, transform.position);
        }
    }

    // Subscribe to events here
    private void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (GameManager.Instance)
        {
            TileDataController tileDataController = GameManager.Instance.m_tileDataController;
            
            if (tileDataController)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(Position, 0.1f);

                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(GetLowerLeftPosition(), 0.1f);

                Gizmos.color = Color.green;
                foreach(Vector3Int cellPos in GetAllCellPositions())
                {
                    Gizmos.DrawCube(cellPos, Vector3.one * 0.1f);
                }
            }
        }
    }

    public void InitializeFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        this.species = species;
        this.Position = position;
        this.GetComponent<SpriteRenderer>().sprite = species.FoodSourceItem.Icon;
        this.accessibleTerrian = GameManager.Instance.m_tileDataController.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);
    }

    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }

    /// <summary>
    /// Get the world position of the lower left cell of the food source
    /// </summary>
    /// <returns></returns>
    public Vector3 GetLowerLeftPosition()
    {
        // Get tile data controller and the cell size
        TileDataController tileDataController = GameManager.Instance.m_tileDataController;
        Vector3 cellSize = tileDataController.CellSize();

        // Compute an offset from the center of the food source
        // down to the bottom left of the food source
        Vector3 offset = (Vector2)(Species.Size - Vector2Int.one) / 2f;
        offset.x *= cellSize.x;
        offset.y *= cellSize.y;

        // Lower left position is the 
        return GetPosition() - offset;
    }

    /// <summary>
    /// Get the cell position of the lower left corner of the food source
    /// </summary>
    /// <returns></returns>
    public Vector3Int GetCellPosition()
    {
        return GameManager.Instance.m_tileDataController.WorldToCell(GetPosition());
    }

    /// <summary>
    /// Get a list of all the cell positions that the food source takes up
    /// </summary>
    /// <returns></returns>
    public List<Vector3Int> GetAllCellPositions()
    {
        TileDataController tileDataController = GameManager.Instance.m_tileDataController;
        List<Vector3Int> positions = new List<Vector3Int>();

        for(int x = 0; x < Species.Size.x; x++)
        {
            for (int y = 0; y < Species.Size.y; y++)
            {
                // Get the cell size
                Vector2 cellSize = tileDataController.CellSize();

                // Compute an offset from the lower left position
                Vector3 offset = new Vector3(
                    cellSize.x * x,
                    cellSize.y * y,
                    0f);

                // Add the offset to the lower left position
                Vector3 worldPosition = GetLowerLeftPosition() + offset;

                // Compute the grid position for this world position
                positions.Add(tileDataController.WorldToCell(worldPosition));
            }
        }

        return positions;
    }

    /// <summary>
    /// Checks accessible terrain info, ie terrain tile composition
    /// </summary>
    /// <remarks>
    /// Actual checking will only be done once per universal NS update loop,
    /// since terrain will not change during that time
    /// </remarks>
    /// <returns>True is accessible terrain had changed, false otherwise</returns>
    public bool GetAccessibilityStatus()
    {
        // No need to check if terrain was not modified
        if (!GameManager.Instance.m_tileDataController.HasTerrainChanged)
        {
            return false;
        }

        // Return result if have checked
        if (this.hasAccessibilityChecked)
        {
            return this.hasAccessibilityChanged;
        }

        var preTerrain = this.accessibleTerrian;
        var curTerrain = GameManager.Instance.m_tileDataController.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);

        // Accessible terrain had changed
        this.hasAccessibilityChecked = true;
        if(!preTerrain.SequenceEqual(curTerrain))
        {
            this.hasAccessibilityChanged = true;
        }

        return this.hasAccessibilityChanged;
    }

    /// <summary>
    /// Updates the accessible terrain info
    /// </summary>
    public void UpdateAccessibleTerrainInfo()
    {
        if (this.hasAccessibilityChanged)
        {
            this.accessibleTerrian = GameManager.Instance.m_tileDataController.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);
        }

        // Reset flags
        this.hasAccessibilityChecked = false;
        this.hasAccessibilityChanged = false;
    }
}
