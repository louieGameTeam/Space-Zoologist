using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is to attached to the inspector button and handles
/// entering/exiting inspect mode, getting 
/// </summary>
public class Inspector : MonoBehaviour
{
    private bool isInInspectorMode = false;

    [SerializeField] private Text inspectorButtonText = null;
    // To pause/free animals
    [SerializeField] private NeedSystemUpdater needSystemUpdater = null;

    [SerializeField] private GridSystem gridSystem = null;
    [SerializeField] private TileSystem tileSystem = null;
    [SerializeField] private EnclosureSystem enclosureSystem = null;

    // To access other UI elements to toggle
    [SerializeField] private GameObject HUD = null;
    // The inspector window 
    [SerializeField] private GameObject inspectorWindow = null;
    [SerializeField] private Text inspectorWindowText = null;
    [SerializeField] private GameObject areaDropdown = null;
    [SerializeField] private GameObject itemDropdown = null;

    private GameObject lastFoodSourceSelected = null;
    private GameObject lastPopulationSelected = null;
    private List<Vector3Int> lastTilesSelected = new List<Vector3Int>();

    /// <summary>
    /// Toggle displays
    /// </summary>
    public void ToggleInspectMode()
    {
        // Cannot enter inspector mode while in istore
        if (this.needSystemUpdater.isInStore)
        {
            return;
        }

        // Toggle flag
        this.isInInspectorMode = !isInInspectorMode;

        // Toggle button text, displays and pause/free animals
        if (this.isInInspectorMode)
        {
            this.inspectorButtonText.text = "INSPECTOR:ON";
            this.inspectorWindowText.text = "INSPECTOR";
            this.needSystemUpdater.PauseAllAnimals();
            this.inspectorWindow.SetActive(true);
            this.areaDropdown.SetActive(true);
            this.itemDropdown.SetActive(true);
            this.HUD.SetActive(false);
        }
        else
        {
            this.inspectorButtonText.text = "INSPECTOR:OFF";
            this.needSystemUpdater.UnpauseAllAnimals();
            this.inspectorWindow.SetActive(false);
            this.areaDropdown.SetActive(false);
            this.itemDropdown.SetActive(false);
            this.HUD.SetActive(true);
            this.UnHighlightAll();
        }

        //Debug.Log($"Inspector mode is {this.isInInspectorMode}");
    }

    /// <summary>
    /// Listens to mouse clicks and what was selected, then call functions to
    /// display info on inspector window
    /// </summary>
    public void Update()
    {
        if (this.isInInspectorMode && Input.GetMouseButtonDown(0))
        {
            // Update animal locations
            this.gridSystem.UpdateAnimalCellGrid();

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = this.tileSystem.WorldToCell(worldPos);
            TerrainTile tile = this.tileSystem.GetTerrainTileAtLocation(cellPos);

            Debug.Log($"Mouse click at {cellPos}");

            GridSystem.CellData cellData;

            // Handles index out of bound exception
            if (this.gridSystem.isCellinGrid(cellPos.x, cellPos.y))
            {
                cellData = this.gridSystem.CellGrid[cellPos.x, cellPos.y];
            }
            else
            {
                Debug.Log("Grid location selected was out of bounds");
                return;
            }

            // Check if selection is anaiaml
            if (cellData.ContainsAnimal)
            {
                this.UnHighlightAll();
                this.HighlightPopulation(cellData.Animal);
                Debug.Log($"Found animal {cellData.Animal.GetComponent<Animal>().PopulationInfo.Species.SpeciesName} @ {cellPos}");
                this.DisplayAnimalStatus(cellData.Animal.GetComponent<Animal>());
            }
            // Selection is food source or item
            else if (cellData.ContainsFood)
            {
                this.UnHighlightAll();
                this.HighlightFoodSource(cellData.Food);
                Debug.Log($"Foudn item {cellData.Food} @ {cellPos}");
                this.DisplayFoodSourceStatus(cellData.Food.GetComponent<FoodSource>());
            }
            // Selection is liquid tile
            else if (tile.type == TileType.Liquid)
            {
                this.UnHighlightAll();
                Debug.Log($"Selected liquid tile @ {cellPos}");
                this.DisplayLiquidCompisition(cellPos, tile);
            }
            // Selection is enclosed area
            else if (tile && tile.type != TileType.Wall)
            {
                this.UnHighlightAll();
                this.HighlightEnclosedArea(cellPos);
                this.DislplayEnclosedArea(cellPos);
                Debug.Log($"Enclosed are @ {cellPos} selected");
            }
        }
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
            foreach (Transform child in this.lastPopulationSelected.transform)
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
            this.lastPopulationSelected = null;
        }
    }

    private void HighlightPopulation(GameObject animal)
    {
        GameObject population = animal.transform.parent.gameObject;

        foreach (Transform child in population.transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        }

        this.lastPopulationSelected = population;
    }

    private void DisplayAnimalStatus(Animal animal)
    {
        Population population = animal.PopulationInfo;

        string displayText = $"{population.species.SpeciesName} Info: \n";

        displayText += $"Count: {population.Count}\n";

        foreach (Need need in population.Needs.Values)
        {
            displayText += $"{need.NeedName} : {need.NeedValue} [{need.GetCondition(need.NeedValue)}]\n";
        }

        this.inspectorWindowText.text = displayText;
    }

    private void HighlightFoodSource(GameObject foodSource)
    {
        foodSource.GetComponent<SpriteRenderer>().color = Color.blue;
        this.lastFoodSourceSelected = foodSource;
    }

    private void DisplayFoodSourceStatus(FoodSource foodSource)
    {
        string displayText = $"{foodSource.name} Info: \n";

        displayText += $"Output: {foodSource.FoodOutput}/{foodSource.Species.BaseOutput}\n";

        foreach (Need need in foodSource.Needs.Values)
        {
            displayText += $"{need.NeedName} : {need.NeedValue} [{need.GetCondition(need.NeedValue)}]\n";
        }


        this.inspectorWindowText.text = displayText;
    }

    private void HighlightEnclosedArea(Vector3Int selectedLocation)
    {

    }

    private void UnHighSignleTile(Vector3Int location)
    {

    }

    private void DislplayEnclosedArea(Vector3Int cellPos)
    {
        EnclosedArea enclosedArea = enclosureSystem.GetEnclosedArea(cellPos);

        // THe composition is a list of float value in the order of the AtmoshpereComponent Enum
        float[] atmosphericComposition = enclosedArea.atmosphericComposition.GetComposition();
        float[] terrainComposition = enclosedArea.terrainComposition;

        string displayText = "Enclosed Area Info: \n";


        // Atmospheric info
        displayText += "Atmospheric composition: \n";
        foreach (var (value, index) in atmosphericComposition.WithIndex())
        {
            displayText += $"{((AtmosphereComponent)index).ToString()} : {value}\n";
        }

        displayText += "\nTerrain: \n";
        foreach (var (value, index) in terrainComposition.WithIndex())
        {
            displayText += $"{((TileType)index).ToString()} : {value}\n";
        }

        displayText += $"\n Population count: {enclosedArea.populations.Count}";
        displayText += $"\n Food Source count: {enclosedArea.foodSources.Count}";

        this.inspectorWindowText.text = displayText;
    }

    private void HighlightSingleTile(Vector3Int location)
    {

    }

    private void DisplayLiquidCompisition(Vector3Int cellPos, TerrainTile tile)
    {
        float[] compositions = this.tileSystem.GetTileContentsAtLocation(cellPos, tile);

        string displayText = "Liquid composition: \n";

        foreach (var (composition, index) in compositions.WithIndex())
        {
            displayText += $"{((LiquidComposition)index).ToString()} : {composition}\n";
        }

        this.inspectorWindowText.text = displayText;
    }
}