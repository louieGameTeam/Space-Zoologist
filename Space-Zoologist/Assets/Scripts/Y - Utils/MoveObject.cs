using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveObject : MonoBehaviour
{
    private GridSystem gridSystem = default;
    private FoodSourceManager foodSourceManager = default;
    [SerializeField] CursorItem cursorItem = default;
    [SerializeField] GameObject MoveButtonPrefab = default;
    [SerializeField] GameObject DeleteButtonPrefab = default;
    private BuildBufferManager buildBufferManager = default;
    [SerializeField] FoodSourceStoreSection FoodSourceStoreSection = default;
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
        gridSystem = GameManager.Instance.m_gridSystem;
        foodSourceManager = GameManager.Instance.m_foodSourceManager;
        buildBufferManager = GameManager.Instance.m_buildBufferManager;

        tempItem = (Item)ScriptableObject.CreateInstance("Item");
        MoveButton = Instantiate(MoveButtonPrefab, this.transform);
        DeleteButton = Instantiate(DeleteButtonPrefab, this.transform);
        MoveButton.GetComponent<Button>().onClick.AddListener(StartMovement);
        DeleteButton.GetComponent<Button>().onClick.AddListener(RemoveSelectedGameObject);
        MoveButton.SetActive(false);
        DeleteButton.SetActive(false);
        Reset();
    }
    public void StartMovement()
    {
        moving = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (gridSystem.IsDrafting)
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
                    if (SelectedObject != null)
                        objectToMove = SelectedObject;
                }
            }

            if (objectToMove != null)
            {
                if (objectToMove.name == "tile")
                {
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(objectToMove.transform.position);
                    DeleteButton.SetActive(true);
                    DeleteButton.transform.position = screenPos + new Vector3(50, 100, 0);
                }
                else
                {
                    setMoveUI();
                }
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
        else if (DeleteButton.activeSelf)
        {
            Reset();
        }

    }

    private void Reset()
    {
        objectToMove = null;
        moving = false;
        MoveButton.SetActive(false);
        DeleteButton.SetActive(false);
        gridSystem.ClearHighlights();
    }

    private void setMoveUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(objectToMove.transform.position);
        MoveButton.SetActive(true);
        DeleteButton.SetActive(true);
        MoveButton.transform.position = screenPos + new Vector3(-50, 100, 0);
        DeleteButton.transform.position = screenPos + new Vector3(50, 100, 0);
    }

    private GameObject SelectGameObjectAtMousePosition()
    {
        // Update animal location reference
        this.gridSystem.UpdateAnimalCellGrid();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int pos = this.gridSystem.WorldToCell(worldPos);
        GridSystem.TileData tileData = gridSystem.GetTileData(pos);
        GameObject toMove = null;

        if (tileData == null)
        {
            return null;
        }

        if (tileData.Animal)
        {
            toMove = tileData.Animal;
            movingAnimal = true;
            string ID = toMove.GetComponent<Animal>().PopulationInfo.Species.SpeciesName;
            tempItem.SetupData(ID, "Pod", ID, 0);
        }
        else if (tileData.Food)
        {
            toMove = tileData.Food;
            movingAnimal = false;
            string ID = toMove.GetComponent<FoodSource>().Species.SpeciesName;
            tempItem.SetupData(ID, "Food", ID, 0);
        }
        else if (buildBufferManager.IsConstructing(pos.x, pos.y))
        {
            GameObject tileToDelete = new GameObject();
            tileToDelete.transform.position = pos;
            toMove = tileToDelete;
            tileToDelete.name = "tile";
        }

        if (toMove != null) initialPos = toMove.transform.position;
        return toMove;
    }

    public void RemoveSelectedGameObject()
    {
        if (movingAnimal)
        {
            objectToMove.GetComponent<Animal>().PopulationInfo.RemoveAnimal(objectToMove);
        }
        else
        {
            removeOriginalFood(objectToMove.GetComponent<FoodSource>());
        }
        Reset();
    }

    private void GameObjectFollowMouse(GameObject toMove)
    {
        float z = toMove.transform.position.z;
        curPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curPos.z = z;
        toMove.transform.position = curPos;
    }

    private void HighlightGrid()
    {
        curPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!gridSystem.IsWithinGridBounds(curPos)) return;

        Vector3Int gridLocation = gridSystem.WorldToCell(curPos);
        if (this.gridSystem.IsOnWall(gridLocation)) return;

        // Different position: need to repaint
        if (gridLocation.x != previousLocation.x || gridLocation.y != previousLocation.y)
        {
            previousLocation = gridLocation;
            gridSystem.ClearHighlights();
            gridSystem.updateVisualPlacement(gridLocation, tempItem);
        }
    }

    private void TryPlaceAnimal(Vector3 worldPos, GameObject toMove)
    {
        Population population = toMove.GetComponent<Animal>().PopulationInfo;
        AnimalSpecies species = population.Species;

        float cost = FixedCost + species.Size * CostPerUnitSizeAnimal;
        bool valid = gridSystem.IsPodPlacementValid(worldPos, species) && GameManager.Instance.Balance >= cost;

        // placement is valid and population did not already reach here
        if (valid && !GameManager.Instance.m_reservePartitionManager.CanAccess(population, worldPos) && gridSystem.IsPodPlacementValid(worldPos, species))
        {
            GameManager.Instance.m_populationManager.UpdatePopulation(species, worldPos);
            GameManager.Instance.SubtractFromBalance(cost);
            population.RemoveAnimal(toMove);
        }
        toMove.transform.position = worldPos; // always place animal back because animal movement will be handled by pop manager
    }

    private void TryPlaceFood(Vector3 worldPos, GameObject toMove)
    {
        FoodSource foodSource = toMove.GetComponent<FoodSource>();
        FoodSourceSpecies species = foodSource.Species;
        Vector3Int pos = this.gridSystem.WorldToCell(worldPos);

        float cost = FixedCost + species.Size * CostPerUnitSizeFood;
        bool valid = gridSystem.IsFoodPlacementValid(worldPos, null, species) && GameManager.Instance.Balance >= cost;

        if (valid)
        {
            /*            ConstructionCountdown cc = GetBufferCountDown(foodSource);
                        int ttb = 0;
                        if (cc != null)
                        {
                            ttb = cc.target;
                        }*/
            //foodSourceManager.PlaceFood(pos, species, ttb);
            removeOriginalFood(foodSource);
            placeFood(pos, species);
            GameManager.Instance.SubtractFromBalance(cost);
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
            Vector3 FoodLocation = gridSystem.CellToWorld(mouseGridPosition); //equivalent since cell and world is 1:1, but in Vector3
            FoodLocation += Temp;
            FoodLocation /= 2f;

            GameObject Food = foodSourceManager.CreateFoodSource(species.SpeciesName, FoodLocation);

            gridSystem.AddFood(mouseGridPosition, species.Size, Food);
            buildBufferManager.CreateSquareBuffer(new Vector2Int(mouseGridPosition.x, mouseGridPosition.y), this.GetStoreItem(species).buildTime, species.Size, foodSourceManager.constructionColor);
        }
        else
        {
            //size is even: place it at cross-center (position of tile)
            Vector3 FoodLocation = gridSystem.CellToWorld(Temp); //equivalent since cell and world is 1:1, but in Vector3
            GameObject Food = foodSourceManager.CreateFoodSource(species.SpeciesName, FoodLocation);

            gridSystem.AddFood(mouseGridPosition, species.Size, Food);
            buildBufferManager.CreateSquareBuffer(new Vector2Int(mouseGridPosition.x, mouseGridPosition.y), this.GetStoreItem(species).buildTime, species.Size, foodSourceManager.constructionColor);
        }
    }
    private Item GetStoreItem(FoodSourceSpecies foodSourceSpecies)
    {
        string itemID = "";
        foreach (KeyValuePair<string, FoodSourceSpecies> nameToFoodSpecies in GameManager.Instance.FoodSources)
        {
            if (nameToFoodSpecies.Value == foodSourceSpecies)
            {
                itemID = nameToFoodSpecies.Key;
            }
        }
        return this.FoodSourceStoreSection.GetItemByID(itemID);
    }
    public void removeOriginalFood(FoodSource foodSource)
    {
        Vector3Int FoodLocation = gridSystem.WorldToCell(initialPos);
        gridSystem.RemoveFood(FoodLocation);
        foodSourceManager.DestroyFoodSource(foodSource);
        int sizeShift = foodSource.Species.Size - 1; // Finds the lower left cell the food occupies
        Vector2Int shiftedPos = new Vector2Int(FoodLocation.x - sizeShift, FoodLocation.y - sizeShift);
        buildBufferManager.DestroyBuffer(shiftedPos, foodSource.Species.Size);
    }
}