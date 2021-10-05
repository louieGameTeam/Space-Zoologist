using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class StoreItemCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item { get; private set; }

    [SerializeField] Image itemImage = default;
    [SerializeField] Image highlightImage = default;
    [SerializeField] TextMeshProUGUI ItemName = default;
    [SerializeField] Text RemainingAmountText = default;
    [SerializeField] Button RequestButton = default;
    public int RemainingAmount = -1;

    public delegate void ItemSelectedHandler(Item item);
    public event ItemSelectedHandler onSelected;

    public void Initialize(Item item, ItemSelectedHandler itemSelectedHandler)
    {
        this.item = item;
        this.itemImage.sprite = item.Icon;
        this.onSelected += itemSelectedHandler;
        this.ItemName.text = this.item.ItemID.Data.Name.Get(global::ItemName.Type.Colloquial);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onSelected.Invoke(item);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightImage.enabled = true;
        RemainingAmountText.color = Color.green;
    }
    public void Update()
    {
        this.RemainingAmountText.text = "" + this.RemainingAmount;

        if(RemainingAmount <= 0)
        {
            RemainingAmountText.rectTransform.offsetMin = new Vector2(0f, 20f);
            RequestButton.gameObject.SetActive(true);
        }
        else
        {
            RemainingAmountText.rectTransform.offsetMin = Vector2.zero;
            RequestButton.gameObject.SetActive(false);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        highlightImage.enabled = false;
        RemainingAmountText.color = Color.white;
    }
}
