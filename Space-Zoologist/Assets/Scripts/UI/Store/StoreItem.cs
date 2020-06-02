using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoreItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public StoreItemSO item { get; private set; }

    [SerializeField] Image itemImage = default;
    [SerializeField] Image highlightImage = default;

    public void Initialize(StoreItemSO item)
    {
        this.item = item;
        this.itemImage.sprite = item.Sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(item.ItemName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightImage.enabled = false;
    }
}
