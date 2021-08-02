using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ResearchNotesUI : NotebookUIChild
{
    // Current notes to edit
    public ResearchNotes CurrentNotes => UIParent.NotebookModel.NotebookResearch.GetEntry(categoryPicker.SelectedCategory).Notes;

    [SerializeField]
    [Tooltip("Reference to the picker object that selects the research category")]
    private ResearchCategoryPicker categoryPicker;
    [SerializeField]
    [Tooltip("Reference to the input field used to edit the notes")]
    private TMP_InputField notesInputField;
    //[SerializeField]
    //[Tooltip("Prefab instantiated for each research note")]
    //private ResearchSingleNoteUI notePrefab;
    //[SerializeField]
    //[Tooltip("Parent that all notes are instantiated under")]
    //private Transform noteParent;
    [SerializeField]
    [Tooltip("Text that displays the name of the current category taking notes on")]
    private TextMeshProUGUI titleText;

    // Maps the group of notes to the research category
    //private Dictionary<ResearchCategory, ResearchSingleNoteUIGroup> groups = new Dictionary<ResearchCategory, ResearchSingleNoteUIGroup>();
    //private ResearchSingleNoteUIGroup currentGroup;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        //foreach(KeyValuePair<ResearchCategory, ResearchEntry> entry in UIParent.NotebookModel.NotebookResearch.ResearchDictionary)
        //{
        //    // Add the group for this key
        //    groups.Add(entry.Key, new ResearchSingleNoteUIGroup());

        //    // Store a reference to the notes in the current entry
        //    ResearchNotes notes = entry.Value.Notes;

        //    // Loop through each label in the research notes
        //    foreach(string label in notes.Labels.Labels)
        //    {
        //        // Instantiate a single note
        //        ResearchSingleNoteUI note = Instantiate(notePrefab, noteParent);
        //        note.Setup(label, notes.ReadNote(label), OnNoteValueChanged);
        //        // Add the note to the group
        //        groups[entry.Key].Add(note);
        //    }

        //    // All research notes groups start inactive
        //    groups[entry.Key].SetActive(false);
        //}

        // Call the method when the input field ends edit
        notesInputField.onEndEdit.AddListener(OnNoteValueChanged);

        // If category picker already has a selected category,
        // then we know it initialized before us, so we need to update our UI
        if (categoryPicker.HasBeenInitialized) OnResearchCategoryChanged(categoryPicker.SelectedCategory);

        // Add listener for the research category change
        categoryPicker.OnResearchCategoryChanged.AddListener(OnResearchCategoryChanged);
    }

    private void OnResearchCategoryChanged(ResearchCategory newCategory)
    {
        // Set the title text to the name of the category
        titleText.text = "Space " + categoryPicker.SelectedCategory.Name;

        // Set the text on the input field to the text in the notes of the current entry
        notesInputField.SetTextWithoutNotify(CurrentNotes.Notes);
    }

    // Set the note with the given label on the research notes
    private void OnNoteValueChanged(string newNote)
    {
        CurrentNotes.Notes = newNote;
    }
}
