using UnityEngine.UI;
using UnityEngine;

public class StoreItemPopupDisplayLogic : MonoBehaviour
{
    [Header("Add children UI components")]
    [SerializeField] Text itemName = default;
    [SerializeField] Text itemCost = default;
    [SerializeField] Text itemDescription = default;

    /// <summary>
    /// Invoked on PointerEnter
    /// </summary>
    /// <param name="item"></param>
    public void SetupStoreItemDisplay(GameObject item)
    {
        StoreItemSO itemData = item.GetComponent<ItemData>().StoreItemData;
        this.itemName.text = itemData.ItemName;
        this.itemCost.text = "$" + itemData.ItemCost.ToString();
        this.itemDescription.text = itemData.StoreItemDescription;
        this.gameObject.SetActive(true);
    }

    public void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }
}
