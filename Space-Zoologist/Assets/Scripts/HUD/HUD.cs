using UnityEngine;
using UnityEngine.UI;

// Listens to events and pulls the data it needs to display so lots of references 
public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject StoreDisplay = default;
    [SerializeField] GameObject StoreDisplayButton = default;
    [SerializeField] GameObject CloseDisplayButton = default;
    [SerializeField] PlayerController PlayerInfo = default;
    [SerializeField] Text PlayerFundsDisplay = default;
    private bool DisplayFunds = false;

    public void Start()
    {
        this.DisplayCostOfPurchase(0, this.PlayerInfo.PlayerFunds);
    }

    public void Update()
    {
        if (this.DisplayFunds)
        {
            if (this.PlayerInfo.ItemSelected != null)
            {
                this.PlayerInfo.ItemSelected.TryGetComponent(out SelectableItem itemSelected);
                float cost = itemSelected.ItemInfo.ItemCost * this.PlayerInfo.NumCopies;
                this.DisplayCostOfPurchase(cost, this.PlayerInfo.PlayerFunds);
            }
            else
            {
                this.DisplayCostOfPurchase(0, this.PlayerInfo.PlayerFunds);
            }
            // can setup item sprites to follow cursor when sprites are more finalized
        }
    }

    public void DisplayStore()
    {
        this.StoreDisplay.SetActive(true);
        this.CloseDisplayButton.SetActive(true);
        this.DisplayFunds = true;
    }

    public void CloseStoreDisplay()
    {
        this.StoreDisplay.SetActive(false);
        this.CloseDisplayButton.SetActive(false);
        this.StoreDisplayButton.SetActive(true);
    }

    public void ClosePlayerFundsDisplay()
    {
        this.DisplayFunds = false;
        this.DisplayCostOfPurchase(0, this.PlayerInfo.PlayerFunds);
    }

    public void DisplayCostOfPurchase(float totalCost, float playerFunds)
    {
        if (totalCost > 0)
        {
            // TODO: test if caching the GetComponent and changign it works
            this.PlayerFundsDisplay.text = "Funds: " + playerFunds +
            "(-" + totalCost + ")";
        }
        else
        {
            this.PlayerFundsDisplay.text = "Funds: " + playerFunds;
        }
    }
}
