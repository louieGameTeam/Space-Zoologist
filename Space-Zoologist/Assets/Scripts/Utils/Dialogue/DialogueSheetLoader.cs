using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parses data from a cvs file into dialgoue output data
/// </summary>
public class DialogueSheetLoader
{
    public void LoadDialogueTemplates(string type, List<string> dialogueTemplates)
    {
        switch(type)
        {
            case "Population":
                break;
            default:
                break;
        }
    }

    public void LoadSpeciesNeedDialogue(Dictionary<(AnimalSpecies, Need), DialogueOptionData> specieseNeedDialogoues)
    {

    }
}
