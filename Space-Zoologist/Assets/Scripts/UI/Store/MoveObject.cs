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

    GameObject toMove = null;
    bool movingAnimal = false;
    Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (reserveDraft.IsToggled){
            if (Input.GetMouseButtonDown(0)) {

                // TODO check that we are not currently placing items
                bool notPlacingItem = true;
                if (notPlacingItem) {

                    // Imported from Inspector.cs -- prevents selecting UI element
                    if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 5)
                    {
                        return;
                    }


                    // Update animal location reference
                    this.gridSystem.UpdateAnimalCellGrid();
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3Int pos = this.tileSystem.WorldToCell(worldPos);
                    GridSystem.CellData cellData = getCellData(pos);


                    if (cellData.OutOfBounds)
                    {
                        return;
                    }


                    if (cellData.ContainsAnimal)
                    {
                        toMove = cellData.Animal;
                        movingAnimal = true;
                    }
                    else if (cellData.ContainsFood)
                    {
                        toMove = cellData.Food;
                        movingAnimal = false;
                    }
                    if (toMove != null) initialPos = toMove.transform.position;
                }
            }

            if (toMove != null) {
                float z = toMove.transform.position.z;
                Vector3 newPosition = referenceCamera.ScreenToWorldPoint(Input.mousePosition);
                newPosition.z = z;
                toMove.transform.position = newPosition;
            }

            if (Input.GetMouseButtonUp(0) && toMove != null)
            {
                // Update animal location reference
                this.gridSystem.UpdateAnimalCellGrid();
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int pos = this.tileSystem.WorldToCell(worldPos);


                if (movingAnimal)
                {
                    Population population = toMove.GetComponent<Animal>().PopulationInfo;
                    AnimalSpecies species = population.Species;

                    bool valid = gridSystem.PlacementValidation.IsPodPlacementValid(worldPos, species);

                    if (valid) {
                        populationManager.CreatePopulation(species, population.Count, worldPos);

                        // TODO Delete the existing population


                    }
                    else
                    {
                        toMove.transform.position = initialPos;
                    }
                }
                else
                {
                    FoodSource foodSource = toMove.GetComponent<FoodSource>();
                    FoodSourceSpecies species = foodSource.Species;
                    bool valid = gridSystem.PlacementValidation.IsFoodPlacementValid(worldPos, species);

                    print(valid);
                    if (valid)
                    {
                        placeFood(pos, species);
                        removeFood(foodSource);
                    }
                    else {
                        toMove.transform.position = initialPos;
                    }
                }

                toMove = null;
            }
        }

    }
    public void placeFood(Vector3Int mouseGridPosition, FoodSourceSpecies species)
    {
        Vector3Int pos;
        Vector2 mousePosition = new Vector2(mouseGridPosition.x, mouseGridPosition.y);
        //gridSystem.CellGrid[mouseGridPosition.x, mouseGridPosition.y].Food = FoodSourceManager.CreateFoodSource(selectedItem.ID, mousePosition);
        int radius = species.Size / 2;
        print(mouseGridPosition);

        // Offset
        Vector3Int Temp = mouseGridPosition;
        Temp.x += 1;
        Temp.y += 1;

        if (species.Size % 2 == 1)
        {
            //size is odd: center it
            Vector3 FoodLocation = gridSystem.Grid.CellToWorld(mouseGridPosition);
            FoodLocation += Temp;
            FoodLocation /= 2f;

            GameObject Food = foodSourceManager.CreateFoodSource(species.SpeciesName, FoodLocation);

            // Check if the whole object is in bounds
            for (int x = -1 * radius; x <= radius; x++)
            {
                for (int y = -1 * radius; y <= radius; y++)
                {
                    pos = mouseGridPosition;
                    pos.x += x;
                    pos.y += y;
                    gridSystem.CellGrid[pos.x, pos.y].ContainsFood = true;
                    gridSystem.CellGrid[pos.x, pos.y].Food = Food;
                }
            }

        }
        else
        {
            //size is even: place it at cross-center (position of tile)
            Vector3 FoodLocation = gridSystem.Grid.CellToWorld(Temp);
            GameObject Food = foodSourceManager.CreateFoodSource(species.SpeciesName, FoodLocation);

            // Check if the whole object is in bounds
            for (int x = -1 * (radius - 1); x <= radius; x++)
            {
                for (int y = -1 * (radius - 1); y <= radius; y++)
                {
                    pos = mouseGridPosition;
                    pos.x += x;
                    pos.y += y;
                    gridSystem.CellGrid[pos.x, pos.y].ContainsFood = true;
                    gridSystem.CellGrid[pos.x, pos.y].Food = Food;
                }
            }
        }
    }

    public void removeFood(FoodSource foodSource)
    {
        FoodSourceSpecies species = foodSource.Species;
        Vector3Int pos;
        Vector3Int foodSourceGridPos = tileSystem.WorldToCell(new Vector3(foodSource.Position.x, foodSource.Position.y));
        //gridSystem.CellGrid[mouseGridPosition.x, mouseGridPosition.y].Food = FoodSourceManager.CreateFoodSource(selectedItem.ID, mousePosition);
        int radius = species.Size / 2;
        print(foodSourceGridPos);

        if (species.Size % 2 == 1)
        {
            // Check if the whole object is in bounds
            for (int x = -1 * radius; x <= radius; x++)
            {
                for (int y = -1 * radius; y <= radius; y++)
                {
                    pos = foodSourceGridPos;
                    pos.x += x;
                    pos.y += y;
                    gridSystem.CellGrid[pos.x, pos.y].ContainsFood = false;
                    gridSystem.CellGrid[pos.x, pos.y].Food = null;
                    print(gridSystem.CellGrid[pos.x, pos.y].ContainsFood);
                }
            }

        }
        else
        {
            // Check if the whole object is in bounds
            for (int x = -1 * (radius - 1); x <= radius; x++)
            {
                for (int y = -1 * (radius - 1); y <= radius; y++)
                {
                    pos = foodSourceGridPos;
                    pos.x += x;
                    pos.y += y;
                    gridSystem.CellGrid[pos.x, pos.y].ContainsFood = false;
                    gridSystem.CellGrid[pos.x, pos.y].Food = null;
                }
            }
        }

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
