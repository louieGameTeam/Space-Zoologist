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

    public GameObject SetupStoreItemExtendedDisplay(GameObject extendedDisplay, Transform extendedContentDisplay)
    {
        GameObject newItem = Instantiate(extendedDisplay, extendedContentDisplay);
        extendedDisplay.transform.GetChild(0).GetComponent<Text>().text = this.ItemInformation.ItemName;
        extendedDisplay.transform.GetChild(1).GetComponent<Text>().text = this.ItemInformation.ItemCost.ToString();
        extendedDisplay.transform.GetChild(2).GetComponent<Text>().text = this.ItemInformation.StoreItemDescription;
        extendedDisplay.transform.GetChild(3).GetComponent<Image>().sprite = this.ItemInformation.Sprite;
        return newItem;
    }
}
