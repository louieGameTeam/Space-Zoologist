using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Initialize EventResponses with callbacks based on their Event Type.
/// </summary>
public class EventResponseManager : MonoBehaviour
{
    [SerializeField] List<EventResponse> EventResponses = default;
    private List<EventType> eventTypes = new List<EventType>();
    public List<Action> responses = new List<Action>();
    public List<EventType> EventTypes => eventTypes;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void InitializeEventManagerResponse(EventType eventType)
    {
        foreach (EventResponse eventResponse in EventResponses)
        {
            if (eventResponse.EventType.Equals(EventType.NPCDialogue))
            {
                EventManager.Instance.SubscribeToEvent(eventType, eventResponse.triggerResponse);
                return;
            }
        }
    }

    // Triggers first dialogue type response
    public void TriggerDialogueResponse()
    {
        foreach (EventResponse eventResponse in EventResponses)
        {
            if (eventResponse.EventType.Equals(EventType.NPCDialogue))
            {
                eventResponse.triggerResponse();
                return;
            }
        }
    }

    // Sets up response to be triggered multiple times with different data according to eventType.
    public void InitializeResponseHandler(EventType eventType, eventResponse response)
    {
        foreach (EventResponse eventResponse in EventResponses)
        {
            if (eventResponse.EventType.Equals(eventType))
            {
                eventResponse.response = response;
                return;
            }
        }
    }
}
