using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Interacts with store systems to setup store display and player funds display
/// </summary>
public class StoreDisplayLogic : MonoBehaviour
{
    [SerializeField] private GameObject playerFundsDisplay = default;
    [SerializeField] private GameObject SellPopupDisplay = default;
    [SerializeField] private StoreManager StoreManager = default;
    [SerializeField] private PlayerInfo PlayerInfo = default;
    [SerializeField] private GameObject storeItemController = default;
    private StoreItemController itemController = default;
    private Text playerFundsDisplayText = default;
    public bool IsSelling {get; set;}

    public void Start()
    {
        this.IsSelling = false;
        this.playerFundsDisplayText = this.playerFundsDisplay.GetComponent<Text>();
        this.itemController = this.storeItemController.GetComponent<StoreItemController>();
    }

    public void Update()
    {
        if (this.gameObject.activeSelf)
        {
            this.HandlePlayerFundsDisplay();
        }
    }

    public void DisplayStore()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        this.storeItemController.SetActive(!this.storeItemController.activeSelf);
    }

    public void UpdateSelling()
    {
        this.IsSelling = !this.IsSelling;
        this.SellPopupDisplay.SetActive(false);
    }

    public void DoneSelling()
    {
        this.IsSelling = false;
        this.SellPopupDisplay.SetActive(false);
    }

    public void ActivateSellPopup(GameObject itemSelected) 
    {
        this.SellPopupDisplay.SetActive(true);
        this.SellPopupDisplay.GetComponent<SellPopupDisplayLogic>().UpdateObjectToSell(itemSelected);
    }

    public void HandlePlayerFundsDisplay()
    {
        if (this.IsSelling)
        {
            this.playerFundsDisplayText.text = "$" + this.PlayerInfo.Funds + " Selling!"; // + this.itemController.GetItemSoldInfo();
        }
        else
        {
            this.playerFundsDisplayText.text = "$" + this.PlayerInfo.Funds + this.itemController.GetItemPreviewingInfo();
        }
    }

    // Gets the name of the button clicked and then store manager compares to category of available items 
    public void SetupDisplay()
    {
        string storeSelection = EventSystem.current.currentSelectedGameObject.name;
        this.StoreManager.DisplaySelection(storeSelection);
    }
}
