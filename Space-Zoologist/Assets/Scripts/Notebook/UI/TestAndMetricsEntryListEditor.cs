using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAndMetricsEntryListEditor : NotebookUIChild
{
    [SerializeField]
    [Tooltip("Script used to pick the enclosure we are currently taking notes on")]
    private EnclosureIDPicker enclosurePicker;
    [SerializeField]
    [Tooltip("Reference to the prefab used to edit a single test and metrics entry")]
    private TestAndMetricEntryEditor editorPrefab;
    [SerializeField]
    [Tooltip("Parent transform for the editor of the individual entries")]
    private LayoutGroup editorParent;

    private List<TestAndMetricEntryEditor> currentEntries = new List<TestAndMetricEntryEditor>();

    public override void Setup()
    {
        base.Setup();

        // Add listener for the enclosure picked event
        enclosurePicker.OnEnclosureIDPicked.AddListener(OnEnclosureSelected);
        OnEnclosureSelected(EnclosureID.FromCurrentSceneName());
    }

    private void OnEnclosureSelected(EnclosureID id)
    {
        Debug.Log("Enclosure selected: " + id);

        // Destroy all existing editors
        foreach(TestAndMetricEntryEditor editor in currentEntries)
        {
            Destroy(editor.gameObject);
        }
        // Clear out the list
        currentEntries.Clear();

        // Foreach entry in the selected list, add an editor
        foreach(TestAndMetricsEntry entry in UIParent.Notebook.GetTestAndMetricsEntryList(id).Entries)
        {
            TestAndMetricEntryEditor editor = Instantiate(editorPrefab, editorParent.transform);
            editor.Setup(id, entry);
            currentEntries.Add(editor);
        }

        // If the enclosure selected is the current enclosure, then add a new editor
        // that we can use to add more entries
        if(id == EnclosureID.FromCurrentSceneName())
        {
            CreateAddingEntry();
        }

        // Force the layout group to update
        //editorParent.SetLayoutHorizontal();
        //editorParent.SetLayoutVertical();
    }

    private void OnNewEntryCreated()
    {
        CreateAddingEntry();
    }

    private void CreateAddingEntry()
    {
        TestAndMetricEntryEditor editor = Instantiate(editorPrefab, editorParent.transform);
        editor.Setup(EnclosureID.FromCurrentSceneName(), null);
        editor.OnNewEntryCreated.AddListener(OnNewEntryCreated);
        currentEntries.Add(editor);
    }
}
