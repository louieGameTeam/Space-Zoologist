using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class ResourceRequestEditor : NotebookUIChild
{
    #region Public Properties
    public RectTransform RectTransform => rectTransform;
    public TMP_InputField QuantityInput => quantityInput;
    public ItemDropdown ItemRequestedDropdown => itemRequestedDropdown;
    public ResourceRequest Request
    {
        get => request;
        set
        {
            if (value != null)
            {
                request = value;
                UpdateUI();
            }
            else throw new System.NullReferenceException($"{nameof(ResourceRequestEditor)}: cannot set request to 'null'");
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the rect transform of this editor")]
    private RectTransform rectTransform = null;
    [SerializeField]
    [Tooltip("Reference to the dropdown that gets a research category")]
    private CategoryFilteredItemDropdown targetDropdown = null;
    [SerializeField]
    [Tooltip("Reference to the text that displays the need")]
    private TextMeshProUGUI needDisplay = null;
    [SerializeField]
    [Tooltip("Text input field that sets the quantity of resources to request")]
    private TMP_InputField quantityInput = null;
    [SerializeField]
    [Tooltip("Dropdown used to select the item name to request")]
    private CategoryFilteredItemDropdown itemRequestedDropdown = null;
    [SerializeField]
    [Tooltip("Button that deletes this request when clicked")]
    private Button clearButton = null;
    #endregion

    #region Private Fields
    // Resource request to edit
    private ResourceRequest request = new ResourceRequest();
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();
        // only generate dropdowns using level data's items
        var source = GameManager.Instance.LevelData?.ItemQuantities.Select(x => x.itemObject.ID).ToArray();
        // Setup each dropdown, using the level data items as the source
        targetDropdown.SetSource(source);
        itemRequestedDropdown.SetSource(source);
        targetDropdown.Setup(ItemRegistry.Category.Food, ItemRegistry.Category.Species);
        itemRequestedDropdown.Setup(ItemRegistry.Category.Food, ItemRegistry.Category.Tile);
        // Set private data
        ResetRequest();
        // Set the private data to the top of the dropdowns

        // Add listeners
        targetDropdown.OnItemSelected.AddListener(x => 
        {
            request.ItemAddressed = x;
            OnAnyRequestPropertyChanged();
        });
        quantityInput.onEndEdit.AddListener(x =>
        {
            if (!string.IsNullOrWhiteSpace(x)) request.QuantityRequested = int.Parse(x);
            OnAnyRequestPropertyChanged();
        });
        itemRequestedDropdown.OnItemSelected.AddListener(x => 
        {
            request.ItemRequested = x;
            OnAnyRequestPropertyChanged();
        });

        // Delete the request when the button is clicked
        clearButton.onClick.AddListener(ResetRequest);
    }
    public void ResetRequest()
    {
        request.ItemAddressed = targetDropdown.SelectedItem;
        request.ItemRequested = itemRequestedDropdown.SelectedItem;
        request.QuantityRequested = 1;
        UpdateUI();
    }

    public void UpdateUI()
    {
        targetDropdown.SetSelectedItem(request.ItemAddressed);
        needDisplay.text = request.NeedAddressed + " Need";
        quantityInput.text = request.QuantityRequested.ToString();
        itemRequestedDropdown.SetSelectedItem(request.ItemRequested);
    }
    #endregion

    #region Private Methods
    private void OnAnyRequestPropertyChanged()
    {
        if (request != null)
        {
            needDisplay.text = request.NeedAddressed + " Need";
        }
        else needDisplay.text = "<No Need Addressed>";
    }
    #endregion
}
