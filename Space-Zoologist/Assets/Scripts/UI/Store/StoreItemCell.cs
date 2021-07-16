using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoreItemCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item { get; private set; }

    [SerializeField] Image itemImage = default;
    [SerializeField] Image highlightImage = default;
    [SerializeField] Text ItemName = default;
    [SerializeField] Text RemainingAmountText = default;
    [SerializeField] Text Cost = default;
    public int RemainingAmount = -1;

    public delegate void ItemSelectedHandler(Item item);
    public event ItemSelectedHandler onSelected;

    public void Initialize(Item item, ItemSelectedHandler itemSelectedHandler)
    {
        this.item = item;
        this.itemImage.sprite = item.Icon;
        this.onSelected += itemSelectedHandler;
        this.ItemName.text = this.item.ItemName;
        this.Cost.text = ""+this.item.Price;

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

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightImage.enabled = false;
        RemainingAmountText.color = Color.white;
    }
}
