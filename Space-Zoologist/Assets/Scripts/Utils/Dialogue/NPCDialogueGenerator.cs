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

    public void NPCTalksAboutObjectiveStatus()
    {
        this.dialogueTextMeshPro.text = $"Displaying something about the objectives";
    }

    public void NPCTalksAboutPopulationStatus()
    {
        this.dialogueTextMeshPro.text = $"Displaying something about the population";
    }

    public void NPCTalksAboutJournalProgress()
    {
        this.dialogueTextMeshPro.text = $"Displaying something about the journal progress";
    }
}
