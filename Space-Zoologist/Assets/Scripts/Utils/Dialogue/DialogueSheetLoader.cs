using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parses data from a cvs file into dialgoue output data
/// </summary>
public class DialogueSheetLoader : MonoBehaviour
{
    [SerializeField] private TextAsset templateDialogueSheet = default;
    [SerializeField] private TextAsset needDialogueSheet = default;
    [SerializeField] private TextAsset eventDialogueSheet = default;

    [SerializeField] private LevelData levelData = default;

    private readonly char lineSeperater = '\n'; 
    private readonly char fieldSeperator = ','; 
    private readonly char fieldQutation = '"';

    /// <summary>
    /// Parser for .csv google sheets
    /// </summary>
    /// <param name="csvFile"> The file to be parsed </param>
    /// <returns></returns>
    private List<List<string>> parseCsv(TextAsset csvFile)
    {
        // Make sure that csv file is there
        Debug.Assert(!csvFile, "Template sheet not found!");

        // Split lines
        string[] rows = csvFile.text.Trim().Split(this.lineSeperater);

        List<List<string>> parsedFile = new List<List<string>>();

        // Process each row
        foreach (string row in rows)
        {
            List<string> fields = new List<string>();
            string tempField = "";
            // Store how many quations seen, even count means there is a pair
            byte qutationCounter = 0;
            char preChar = ',';

            // Process the characters in each row
            foreach (char c in row)
            {
                // Finished a field
                if (c == this.fieldSeperator && qutationCounter % 2 == 0)
                {
                    // Remove the qutation at the end of field
                    if (preChar == this.fieldQutation)
                    {
                        tempField = tempField.Remove(tempField.Length - 1);
                    }

                    fields.Add(tempField);
                    tempField = "";
                }
                // See a ',' but it is part of a dialogue
                else if (c == this.fieldSeperator && qutationCounter % 2 != 0)
                {
                    tempField += c;
                }
                // See " 
                else if (c == this.fieldQutation)
                {
                    qutationCounter++;

                    // If two "s only add the first one
                    if (preChar == this.fieldQutation)
                    {
                        // Do not add to temp field
                    }
                    // Handles the " at the start of the field
                    else if (tempField == "")
                    {
                        // Do not add the temo field
                    }
                    else
                    {
                        tempField += c;
                    }
                }
                // Normal characters
                else
                {
                    tempField += c;
                    preChar = c;
                }
            }

            // Added the last field
            fields.Add(tempField.Trim());

            parsedFile.Add(fields);
        }

        return parsedFile;
    }

    /// <summary>
    /// Parses the returns the dialogue template of the give type of dialogue.
    /// </summary>
    /// <param name="type"> The name of the template type, eg Population, Objective etc. </param>
    /// <returns>List of templates.</returns>
    public List<string> LoadDialogueTemplates(string type)
    {
        List<List<string>> parsedCsv = this.parseCsv(this.templateDialogueSheet);

        foreach (List<string> row in parsedCsv)
        {
            if (row[0] == type)
            {
                return row.GetRange(1, row.Count - 1);
            }
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, DialogueOptionData> LoadSpeciesNeedDialogue()
    {
        List<List<string>> parsedCsv = this.parseCsv(this.needDialogueSheet);

        Dictionary<string, DialogueOptionData> specieseNeedDialogoues = new Dictionary<string, DialogueOptionData>();

        foreach (List<string> row in parsedCsv)
        {
            // specicesName-needName
            string name = $"{row[0]}-{row[1]}";

            if (!specieseNeedDialogoues.ContainsKey(name))
            {
                  specieseNeedDialogoues.Add(name, new DialogueOptionData(
                    name,
                    row.GetRange(2,3),
                    row.GetRange(5,3),
                    row.GetRange(8, row.Count-8)
                ));
            }
        }

        return specieseNeedDialogoues;
    }

    public void LoadEventDialogue(Dictionary<string, List<string>> eventDialogue)
    {
        List<List<string>> parsedCsv = this.parseCsv(this.eventDialogueSheet);

        foreach (List<string> row in parsedCsv)
        {
            if (!eventDialogue.ContainsKey(row[0]))
            {
                eventDialogue.Add(row[0], row.GetRange(1, row.Count - 1));
            }
        }
    }
}
