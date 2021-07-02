using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveObject : MonoBehaviour
{
    [SerializeField] ReserveDraft reserveDraft = default;
    [SerializeField] GridSystem gridSystem = default;
    [SerializeField] TileSystem tileSystem = default;
    [SerializeField] Camera referenceCamera = default;
    [SerializeField] PopulationManager populationManager = default;
    [SerializeField] FoodSourceManager foodSourceManager = default;
    [SerializeField] ReservePartitionManager reservePartitionManager = default;
    [SerializeField] CursorItem cursorItem = default;
    [SerializeField] PlayerBalance playerBalance = default;
    [SerializeField] GameObject MoveButtonPrefab = default;
    [SerializeField] GameObject DeleteButtonPrefab = default;

    Item tempItem;

    GameObject objectToMove = null;
    GameObject MoveButton = null;
    GameObject DeleteButton = null;
    bool movingAnimal = false;
    Vector3Int previousLocation = default; // previous grid location
    Vector3 initialPos;
    Vector3 curPos;
    bool moving;

    const float FixedCost = 0;
    const float CostPerUnitSizeAnimal = 10;
    const float CostPerUnitSizeFood = 10;

    private void Start()
    {
        tempItem = new Item();
        MoveButton = Instantiate(MoveButtonPrefab, this.transform);
        DeleteButton = Instantiate(DeleteButtonPrefab, this.transform);
        MoveButton.GetComponent<Button>().onClick.AddListener(StartMovement);
        DeleteButton.GetComponent<Button>().onClick.AddListener(RemoveSelectedGameObject);
        MoveButton.SetActive(false);
        DeleteButton.SetActive(false);
        Reset();
    }

    public void StartMovement() {
        moving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (reserveDraft.IsToggled)
        {
            if (!moving && Input.GetMouseButtonDown(0))
            {
                // currently has no cursorItem
                bool notPlacingItem = !cursorItem.IsOn;

                if (notPlacingItem)
                {

                    // Imported from Inspector.cs -- prevents selecting UI element
                    if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 5)
                    {
                        return;
                    }

                    // Select the food or animal at mouse position
                    GameObject SelectedObject = SelectGameObjectAtMousePosition();
                    if(SelectedObject != null)
                        objectToMove = SelectedObject;
                }
            }

            if (objectToMove != null)
            {
                Vector3 screenPos = referenceCamera.WorldToScreenPoint(objectToMove.transform.position);
                MoveButton.SetActive(true);
                DeleteButton.SetActive(true);
                MoveButton.transform.position = screenPos + new Vector3(-50, 100, 0);
                DeleteButton.transform.position = screenPos + new Vector3(50, 100, 0);
            }

            if (objectToMove != null && moving)
            {
                // Preview placement
                GameObjectFollowMouse(objectToMove);
                HighlightGrid();

                // If trying to place
                if (Input.GetMouseButtonDown(0))
                {
                    // Imported from Inspector.cs -- prevents selecting UI element
                    if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 5)
                    {
                        return;
                    }

                    // Update animal location reference
                    this.gridSystem.UpdateAnimalCellGrid();
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    worldPos.z = 0;


                    if (movingAnimal)
                    {
                        TryPlaceAnimal(worldPos, objectToMove);
                    }
                    else
                    {
                        TryPlaceFood(worldPos, objectToMove);
                    }

                    Reset();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (objectToMove != null) objectToMove.transform.position = initialPos;
                Reset();
            }
        }

    }

    private void Reset()
    {
        objectToMove = null;
        moving = false;
        MoveButton.SetActive(false);
        DeleteButton.SetActive(false);
        gridSystem.ClearColors();
    }

    private GameObject SelectGameObjectAtMousePosition()
    {
        // Update animal location reference
        this.gridSystem.UpdateAnimalCellGrid();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int pos = this.tileSystem.WorldToCell(worldPos);
        GridSystem.CellData cellData = getCellData(pos);
        GameObject toMove = null;

        if (cellData.OutOfBounds)
        {
            return null;
        }

        if (cellData.ContainsAnimal)
        {
            toMove = cellData.Animal;
            movingAnimal = true;
            string ID = toMove.GetComponent<Animal>().PopulationInfo.Species.SpeciesName;
            tempItem.SetupData(ID, "Pod", ID, 0);
        }
        else if (cellData.ContainsFood)
        {
            toMove = cellData.Food;
            movingAnimal = false;
            string ID = toMove.GetComponent<FoodSource>().Species.SpeciesName;
            tempItem.SetupData(ID, "Food", ID, 0);
        }

        if (toMove != null) initialPos = toMove.transform.position;
        return toMove;
    }

    public void RemoveSelectedGameObject()
    {
        if (movingAnimal)
            objectToMove.GetComponent<Animal>().PopulationInfo.RemoveAnimal(objectToMove);
        else
            removeOriginalFood(objectToMove.GetComponent<FoodSource>());

        Reset();
    }

    private void GameObjectFollowMouse(GameObject toMove)
    {
        float z = toMove.transform.position.z;
        curPos = referenceCamera.ScreenToWorldPoint(Input.mousePosition);
        curPos.z = z;
        toMove.transform.position = curPos;
    }

    private void HighlightGrid()
    {

        curPos = referenceCamera.ScreenToWorldPoint(Input.mousePosition);
        if (!gridSystem.IsWithinGridBounds(curPos)) return;

        Vector3Int gridLocation = gridSystem.Grid.WorldToCell(curPos);
        if (this.gridSystem.IsOnWall(gridLocation)) return;

        // Different position: need to repaint
        if (gridLocation.x != previousLocation.x || gridLocation.y != previousLocation.y)
        {
            previousLocation = gridLocation;
            gridSystem.ClearColors();
            gridSystem.updateVisualPlacement(gridLocation, tempItem);
        }
    }

    private void TryPlaceAnimal(Vector3 worldPos, GameObject toMove)
    {
        Population population = toMove.GetComponent<Animal>().PopulationInfo;
        AnimalSpecies species = population.Species;

        float cost = FixedCost + species.Size * CostPerUnitSizeAnimal;
        bool valid = gridSystem.IsPodPlacementValid(worldPos, species) && playerBalance.Balance >= cost;

        // placement is valid and population did not already reach here
        if (valid && !reservePartitionManager.CanAccess(population, worldPos) && gridSystem.IsPodPlacementValid(worldPos, species))
        {
            populationManager.UpdatePopulation(species,  worldPos);
            playerBalance.SubtractFromBalance(cost);
            population.RemoveAnimal(toMove);
        }
        toMove.transform.position = worldPos; // always place animal back because animal movement will be handled by pop manager
    }

    private void TryPlaceFood(Vector3 worldPos, GameObject toMove)
    {
        FoodSource foodSource = toMove.GetComponent<FoodSource>();
        FoodSourceSpecies species = foodSource.Species;
        Vector3Int pos = this.tileSystem.WorldToCell(worldPos);

        float cost = FixedCost + species.Size * CostPerUnitSizeFood;
        bool valid = gridSystem.IsFoodPlacementValid(worldPos, species) && playerBalance.Balance >= cost;

        if (valid)
        {
            removeOriginalFood(foodSource);
            placeFood(pos, species);
            playerBalance.SubtractFromBalance(cost);
        }
        else
        {
            toMove.transform.position = initialPos;
        }
    }

    public void placeFood(Vector3Int mouseGridPosition, FoodSourceSpecies species)
    {
        Vector3Int Temp = mouseGridPosition;
        Temp.x += 1;
        Temp.y += 1;
        if (species.Size % 2 == 1)
        {
            //size is odd: center it
            Vector3 FoodLocation = gridSystem.Grid.CellToWorld(mouseGridPosition); //equivalent since cell and world is 1:1, but in Vector3
            FoodLocation += Temp;
            FoodLocation /= 2f;

            GameObject Food = foodSourceManager.CreateFoodSource(species.SpeciesName, FoodLocation);

            gridSystem.AddFood(mouseGridPosition, species.Size, Food);
        }
        else
        {
            //size is even: place it at cross-center (position of tile)
            Vector3 FoodLocation = gridSystem.Grid.CellToWorld(Temp); //equivalent since cell and world is 1:1, but in Vector3
            GameObject Food = foodSourceManager.CreateFoodSource(species.SpeciesName, FoodLocation);

            gridSystem.AddFood(mouseGridPosition, species.Size, Food);
        }
    }

    public void removeOriginalFood(FoodSource foodSource)
    {
        Vector3Int FoodLocation = gridSystem.Grid.WorldToCell(initialPos);
        gridSystem.RemoveFood(FoodLocation);
        foodSourceManager.DestroyFoodSource(foodSource);
    }

    private GridSystem.CellData getCellData(Vector3Int cellPos)
    {
        GridSystem.CellData cellData = new GridSystem.CellData();
        // Handles index out of bound exception
        if (this.gridSystem.isCellinGrid(cellPos.x, cellPos.y))
        {
            cellData = this.gridSystem.CellGrid[cellPos.x, cellPos.y];
        }
        else
        {
            cellData.OutOfBounds = true;
        }
        return cellData;
    }
}