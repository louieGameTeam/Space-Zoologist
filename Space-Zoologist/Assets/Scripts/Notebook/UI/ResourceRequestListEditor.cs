using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceRequestListEditor : NotebookUIChild
{
    [SerializeField]
    [Tooltip("Reference to the prefab used to edit a single test and metrics entry")]
    private ResourceRequestEditor editorPrefab;
    [SerializeField]
    [Tooltip("Parent transform for the editor of the individual entries")]
    private LayoutGroup editorParent;
    [SerializeField]
    [Tooltip("Reference to the scroll rect that the editors will fit into")]
    private ScrollRect editorScroller;

    private List<ResourceRequestEditor> currentEditors = new List<ResourceRequestEditor>();

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

        // Foreach entry in the selected list, add an editor
        foreach (ResourceRequest request in list.Requests)
        {
            ResourceRequestEditor editor = Instantiate(editorPrefab, editorParent.transform);
            editor.Setup(id, request, editorScroller);
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
        editor.Setup(EnclosureID.FromCurrentSceneName(), null, editorScroller);
        editor.OnNewRequestCreated.AddListener(OnNewEntryCreated);
        currentEditors.Add(editor);
    }
    #endregion
}
