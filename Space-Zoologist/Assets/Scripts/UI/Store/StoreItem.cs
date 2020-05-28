using UnityEngine.UI;
using UnityEngine;

public class StoreItem : MonoBehaviour
{
    public StoreItemSO ItemInformation { get; set; }

    public void InitializeStoreItem(StoreItemSO itemInfo)
    {
        this.ItemInformation = itemInfo;
        this.SetupStoreItemSmallDisplay();
    }

    private void SetupStoreItemSmallDisplay()
    {
        this.gameObject.GetComponent<Image>().sprite = this.ItemInformation.Sprite;
    }

    public GameObject SetupStoreItemExtendedDisplay(GameObject extendedDisplay, GameObject extendedDisplayView)
    {
        GameObject newItem = Instantiate(extendedDisplay, extendedDisplayView.transform);
        newItem.transform.position = new Vector2(extendedDisplayView.transform.position.x, this.gameObject.transform.position.y);
        newItem.transform.GetChild(0).GetComponent<Text>().text = this.ItemInformation.ItemName;
        newItem.transform.GetChild(1).GetComponent<Text>().text = "$" + this.ItemInformation.ItemCost.ToString();
        newItem.transform.GetChild(2).GetComponent<Text>().text = this.ItemInformation.StoreItemDescription;
        return newItem;
    }
}
