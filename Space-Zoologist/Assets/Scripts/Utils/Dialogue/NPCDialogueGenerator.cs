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
    public enum DialogueOutputOption { Good, Bad, Hint };

    /// <summary>
    /// Data structure to store parsed dialogues from sheet
    /// </summary>
    public class DialogueOption
    {
        public string Name => this.name;

        private string name;
        private List<string> goodDialgoues;
        private List<string> badDialogues;
        private List<string> hintDialogues;

        public string GetDialogue(DialogueOutputOption option)
        {
            switch(option)
            {
                case DialogueOutputOption.Good:
                    break;
                case DialogueOutputOption.Bad:
                    break;
                case DialogueOutputOption.Hint:
                    break;
                default:
                    break;
            }

            return $"{option} is not valid!";
        }
    }


    [SerializeField] private TextMeshProUGUI dialogueTextMeshPro = default;
    [SerializeField] private ObjectiveManager objectiveManager = default;

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
