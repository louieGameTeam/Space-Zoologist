using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingManager : MonoBehaviour
{
    [SerializeField] GridSystem gridSystem = default;
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

    public void StopSelling()
    {
        IsSelling = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        IsSelling = false;
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

            // Find the cell that the player clicked on
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = gridSystem.WorldToCell(worldPos);

            // What is on the tile?
            GameTile tile = gridSystem.GetGameTileAt(cellPos);
            GridSystem.TileData tileData;

            // Find out what is on the tile if it is in bounds
            if (gridSystem.isCellinGrid(cellPos.x, cellPos.y))
            {
                tileData = gridSystem.GetTileData(cellPos);
            }
            else
            {
                // Out of bounds, nothing to sell
                return;
            }


            // Only deleting 1 item on each click, so split into if/else. By priority:
            // 1. Sell the food
            if (tileData.Food)
            {
                SellFoodOnTile(tileData, cellPos);

            }
            // 2. Sell the wall
            else if (tile.type == TileType.Wall)
            {
                SellWallOnTile(cellPos);
            }
        }
    }

    private void SellFoodOnTile(GridSystem.TileData tileData, Vector3Int cellPos)
    {
        GameObject food = tileData.Food;
        string id = FindObjectOfType<FoodSourceManager>().GetSpeciesID(food.GetComponent<FoodSource>().Species);
        foreach (Item item in LevelDataReference.LevelData.Items)
        {
            if (item.ID.Equals(id))
            {
                PlayerBalance.SubtractFromBalance(-1 * item.Price);
                break;
            }
        }
        FindObjectOfType<FoodSourceManager>().DestroyFoodSource(food.GetComponent<FoodSource>());


        // Clean up CellData
        tileData.Food = null;
    }

    private void SellWallOnTile(Vector3Int cellPos)
    {
        //TODO work with TileSystem to delete the wall

    }
}
