using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class ResearchNotesUI : NotebookUIChild
{
    #region Public Properties
    public ResearchNotes CurrentNotes => UIParent.Notebook.Research.GetEntry(categoryPicker.SelectedCategory).Notes;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the picker object that selects the research category")]
    private ResearchCategoryPicker categoryPicker;
    [SerializeField]
    [Tooltip("Text that displays the name of the current category taking notes on")]
    private TextMeshProUGUI titleText;
    [SerializeField]
    [Tooltip("Scroll view used to view all of the notes")]
    private ScrollRect scrollView;
    [SerializeField]
    [Tooltip("Layout group used as the parent for all notes")]
    private LayoutGroup noteParent;
    [SerializeField]
    [Tooltip("Prefab to instantiate for each note")]
    private ResearchSingleNoteUI notePrefab;
    #endregion

    #region Private Fields
    private List<ResearchSingleNoteUI> currentNotes = new List<ResearchSingleNoteUI>();
    #endregion

    #region Public Methods
    // Start is called before the first frame update
    public override void Setup()
    {
        base.Setup();

        // If category picker already has a selected category,
        // then we know it initialized before us, so we need to update our UI
        if (categoryPicker.HasBeenInitialized) OnResearchCategoryChanged(categoryPicker.SelectedCategory);

        // Add listener for the research category change
        categoryPicker.OnResearchCategoryChanged.AddListener(OnResearchCategoryChanged);
    }
    #endregion

    #region Private Methods
    private void OnResearchCategoryChanged(ResearchCategory newCategory)
    {
        // Set the title text to the name of the category
        titleText.text = categoryPicker.SelectedCategory.Name + ": Target Specifications";

        // Destroy all notes
        foreach(ResearchSingleNoteUI note in currentNotes)
        {
            Destroy(note.gameObject);
        }
        currentNotes.Clear();

        // Create a new note for every label
        foreach(string label in CurrentNotes.Labels.Labels)
        {
            ResearchSingleNoteUI clone = Instantiate(notePrefab, noteParent.transform);
            clone.Setup(newCategory, label, scrollView);
            currentNotes.Add(clone);
        }
    }
    #endregion
}
