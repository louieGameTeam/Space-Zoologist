using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script to create custome NPC dialogue
/// </summary>
public class NPCDialogueGenerator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueTextMeshPro = default;
    [SerializeField] private ObjectiveManager objectiveManager = default;

    // Loader script to read in dialogue ouuputs
    private DialogueSheetLoader dialgoueSheetLoader = new DialogueSheetLoader();

    private Dictionary<string, List<string>> dialogueTemplates = default;

    private Dictionary<(AnimalSpecies, Need), DialogueOptionData> specieseNeedDialogoues = default;
    private Dictionary<Objective, DialogueOptionData> objectiveStatusDialogue = default;

    // Read in and parse the sheet
    private void Awake()
    {
        this.loadDialogueOutputDatas();
        this.loadDialogueTemplates();
    }

    private void loadDialogueOutputDatas()
    {
        this.dialgoueSheetLoader.LoadSpeciesNeedDialogue(this.specieseNeedDialogoues);
    }

    private void loadDialogueTemplates()
    {
        this.dialogueTemplates = new Dictionary<string, List<string>>();

        // Loading in dialogue templates about need
        this.dialogueTemplates.Add("Population", new List<string>());
        this.dialgoueSheetLoader.LoadDialogueTemplates("Population", this.dialogueTemplates["Population"]);
    }

    public void NPCTalksAboutObjectiveStatus()
    {
        this.dialogueTextMeshPro.text = $"Displaying something about the objectives";
    }

    public void NPCTalksAboutPopulationStatus()
    {
        this.dialogueTextMeshPro.text = $"You can select the populaion you want to ";
    }

    public void NPCTalksAboutJournalProgress()
    {
        this.dialogueTextMeshPro.text = $"Displaying something about the journal progress";
    }
}
