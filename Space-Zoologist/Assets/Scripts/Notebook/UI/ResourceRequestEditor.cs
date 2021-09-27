using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class ResourceRequestEditor : NotebookUIChild
{
    #region Public Properties
    public RectTransform RectTransform => rectTransform;
    public ResourceRequest Request => request;
    public UnityEvent OnNewRequestCreated => onNewRequestCreated;
    public UnityEvent OnPriorityUpdate => onPriorityUpdated;
    public UnityEvent OnRequestDeleted => onRequestDeleted;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the rect transform of this editor")]
    private RectTransform rectTransform;
    [SerializeField]
    [Tooltip("Reference to the canvas group used to adjust the alpha transparency and interactability of the full editor")]
    private CanvasGroup group;
    [SerializeField]
    [Tooltip("UI element used to display the status of the request")]
    private ResourceRequestStatusUI statusUI;

    [Space]

    [SerializeField]
    [Tooltip("Text input field that sets the priority of the request editor")]
    private TMP_InputField priorityInput;
    [SerializeField]
    [Tooltip("Reference to the dropdown that gets a research category")]
    private CategoryFilteredItemDropdown targetDropdown;
    [SerializeField]
    [Tooltip("Reference to the dropdown that gets the need")]
    private NeedTypeDropdown needDropdown;
    [SerializeField]
    [Tooltip("Text input field that sets the quantity of resources to request")]
    private TMP_InputField quantityInput;
    [SerializeField]
    [Tooltip("Dropdown used to select the item name to request")]
    private CategoryFilteredItemDropdown itemRequestedDropdown;

    [Space]

    [SerializeField]
    [Tooltip("Button that deletes this request when clicked")]
    private Button deleteButton;
    
    [Space]

    [SerializeField]
    [Tooltip("Event invoked when the editor creates a new request")]
    private UnityEvent onNewRequestCreated;
    [SerializeField]
    [Tooltip("Event invoked whenever the priority of the request changes")]
    private UnityEvent onPriorityUpdated;
    [SerializeField]
    [Tooltip("Event invoked when this request is deleted")]
    private UnityEvent onRequestDeleted;
    #endregion

    #region Private Fields
    // ID of the request to edit
    private EnclosureID enclosureID;
    // Resource request to edit
    private ResourceRequest request;
    #endregion

    #region Public Methods
    public void Setup(EnclosureID enclosureID, ResourceRequest request, ScrollRect scrollTarget, UnityAction priorityUpdatedCallback, UnityAction requestDeletedCallback)
    {
        base.Setup();

        // Set private data
        this.enclosureID = enclosureID;
        this.request = request;
        onPriorityUpdated.AddListener(priorityUpdatedCallback);
        onRequestDeleted.AddListener(requestDeletedCallback);

        // Setup each dropdown
        targetDropdown.Setup(ItemRegistry.Category.Food, ItemRegistry.Category.Species);
        needDropdown.Setup(new NeedType[] { NeedType.FoodSource, NeedType.Terrain, NeedType.Liquid });
        itemRequestedDropdown.Setup(ItemRegistry.Category.Food, ItemRegistry.Category.Tile);

        if (request != null)
        {
            priorityInput.text = request.Priority.ToString();
            targetDropdown.SetSelectedItem(request.Target);
            needDropdown.SetNeedTypeValue(request.ImprovedNeed);
            quantityInput.text = request.QuantityRequested.ToString();
            itemRequestedDropdown.SetSelectedItem(request.ItemRequested);
        }
        else
        {
            priorityInput.text = "0";
            targetDropdown.SetDropdownValue(0);
            needDropdown.SetDropdownValue(0);
            quantityInput.text = "0";
            itemRequestedDropdown.Dropdown.value = 0;
        }

        // Cache current id
        EnclosureID current = EnclosureID.FromCurrentSceneName();
        // Only add listeners if this editor is in the current scene
        if(enclosureID == current)
        {
            // Add listeners
            priorityInput.onEndEdit.AddListener(x =>
            {
                if (!string.IsNullOrWhiteSpace(x))
                {
                    GetOrCreateResourceRequest().Priority = int.Parse(x);
                    onPriorityUpdated.Invoke();
                }
            });
            targetDropdown.OnItemSelected.AddListener(x => GetOrCreateResourceRequest().Target = x);
            needDropdown.OnNeedTypeSelected.AddListener(x => GetOrCreateResourceRequest().ImprovedNeed = x);
            quantityInput.onEndEdit.AddListener(x =>
            {
                if (!string.IsNullOrWhiteSpace(x)) GetOrCreateResourceRequest().QuantityRequested = int.Parse(x);
            });
            itemRequestedDropdown.OnItemSelected.AddListener(x => GetOrCreateResourceRequest().ItemRequested = x);
        }

        // Elements only interactable if editing for the current enclosure
        group.interactable = enclosureID == current;

        // If request is null then the group is slightly faded - it is a group that adds a new request once it is edited
        if (request != null) group.alpha = 1f;
        else group.alpha = 0.5f;

        // Update status display
        statusUI.UpdateDisplay(request);

        // Delete the request when the button is clicked
        deleteButton.onClick.AddListener(DeleteRequest);

        // Add scroll intercecptors to the input fields so that the scroll event goes to the 
        // containing scroll rect instead of the input fields
        OnScrollEventInterceptor interceptor = priorityInput.gameObject.AddComponent<OnScrollEventInterceptor>();
        interceptor.InterceptTarget = scrollTarget;
        interceptor = quantityInput.gameObject.AddComponent<OnScrollEventInterceptor>();
        interceptor.InterceptTarget = scrollTarget;
    }

    // Update the review UI for this resource request
    public void UpdateReviewUI()
    {
        statusUI.UpdateDisplay(request);
        group.interactable = false;
    }
    #endregion

    #region Private Methods
    private ResourceRequest GetOrCreateResourceRequest()
    {
        if (request == null)
        {
            if (string.IsNullOrWhiteSpace(priorityInput.text)) priorityInput.text = "0";
            if (string.IsNullOrWhiteSpace(quantityInput.text)) quantityInput.text = "0";

            // Create a new request
            request = new ResourceRequest
            {
                Priority = int.Parse(priorityInput.text),
                Target = targetDropdown.SelectedItem,
                ImprovedNeed = needDropdown.SelectedNeed,
                QuantityRequested = int.Parse(quantityInput.text),
                ItemRequested = itemRequestedDropdown.SelectedItem
            };

            // Get the list and add the new request
            ResourceRequestList list = UIParent.Notebook.Concepts.GetResourceRequestList(enclosureID);
            list.Requests.Add(request);

            // Put all elements at full alpha again
            group.alpha = 1f;

            // Invoke the event for creating a new request
            onNewRequestCreated.Invoke();
        }
        return request;
    }
    private void DeleteRequest()
    {
        if(request != null && request.CurrentStatus == ResourceRequest.Status.NotReviewed)
        {
            // Remove this request from the requests on the notebook
            UIParent.Notebook.Concepts.GetResourceRequestList(enclosureID).Requests.Remove(request);
            onRequestDeleted.Invoke();
            // Destroy this editor
            Destroy(gameObject);
        }
    }
    #endregion
}
