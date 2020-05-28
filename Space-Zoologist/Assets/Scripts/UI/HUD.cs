using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// TODO: if cursor over any HUD, prevent clicking through them
// Listens to events and pulls the data it needs to display so lots of references 
public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject StoreDisplay = default;
    [SerializeField] private ReserveStore StoreManager = default;
    [SerializeField] private PlayerInfo PlayerInfo = default;
    [SerializeField] private TilePlacementController tilePlacementController = default;
    [SerializeField] private GameObject PlayerFundsDisplay = default;
    private StoreItemSO ItemToBuy = default;

    public void Update()
    {
        if (this.StoreDisplay.activeSelf)
        {
            this.HandlePlayerHUD();
        }
    }

    private void HandlePlayerHUD()
    {
        // Can use numTilesPlaced to determine if purchase cancelled
            if (this.ItemToBuy != null)
            {
                float totalCost = 0;
                if (this.ItemToBuy.StoreItemCategory.Equals("Tiles"))
                {   
                    totalCost = this.ItemToBuy.ItemCost * this.tilePlacementController.PlacedTileCount();
                }
                else 
                {
                    totalCost = this.ItemToBuy.ItemCost * 1f;
                }
                this.UpdatePlayerFundsDisplay(totalCost);
            }
            else
            {
                this.UpdatePlayerFundsDisplay(0f);
            }
    }

    public void DisplaySelection()
    {
        string storeSelectionClicked = EventSystem.current.currentSelectedGameObject.name;
        this.StoreManager.SetupDisplay(storeSelectionClicked);
        this.UpdatePlayerFundsDisplay(totalCost: 0f);   
    }

    public void UpdateItemToBuy(GameObject itemSelected)
    {
        this.ItemToBuy = itemSelected.GetComponent<StoreItem>().ItemInformation;
    }

    private void UpdatePlayerFundsDisplay(float totalCost)
    {
        if (totalCost > 0 && this.ItemToBuy != null)
        {
            // TODO: test if caching the GetComponent and changign it works
            this.PlayerFundsDisplay.GetComponent<Text>().text = "$" + this.PlayerInfo.Funds +
            "(-" + totalCost + ")";
        }
        else
        {
            this.PlayerFundsDisplay.GetComponent<Text>().text = "$" + this.PlayerInfo.Funds;
        }
    }
}
