using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

/// <summary>
/// This script is to attached to the inspector button and handles
/// entering/exiting inspect mode, getting 
/// </summary>
public class Inspector : MonoBehaviour
{
    public bool IsInInspectorMode { get; private set; }

    [SerializeField] private GridSystem gridSystem = null;
    [SerializeField] private TileSystem tileSystem = null;
    [SerializeField] private EnclosureSystem enclosureSystem = null;
    [SerializeField] private ReservePartitionManager rpm = default;

    [SerializeField] private Tilemap highLight = default;
    [SerializeField] private GameTile highLightTile = default;

    // The inspector window
    [SerializeField] private GameObject areaDropdownMenu = null;
    [SerializeField] private GameObject itemDropdownMenu = null;
    [SerializeField] private GameObject inspectorWindow = null;
    [SerializeField] private Text inspectorWindowText = null;

    [SerializeField] private Selector selector = default;

    [SerializeField] private bool relocatingPopulation = false;

    private GameObject lastFoodSourceSelected = null;
    private GameObject lastPopulationSelected = null;
    private List<Vector3Int> lastTilesSelected = new List<Vector3Int>();
    private Dropdown enclosedAreaDropdown;
    private Dropdown itemsDropdown;
    private DisplayInspectorText inspectorWindowDisplayScript;
    private GameObject PopulationHighlighted = null;

    //TODO This does not feels right to be here
    private List<Life> itemsInEnclosedArea = new List<Life>();

    private void Start()
    {
        this.IsInInspectorMode = false;
        this.enclosedAreaDropdown = this.areaDropdownMenu.GetComponent<Dropdown>();
        this.itemsDropdown = this.itemDropdownMenu.GetComponent<Dropdown>();
        this.enclosedAreaDropdown.onValueChanged.AddListener(selectEnclosedArea);
        this.itemsDropdown.onValueChanged.AddListener(selectItem);
        this.inspectorWindowDisplayScript = this.inspectorWindow.GetComponent<DisplayInspectorText>();

        // Have the dropdown options be refreshed when new items created
        EventManager.Instance.SubscribeToEvent(EventType.NewEnclosedArea, this.UpdateDropdownMenu);
        EventManager.Instance.SubscribeToEvent(EventType.NewFoodSource, this.UpdateDropdownMenu);
        EventManager.Instance.SubscribeToEvent(EventType.NewPopulation, this.UpdateDropdownMenu);
    }

    public void CloseInspector()
    {
        if (this.IsInInspectorMode)
        {
            this.inspectorWindow.SetActive(false);
            this.areaDropdownMenu.SetActive(false);
            this.itemDropdownMenu.SetActive(false);
            //this.HUD.SetActive(true);
            this.UnHighlightAll();
            EventManager.Instance.InvokeEvent(EventType.InspectorClosed, null);
            this.IsInInspectorMode = !IsInInspectorMode;
        }

    }

    public void OpenInspector()
    {
        this.inspectorWindowText.text = "Inspector";
        this.inspectorWindow.SetActive(true);
        this.gridSystem.UpdateAnimalCellGrid();
        this.UpdateDropdownMenu();
        this.areaDropdownMenu.SetActive(true);
        this.itemDropdownMenu.SetActive(true);
        //this.HUD.SetActive(false);
        EventManager.Instance.InvokeEvent(EventType.InspectorOpened, null);
        this.IsInInspectorMode = !IsInInspectorMode;
    }

    /// <summary>
    /// Toggle displays
    /// </summary>
    public void ToggleInspectMode()
    {
        // Toggle button text, displays and pause/free animals
        if (!this.IsInInspectorMode)
        {
            this.OpenInspector();
        }
        else
        {
            this.CloseInspector();
        }

        //Debug.Log($"Inspector mode is {this.IsInInspectorMode}");
    }

    private void UpdateDropdownMenu()
    {
        this.enclosedAreaDropdown.options.Clear();

        this.itemsDropdown.options.Clear();
        this.itemsInEnclosedArea.Clear();
        this.itemsDropdown.options.Add(new Dropdown.OptionData { text = $"Select an item" });

        // Add empty option
        this.enclosedAreaDropdown.options.Add(new Dropdown.OptionData { text = $"Select an area" });

        foreach (EnclosedArea enclosedArea in this.enclosureSystem.EnclosedAreas)
        {
            this.enclosedAreaDropdown.options.Add(new Dropdown.OptionData { text = $"Enclosed Area {enclosedArea.id}"});
        }
    }

    private void selectEnclosedArea(int selection)
    {
        // Selected placeholder option
        if (selection == 0)
        {
            return;
        }

        EnclosedArea enclosedAreaSelected = this.enclosureSystem.EnclosedAreas[selection-1];

        //Debug.Log($"Enclosed area {enclosedAreaSelected.id} selected from dropdown");

        this.itemsDropdown.options.Clear();
        this.itemsInEnclosedArea.Clear();

        this.itemsDropdown.options.Add(new Dropdown.OptionData { text = $"Select an item" });

        foreach (Population population in enclosedAreaSelected.populations)
        {
            this.itemsDropdown.options.Add(new Dropdown.OptionData { text = $"{population.Species.SpeciesName}" });
            this.itemsInEnclosedArea.Add(population);
        }

        foreach (FoodSource foodSource in enclosedAreaSelected.foodSources)
        {
            this.itemsDropdown.options.Add(new Dropdown.OptionData { text = $"{foodSource.Species.SpeciesName}" });
            this.itemsInEnclosedArea.Add(foodSource);
        }

        // Set item selection to placeholder option
        this.itemsDropdown.value = 0;

        this.inspectorWindowDisplayScript.DislplayEnclosedArea(enclosedAreaSelected);
    }

    private void selectItem(int selection)
    {
        // Selected placeholder option
        if (selection == 0)
        {
            return;
        }

        //Debug.Log($"selected item {selection} from dropdown");

        Life itemSelected = this.itemsInEnclosedArea[selection-1];

        if (itemSelected.GetType() == typeof(Population))
        {
            this.HighlightPopulation(((Population)itemSelected).gameObject);
            this.inspectorWindowDisplayScript.DisplayPopulationStatus((Population)itemSelected);
        }
        if (itemSelected.GetType() == typeof(FoodSource))
        {
            this.HighlightFoodSource(((FoodSource)itemSelected).gameObject);
            this.inspectorWindowDisplayScript.DisplayFoodSourceStatus((FoodSource)itemSelected);
        }

        // Set enclosed area dropdown to placeholder selection
        this.enclosedAreaDropdown.value = 0;
    }

    public void StartRelocation() {
        relocatingPopulation = true;
    }

    public void RelocatePopulation(Population population, Vector3 position) {
        if (population == null) return;
        population.transform.position = position;
        rpm.RemovePopulation(population);
        rpm.AddPopulation(population);
    }

    /// <summary>
    /// Listens to mouse clicks and what was selected, then call functions to
    /// display info on inspector window
    /// </summary>
    public void Update()
    {
        if (this.IsInInspectorMode && Input.GetMouseButtonDown(0))
        {
            // Update animal locations
            this.gridSystem.UpdateAnimalCellGrid();

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = this.tileSystem.WorldToCell(worldPos);
            GameTile tile = this.tileSystem.GetGameTileAt(cellPos);


            if (relocatingPopulation == true) {
                if (lastPopulationSelected) {
                    Population population = lastPopulationSelected.GetComponent<Population>();
                    RelocatePopulation(population, worldPos);
                }
                relocatingPopulation = false;
                return;
            }

            //Debug.Log($"Mouse click at {cellPos}");

            GridSystem.CellData cellData;

            // Handles index out of bound exception
            if (this.gridSystem.isCellinGrid(cellPos.x, cellPos.y))
            {
                cellData = this.gridSystem.CellGrid[cellPos.x, cellPos.y];
            }
            else
            {
                // Debug.Log($"Grid location selected was out of bounds @ {cellPos}");
                return;
            }

            this.UnHighlightAll();

            // Check if selection is animal
            if (cellData.ContainsAnimal)
            {
                this.HighlightPopulation(cellData.Animal.transform.parent.gameObject);
                //Debug.Log($"Found animal {cellData.Animal.GetComponent<Animal>().PopulationInfo.Species.SpeciesName} @ {cellPos}");
                this.inspectorWindowDisplayScript.DisplayPopulationStatus(cellData.Animal.GetComponent<Animal>().PopulationInfo);
            }
            // Selection is food source or item
            else if (cellData.ContainsFood)
            {
                this.HighlightFoodSource(cellData.Food);
                //Debug.Log($"Foudn item {cellData.Food} @ {cellPos}");
                this.inspectorWindowDisplayScript.DisplayFoodSourceStatus(cellData.Food.GetComponent<FoodSource>());
            }
            // Selection is liquid tile
            else if (tile.type == TileType.Liquid)
            {
                this.HighlightSingleTile(cellPos);
                //Debug.Log($"Selected liquid tile @ {cellPos}");
                float[] compositions = this.tileSystem.GetTileContentsAt(cellPos, tile);
                this.inspectorWindowDisplayScript.DisplayLiquidCompisition(compositions);
            }
            // Selection is enclosed area
            else if (tile && tile.type != TileType.Wall)
            {
                this.HighlightEnclosedArea(cellPos);
                this.enclosureSystem.UpdateEnclosedAreas();
                this.inspectorWindowDisplayScript.DislplayEnclosedArea(this.enclosureSystem.GetEnclosedAreaByCellPosition(cellPos));
                //Debug.Log($"Enclosed are @ {cellPos} selected");
            }

            // Reset dropdown selections
            this.enclosedAreaDropdown.value = 0;
            this.itemsDropdown.value = 0;
        }
        if (this.IsInInspectorMode)
        {
            if (this.PopulationHighlighted != null)
            {
                this.HighlightPopulation(this.PopulationHighlighted);
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
            this.PopulationHighlighted = null;
            foreach (Transform child in this.lastPopulationSelected.transform)
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
            gridSystem.UnhighlightHomeLocations();
            this.lastPopulationSelected = null;
        }

        foreach (Vector3Int pos in this.lastTilesSelected)
        {
            this.highLight.SetTile(pos, null);
        }

        lastTilesSelected.Clear();
    }

    private void HighlightPopulation(GameObject population)
    {
        this.PopulationHighlighted = population;
        foreach (Transform child in population.transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        }

        gridSystem.HighlightHomeLocations();
        this.lastPopulationSelected = population;
    }


    private void HighlightFoodSource(GameObject foodSourceGameObject)
    {
        // Highlight food source object
        foodSourceGameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        this.lastFoodSourceSelected = foodSourceGameObject;

        FoodSource foodSource = foodSourceGameObject.GetComponent<FoodSource>();

        // Hightlight
        List<Vector3Int> foodSourceRadiusRange = this.tileSystem.AllCellLocationsinRange(this.tileSystem.WorldToCell(foodSourceGameObject.transform.position), foodSource.Species.RootRadius);
        foreach (Vector3Int pos in foodSourceRadiusRange)
        {
            this.highLight.SetTile(pos, this.highLightTile);
            this.lastTilesSelected.Add(pos);
        }
    }

    // TODO implement the "HighlightSingleTile" then use it here
    private void HighlightEnclosedArea(Vector3Int selectedLocation)
    {
        EnclosedArea enclosedArea = enclosureSystem.GetEnclosedAreaByCellPosition(selectedLocation);
        foreach (EnclosedArea.Coordinate pos in enclosedArea.coordinates)
        {
            Vector3Int position = new Vector3Int(pos.x, pos.y, 0);
            lastTilesSelected.Add(position);
            this.highLight.SetTile(position, this.highLightTile);
        }
    }

    // TODO find a way to unhighlight the hightlighted tiles
    private void UnhighlightEnclosedArea(Vector3Int selectedLocation)
    {
        EnclosedArea enclosedArea = enclosureSystem.GetEnclosedAreaByCellPosition(selectedLocation);
        foreach (EnclosedArea.Coordinate pos in enclosedArea.coordinates)
        {
            Vector3Int position = new Vector3Int(pos.x, pos.y, 0);
            this.highLight.SetTile(position, null);
        }
    }

    // TODO check out "HighlightFoodSource" to see how to tile can be highlighted
    private void HighlightSingleTile(Vector3Int location)
    {
        this.highLight.SetTile(location, this.highLightTile);

        lastTilesSelected.Add(location);
    }

    private void UnHighlightSingleTile(Vector3Int location)
    {
        this.highLight.SetTile(location, null);
    }
}