using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EventType { };

/// <summary>
/// Publisher of a type of event
/// </summary>
public class Publisher
{
    public event Action Publish;

    public void PublishTheEvent()
    {
        if (Publish != null)
            Publish();
    }
}

/// <summary>
/// This system will act as a publisher and keep track of a list of subs.
/// An event can be invoke by anyone but only the subscribers will be notified.
/// </summary>
public class EventManager : MonoBehaviour
{
    private Dictionary<EventType, Publisher> eventPublishers = new Dictionary<EventType, Publisher>();

    /// <summary>
    /// Invoke an event.
    /// </summary>
    /// <param name="eventType">Type of the event that just happens</param>
    public void InvokeEvent(EventType eventType)
    {
        this.eventPublishers[eventType].PublishTheEvent();
    }

    /// <summary>
    /// Suscribe to an event with a corresponding action.
    /// </summary>
    /// <param name="eventType">The event that suscripber is interested in</param>
    /// <param name="action">What action to be triggered when event happens</param>
    public void SubscribeToEvent(EventType eventType, Action action)
    {
        this.eventPublishers[eventType].Publish += action;
    }

    /// <summary>
    /// Unsubscribes to an event
    /// </summary>
    public void UnsubscribeToEvent(EventType eventType, Action action)
    {
        this.eventPublishers[eventType].Publish -= action;
    }
}