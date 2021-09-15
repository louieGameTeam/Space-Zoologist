using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class DialogueWarning : MonoBehaviour
{
    [SerializeField] PopulationManager populationManager = default;
    private Dictionary<Population, int> previousPopulationSize = new Dictionary<Population, int>();
    [SerializeField] DialogueManager dialogueManager = default;
    [SerializeField] NPCConversation warningDialouge = default;

    // Warn player when population decreases this much
    [SerializeField] int decreaseRateThreshold = 5;

    void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.NextDay, () =>
        {
            CheckPopulationSizes();
        });
    }

    private void CheckPopulationSizes()
    {
        foreach(Population population in populationManager.Populations)
        {
            if (previousPopulationSize.ContainsKey(population))
            {
                if (previousPopulationSize[population] >= population.Count + decreaseRateThreshold)
                {
                    dialogueManager.SetNewDialogue(warningDialouge);
                    dialogueManager.StartInteractiveConversation();
                }
                previousPopulationSize[population] = population.Count;
            }
            else
            {
                previousPopulationSize.Add(population, population.Count);
            }
        }
    }
}
