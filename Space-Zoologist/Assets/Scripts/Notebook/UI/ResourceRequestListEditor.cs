using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceRequestListEditor : NotebookUIChild
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the prefab used to edit a single test and metrics entry")]
    private ResourceRequestEditor editorPrefab;
    [SerializeField]
    [Tooltip("Parent transform for the editor of the individual entries")]
    private LayoutGroup editorParent;
    [SerializeField]
    [Tooltip("Reference to the scroll rect that the editors will fit into")]
    private ScrollRect editorScroller;
    #endregion

    #region Private Fields
    private List<ResourceRequestEditor> currentEditors = new List<ResourceRequestEditor>();
    #endregion

    #region Public Methods
    public void UpdateListEdited(EnclosureID id, ResourceRequestList list)
    {
        // Destroy all existing editors
        foreach (ResourceRequestEditor editor in currentEditors)
        {
            Destroy(editor.gameObject);
        }
        // Clear out the list
        currentEditors.Clear();

        // Get the requests and sort them
        List<ResourceRequest> sortedRequests = new List<ResourceRequest>(list.Requests);
        sortedRequests.Sort();

        // Foreach entry in the selected list, add an editor
        foreach (ResourceRequest request in sortedRequests)
        {
            ResourceRequestEditor editor = Instantiate(editorPrefab, editorParent.transform);
            editor.Setup(id, request, editorScroller, SortEditors, () => OnRequestDeleted(editor));
            currentEditors.Add(editor);
        }

        // If the enclosure selected is the current enclosure, then add a new editor
        // that we can use to add more entries
        if (id == EnclosureID.FromCurrentSceneName())
        {
            CreateAddingEntry();
        }
    }
    public void UpdateReviewUI()
    {
        foreach(ResourceRequestEditor editor in currentEditors)
        {
            editor.UpdateReviewUI();
        }
        // Reorder them visually
        SortEditors();
    }
    #endregion

    #region Private Methods
    private void OnNewEntryCreated()
    {
        CreateAddingEntry();
    }
    private void CreateAddingEntry()
    {
        ResourceRequestEditor editor = Instantiate(editorPrefab, editorParent.transform);
        editor.Setup(EnclosureID.FromCurrentSceneName(), null, editorScroller, SortEditors, () => OnRequestDeleted(editor));
        editor.OnNewRequestCreated.AddListener(OnNewEntryCreated);
        currentEditors.Add(editor);

        // Sort the editors now that the new one is added
        SortEditors();
    }
    private void OnRequestDeleted(ResourceRequestEditor editorDeleted)
    {
        currentEditors.Remove(editorDeleted);
    }
    private void SortEditors()
    {
        currentEditors.Sort((x, y) => ResourceRequestVisualComparer(x.Request, y.Request));

        // Set the sibling index of each editor transform
        for(int i = 0; i < currentEditors.Count; i++)
        {
            currentEditors[i].RectTransform.SetSiblingIndex(i);
        }
    }
    private int ResourceRequestVisualComparer(ResourceRequest x, ResourceRequest y)
    {
        // If x is null check if y is null
        if (x == null)
        {
            // If y is not null
            if (y != null)
            {
                // null goes below not reviewed requests and above reviewed requests
                if (y.CurrentStatus == ResourceRequest.Status.NotReviewed) return 1;
                else return -1;
            }
            // Nulls are equal
            else return 0;
        }
        // If x is not null and y is null
        else if (y == null)
        {
            // not reviewed requests go above null
            if (x.CurrentStatus == ResourceRequest.Status.NotReviewed) return -1;
            // reviewed requests go below null
            else return 1;
        }
        // If x and y are not null
        else
        {
            // Sort by higher priority for equal statuses
            if (x.CurrentStatus == y.CurrentStatus) return y.Priority.CompareTo(x.Priority);
            // Unequal status compare by status
            else return x.CurrentStatus.CompareTo(y.CurrentStatus);
        }
    }
    #endregion
}
