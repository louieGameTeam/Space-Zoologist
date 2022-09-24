using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// This script is to attached to the inspector button and handles
/// entering/exiting inspect mode, getting 
/// </summary>
public class Inspector : MonoBehaviour
{
    public bool IsInInspectorMode { get; private set; }
    public UnityEvent SelectionChangedEvent => selectionChangedEvent;

    // The inspector window
    [SerializeField] private GameObject inspectorWindow = null;
    [SerializeField] private DisplayInspectorText inspectorWindowDisplayScript = null;
    [SerializeField] private GameObject GrowthInfo = default;
    [SerializeField] private UnityEvent selectionChangedEvent = null;

    private GameObject lastFoodSourceSelected = null;
    private GameObject lastPopulationSelected = null;
    private List<Vector3Int> lastTilesSelected = new List<Vector3Int>();
    public GameObject PopulationHighlighted { get; private set; } = null;
    public Vector3Int selectedPosition { get; private set; }

    // per-frame mouse data
    private Vector3 mouseWorldPos;
    private Vector3Int mouseCellPos;
    private GameTile mouseTileData;
    private TileData mouseCellData;
    // cache target population/celldata
    private TileData currentlyInspectingCellData;
    private Population currentlyInspectingPopulationData;
    // hover
    private GameObject currentHoverTarget = null;

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
        EventManager.Instance.InvokeEvent(EventType.InspectorToggled, false);
        EventManager.Instance.UnsubscribeToEvent(EventType.FoodCacheRebuilt, UpdateCurrentDisplay);
        EventManager.Instance.UnsubscribeToEvent(EventType.PopulationCacheRebuilt, UpdateCurrentDisplay);
        this.IsInInspectorMode = false;
    }

    public void ResetSelection()
    {
        this.inspectorWindowDisplayScript.ClearInspectorWindow();
        this.UnHighlightAll();
        currentlyInspectingCellData = null;
        currentlyInspectingPopulationData = null;
    }

    // Referenced by the details button
    public void ToggleDetails()
    {
        this.GrowthInfo.SetActive(!this.GrowthInfo.activeSelf);
    }

    public void OpenInspector()
    {
        this.inspectorWindow.SetActive(true);
        GameManager.Instance.m_tileDataController.UpdateAnimalCellGrid();
        //this.HUD.SetActive(false);
        EventManager.Instance.InvokeEvent(EventType.InspectorToggled, true);
        EventManager.Instance.SubscribeToEvent(EventType.FoodCacheRebuilt, UpdateCurrentDisplay);
        EventManager.Instance.SubscribeToEvent(EventType.PopulationCacheRebuilt, UpdateCurrentDisplay);
        this.IsInInspectorMode = true;
    }

    /// <summary>
    /// Listens to mouse clicks and what was selected, then call functions to
    /// display info on inspector window
    /// </summary>
    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
            ResetSelection();
        if (this.IsInInspectorMode && FindObjectOfType<StoreSection>().SelectedItem == null)
        {
            UpdateMousePositionState();
            UpdateHoverSelection();
            if(Input.GetMouseButtonDown(0))
            {
                if(mouseCellData == null)
                {
                    if(!EventSystem.current.IsPointerOverGameObject()) 
                        ResetSelection();
                    else
                        UpdateCurrentDisplay();
                    return;
                }
                if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 5)
                {
                    return;
                }
                currentlyInspectingCellData = mouseCellData;
                this.UpdateInspectorValues();
            }
        }
        if (this.IsInInspectorMode)
        {
            // performance heavy so slower update interval
            if(Time.frameCount % 5 == 0)
                GameManager.Instance.m_tileDataController.UpdateAnimalCellGrid();
            if (this.PopulationHighlighted != null)
            {
                this.HighlightPopulation(this.PopulationHighlighted);
            }
        }
    }

    private void AttemptInspect()
    {

    }

    // TODO break this up and refactor
    public void UpdateInspectorValues()
    {
        // Update animal location reference
        GameManager.Instance.m_tileDataController.UpdateAnimalCellGrid();

        bool somethingSelected = true;
        this.UnHighlightAll();
        if (mouseCellData.Animal)
        {
            currentlyInspectingPopulationData = mouseCellData.Animal.transform.parent.GetComponent<Population>();
            DisplayPopulationText(currentlyInspectingPopulationData);
            selectedPosition = mouseCellPos;
            HighlightPopulation(currentlyInspectingPopulationData.gameObject);
        }
        // Selection is food source or item
        else if (mouseCellData.Food)
        {
            DisplayFoodText(mouseCellData);
            selectedPosition = mouseCellPos;
        }
        // Selection is liquid tile
        else if (mouseTileData && mouseTileData.type == TileType.Liquid)
        {
            DisplayLiquidText(mouseCellPos);
            selectedPosition = mouseCellPos;
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
            ResetSelection();
            somethingSelected = false;
        }
        if (somethingSelected)
        {
            AudioManager.instance.PlayOneShot(SFXType.Observation);
            EventManager.Instance.InvokeEvent(EventType.InspectorSelectionChanged, true);
            selectionChangedEvent.Invoke();
        }
    }

    private void UpdateMousePositionState()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseCellPos = GameManager.Instance.m_tileDataController.WorldToCell(mouseWorldPos);
        mouseCellData = GameManager.Instance.m_tileDataController.GetTileData(mouseCellPos);
        mouseTileData = GameManager.Instance.m_tileDataController.GetGameTileAt(mouseCellPos);
        if (EventSystem.current.IsPointerOverGameObject())
        {
            mouseCellData = null;
            mouseTileData = null;
        }
    }

    private void UpdateHoverSelection()
    {
        if (mouseCellData != null && (mouseCellData.Animal || mouseCellData.Food || (mouseTileData && mouseTileData.type == TileType.Liquid)))
        {
            EventManager.Instance.InvokeEvent(EventType.InspectorHoverTargetChange, mouseTileData);
        }
        else
        {
            EventManager.Instance.InvokeEvent(EventType.InspectorHoverTargetChange, null);
        }
    }

    public void UpdateCurrentDisplay()
    {
        TileData cellData = currentlyInspectingCellData;
        if (cellData == null) return;
        this.UnHighlightAll();
        switch (inspectorWindowDisplayScript.CurrentDisplay)
        {
            case DisplayInspectorText.InspectorText.Population:
                if (!currentlyInspectingPopulationData)
                {
                    return;
                }
                DisplayPopulationText(currentlyInspectingPopulationData);
                this.HighlightPopulation(currentlyInspectingPopulationData.gameObject);
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

    private void DisplayPopulationText(Population population)
    {
        // Get the animal's population info

        // Check to make sure the population manager still has this population in it
        if (GameManager.Instance.m_populationManager.Populations.Contains(population))
        {
            //Debug.Log($"Found animal {cellData.Animal.GetComponent<Animal>().PopulationInfo.Species.SpeciesName} @ {cellPos}");
            this.inspectorWindowDisplayScript.DisplayPopulationStatus(population);
        }
    }

    private void DisplayFoodText(TileData cellData)
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
                GameManager.Instance.m_tileDataController.HighlightRadius(foodPositionInt + new Vector3Int(x, y, 0), Color.blue, rootRadius);
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
        float[] compositions = new float[] { 0, 0, 0 };
        LiquidbodyController.Instance.GetLiquidContentsAt(cellPos, out compositions, out bool constructing);
        this.inspectorWindowDisplayScript.DisplayLiquidCompisition(compositions);
    }


    private void UnHighlightAll()
    {
        if ( this.lastFoodSourceSelected)
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

        GameManager.Instance.m_tileDataController.ClearHighlights();
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
        GameManager.Instance.m_tileDataController.HighlightTiles(accessibleTiles, Color.green);
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

    public void GetGameTileAndTileData(out GameTile gameTile, out TileData tileData)
    {
        // Update animal location reference
        GameManager.Instance.m_tileDataController.UpdateAnimalCellGrid();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int pos = GameManager.Instance.m_tileDataController.WorldToCell(worldPos);
        gameTile = GameManager.Instance.m_tileDataController.GetGameTileAt(pos);
        tileData = GameManager.Instance.m_tileDataController.GetTileData(pos);
    }
}
