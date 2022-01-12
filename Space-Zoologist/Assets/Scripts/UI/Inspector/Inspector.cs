using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// This script is to attached to the inspector button and handles
/// entering/exiting inspect mode, getting 
/// </summary>
public class Inspector : MonoBehaviour
{
    public bool IsInInspectorMode { get; private set; }

    // The inspector window
    [SerializeField] private GameObject inspectorWindow = null;
    [SerializeField] private DisplayInspectorText inspectorWindowDisplayScript;
    [SerializeField] private GameObject GrowthInfo = default;

    private GameObject lastFoodSourceSelected = null;
    private GameObject lastPopulationSelected = null;
    private List<Vector3Int> lastTilesSelected = new List<Vector3Int>();
    public GameObject PopulationHighlighted { get; private set; } = null;
    private Vector3Int selectedPosition;

    //TODO This does not feels right to be here
    private List<Life> itemsInEnclosedArea = new List<Life>();

    public void Initialize()
    {
        this.IsInInspectorMode = false;
    }

    public void CloseInspector()
    {
        this.inspectorWindowDisplayScript.ClearInspectorWindow();
        this.inspectorWindow.SetActive(false);
        //this.HUD.SetActive(true);
        this.UnHighlightAll();
        EventManager.Instance.InvokeEvent(EventType.InspectorClosed, null);
        this.IsInInspectorMode = false;
    }

    public void ResetSelection()
    {
        this.inspectorWindowDisplayScript.ClearInspectorWindow();
        this.UnHighlightAll();
    }

    // Referenced by the details button
    public void ToggleDetails()
    {
        this.GrowthInfo.SetActive(!this.GrowthInfo.activeSelf);
    }

    public void OpenInspector()
    {
        this.inspectorWindow.SetActive(true);
        GameManager.Instance.m_gridSystem.UpdateAnimalCellGrid();
        //this.HUD.SetActive(false);
        EventManager.Instance.InvokeEvent(EventType.InspectorOpened, null);
        this.IsInInspectorMode = true;
    }

    /// <summary>
    /// Listens to mouse clicks and what was selected, then call functions to
    /// display info on inspector window
    /// </summary>
    public void Update()
    {
        if (this.IsInInspectorMode && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 5)
            {
                return;
            }
            this.UpdateInspectorValues();
        }
        if (this.IsInInspectorMode)
        {
            if (this.PopulationHighlighted != null)
            {
                this.HighlightPopulation(this.PopulationHighlighted);
            }
        }
    }

    // TODO break this up and refactor
    public void UpdateInspectorValues()
    {
        // Update animal location reference
        GameManager.Instance.m_gridSystem.UpdateAnimalCellGrid();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int pos = GameManager.Instance.m_gridSystem.WorldToCell(worldPos);
        GameTile tile = GameManager.Instance.m_gridSystem.GetGameTileAt(pos);
        GridSystem.TileData cellData = GameManager.Instance.m_gridSystem.GetTileData(pos);

        if (cellData == null) { 
            return;
        }
        bool somethingSelected = true;
        this.UnHighlightAll();
        if (cellData.Animal)
        {
            DisplayPopulationText(cellData);
            selectedPosition = pos;
        }
        // Selection is food source or item
        else if (cellData.Food)
        {
            DisplayFoodText(cellData);
            selectedPosition = pos;
        }
        // Selection is liquid tile
        else if (tile && tile.type == TileType.Liquid)
        {
            DisplayLiquidText(pos);
            selectedPosition = pos;
        }
        // Selection is enclosed area
        // Disabled
        //else if (tile && tile.type != TileType.Wall)
        //{
        //    DisplayAreaText(pos);
        //    selectedPosition = pos;
        //    somethingSelected = false;
        //}
        else
        {
            somethingSelected = false;
        }
        if (somethingSelected)
        {
            AudioManager.instance.PlayOneShot(SFXType.Observation);
        }
    }

    public void UpdateCurrentDisplay()
    {
        GridSystem.TileData cellData = GameManager.Instance.m_gridSystem.GetTileData(selectedPosition);
        switch (inspectorWindowDisplayScript.CurrentDisplay)
        {
            case DisplayInspectorText.InspectorText.Population:
                if (!cellData.Animal)
                {
                    return;
                }
                DisplayPopulationText(cellData);
                break;
            case DisplayInspectorText.InspectorText.Food:
                if (!cellData.Food)
                {
                    return;
                }
                DisplayFoodText(cellData);
                break;
            case DisplayInspectorText.InspectorText.Area:
                DisplayAreaText(selectedPosition);
                break;
            case DisplayInspectorText.InspectorText.Liquid:
                DisplayLiquidText(selectedPosition);
                break;
            default:
                break;
        }
    }

    private void DisplayPopulationText(GridSystem.TileData tileData)
    {
        this.HighlightPopulation(tileData.Animal.transform.parent.gameObject);

        //Debug.Log($"Found animal {cellData.Animal.GetComponent<Animal>().PopulationInfo.Species.SpeciesName} @ {cellPos}");
        this.inspectorWindowDisplayScript.DisplayPopulationStatus(tileData.Animal.GetComponent<Animal>().PopulationInfo);
    }

    private void DisplayFoodText(GridSystem.TileData cellData)
    {
        GameObject FoodObject = cellData.Food;

        this.HighlightFoodSource(FoodObject);
        //Debug.Log($"Foudn item {cellData.Food} @ {cellPos}");
        // root radius here
        FoodSource foodSource = FoodObject.GetComponent<FoodSource>();
        float rootRadius = foodSource.Species.RootRadius;
        Vector2Int foodSize = foodSource.Species.Size;

        Vector3 foodPosition = FoodObject.transform.position;
        Vector3Int foodPositionInt = new Vector3Int((int)foodPosition.x - foodSize.x / 2, (int)foodPosition.y - foodSize.y / 2, (int)foodPosition.z);
        
        for (int x = -(foodSize.x - 1) / 2; x <= foodSize.x / 2; x++)
        {
            for (int y = -(foodSize.x - 1) / 2; y <= foodSize.y / 2; y++)
            {
                GameManager.Instance.m_gridSystem.HighlightRadius(foodPositionInt + new Vector3Int(x, y, 0), Color.blue, rootRadius);
            }
        }

        this.inspectorWindowDisplayScript.DisplayFoodSourceStatus(cellData.Food.GetComponent<FoodSource>());
    }

    private void DisplayAreaText(Vector3Int cellPos)
    {
        //Debug.Log($"Enclosed are @ {cellPos} selected");
        GameManager.Instance.m_enclosureSystem.UpdateEnclosedAreas();
    }

    private void DisplayLiquidText(Vector3Int cellPos)
    {
        //Debug.Log($"Selected liquid tile @ {cellPos}");
        GridSystem.TileData td = GameManager.Instance.m_gridSystem.GetTileData(cellPos);
        float[] compositions = td.contents;
        this.inspectorWindowDisplayScript.DisplayLiquidCompisition(compositions);
    }


    private void UnHighlightAll()
    {
        if( this.lastFoodSourceSelected)
        {
            this.lastFoodSourceSelected.GetComponent<SpriteRenderer>().color = Color.white;
            this.lastFoodSourceSelected = null;
        }
        if (this.lastPopulationSelected)
        {
            this.PopulationHighlighted = null;
            foreach (Transform child in this.lastPopulationSelected.transform)
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
            this.lastPopulationSelected = null;
        }

        GameManager.Instance.m_gridSystem.ClearHighlights();
    }

    private void HighlightPopulation(GameObject population)
    {
        this.PopulationHighlighted = population;
        foreach (Transform child in population.transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        }

        // highlight their accessible terrain too
        Population populationScript = population.GetComponent<Population>();
        List<Vector3Int> accessibleTiles = GameManager.Instance.m_reservePartitionManager.AccessibleArea[populationScript];

        foreach (Vector3Int tilePosition in accessibleTiles)
            GameManager.Instance.m_gridSystem.HighlightTile(tilePosition, Color.green);

        this.lastPopulationSelected = population;
    }


    private void HighlightFoodSource(GameObject foodSourceGameObject)
    {
        // Highlight food source object
        foodSourceGameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        this.lastFoodSourceSelected = foodSourceGameObject;
    }

    public GameObject GetAnimalSelected()
    {
        return lastPopulationSelected;
    }

    public void GetGameTileAndTileData(out GameTile gameTile, out GridSystem.TileData tileData)
    {
        // Update animal location reference
        GameManager.Instance.m_gridSystem.UpdateAnimalCellGrid();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int pos = GameManager.Instance.m_gridSystem.WorldToCell(worldPos);
        gameTile = GameManager.Instance.m_gridSystem.GetGameTileAt(pos);
        tileData = GameManager.Instance.m_gridSystem.GetTileData(pos);
    }
}
