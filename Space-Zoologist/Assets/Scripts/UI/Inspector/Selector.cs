using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Hanldes the selection of enclosed area/food source/population
/// </summary>
public class Selector : MonoBehaviour
{
    public bool IsInSelectionMode { get; private set; }

    public GameObject SelectedGameObject { get; private set; }
    public Population SelectedPopulation { get; private set; }
    public FoodSource SelectedFoodSource { get; private set; }
    public Vector3Int SelectedTileLocation { get; private set; }
    public GameTile SelectedTile { get; private set; }
    public List<Vector3Int> SelectedTiles { get; private set; }
    public EnclosedArea SelectedEnclosedArea { get; private set; }

    [SerializeField] private TileDataController gridSystem = default;

    [SerializeField] private Tilemap highLight = default;
    [SerializeField] private TerrainTile highLightTile = default;

    public void EnableSelection() { this.ResetSelection();  this.IsInSelectionMode = true; }
    public void UnenableSelection() { this.IsInSelectionMode = false; }

    private void Start()
    {
        this.SelectedTiles = new List<Vector3Int>();
    }

    /// <summary>
    /// Wait for mouse down event and check selection
    /// </summary>
    private void Update()
    {
        if (this.IsInSelectionMode && Input.GetMouseButtonDown(0))
        {
            // Update animal locations
            this.gridSystem.UpdateAnimalCellGrid();

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = this.gridSystem.WorldToCell(worldPos);
            this.SelectedTile = this.gridSystem.GetGameTileAt(cellPos);

            //Debug.Log($"Mouse click at {cellPos}");

            TileData cellData = gridSystem.GetTileData(cellPos);

            if (cellData == null)
                return;

            this.UnHighlightAll();
            //this.ResetSelection();

            // Check if selection is anaiaml
            if (cellData.Animal)
            {
                // Highlight selected population 
                this.HighlightPopulation(cellData.Animal.transform.parent.gameObject);

                // Set population selection
                this.SelectedGameObject = cellData.Animal.transform.parent.gameObject;
                this.SelectedPopulation = cellData.Animal.GetComponent<Animal>().PopulationInfo;
            }
            // Selection is food source or item
            else if (cellData.Food)
            {
                this.HighlightFoodSource(cellData.Food);

                this.SelectedGameObject = cellData.Food;
                this.SelectedFoodSource = cellData.Food.GetComponent<FoodSource>();
            }
        }
    }

    private void ResetSelection()
    {
        this.SelectedGameObject = null;
        this.SelectedPopulation = null;
        this.SelectedFoodSource = null;
        this.SelectedTileLocation = Vector3Int.zero; // set to (0,0,0) not null val
        this.SelectedTile = null;
        this.SelectedTiles.Clear();
    }

    private void HighlightPopulation(GameObject population)
    {
        this.SelectedGameObject = population;
        foreach (Transform child in population.transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    private void HighlightFoodSource(GameObject foodSourceGameObject)
    {
        // Highlight food source object
        foodSourceGameObject.GetComponent<SpriteRenderer>().color = Color.blue;

        FoodSource foodSource = foodSourceGameObject.GetComponent<FoodSource>();

        // Hightlight
        List<Vector3Int> foodSourceRadiusRange = this.gridSystem.AllCellLocationsinRange(this.gridSystem.WorldToCell(foodSourceGameObject.transform.position), foodSource.Species.RootRadius);
        foreach (Vector3Int pos in foodSourceRadiusRange)
        {
            this.highLight.SetTile(pos, this.highLightTile);
        }

        this.SelectedTiles = foodSourceRadiusRange;
    }

    private void UnHighlightAll()
    {
        if (this.SelectedGameObject)
        {
            //// Unhighlight selected game object (food source)
            //if (this.SelectedGameObject.GetComponent<SpriteRenderer>())
            //{
            //    this.SelectedGameObject.GetComponent<SpriteRenderer>().color = Color.white;
            //}

            // Unhighlight child of selected game object (population)
            foreach (Transform child in this.SelectedGameObject.transform)
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        //if (this.SelectedTiles.Count > 0)
        //{
        //    foreach (Vector3Int pos in this.SelectedTiles)
        //    {
        //        this.highLight.SetTile(pos, null);
        //    }
        //}
    }
}
