using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField] private GridOverlay gridOverlay = default;
    Item tempItem;

    GameObject objectToMove = null;
    bool movingAnimal = false;
    Vector3Int previousLocation = default; // previous grid location
    Vector3 initialPos;
    Vector3 curPos;

    private void Start()
    {
        tempItem = new Item();
    }

    // Update is called once per frame
    void Update()
    {
        if (reserveDraft.IsToggled)
        { //TODO? replace this with a mode in store that toggles drag & drop?

            if (Input.GetMouseButtonDown(0))
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
                    objectToMove = SelectGameObjectAtMousePosition();
                }
            }

            if (objectToMove != null)
            {
                GameObjectFollowMouse(objectToMove);
                HighlightGrid();
            }

            if (Input.GetMouseButtonUp(0) && objectToMove != null)
            {
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

                objectToMove = null;
            }
        }

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
        if (this.gridSystem.PlacementValidation.IsOnWall(gridLocation)) return;

        // Different position: need to repaint
        if (gridLocation.x != previousLocation.x || gridLocation.y != previousLocation.y)
        {
            previousLocation = gridLocation;
            gridOverlay.ClearColors();
            gridSystem.PlacementValidation.updateVisualPlacement(gridLocation, tempItem);
        }
    }

    private void TryPlaceAnimal(Vector3 worldPos, GameObject toMove)
    {
        Population population = toMove.GetComponent<Animal>().PopulationInfo;
        AnimalSpecies species = population.Species;

        bool valid = gridSystem.PlacementValidation.IsPodPlacementValid(worldPos, species);

        // placement is valid and population did not already reach here
        if (valid && !reservePartitionManager.CanAccess(population, worldPos) && gridSystem.PlacementValidation.IsPodPlacementValid(worldPos, species))
        {
            populationManager.UpdatePopulation(species,  worldPos);
            population.RemoveAnimal();
        }
        toMove.transform.position = initialPos;
    }

    private void TryPlaceFood(Vector3 worldPos, GameObject toMove)
    {
        FoodSource foodSource = toMove.GetComponent<FoodSource>();
        FoodSourceSpecies species = foodSource.Species;
        Vector3Int pos = this.tileSystem.WorldToCell(worldPos);
        bool valid = gridSystem.PlacementValidation.IsFoodPlacementValid(worldPos, species);

        if (valid)
        {
            removeOriginalFood(foodSource);
            placeFood(pos, species);
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