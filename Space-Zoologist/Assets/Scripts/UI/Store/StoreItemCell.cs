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
    [SerializeField] GameObject PriceRoot = default;
    [SerializeField] TextMeshProUGUI PriceText = default;
    public int RemainingAmount = -1;

    public delegate void ItemSelectedHandler(Item item);
    public event ItemSelectedHandler onSelected;

    #region Public Methods
    public void Initialize(Item item, bool displayPrice, ItemSelectedHandler itemSelectedHandler)
    {
        this.item = item;
        this.itemImage.sprite = item.Icon;
        this.ItemName.text = this.item.ID.Data.Name.Get(global::ItemName.Type.Colloquial);

        // Display the price
        PriceRoot.SetActive(displayPrice);
        if (displayPrice) PriceText.text = item.Price.ToString();

        // Check if the selected handler is null. If not then add it to the event
        if(itemSelectedHandler != null) this.onSelected += itemSelectedHandler;

        RequestButton.onClick.AddListener(() =>
        { 
            if(GameManager.Instance)
            {
                // Reference the notebook ui
                NotebookUI notebookUI = GameManager.Instance.NotebookUI;
                // Tab picker reference
                NotebookTabPicker tabPicker = notebookUI.TabPicker;

                // Create the bookmark to navigate to
                Bookmark bookmark = new Bookmark(string.Empty, new BookmarkData(tabPicker.name, NotebookTab.Concepts));
                // Create a request to prefill in the notebook
                ResourceRequest request = new ResourceRequest()
                {
                    QuantityRequested = 1,
                    ItemRequested = item.ID
                };

                notebookUI.NavigateToBookmark(bookmark);
                notebookUI.FillResourceRequest(request);
            }
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onSelected?.Invoke(item);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightImage.enabled = true;
    }
    public void Update()
    {
        this.RemainingAmountText.text = "" + this.RemainingAmount;
        RequestButton.gameObject.SetActive(RemainingAmount <= 0 && !PriceRoot.activeInHierarchy && item.ID.Category != ItemRegistry.Category.Species);

        if (RemainingAmount > 0)
        {
            if (highlightImage.enabled) RemainingAmountText.color = Color.green;
            else RemainingAmountText.color = Color.white;
        }
        else RemainingAmountText.color = Color.red;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        highlightImage.enabled = false;
    }
    #endregion
}
