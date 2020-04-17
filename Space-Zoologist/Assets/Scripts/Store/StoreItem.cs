using UnityEngine.UI;
using UnityEngine;

public class StoreItem : MonoBehaviour
{
    public StoreItemSO ItemInformation { get; set; }

    public void InitializeStoreItem(StoreItemSO itemInfo, GameObject storeItemPrefab)
    {
        this.ItemInformation = itemInfo;
        this.SetupStoreItemPrefab(storeItemPrefab);
    }

    public void SetupStoreItemPrefab(GameObject StoreItemPrefab)
    {
        ColorBlock cb = this.GetComponent<Button>().colors;
        cb.normalColor = this.ItemInformation.StoreItemCategory;
        this.GetComponent<Button>().colors = cb;
        this.transform.GetChild(0).GetComponent<Text>().text = this.ItemInformation.ItemName;
        this.transform.GetChild(1).GetComponent<Text>().text = this.ItemInformation.ItemCost.ToString();
        this.transform.GetChild(3).GetComponent<Image>().sprite = this.ItemInformation.Sprite;
    }
}
