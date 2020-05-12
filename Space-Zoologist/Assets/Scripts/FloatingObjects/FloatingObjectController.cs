using UnityEngine;

public class FloatingObjectController : MonoBehaviour, IHandler
{
    [SerializeField] private StoreDisplayLogic storeDisplayLogic = default;
    [SerializeField] private PlayerInfo playerInfo = default;
    private GameObject ItemSelected = default;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.storeDisplayLogic.DoneSelling();
        }
    }

    public void OnItemSelectedEvent(GameObject itemSelected)
    {
        this.ItemSelected = itemSelected;
        if (this.storeDisplayLogic.IsSelling)
        {   
            this.storeDisplayLogic.ActivateSellPopup(itemSelected);
        }
    }
    
    public void ItemSold()
    {
        this.playerInfo.Funds += this.ItemSelected.GetComponent<StoreItemData>().ItemData.ItemCost/2;
    }
}
