using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parses data from a cvs file into dialgoue output data
/// </summary>
public class DialogueSheetLoader : MonoBehaviour
{
    [SerializeField] private TextAsset templateDialogueSheet = default;
    [SerializeField] private TextAsset needDialogueSheet = default;

    [SerializeField] private LevelData levelData = default;

    private char lineSeperater = '\n'; 
    private char fieldSeperator = ','; 
    private char fieldQutation = '"';

    private List<List<string>> parsedTemplate = new List<List<string>>();
    private List<List<string>> parsedNeedDialogue = new List<List<string>>();

    // Parse the csv files
    private void Start()
    {
        this.templateDialogueSheet = Resources.Load<TextAsset>(Path.Combine("LevelData", "Level" + 1));
        Resources.Load<TextAsset>(Path.Combine("NPC", "NPC Dialogue template"));
        this.needDialogueSheet = Resources.Load<TextAsset>(Path.Combine("NPC", "NPC Dialogue need"));

        this.parsedTemplate = this.parseCsv(this.templateDialogueSheet);
        this.parsedNeedDialogue = this.parseCsv(this.needDialogueSheet);
    }

    private List<List<string>> parseCsv(TextAsset csvFile)
    {
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

            parsedFile.Add(fields);
        }

        return parsedFile;
    }

    public void LoadDialogueTemplates(string type, List<string> dialogueTemplates)
    {
        foreach (List<string> row in this.parsedTemplate)
        {
            if (row[0] == type)
            {
                dialogueTemplates = row.GetRange(1, row.Count - 1);
                return;
            }
        }
    }

    public void LoadSpeciesNeedDialogue(Dictionary<string, DialogueOptionData> specieseNeedDialogoues)
    {
        foreach (List<string> row in this.parsedTemplate)
        {
            // specicesName-needName
            string name = $"{row[0]}-{row[1]}";

            if (!specieseNeedDialogoues.ContainsKey(name))
            {
                specieseNeedDialogoues.Add("name", new DialogueOptionData(
                    name,
                    new List<string>() { row[2], row[3], row[4] },
                    new List<string>() { row[5], row[6], row[7] },
                    new List<string>() { row[8], row[9], row[10] }
                ));
            }
        }
    }
}
