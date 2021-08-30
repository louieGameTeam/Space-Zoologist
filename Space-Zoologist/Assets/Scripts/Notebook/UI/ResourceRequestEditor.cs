using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class ResourceRequestEditor : NotebookUIChild
{
    public ResourceRequest Request
    {
        get
        {
            if(request == null)
            {
                if (string.IsNullOrWhiteSpace(priorityInput.text)) priorityInput.text = "0";
                if (string.IsNullOrWhiteSpace(quantityInput.text)) quantityInput.text = "0";

                // Create a new request
                request = new ResourceRequest
                {
                    Priority = int.Parse(priorityInput.text),
                    Target = categoryDropdown.SelectedCategory,
                    ImprovedNeed = needDropdown.SelectedNeed,
                    Quantity = int.Parse(quantityInput.text),
                    Item = resourcePicker.ItemSelected
                };

                // Get the list and add the new request
                ResourceRequestList list = UIParent.Notebook.GetResourceRequestList(enclosureID);
                list.Requests.Add(request);
                // Invoke the event for creating a new request
                onNewRequestCreated.Invoke();
            }
            return request;
        }
    }
    public UnityEvent OnNewRequestCreated => onNewRequestCreated;

    [SerializeField]
    [Tooltip("Text input field that sets the priority of the request editor")]
    private TMP_InputField priorityInput;
    [SerializeField]
    [Tooltip("Reference to the dropdown that gets a research category")]
    private TypeFilteredResearchCategoryDropdown categoryDropdown;
    [SerializeField]
    [Tooltip("Reference to the dropdown that gets the need")]
    private NeedTypeDropdown needDropdown;
    [SerializeField]
    [Tooltip("Text input field that sets the quantity of resources to request")]
    private TMP_InputField quantityInput;
    [SerializeField]
    [Tooltip("Dropdown used to select the item name to request")]
    private ResourcePicker resourcePicker;
    [SerializeField]
    [Tooltip("Event invoked when the editor creates a new request")]
    private UnityEvent onNewRequestCreated;

    // ID of the request to edit
    private EnclosureID enclosureID;
    // Resource request to edit
    private ResourceRequest request;

    public void Setup(EnclosureID enclosureID, ResourceRequest request, ScrollRect scrollTarget)
    {
        base.Setup();

        // Set private data
        this.enclosureID = enclosureID;
        this.request = request;

        // Setup each dropdown
        categoryDropdown.Setup(ResearchCategoryType.Food, ResearchCategoryType.Species);
        needDropdown.Setup(new NeedType[] { NeedType.FoodSource, NeedType.Terrain, NeedType.Liquid });
        resourcePicker.Setup();

        if (request != null)
        {
            priorityInput.text = request.Priority.ToString();
            categoryDropdown.SetResearchCategory(request.Target);
            needDropdown.SetNeedTypeValue(request.ImprovedNeed);
            quantityInput.text = request.Quantity.ToString();
            resourcePicker.ItemSelected = request.Item;
        }
        else
        {
            priorityInput.text = "0";
            categoryDropdown.SetDropdownValue(0);
            needDropdown.SetDropdownValue(0);
            quantityInput.text = "0";
            resourcePicker.Dropdown.value = 0;
        }

        // Cache current id
        EnclosureID current = EnclosureID.FromCurrentSceneName();
        // Only add listeners if this editor is in the current scene
        if(enclosureID == current)
        {
            // Add listeners
            quantityInput.onEndEdit.AddListener(x =>
            {
                if (!string.IsNullOrWhiteSpace(x)) Request.Priority = int.Parse(x);
            });
            categoryDropdown.OnResearchCategorySelected.AddListener(x => Request.Target = x);
            needDropdown.OnNeedTypeSelected.AddListener(x => Request.ImprovedNeed = x);
            quantityInput.onEndEdit.AddListener(x =>
            {
                if (!string.IsNullOrWhiteSpace(x)) Request.Quantity = int.Parse(x);
            });
            resourcePicker.OnItemSelected.AddListener(x => Request.Item = x);
        }

        // Elements only interactable if editing for the current enclosure
        priorityInput.readOnly = current != enclosureID;
        categoryDropdown.Dropdown.interactable = current == enclosureID;
        needDropdown.Dropdown.interactable = current == enclosureID;
        quantityInput.readOnly = current != enclosureID;
        resourcePicker.Dropdown.interactable = current == enclosureID;

        // Add scroll intercecptors to the input fields so that the scroll event goes to the 
        // containing scroll rect instead of the input fields
        OnScrollEventInterceptor interceptor = priorityInput.gameObject.AddComponent<OnScrollEventInterceptor>();
        interceptor.InterceptTarget = scrollTarget;
        interceptor = quantityInput.gameObject.AddComponent<OnScrollEventInterceptor>();
        interceptor.InterceptTarget = scrollTarget;
    }
}
