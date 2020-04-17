using UnityEngine;
using UnityEngine.UI;

// TODO: if cursor over any HUD, prevent clicking through them
// TODO: rework to be more flexible like how store display should be
// Listens to events and pulls the data it needs to display so lots of references 
public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject StoreDisplay = default;
    [SerializeField] private ReserveStore StoreManager = default;
    [SerializeField] private GameObject StoreDisplayButton = default;
    [SerializeField] private GameObject CloseDisplayButton = default;
    [SerializeField] private PlayerInformation PlayerInfo = default;
    [SerializeField] private TilePlacementController tilePlacementController = default;
    [SerializeField] private GameObject PlayerFundsDisplay = default;
    private StoreItemSO ItemToBuy = default;

    public void Update()
    {
        if (this.StoreDisplay.activeSelf)
        {
            // Can use numTilesPlaced to determine if purchase cancelled
            if (this.ItemToBuy != null)
            {
                float totalCost = this.ItemToBuy.ItemCost * 1f; // this.tilePlacementController.numTilesPlaced
                this.UpdatePlayerFundsDisplay(totalCost, this.PlayerInfo.PlayerFunds);
            }
            else
            {
                this.UpdatePlayerFundsDisplay(0f, this.PlayerInfo.PlayerFunds);
            }
        }
        else
        {
            this.PlayerFundsDisplay.SetActive(false);
        }
    }

    public void DisplayStore()
    {
        if (!this.StoreDisplay.activeSelf)
        {
            this.StoreDisplay.SetActive(true);
            this.CloseDisplayButton.SetActive(true);
            this.PlayerFundsDisplay.SetActive(true);
            this.UpdatePlayerFundsDisplay(totalCost: 0f, this.PlayerInfo.PlayerFunds);
        }
        else
        {
            // this.CloseSelection();
        }
    }

    public void CloseSelection()
    {
        this.StoreDisplay.SetActive(false);
        this.CloseDisplayButton.SetActive(false);
        this.StoreDisplayButton.SetActive(true);
        this.ItemToBuy = null;
    }

    public void UpdateItemToBuy(GameObject itemSelected)
    {
        this.ItemToBuy = itemSelected.GetComponent<StoreItem>().ItemInformation;
    }

    public void UpdatePlayerFundsDisplay(float totalCost, float playerFunds)
    {
        if (totalCost > 0 && totalCost < playerFunds)
        {
            // TODO: test if caching the GetComponent and changign it works
            this.PlayerFundsDisplay.GetComponent<Text>().text = "Funds: " + playerFunds +
            "(-" + totalCost + ")";
        }
        else
        {
            this.PlayerFundsDisplay.GetComponent<Text>().text = "Funds: " + playerFunds;
        }
    }
}
