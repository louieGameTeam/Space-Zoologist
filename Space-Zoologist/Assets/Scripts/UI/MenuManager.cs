using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO refactor PodMenu to inherit same as other store menus
public class MenuManager : MonoBehaviour
{
    GameObject currentMenu = null;
    [SerializeField] GameObject PlayerBalanceHUD = default;
    [SerializeField] List<StoreSection> StoreMenus = default;
    // PodMenu had original different design so could refactor to align with store sections but works for now
    [SerializeField] PodMenu PodMenu = default;
    [SerializeField] PauseManager PauseManager = default;
    [SerializeField] GameObject PauseButton = default;
    [Header("Shared menu dependencies")]
    [SerializeField] PlayerBalance PlayerBalance = default;
    [SerializeField] CanvasObjectStrobe PlayerBalanceDisplay = default;
    [SerializeField] LevelDataReference LevelDataReference = default;
    [SerializeField] CursorItem CursorItem = default;
    [SerializeField] GridSystem GridSystem = default;
    [SerializeField] List<RectTransform> UIElements = default;
    public bool IsInStore { get; private set; }
    public bool IsSelling { get; private set; }

    public void Start()
    {
        this.IsInStore = false;
        foreach (StoreSection storeMenu in this.StoreMenus)
        {
            storeMenu.SetupDependencies(this.LevelDataReference, this.CursorItem, this.UIElements, this.GridSystem, this.PlayerBalance, this.PlayerBalanceDisplay);
            storeMenu.Initialize();
        }
        PodMenu.SetupDependencies(this.LevelDataReference, this.CursorItem, this.UIElements, this.GridSystem);
        PodMenu.Initialize();
        this.PlayerBalanceHUD.GetComponent<TopHUD>().SetupPlayerBalance(this.PlayerBalance);
    }

    public void OnToggleMenu(GameObject menu)
    {
        if (currentMenu != menu)
        {
            if (!this.IsInStore)
            {
                this.PauseManager.TryToPause();
                EventManager.Instance.InvokeEvent(EventType.StoreOpened, null);
            }
            this.StoreToggledOn(menu);
        }
        else
        {
            this.StoreToggledOff(menu);
        }
    }

    public void CloseStore()
    {
        this.StoreToggledOff(this.currentMenu);
    }

    private void StoreToggledOn(GameObject menu)
    {
        if (this.currentMenu)
        {
            this.currentMenu.SetActive(false);
        }
        menu.SetActive(true);
        currentMenu = menu;
        this.PlayerBalanceHUD.SetActive(true);
        this.IsInStore = true;
        this.PauseButton.SetActive(false);
    }

    private void StoreToggledOff(GameObject menu)
    {
        if (menu != null)
        {
            menu.SetActive(false);
            this.currentMenu = null;
            this.PlayerBalanceHUD.SetActive(false);
            this.IsInStore = false;
            this.PauseManager.TryToUnpause();
            this.PauseButton.SetActive(true);
        }

        EventManager.Instance.InvokeEvent(EventType.StoreClosed, null);
    }
    public void OnToggleSell() {
        IsSelling = !IsSelling;
        if (IsSelling) {
            Inspector inspector = FindObjectOfType<Inspector>();
            if (inspector.IsInInspectorMode) {
                inspector.ToggleInspectMode();
            }

        }
    }
    public void Update()
    {
        // Much taken from Inspector
        if (IsSelling && Input.GetMouseButtonDown(0)) {
            GridSystem gridSystem = FindObjectOfType<GridSystem>();
            TileSystem tileSystem = FindObjectOfType<TileSystem>();
            // Update animal locations
            gridSystem.UpdateAnimalCellGrid();

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = tileSystem.WorldToCell(worldPos);
            TerrainTile tile = tileSystem.GetTerrainTileAtLocation(cellPos);

            //Debug.Log($"Mouse click at {cellPos}");

            GridSystem.CellData cellData;

            // Handles index out of bound exception
            if (gridSystem.isCellinGrid(cellPos.x, cellPos.y))
            {
                cellData = gridSystem.CellGrid[cellPos.x, cellPos.y];
            }
            else
            {
                // Debug.Log($"Grid location selected was out of bounds @ {cellPos}");
                return;
            }

            if (cellData.ContainsMachine) {
                if (cellData.Machine.GetComponent<AtmosphereMachine>())
                {
                    foreach (Item item in LevelDataReference.LevelData.Items) {
                        if (item.ID.Equals("AtmosphereMachine")) {
                            PlayerBalance.SubtractFromBalance(-1 * item.Price);
                            break;
                        }
                    }
                }
                else if (cellData.Machine.GetComponent<LiquidMachine>()) {
                    foreach (Item item in LevelDataReference.LevelData.Items)
                    {
                        if (item.ID.Equals("LiquidMachine"))
                        {
                            PlayerBalance.SubtractFromBalance(-1 * item.Price);
                            break;
                        }
                    }
                }
                Destroy(cellData.Machine);
                cellData.ContainsMachine = false;
                cellData.Machine = null;
            }
            // Selection is food source or item
            else if (cellData.ContainsFood)
            {
                string id = FindObjectOfType<FoodSourceManager>().GetSpeciesID(cellData.Food.GetComponent<FoodSource>().Species);
                foreach (Item item in LevelDataReference.LevelData.Items)
                {
                    if (item.ID.Equals(id))
                    {
                        PlayerBalance.SubtractFromBalance(-1 * item.Price);
                        break;
                    }
                }
                FindObjectOfType<FoodSourceManager>().DestroyFoodSource(cellData.Food.GetComponent<FoodSource>());
                cellData.ContainsFood = false;
                cellData.Food = null;
            }
            // Selection is wall
            else if (tile.type == TileType.Wall)
            {

            }
        }
    }
}
