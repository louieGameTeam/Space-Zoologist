using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class MoveObject : MonoBehaviour
{
    public UnityEvent OnObjectMoved => onObjectMoved;
    private TileDataController gridSystem = default;
    private FoodSourceManager foodSourceManager = default;
    [SerializeField] UICursorInput cursorItem = default;
    [SerializeField] GameObject MoveButtonPrefab = default;
    [SerializeField] GameObject DeleteButtonPrefab = default;
    [SerializeField] FoodSourceStoreSection FoodSourceStoreSection = default;
    [SerializeField] TileStoreSection TileStoreSection = default;
    // do this better
    [SerializeField] Sprite LiquidSprite = default;

    Dictionary<ItemID, Item> itemByID = new Dictionary<ItemID, Item>();
    Item tempItem;
    private enum ItemType { NONE, FOOD, ANIMAL, TILE }

    // ONLY USED FOR TILES
    private Item tileToMoveItem;

    GameObject objectToMove = null;
    ItemPlaceCursorPreviewMover cursorPreviewMover = null;
    GameObject MoveButton = null;
    GameObject DeleteButton = null;
    private ItemType movingItemType;
    Vector3Int previousLocation = default; // previous grid location
    Vector3 initialPos;
    Vector3 curPos;
    bool moving;
    int moveCost = 0;
    int sellBackCost = 0;

    const float MoveCost = 0.25f;
    const float SellBackRefund = 0.5f;
    const float FixedCost = 0;
    const float CostPerUnitSizeAnimal = 10;
    const float CostPerUnitSizeFood = 10;

    private GameObject tileToDelete;
    private Vector3Int initialTilePosition;
    private GameTile initialTile;
    private float[] initialTileContents;
    private UnityEvent onObjectMoved = new UnityEvent();

    // audio
    private int currentAudioIndex;

    private void Start()
    {
        gridSystem = GameManager.Instance.m_tileDataController;
        foodSourceManager = GameManager.Instance.m_foodSourceManager;
        foreach (var itemData in ItemRegistry.GetAllItems()) {
            // Primarily checks for liquids, which may have the same id. Liquids are handled by a separate function
            if(!itemByID.ContainsKey(itemData.ShopItem.ID))
                itemByID.Add(itemData.ShopItem.ID, itemData.ShopItem);
        }

        MoveButton = Instantiate(MoveButtonPrefab, this.transform);
        DeleteButton = Instantiate(DeleteButtonPrefab, this.transform);
        MoveButton.GetComponent<Button>().onClick.AddListener(StartMovement);
        DeleteButton.GetComponent<Button>().onClick.AddListener(RemoveSelectedGameObject);
        MoveButton.SetActive(false);
        DeleteButton.SetActive(false);
        Reset();

        movingItemType = ItemType.NONE;

        tileToDelete = GameObject.FindGameObjectWithTag("tiletodelete");
        if (!tileToDelete)
        {
            tileToDelete = new GameObject();
            tileToDelete.AddComponent<SpriteRenderer>();
            tileToDelete.tag = "tiletodelete";
            tileToDelete.name = "Tile To Delete (Helper for MoveObject.cs)";
        }
    }

    public void StartMovement()
    {
        if (moveCost > GameManager.Instance.Balance)
            return;
        moving = true;
        MoveButton.SetActive(false);
        DeleteButton.SetActive(false);
        bool shouldSnap = false;
        Vector2Int size = Vector2Int.zero;
        Sprite sprite = tempItem.Icon;
        if (movingItemType == ItemType.TILE)
        {
            tileToDelete.SetActive(true);
            shouldSnap = true;
            size = new Vector2Int(1, 1);
        }            
        else if(movingItemType == ItemType.FOOD)
        {
            shouldSnap = true;
            size = objectToMove.GetComponent<FoodSource>().Species.Size;
        }
        cursorPreviewMover = new ItemPlaceCursorPreviewMover(gridSystem, sprite , size, shouldSnap);
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

                    // Select the food or animal at mouse position(Cannot select through UI)
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        GameObject SelectedObject = SelectGameObjectAtMousePosition();
                        if (SelectedObject != null)
                            objectToMove = SelectedObject;
                    }
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
                else {
                    UpdateMoveUIPosition();
                }
            }

            if (objectToMove != null && moving)
            {
                HighlightGrid();
                cursorPreviewMover.UpdatePosition();
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

                    bool successfullyMoved = true;
                    switch (movingItemType)
                    {
                        case ItemType.ANIMAL:
                            successfullyMoved = TryPlaceAnimal(worldPos, objectToMove);
                            break;
                        case ItemType.FOOD:
                            successfullyMoved = TryPlaceFood(worldPos, objectToMove);
                            break;
                        case ItemType.TILE:
                            TryPlaceTile(worldPos, objectToMove);
                            break;
                        default:
                            break;
                    }
                    if(successfullyMoved)
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

    protected virtual void HandleAudio(Item selectedItem)
    {
        if (selectedItem.AudioClips.Count == 0)
        {
            Debug.Log("Selected item " + selectedItem.ItemName + " has no audio sources!");
            return;
        }
        currentAudioIndex = (currentAudioIndex + 1) % selectedItem.AudioClips.Count;
        AudioManager.instance.PlayOneShot(selectedItem.AudioClips[currentAudioIndex]);
    }

    private void Reset()
    {
        if (objectToMove && objectToMove.name == "tile")
        {
            Destroy(objectToMove);
        }
        tileToDelete?.SetActive(false);
        objectToMove = null;
        moving = false;
        MoveButton.SetActive(false);
        DeleteButton.SetActive(false);
        moveCost = 0;
        sellBackCost = 0;
        gridSystem.ClearHighlights();
        cursorPreviewMover?.Clear();
        cursorPreviewMover = null;
    }
    // Set up UI for move and delete
    private void SetMoveUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(objectToMove.transform.position);
        MoveButton.SetActive(true);
        DeleteButton.SetActive(true);
        MoveButton.transform.position = screenPos + new Vector3(-50, 100, 0);
        DeleteButton.transform.position = screenPos + new Vector3(50, 100, 0);

        switch (movingItemType)
        {
            case ItemType.ANIMAL:
                int price = itemByID[objectToMove.GetComponent<Animal>().PopulationInfo.species.ID].Price;
                moveCost = Mathf.RoundToInt(MoveCost * price);
                sellBackCost = Mathf.RoundToInt(SellBackRefund * price);
                break;
            case ItemType.FOOD:
                FoodSourceSpecies species = objectToMove.GetComponent<FoodSource>().Species;
                price = itemByID[species.ID].Price;
                moveCost = Mathf.RoundToInt(MoveCost * price);
                sellBackCost = Mathf.RoundToInt(SellBackRefund * price);
                break;
            case ItemType.TILE:
                sellBackCost = Mathf.RoundToInt(SellBackRefund * tileToMoveItem.Price);
                break;
            default:
                break;
        }
        MoveButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"-${moveCost}";
        DeleteButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"+${sellBackCost}";
    }

    private void UpdateMoveUIPosition()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(objectToMove.transform.position);
        MoveButton.transform.position = screenPos + new Vector3(-50, 100, 0);
        DeleteButton.transform.position = screenPos + new Vector3(50, 100, 0);
    }

    private GameObject SelectGameObjectAtMousePosition()
    {
        // Update animal location reference
        this.gridSystem.UpdateAnimalCellGrid();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int pos = this.gridSystem.WorldToCell(worldPos);
        return SelectGameObjectAtGridPosition(pos);
    }

    // Find what the mouse clicked on
    private GameObject SelectGameObjectAtGridPosition(Vector3Int pos)
    {
        TileData tileData = gridSystem.GetTileData(pos);
        GameObject toMove = null;
        if (tileData == null)
        {
            return null;
        }

        if (tileData.Animal)
        {
            toMove = tileData.Animal;
            movingItemType = ItemType.ANIMAL;
            tempItem = toMove.GetComponent<Animal>().PopulationInfo.Species.ID.Data.ShopItem;
        }
        else if (tileData.Food)
        {
            toMove = tileData.Food;
            movingItemType = ItemType.FOOD;
            tempItem = toMove.GetComponent<FoodSource>().Species.ID.Data.ShopItem;
        }
        else if (gridSystem.IsWithinGridBounds(pos))
        {
            if (gridSystem.IsConstructing(pos.x, pos.y))
            {
                var tile = gridSystem.GetGameTileAt(pos);
                tileToDelete.name = tile.TileName;
                if (tileToDelete.name.Equals("liquid"))
                {
                    tileToDelete.GetComponent<SpriteRenderer>().sprite = LiquidSprite;
                    initialTileContents = new float[] { 0, 0, 0 };
                    LiquidbodyController.Instance.GetLiquidContentsAt(pos, out initialTileContents, out bool constructing);
                }
                else
                {
                    tileToDelete.GetComponent<SpriteRenderer>().sprite = gridSystem.GetTileItemData(tile.type).ShopItem.Icon;
                }

                movingItemType = ItemType.TILE;
                tileToDelete.transform.position = (Vector3)pos + new Vector3(0.5f, 0.5f, 0);
                toMove = tileToDelete;
                string tileName = tileToDelete.name;
                tempItem = ItemRegistry.Get(ItemRegistry.FindTile(gridSystem.GetGameTileAt(pos).type)).ShopItem;
                tempItem.SetupData(tileName, 0);
                tileToMoveItem = tempItem;

                initialTilePosition = pos;
                initialTile = gridSystem.GetTileData(pos).currentTile;
                initialTile.defaultContents = initialTileContents;
            }
        }

        if (toMove != null) initialPos = toMove.transform.position;
        return toMove;
    }

    public void RemoveSelectedGameObject()
    {
        switch (movingItemType)
        {
            case ItemType.ANIMAL:
                GameManager.Instance.AddToBalance(sellBackCost);
                objectToMove.GetComponent<Animal>().PopulationInfo.RemoveAnimal(objectToMove);
                break;
            case ItemType.FOOD:
                GameManager.Instance.AddToBalance(sellBackCost);
                FoodSource foodSource = objectToMove.GetComponent<FoodSource>();
                removeOriginalFood(foodSource);

	            // Selling items no longer return them to inventory
	            //Item foodItem = GameManager.Instance.LevelData.itemQuantities.Find(x => x.itemObject.ID.Equals(foodSource.Species.SpeciesName)).itemObject;

	            //if (!foodItem)
	            //    return;

	            //FoodSourceStoreSection.AddItemQuantity(foodItem);
                break;
            case ItemType.TILE:
                if (initialTile.type == TileType.Liquid)
                {
                    LiquidbodyController.Instance.RemoveConstructingLiquidContent(gridSystem.WorldToCell(objectToMove.transform.position));
                }


                GameManager.Instance.AddToBalance(sellBackCost);
                TileData tileData = gridSystem.GetTileData(gridSystem.WorldToCell(objectToMove.transform.position));
                tileData.Revert();
                gridSystem.ApplyChangeToTilemapTexture(gridSystem.WorldToCell(objectToMove.transform.position));
                if (tileData.currentTile == null)
                    tileData.Clear();
                //gridSystem.RemoveTile(gridSystem.WorldToCell(objectToMove.transform.position));
                gridSystem.RemoveBuffer((Vector2Int)gridSystem.WorldToCell(objectToMove.transform.position));

                tileToDelete.SetActive(false);
                // Selling items no longer return them to inventory
                //LevelData.ItemData tileItemData = GameManager.Instance.LevelData.itemQuantities.Find(x => x.itemObject.ID.ToLower().Equals(objectToMove.name));

                //if (tileItemData == null)
                //    return;

                //TileStoreSection.AddItemQuantity(tileItemData.itemObject);
                break;
            default:
                break;
        }
        Reset();
    }

    private void HighlightGrid()
    {
        curPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);       
        Vector3Int gridLocation = gridSystem.WorldToCell(curPos);
        if (!gridSystem.IsWithinGridBounds(curPos))
        {
            previousLocation = gridLocation;
            gridSystem.ClearHighlights();
            return;
        }
        //if (this.gridSystem.IsOnWall(gridLocation)) return;

        // Different position: need to repaint
        if (gridLocation.x != previousLocation.x || gridLocation.y != previousLocation.y)
        {
            previousLocation = gridLocation;
            gridSystem.ClearHighlights();
            gridSystem.updateVisualPlacement(gridLocation, tempItem);
        }
    }

    private bool TryPlaceAnimal(Vector3 worldPos, GameObject toMove)
    {
        Population population = toMove.GetComponent<Animal>().PopulationInfo;
        AnimalSpecies species = population.Species;

        float cost = moveCost;
        // Move is valid if the pod placement is valid...
        bool valid = gridSystem.IsPodPlacementValid(worldPos, species) &&
            // ...the player has money to move the animal...
            GameManager.Instance.Balance >= cost; /*&&
            // ...and the animal can't already access the terrian here (is this condition necessary?)
            !GameManager.Instance.m_reservePartitionManager.CanAccess(population, worldPos);*/

        // placement is valid and population did not already reach here
        if (!valid)
        {
            return false;
        }
        
        toMove.transform.position = worldPos;
        GameManager.Instance.m_populationManager.SpawnAnimal(species, worldPos);
        GameManager.Instance.SubtractFromBalance(cost);
        HandleAudio(species.AnimalShopItem);
        population.RemoveAnimal(toMove);
        onObjectMoved.Invoke();
        return true;
    }

    private bool TryPlaceFood(Vector3 worldPos, GameObject toMove)
    {
        FoodSource foodSource = toMove.GetComponent<FoodSource>();
        FoodSourceSpecies species = foodSource.Species;
        Vector3Int pos = this.gridSystem.WorldToCell(worldPos);
        Vector3Int sizeOffset = new Vector3Int(foodSource.Species.Size.x / 2, foodSource.Species.Size.y / 2, 0);
        Vector3Int initialGridPos = gridSystem.WorldToCell(initialPos) - sizeOffset;
        //If the player clicks on the food source's original position, don't bother with the mess below
        if (pos == initialGridPos)
        {
            return false;
        }
        //Check if the food source is under construction and if so, grab its build progress
        TileDataController.ConstructionCluster cluster = this.gridSystem.GetConstructionClusterAtPosition(initialGridPos);
        int buildProgress = this.GetStoreItem(species).buildTime;
        if(cluster != null)
            buildProgress = cluster.currentDays;

        //Check for placement validity using the initial position as a blind spot
        float cost = moveCost;
        bool valid = gridSystem.IsFoodPlacementValid(worldPos, initialGridPos, null, species) && GameManager.Instance.Balance >= cost;
        if (valid) //If valid, place the food at the mouse destination and remove the previous food
        {
            removeOriginalFood(foodSource);
            placeFood(pos, species);
            GameManager.Instance.SubtractFromBalance(cost);
            HandleAudio(foodSource.Species.FoodSourceItem);
            onObjectMoved.Invoke();
        }
        else //Otherwise ignore the placement command entirely
        {
            return false;
        }
        return true;
    }

    private void TryPlaceTile(Vector3 worldPos, GameObject toMove)
    {
        Vector3Int tilePos = gridSystem.WorldToCell(worldPos);
        var currentGameTile = gridSystem.GetTileData(tilePos)?.currentTile;
        if (gridSystem.IsTilePlacementValid (tilePos, currentGameTile, initialTile.type))
        {
            // undo current progress on existing tile
            gridSystem.GetTileData(initialTilePosition).Revert();
            gridSystem.RemoveBuffer((Vector2Int)initialTilePosition);
            gridSystem.ApplyChangeToTilemapTexture(initialTilePosition);
            
            tileToDelete.SetActive(false);

            // add the new tile
            gridSystem.SetTile(tilePos, initialTile);
            gridSystem.CreateUnitBuffer((Vector2Int)tilePos, 1, TileDataController.ConstructionCluster.ConstructionType.TILE);
            gridSystem.ApplyChangeToTilemapTexture(tilePos);
            var tileItem = gridSystem.GetTileItemData(initialTile.type);
            HandleAudio(tileItem.ShopItem);
        }
    }

    // placing food is more complicated due to grid
    public void placeFood(Vector3Int mouseGridPosition, FoodSourceSpecies species, int buildProgress = 0)
    {
        Vector3 FoodLocation = gridSystem.CellToWorld(mouseGridPosition); //equivalent since cell and world is 1:1, but in Vector3
        FoodLocation += new Vector3((float)species.Size.x / 2, (float)species.Size.y / 2, 0);

        GameObject Food = foodSourceManager.CreateFoodSource(species, FoodLocation);
        FoodSource foodSource = Food.GetComponent<FoodSource>();

        gridSystem.AddFoodReferenceToTile(mouseGridPosition, species.Size, Food);
        
        if(buildProgress < this.GetStoreItem(species).buildTime) //If the food source has yet to fully construct, add a build buffer
        {
            foodSource.isUnderConstruction = true;
            GameManager.Instance.m_tileDataController.ConstructionFinishedCallback(() =>
            {
                foodSource.isUnderConstruction = false;
            });

            // Determine the construction type based on if this is a tree or just one food
            TileDataController.ConstructionCluster.ConstructionType constructionType;

            if (species.ID.Data.Name.Get(ItemName.Type.Serialized).Equals("Gold Space Maple") ||
                species.ID.Data.Name.Get(ItemName.Type.Serialized).Equals("Space Maple"))
            {
                constructionType = TileDataController.ConstructionCluster.ConstructionType.TREE;
            }
            else constructionType = TileDataController.ConstructionCluster.ConstructionType.ONEFOOD;

            // Create a rectangle buffer for the given construction type
            gridSystem.CreateRectangleBuffer(
                new Vector2Int(mouseGridPosition.x, mouseGridPosition.y), 
                this.GetStoreItem(species).buildTime, species.Size,
                constructionType, buildProgress);
        }
        else //Otherwise, make sure its needs are up to date
        {
            // Note: This is now handled by the NeedCache
            // We may want to change this to rebuild the cache for only the food source that was moved
/*            GameManager.Instance.Needs.Rebuild();
            GameManager.Instance.m_inspector.UpdateCurrentDisplay();*/
        }

    }
    private Item GetStoreItem(FoodSourceSpecies foodSourceSpecies)
    {
        return this.FoodSourceStoreSection.GetItemByID(foodSourceSpecies.ID);
    }
    public void removeOriginalFood(FoodSource foodSource)
    {
        Vector3Int FoodLocation = gridSystem.WorldToCell(initialPos);
        TileData data = gridSystem.GetTileData(FoodLocation);
        gridSystem.RemoveFoodReference(FoodLocation);
        foodSourceManager.DestroyFoodSource(foodSource); // Finds the lower left cell the food occupies
        Vector2Int shiftedPos = new Vector2Int(FoodLocation.x, FoodLocation.y) - foodSource.Species.Size / 2;
        gridSystem.RemoveBuffer(shiftedPos, foodSource.Species.Size.x, foodSource.Species.Size.y);
    }
}
