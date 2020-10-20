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
    [SerializeField] private Selector selector = default;

    // Loader script to read in dialogue ouuputs
    [SerializeField] private DialogueSheetLoader dialgoueSheetLoader = default;

    private Dictionary<string, List<string>> dialogueTemplates = new Dictionary<string, List<string>>();

    private Dictionary<string, DialogueOptionData> specieseNeedDialogoues = new Dictionary<string, DialogueOptionData>();
    private Dictionary<string, DialogueOptionData> objectiveStatusDialogue = new Dictionary<string, DialogueOptionData>();

    private System.Random random = new System.Random();

    // Call the dialogue sheet loader to load the dialogues
    private void Start()
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

        // Loading template about population status
        this.dialogueTemplates.Add("Population", new List<string>());
        this.dialgoueSheetLoader.LoadDialogueTemplates("Population", this.dialogueTemplates["Population"]);

        // Load template about objective status
        this.dialogueTemplates.Add("Objective", new List<string>());
        this.dialgoueSheetLoader.LoadDialogueTemplates("Objective", this.dialogueTemplates["Objective"]);
    }

    public void NPCTalksAboutObjectiveStatus()
    {
        // Pick a template
        string template = this.dialogueTemplates["Objectve"][this.random.Next(this.dialogueTemplates["Population"].Count)];

        this.dialogueTextMeshPro.text = this.fillObjectiveTemplate(template);
    }

    public void GetPopulationToTalkAbout()
    {
        this.selector.EnableSelection();
    }

    public void NPCTalksAboutPopulationStatus()
    {  
        string dialogue = "";

        if (this.selector.SelectedPopulation)
        {
            this.selector.UnenableSelection();

            // Pick a template
            string template = this.dialogueTemplates["Population"][this.random.Next(this.dialogueTemplates["Population"].Count)];

            dialogue = this.fillPopulationStatusTemplate(template, this.selector.SelectedPopulation);

            this.selector.UnenableSelection();

            //dialogue = $"{this.selector.SelectedPopulation} was selected";
        }
        else
        {
            dialogue = "You did NOT select a population!";
        }

        this.dialogueTextMeshPro.text = dialogue;

    }

    public void NPCTalksAboutJournalProgress()
    {
        this.dialogueTextMeshPro.text = $"Displaying something about the journal progress";
    }

    // TODO: re-think on how to talk about objectives.
    private string fillObjectiveTemplate(string template)
    {
        string dialogue = template;

        if (this.objectiveManager.IsGameOver)
        {
            List<string> parsed = new List<string>(dialogue.Split(' '));

            // Replace text with dialogue when seen keyworkds, remove '{' and '}' used for syntax.
            for (int i = 0; i < parsed.Count; i++)
            {
                if (parsed[i] == "MAIN-GOOD/BAD")
                {
                    
                }
                else if (parsed[i] == "MAIN-HINT")
                {

                }
                else if (parsed[i] == "{" || parsed[i] == "}")
                {
                    parsed.RemoveAt(i);
                }
            }

            return string.Join(" ", parsed);
        }

        return null;
    }

    private string fillPopulationStatusTemplate(string template, Population population)
    {
        string dialogue = template;

        List<string> parsed = new List<string>(dialogue.Split(' '));
        List<string> goodNeeds = new List<string>();
        List<string> badNeeds = new List<string>();

        // Compute a list of good needs and bad needs
        foreach (Need need in population.Needs.Values)
        {
            if (need.GetCondition(need.NeedValue) == NeedCondition.Good)
            {
                goodNeeds.Add(need.NeedName);
            }
            else if (need.GetCondition(need.NeedValue) == NeedCondition.Bad)
            {
                badNeeds.Add(need.NeedName);
            }
        }

        // Replace text with dialogue when seen keyworkds, remove '{' and '}' used for syntax.
        for (int i = 0; i < parsed.Count; i++)
        {
            if (parsed[i] == "POPULATION")
            {
                parsed[i] = population.species.SpeciesName;
            }
            else if (parsed[i] == "GOOD")
            {
                string text = "";

                int index = this.random.Next(goodNeeds.Count) - 1;

                // Pick two randow good dialogue
                if (this.specieseNeedDialogoues.ContainsKey($"{population.species.SpeciesName}-{goodNeeds[index]}"))
                {
                    text += this.specieseNeedDialogoues[$"{population.species.SpeciesName}-{goodNeeds[index]}"].GetDialogue(DialogueOutputOption.Good);
                }
                if (this.specieseNeedDialogoues.ContainsKey($"{population.species.SpeciesName}-{goodNeeds[index]}"))
                {
                    text += this.specieseNeedDialogoues[$"{population.species.SpeciesName}-{goodNeeds[index+1]}"].GetDialogue(DialogueOutputOption.Good);
                }

                // In case no dialogue found
                if (text == "")
                {
                    text = "I have nothing to say about this...";
                }

                parsed[i] = text;
            }
            else if (parsed[i] == "BAD")
            {
                string text = "";

                int index = this.random.Next(badNeeds.Count) - 1;

                // Pick two randow good dialogue
                if (this.specieseNeedDialogoues.ContainsKey($"{population.species.SpeciesName}-{badNeeds[index]}"))
                {
                    text += this.specieseNeedDialogoues[$"{population.species.SpeciesName}-{badNeeds[index]}"].GetDialogue(DialogueOutputOption.Bad);
                }
                if (this.specieseNeedDialogoues.ContainsKey($"{population.species.SpeciesName}-{badNeeds[index]}"))
                {
                    text += this.specieseNeedDialogoues[$"{population.species.SpeciesName}-{badNeeds[index + 1]}"].GetDialogue(DialogueOutputOption.Bad);
                }

                // In case no dialogue found
                if (text == "")
                {
                    text = "I have nothing to say about this...";
                }

                parsed[i] = text;
            }
            else if (parsed[i] == "{" || parsed[i] == "}")
            {
                parsed.RemoveAt(i);
            }

        }

        return string.Join(" ", parsed);
    }
}
