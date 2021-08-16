using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceRequestListEditor : NotebookUIChild
{
    [SerializeField]
    [Tooltip("Script used to pick the enclosure we are currently taking notes on")]
    private EnclosureIDPicker enclosurePicker;
    [SerializeField]
    [Tooltip("Reference to the prefab used to edit a single test and metrics entry")]
    private ResourceRequestEditor editorPrefab;
    [SerializeField]
    [Tooltip("Parent transform for the editor of the individual entries")]
    private LayoutGroup editorParent;

    private List<ResourceRequestEditor> currentEditors = new List<ResourceRequestEditor>();

    public override void Setup()
    {
        base.Setup();

        // Add listener for the enclosure picked event
        enclosurePicker.OnEnclosureIDPicked.AddListener(OnEnclosureSelected);
        OnEnclosureSelected(EnclosureID.FromCurrentSceneName());
    }

    private void OnEnclosureSelected(EnclosureID id)
    {
        // Destroy all existing editors
        foreach (ResourceRequestEditor editor in currentEditors)
        {
            Destroy(editor.gameObject);
        }
        // Clear out the list
        currentEditors.Clear();

        // Foreach entry in the selected list, add an editor
        foreach (ResourceRequest request in UIParent.Notebook.GetResourceRequestList(id).Requests)
        {
            ResourceRequestEditor editor = Instantiate(editorPrefab, editorParent.transform);
            editor.Setup(id, request);
            currentEditors.Add(editor);
        }

        // If the enclosure selected is the current enclosure, then add a new editor
        // that we can use to add more entries
        if (id == EnclosureID.FromCurrentSceneName())
        {
            CreateAddingEntry();
        }
    }

    private void OnNewEntryCreated()
    {
        CreateAddingEntry();
    }

    private void CreateAddingEntry()
    {
        ResourceRequestEditor editor = Instantiate(editorPrefab, editorParent.transform);
        editor.Setup(EnclosureID.FromCurrentSceneName(), null);
        editor.OnNewRequestCreated.AddListener(OnNewEntryCreated);
        currentEditors.Add(editor);
    }
}
