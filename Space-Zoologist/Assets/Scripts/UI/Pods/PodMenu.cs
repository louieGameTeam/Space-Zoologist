using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PodMenu : MonoBehaviour, IStoreMenu
{
    [Header("Handled by Prefab")]
    [SerializeField] GameObject podButtonPrefab = default;
    [SerializeField] Transform PodItemContainer = default;
    [Header("Dependencies")]
    [SerializeField] PopulationManager populationManager = default;
    [SerializeField] TileSystem TileSystem = default;
    private CursorItem cursorItem = default;
    private LevelDataReference LevelDataReference = default;

    public List<GameObject> Pods { get; set; }
    private List<RectTransform> UIElements = default;
    AnimalSpecies selectedSpecies = null;

    public void SetupDependencies(LevelDataReference levelData, CursorItem cursorItem, List<RectTransform> UIElements)
    {
        this.LevelDataReference = levelData;
        this.cursorItem = cursorItem;
        this.UIElements = UIElements;
    }

    public void Initialize()
    {
        this.Pods = new List<GameObject>();
        foreach (AnimalSpecies species in LevelDataReference.LevelData.AnimalSpecies)
        {
            this.AddPod(species);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            DeselectSpecies();
        }
    }

    public void AddPod(AnimalSpecies species)
    {
        GameObject newPodItem = Instantiate(podButtonPrefab, PodItemContainer);
        PodItem podItem = newPodItem.GetComponent<PodItem>();
        this.Pods.Add(newPodItem);
        podItem.Initialize(species, this);
    }

    public void OnSelectSpecies(AnimalSpecies animalSpecies)
    {
        selectedSpecies = animalSpecies;
        cursorItem.Begin(animalSpecies.Icon, OnCursorItemClick);
    }

    public void DeselectSpecies()
    {
        selectedSpecies = null;
        if (cursorItem.isActiveAndEnabled)
        {
            cursorItem.Stop(OnCursorItemClick);
        }
    }

    private void OnDisable()
    {
        DeselectSpecies();
    }

    public void OnCursorItemClick(PointerEventData pointerEventData)
    {
        // If in CursorItem mode and the cursor is clicked while over the menu
        if (IsCursorOverUI(pointerEventData))
        {
            Debug.Log("Clicked over UI");
            DeselectSpecies();
        }
        else if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(pointerEventData.position);
            if (!this.IsPlacementValid(position))
            {
                Debug.Log("Cannot place item that location");
                return;
            }
            populationManager.AddAnimals(selectedSpecies, 1, position);
        }
    }

    public bool IsPlacementValid(Vector3 mouseWorldPosition)
    {
        if (mouseWorldPosition.x >= 0 && mouseWorldPosition.y >= 0
        && mouseWorldPosition.x <= LevelDataReference.MapWidth && mouseWorldPosition.y <= LevelDataReference.MapHeight)
        {
            Vector3Int mouseGridPosition = this.TileSystem.WorldToCell(mouseWorldPosition);
            TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(mouseGridPosition);
            foreach (TileType acceptablTerrain in this.selectedSpecies.AccessibleTerrain)
            {
                if (tile.type.Equals(acceptablTerrain))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsCursorOverUI(PointerEventData eventData)
    {
        foreach (RectTransform UIElement in this.UIElements)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(UIElement, eventData.position))
            {
                return true;
            }
        }
        return false;
    }
}
