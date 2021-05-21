using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// TODO setup resource counter and potentially refactor response data to be cleaner. Then setup scenarios and test more.
/// <summary>
/// Sets up serial responses with different data for each response.
/// </summary>
/// <param name="resourceName"></param>
/// <param name="amount"></param>
public delegate void eventResponse(string resourceName, int amount);
[System.Serializable]
public class EventResponse
{
    [SerializeField] public EventType EventType = default;
    [SerializeField] public DialogueManager DialogueManager = default;
    [SerializeField] public PopulationManager PopulationManager = default;
    [SerializeField] string PopulationName = default;
    [SerializeField] List<EventResponseData> ResponsesData = default;
    public eventResponse response = default;

    public void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType, CheckEventTriggers);
        foreach (Population population in PopulationManager.Populations)
        {
            Debug.Log(population.gameObject.name);
        }
    }

    // Triggerse response if population size is target size. Unsubscribes once all responses triggered.
    public void CheckEventTriggers()
    {
        int alreadyTriggered = 0;
        foreach(EventResponseData eventResponseData in ResponsesData)
        {
            //Debug.Log(((Population)EventManager.Instance.EventData).gameObject.name);
            if (eventResponseData.PopulationTriggerSize == -1)
            {
                alreadyTriggered++;
                continue;
            }
            // TODO refactor eventresponse manager to map to specific populations
            foreach (Population population in PopulationManager.Populations)
            {
                if (eventResponseData.PopulationTriggerSize == population.AnimalPopulation.Count)
                {
                    triggerResponse(eventResponseData);
                    eventResponseData.PopulationTriggerSize = -1;
                }
            }
        }
        if (alreadyTriggered == ResponsesData.Count)
        {
            EventManager.Instance.UnsubscribeToEvent(EventType, CheckEventTriggers);
        }
    }

    public void triggerResponse(EventResponseData eventResponseData)
    {
        if (response == null)
        {
            return;
        }
        response(eventResponseData.resourceName, eventResponseData.amount);
        if (eventResponseData.NPCConversation != null)
        {
            DialogueManager.SetNewDialogue(eventResponseData.NPCConversation);
        }
    }
}

[System.Serializable]
public class EventTrigger
{
    [SerializeField] public Population Population = default;
}
