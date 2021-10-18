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

    #region Public Methods
    public void Initialize(Item item, ItemSelectedHandler itemSelectedHandler)
    {
        this.item = item;
        this.itemImage.sprite = item.Icon;
        this.onSelected += itemSelectedHandler;
        this.ItemName.text = this.item.ItemID.Data.Name.Get(global::ItemName.Type.Colloquial);

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
                    ItemRequested = item.ItemID
                };

                notebookUI.NavigateToBookmark(bookmark);
                notebookUI.FillResourceRequest(request);
            }
        });
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
    #endregion
}
