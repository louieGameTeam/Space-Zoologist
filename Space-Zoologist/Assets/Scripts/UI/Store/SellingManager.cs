using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingManager : MonoBehaviour
{
    GridSystem gridSystem = default;
    TileSystem tileSystem = default;
    [SerializeField] PauseManager PauseManager = default;
    [SerializeField] MenuManager MenuManager = default;
    [SerializeField] Inspector Inspector = default;
    [SerializeField] PlayerBalance PlayerBalance = default;
    [SerializeField] LevelDataReference LevelDataReference = default;
    public bool IsSelling { get; private set; }

    public void OnToggleSell()
    {
        if (!IsSelling)
        {
            this.PauseManager.TryToPause();
            if (Inspector.IsInInspectorMode)
            {
                Inspector.CloseInspector();
            }
            if (MenuManager.IsInStore)
            {
                MenuManager.CloseStore();
            }
        }
        IsSelling = !IsSelling;
    }

    public void StopSelling() {
        IsSelling = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        IsSelling = false;
        gridSystem = FindObjectOfType<GridSystem>();
        tileSystem = FindObjectOfType<TileSystem>();
        // stop selling when store opens
        EventManager.Instance.SubscribeToEvent(EventType.StoreOpened, () =>
        {
            this.IsSelling = false;
        });
    }

    // Update is called once per frame
    void Update()
    {
        // Much taken from Inspector
        if (IsSelling && Input.GetMouseButtonDown(0))
        {
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

            if (cellData.ContainsMachine)
            {
                if (cellData.Machine.GetComponent<AtmosphereMachine>())
                {
                    foreach (Item item in LevelDataReference.LevelData.Items)
                    {
                        if (item.ID.Equals("AtmosphereMachine"))
                        {
                            PlayerBalance.SubtractFromBalance(-1 * item.Price);
                            break;
                        }
                    }
                }
                else if (cellData.Machine.GetComponent<LiquidMachine>())
                {
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
                GameObject food = cellData.Food;
                string id = FindObjectOfType<FoodSourceManager>().GetSpeciesID(food.GetComponent<FoodSource>().Species);
                foreach (Item item in LevelDataReference.LevelData.Items)
                {
                    if (item.ID.Equals(id))
                    {
                        PlayerBalance.SubtractFromBalance(-1 * item.Price);
                        break;
                    }
                }
                gridSystem.CellGrid[cellPos.x, cellPos.y].ContainsFood = false;
                gridSystem.CellGrid[cellPos.x, cellPos.y].Food = null;
                FindObjectOfType<FoodSourceManager>().DestroyFoodSource(food.GetComponent<FoodSource>());
            }
            // Selection is wall
            else if (tile.type == TileType.Wall)
            {

            }
        }
    }
}
