using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueOutputOption { Good, Bad, Hint };

/// <summary>
/// Data structure to store parsed dialogues from sheet.
/// </summary>
public class DialogueOptionData
{
    public string Name => this.name;

    private string name;
    private List<string> goodDialgoues;
    private List<string> badDialogues;
    private List<string> hintDialogues;

    public DialogueOptionData(string name, List<string> goodDialgoues, List<string> badDialogues, List<string> hintDialogues)
    {
        this.name = name;
        this.goodDialgoues = goodDialgoues;
        this.badDialogues = badDialogues;
        this.hintDialogues = hintDialogues;
}

    /// <summary>
    /// Return a random dialgoue with the option given.
    /// </summary>
    /// <param name="option">dialgoue output option: Good/Bad/Hint</param>
    /// <returns>a string as part of the sentence</returns>
    public string GetDialogue(DialogueOutputOption option)
    {
        System.Random random = new System.Random();

        switch (option)
        {
            case DialogueOutputOption.Good:
                return this.goodDialgoues[random.Next(this.goodDialgoues.Count)];
            case DialogueOutputOption.Bad:
                return this.badDialogues[random.Next(this.badDialogues.Count)];
            case DialogueOutputOption.Hint:
                return this.hintDialogues[random.Next(this.hintDialogues.Count)];
            default:
                break;
        }

        return $"{option} is not valid!";
    }
}