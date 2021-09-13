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
    int moveCost = 0;
    int sellBackCost = 0;

    const float FixedCost = 0;
    const float CostPerUnitSizeAnimal = 10;
    const float CostPerUnitSizeFood = 10;

    private void Start()
    {
        gridSystem = GameManager.Instance.m_gridSystem;
        foodSourceManager = GameManager.Instance.m_foodSourceManager;

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
        MoveButton.SetActive(false);
        DeleteButton.SetActive(false);
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
                    else if (DeleteButton.activeSelf)
                    {
                        // The UI is initialized: reset it
                        Reset();
                    }

                    // Select the food or animal at mouse position
                    GameObject SelectedObject = SelectGameObjectAtMousePosition();
                    if (SelectedObject != null)
                        objectToMove = SelectedObject;
                }
            }

            if (objectToMove != null && !moving)
            {
                // Initialize UI
                if (!DeleteButton.activeSelf)
                {
                    if (objectToMove.name == "tile")
                    {
                        Vector3 screenPos = Camera.main.WorldToScreenPoint(objectToMove.transform.position);
                        DeleteButton.GetComponentInChildren<Text>().text = $"${sellBackCost}";
                        DeleteButton.SetActive(true);
                        DeleteButton.transform.position = screenPos + new Vector3(50, 100, 0);
                    }
                    else
                    {
                        SetMoveUI();
                    }
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
        if (objectToMove?.name == "tile")
        {
            Destroy(objectToMove);
        }
        objectToMove = null;
        moving = false;
        MoveButton.SetActive(false);
        DeleteButton.SetActive(false);
        moveCost = 0;
        sellBackCost = 0;
    }

    // Set up UI for move and delete
    private void SetMoveUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(objectToMove.transform.position);
        MoveButton.SetActive(true);
        DeleteButton.SetActive(true);
        MoveButton.transform.position = screenPos + new Vector3(-50, 100, 0);
        DeleteButton.transform.position = screenPos + new Vector3(50, 100, 0);

        if (movingAnimal) {
            moveCost = objectToMove.GetComponent<Animal>().PopulationInfo.species.MoveCost;
            sellBackCost = 0;
        }
        else if (objectToMove.TryGetComponent<FoodSource>(out FoodSource foodSource))
        {
            FoodSourceSpecies species = foodSource.Species;
            moveCost = species.MoveCost;
            sellBackCost = species.SellBackPrice;
        }
        else if (objectToMove.CompareTag("tiletodelete"))
        {
            MoveButton.SetActive(false);

            LevelData.ItemData tileItemData = GameManager.Instance.LevelData.itemQuantities.Find(x => x.itemObject.ID.ToLower().Equals(objectToMove.name));
            sellBackCost = tileItemData.itemObject.Price;
        }
        MoveButton.GetComponentInChildren<Text>().text = $"${moveCost}";
        DeleteButton.GetComponentInChildren<Text>().text = $"${sellBackCost}";
    }

    // Find what the mouse clicked on
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
        else if (gridSystem.IsWithinGridBounds(pos))
        {
            if (gridSystem.IsConstructing(pos.x, pos.y))
            {
                GameObject tileToDelete = GameObject.FindGameObjectWithTag("tiletodelete");
                if (!tileToDelete)
                    tileToDelete = new GameObject();
                tileToDelete.tag = "tiletodelete";

                tileToDelete.transform.position = pos;
                toMove = tileToDelete;
                tileToDelete.name = gridSystem.GetGameTileAt(pos).TileName;
            }
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
        else if (objectToMove.TryGetComponent<FoodSource>(out FoodSource foodSource))
        {
            GameManager.Instance.SubtractFromBalance(-sellBackCost);
            removeOriginalFood(foodSource);
        }
        else if (objectToMove.CompareTag("tiletodelete"))
        {
            GridSystem.TileData tileData = gridSystem.GetTileData(gridSystem.WorldToCell(objectToMove.transform.position));
            tileData.Revert();
            gridSystem.ApplyChangesToTilemap(gridSystem.WorldToCell(objectToMove.transform.position));
            if (tileData.currentTile == null)
                tileData.Clear();
            //gridSystem.RemoveTile(gridSystem.WorldToCell(objectToMove.transform.position));
            gridSystem.RemoveBuffer((Vector2Int)gridSystem.WorldToCell(objectToMove.transform.position));
            GameManager.Instance.SubtractFromBalance(-sellBackCost);
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

        float cost = moveCost; //FixedCost + species.Size * CostPerUnitSizeAnimal;
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

        float cost = moveCost; // FixedCost + species.Size * CostPerUnitSizeFood;
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

    // placing food is more complicated due to grid
    public void placeFood(Vector3Int mouseGridPosition, FoodSourceSpecies species)
    {
        Vector3 FoodLocation = gridSystem.CellToWorld(mouseGridPosition); //equivalent since cell and world is 1:1, but in Vector3
        FoodLocation += new Vector3((float)species.Size.x / 2, (float)species.Size.y / 2, 0);

        GameObject Food = foodSourceManager.CreateFoodSource(species.SpeciesName, FoodLocation);

        gridSystem.AddFood(mouseGridPosition, species.Size, Food);
        gridSystem.CreateSquareBuffer(new Vector2Int(mouseGridPosition.x, mouseGridPosition.y), this.GetStoreItem(species).buildTime, species.Size,
        species.SpeciesName.Equals("Gold Space Maple") || species.SpeciesName.Equals("Space Maple") ? GridSystem.ConstructionCluster.ConstructionType.TREE : GridSystem.ConstructionCluster.ConstructionType.ONEFOOD);
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
        foodSourceManager.DestroyFoodSource(foodSource); // Finds the lower left cell the food occupies
        Vector2Int shiftedPos = new Vector2Int(FoodLocation.x, FoodLocation.y) - foodSource.Species.Size / 2;
        gridSystem.RemoveBuffer(shiftedPos, foodSource.Species.Size.x, foodSource.Species.Size.y);
    }
}
