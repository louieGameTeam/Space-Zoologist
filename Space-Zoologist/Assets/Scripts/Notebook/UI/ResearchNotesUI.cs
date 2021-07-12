using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ResearchNotesUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the research model used to take notes in")]
    private ResearchModel researchModel;
    [SerializeField]
    [Tooltip("Reference to the picker object that selects the research category")]
    private ResearchCategoryPicker categoryPicker;
    [SerializeField]
    [Tooltip("Prefab instantiated for each research note")]
    private ResearchSingleNoteUI notePrefab;
    [SerializeField]
    [Tooltip("Parent that all notes are instantiated under")]
    private Transform noteParent;
    [SerializeField]
    [Tooltip("Text that displays the name of the current category taking notes on")]
    private TextMeshProUGUI titleText;

    // Maps the group of notes to the research category
    private Dictionary<ResearchCategory, ResearchSingleNoteUIGroup> groups = new Dictionary<ResearchCategory, ResearchSingleNoteUIGroup>();
    private ResearchSingleNoteUIGroup currentGroup;

    // Start is called before the first frame update
    void Start()
    {
        foreach(KeyValuePair<ResearchCategory, ResearchEntry> entry in researchModel.ResearchDictionary)
        {
            // Add the group for this key
            groups.Add(entry.Key, new ResearchSingleNoteUIGroup());

            // Store a reference to the notes in the current entry
            ResearchNotes notes = entry.Value.Notes;

            // Loop through each label in the research notes
            foreach(string label in notes.Labels.Labels)
            {
                // Instantiate a single note
                ResearchSingleNoteUI note = Instantiate(notePrefab, noteParent);
                note.Setup(label, notes.ReadNote(label), OnNoteValueChanged);
                // Add the note to the group
                groups[entry.Key].Add(note);
            }

            // If category picker has not initialized itself yet, the boolean statement is always false
            // and this simply disables all notes until the category picker initializes and calls OnResearchCategoryChanged
            // If it has already initialized itself, then this will enable the correct group
            groups[entry.Key].SetActive(entry.Key == categoryPicker.SelectedCategory);
        }

        if (categoryPicker.SelectedCategory != null) titleText.text = "Space " + categoryPicker.SelectedCategory.Name;

        // Add listener for the research category change
        categoryPicker.OnResearchCategoryChanged.AddListener(OnResearchCategoryChanged);
    }

    private void OnResearchCategoryChanged(ResearchCategory newCategory)
    {
        // Disable the previous group
        if (currentGroup != null) currentGroup.SetActive(false);

        // Set and enable the current category
        currentGroup = groups[newCategory];
        currentGroup.SetActive(true);

        // Set the title text to the name of the category
        titleText.text = "Space " + categoryPicker.SelectedCategory.Name;
    }

    // Set the note with the given label on the research notes
    private void OnNoteValueChanged(string label, string newNote)
    {
        researchModel.GetEntry(categoryPicker.SelectedCategory).Notes.WriteNote(label, newNote);
    }
}
