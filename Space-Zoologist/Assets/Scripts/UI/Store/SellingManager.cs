using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingManager : MonoBehaviour
{
    private TileDataController gridSystem = default;
    private Inspector Inspector = default;
    [SerializeField] MenuManager MenuManager = default;
    public bool IsSelling { get; private set; }


    public void OnToggleSell()
    {
        if (!IsSelling)
        {
            //GameManager.Instance.TryToPause("ToggleSell");
            // No one else should be allowed to close the inspector except the InspectorObjectiveUI script
            //if (Inspector.IsInInspectorMode)
            //{
            //    Inspector.CloseInspector();
            //}
            if (MenuManager.IsInStore)
            {
                MenuManager.SetStoreIsOn(false);
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
        gridSystem = GameManager.Instance.m_tileDataController;
        Inspector = GameManager.Instance.m_inspector;

        IsSelling = false;
        // stop selling when store opens
        EventManager.Instance.SubscribeToEvent(EventType.StoreToggled, () => { if ((bool) EventManager.Instance.EventData) this.IsSelling = false; });
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
            TileData tileData;

            // Find out what is on the tile if it is in bounds
            if (gridSystem.IsCellinGrid(cellPos.x, cellPos.y))
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

    private void SellFoodOnTile(TileData tileData, Vector3Int cellPos)
    {
        GameObject food = tileData.Food;
        ItemID id = food.GetComponent<FoodSource>().Species.ID;
        foreach (LevelData.ItemData data in GameManager.Instance.LevelData.ItemQuantities)
        {
            Item item = data.itemObject;
            if (item.ID.Equals(id))
            {
                // NOTE: selling should no longer give the player money - money is spent requesting items, not placing them
                // GameManager.Instance.SubtractFromBalance(-1 * item.Price);
                break;
            }
        }
        FindObjectOfType<FoodSourceManager>().DestroyFoodSource(food.GetComponent<FoodSource>());


        // Clean up CellData
        tileData.Food = null;
    }

    private void SellWallOnTile(Vector3Int cellPos)
    {

    }
}
