using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using DG.Tweening;

public class ItemPicker : NotebookUIChild
{
    #region Public Typedefs
    [System.Serializable] public class ItemIDEvent : UnityEvent<ItemID> { }
    #endregion

    #region Public Properties
    public ItemID SelectedItem
    {
        get => selectedItem;
        set
        {
            // Set the research category on each dropdown (those that can't select this category ignore it)
            foreach(ItemDropdown dropdown in dropdowns)
            {
                dropdown.SetSelectedItem(value);
            }
        }
    }
    public ItemIDEvent OnItemPicked => onItemPicked;
    public bool HasBeenInitialized => selectedItem.Index >= 0;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the prefab used to select a research category")]
    private CategoryFilteredItemDropdown dropdown;
    [SerializeField]
    [Tooltip("Parent transform that the dropdowns are instantiated into")]
    private Transform dropdownParent;
    [SerializeField]
    [Tooltip("Sprite of the dropdown when it is selected")]
    private Sprite selectedSprite;
    [SerializeField]
    [Tooltip("Sprite of the dropdown when it is not selected")]
    private Sprite notSelectedSprite;
    [SerializeField]
    [Tooltip("Scale of the button when it is selected")]
    private float selectedScale = 0.2f;
    [SerializeField]
    [Tooltip("Time it takes for the button to grow/shrink when selected/deselected")]
    private float sizeChangeTime = 0.5f;
    [SerializeField]
    [Tooltip("Reference to the script targetted by the bookmarking system")]
    private BookmarkTarget bookmarkTarget;
    [SerializeField]
    [Tooltip("Event invoked when the research category picker changes category picked")]
    private ItemIDEvent onItemPicked;
    #endregion

    #region Private Fields
    // List of the dropdowns used by the category picker
    private List<ItemDropdown> dropdowns = new List<ItemDropdown>();
    // The item currently selected
    private ItemID selectedItem = new ItemID(ItemRegistry.Category.Species, -1);
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // Setup the bookmark target
        bookmarkTarget.Setup(() => SelectedItem, c => SelectedItem = (ItemID)c);

        ItemRegistry.Category[] categories = (ItemRegistry.Category[])System.Enum.GetValues(typeof(ItemRegistry.Category));

        // Instantiate a dropdown for each type and set it up
        foreach(ItemRegistry.Category category in categories)
        {
            CategoryFilteredItemDropdown clone = Instantiate(dropdown, dropdownParent);
            // Setup the clone's type and add a listener for the category change
            clone.Setup(category);
            clone.OnItemSelected.AddListener(ItemIDChanged);
            // Add to the list of our dropdowns
            dropdowns.Add(clone);
        }

        // Set the first value of the first dropdown
        // NOTE: this should invoke ResearchCategoryChanged immediately
        dropdowns[0].SetDropdownValue(0);
    }
    public ItemDropdown GetDropdown(ItemRegistry.Category category)
    {
        int index = (int)category;

        if (index >= 0 && index < dropdowns.Count) return dropdowns[index];
        else throw new System.IndexOutOfRangeException($"{nameof(ItemPicker)}: " +
            $"No item dropdown associated with category '{category}'. " +
            $"Total dropdowns: {dropdowns.Count}");
    }
    #endregion

    #region Private Methods
    private void ItemIDChanged(ItemID id)
    {
        selectedItem = id;

        // Set the sprites of the backgrounds of the dropdown images
        foreach(CategoryFilteredItemDropdown dropdown in dropdowns)
        {
            // Get this dropdown rect transform. Make sure it finishes any scale animations
            RectTransform dropdownRect = dropdown.GetComponent<RectTransform>();

            if (dropdown.CategoryFilter[0] == id.Category)
            {
                // Make this dropdown a little bigger
                dropdownRect.DOScale(1f + selectedScale, sizeChangeTime);
                // Set the correct sprite
                dropdown.Dropdown.image.sprite = selectedSprite;
            }
            else
            {
                // Make this dropdown the default size
                dropdownRect.DOScale(1f, sizeChangeTime);
                // Set the correct sprite
                dropdown.Dropdown.image.sprite = notSelectedSprite;
            }
        }

        onItemPicked.Invoke(id);
        UIParent.OnContentChanged.Invoke();
    }
    #endregion
}
