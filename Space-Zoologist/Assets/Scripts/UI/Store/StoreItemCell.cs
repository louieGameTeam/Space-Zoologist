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
    [SerializeField] Button SellButton = default;
    [SerializeField] GameObject PriceRoot = default; // TODO: This seems deprecated, gut it
    [SerializeField] TextMeshProUGUI PriceText = default;
    public int RemainingAmount = -1;
    [SerializeField] Color RemainingAmountTextDefaultColor = Color.white;
    [SerializeField] Color RemainingAmountTextHighlightColor = Color.green;
    [SerializeField] Color RemainingAmountTextEmptyColor = Color.red;

    public delegate void ItemSelectedHandler(Item item);
    public event ItemSelectedHandler onSelected;

    private string levelName;

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

        if (!GameManager.Instance) { return; }

        // Disable sell button in tutorial
        levelName = GameManager.Instance.LevelData.Level.Name;
        if (levelName == "Tutorial") {
            SellButton.gameObject.SetActive (false);
            // Request button is disabled by default
        }

        RequestButton.onClick.AddListener(() =>
        {
            if (!GameManager.Instance) { return; }

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
        });

        SellButton.onClick.AddListener(() =>
        {
            if (!GameManager.Instance) { return; }
            // sell
            GameManager.Instance.m_menuManager.TrySellItem(item,1);
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

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightImage.enabled = false;
    }

    void Refresh() {
        this.RemainingAmountText.text = this.RemainingAmount.ToString();

        if (levelName != "Tutorial") {
            RequestButton.gameObject.SetActive (RemainingAmount <= 0 && !PriceRoot.activeInHierarchy && item.ID.Category != ItemRegistry.Category.Species);
            SellButton.gameObject.SetActive (RemainingAmount > 0 && !PriceRoot.activeInHierarchy && item.ID.Category != ItemRegistry.Category.Species);
        }

        if (RemainingAmount > 0) {
            if (highlightImage.enabled) {
                RemainingAmountText.color = RemainingAmountTextHighlightColor;
            } else {
                RemainingAmountText.color = RemainingAmountTextDefaultColor;
            }
        } else {
            RemainingAmountText.color = RemainingAmountTextEmptyColor;
        }
    }
    #endregion

    #region Monobehavior Methods
    // whyyyy is this happening on update ew ew no
    // TODO: make this happen every time something is placed or purchased
    public void Update () {
        Refresh ();
    }
    #endregion
}
