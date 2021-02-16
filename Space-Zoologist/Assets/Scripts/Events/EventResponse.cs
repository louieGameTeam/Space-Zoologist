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
    [SerializeField] public Population TargetPopulation = default;
    [SerializeField] List<EventResponseData> ResponsesData = default;
    public eventResponse response = default;

    public void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType, CheckEventTriggers);
    }

    // Triggerse response if population size is target size. Unsubscribes once all responses triggered.
    public void CheckEventTriggers()
    {
        int alreadyTriggered = 0;
        foreach(EventResponseData eventResponseData in ResponsesData)
        {
            if (eventResponseData.PopulationTriggerSize == -1)
            {
                alreadyTriggered++;
                continue;
            }
            else if (eventResponseData.PopulationTriggerSize == TargetPopulation.AnimalPopulation.Count)
            {
                triggerResponse(eventResponseData);
                eventResponseData.PopulationTriggerSize = -1;
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
