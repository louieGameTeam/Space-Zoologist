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

    private GameObject lastFoodSourceSelected = null;
    private GameObject lastPopulationSelected = null;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Player can only enter inspect mode when not in store</remarks>
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
            this.HUD.SetActive(false);
        }
        else
        {
            this.inspectorButtonText.text = "INSPECTOR:OFF";
            this.needSystemUpdater.UnpauseAllAnimals();
            this.inspectorWindow.SetActive(false);
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
            if (cellPos.x >= this.gridSystem.GridWidth - 1 || cellPos.y >= this.gridSystem.GridHeight - 1 || cellPos.x < 1 || cellPos.y < 1)
            {
                return;
            }
            GridSystem.CellData cellData = this.gridSystem.CellGrid[cellPos.x, cellPos.y];

            //Debug.Log($"Mouse click at {cellPos}");

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
                this.HighLightFoodSource(cellData.Food);
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
            else
            {
                this.UnHighlightAll();
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

        string displayText = $"{population.species.SpeciesName}:\n";

        displayText += $"Count: {population.Count}\n";

        foreach (Need need in population.Needs.Values)
        {
            displayText += $"{need.NeedName} : {need.GetCondition(need.NeedValue)}\n";
        }

        this.inspectorWindowText.text = displayText;
    }

    private void HighLightFoodSource(GameObject foodSource)
    {
        foodSource.GetComponent<SpriteRenderer>().color = Color.blue;
        this.lastFoodSourceSelected = foodSource;
    }

    private void DisplayFoodSourceStatus(FoodSource foodSource)
    {
        string displayText = $"{foodSource.name} Info: \n";

        displayText += $"Output: {foodSource.FoodOutput}\n";

        foreach (Need need in foodSource.Needs.Values)
        {
            displayText += $"{need.NeedName} : {need.GetCondition(need.NeedValue)}\n";
        }


        this.inspectorWindowText.text = displayText;
    }

    private void DislplayEnclosedArea(Vector3Int cellPos)
    {
        AtmosphericComposition atmosphericComposition = enclosureSystem.GetAtmosphericComposition(cellPos);

        // THe composition is a list of float value in the order of the AtmoshpereComponent Enum
        float[] composition = atmosphericComposition.GeComposition();

        string displayText = "Enclosed Area:\n";


        // Atmospheric info
        displayText += "Atmospheric composition: \n";
        foreach (var (value, index) in composition.WithIndex())
        {
            displayText += $"{((AtmosphereComponent)index).ToString()} : {value}\n";
        }

        this.inspectorWindowText.text = displayText;
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