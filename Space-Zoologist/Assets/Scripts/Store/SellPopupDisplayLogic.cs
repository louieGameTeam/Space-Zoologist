using UnityEngine.UI;
using UnityEngine;

public class SellPopupDisplayLogic : MonoBehaviour
{
    [Header("Add children UI components")]
    [SerializeField] Text itemName = default;
    [SerializeField] Text itemCost = default;
    [SerializeField] Text itemDescription = default;
    private GameObject ObjectToSell = default;

    public void UpdateObjectToSell(GameObject objectToSell)
    {
        this.ObjectToSell = objectToSell;
        StoreItemSO itemData = objectToSell.GetComponent<StoreItemData>().ItemData;
        this.itemName.text = itemData.ItemName;
        this.itemCost.text = "$" + (itemData.ItemCost/2).ToString();
        this.itemDescription.text = itemData.StoreItemDescription;
        this.SetPopupPosition(objectToSell);
        this.gameObject.SetActive(true);
    }

    private void SetPopupPosition(GameObject objectToSell)
    {
        this.gameObject.transform.position = new Vector2(objectToSell.transform.position.x, objectToSell.transform.position.y + 1.5f);
    }

    public void SellObject()
    {
        this.ObjectToSell.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void CancelSell()
    {
        this.gameObject.SetActive(false);
    }
}
