﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EventType {
    // TODO: If LogToggled isn't going to be used in the future, remove it
    StoreToggled, InspectorToggled, LogToggled, InspectorSelectionChanged, 
    InspectorHoverTargetChange, // pass in hover target
    NewPopulation, NewFoodSource, NewEnclosedArea, // Pass the created object
    PopulationCountChange, // Pass a tuple (population, bool (true if increased, false if decreased))
    PopulationGrowthChange, PopulationExtinct, // Pass the population
    FoodSourceChange, // Pass the food source
    TerrainChange, TilemapChange, // Pass a list of change tiles
    LiquidChange, // Pass the cell posistion
    NPCDialogue,
    NextDay,
    MainObjectivesCompleted, GameOver, // Pass null is fine
    PopulationCacheRebuilt, FoodCacheRebuilt,
    PreCacheRebuild, // called before any rebuild, but only once (even if multiple rebuilds happen simutaneously)
    ReportBackStart, ReportBackEnd,
    OnJournalOpened, OnJournalClosed, OnTabChanged, OnArticleChanged, OnBookmarkAdded, // Pass null
    OnSetEnd, OnSetPass, OnSetFail, OnQuizEnd, // Pass null
    TriggerSave, // Pass null
};

/// <summary>
/// Publisher of a type of event
/// </summary>
public class Publisher
{ 
    public event Action<object> Handlers;

    public void PublishTheEvent(object eventArgs)
    {
        // Invoke the action when it is not null
        Handlers?.Invoke(eventArgs);
    }

    public void UnSubscribeAllMethods()
    {
        // Removes each action associates to this handler
        foreach (Action<object> handler in Handlers.GetInvocationList())
        {
            Handlers -= handler;
        }
    }

    public string PrintInvocationList()
    {
        string str = "Invocation list:";
        if (Handlers == null) return "";
        foreach (Action<object> handler in Handlers.GetInvocationList())
        {
            str += $"\n\tTarget: {handler.Target}, Method: {handler.Method.Name}";
        }
        str += "\n";
        return str;
    }
}

/// <summary>
/// This system will act as a publisher and keep track of a list of subs.
///
/// An event can be invoke by anyone but only the subscribers will be notified.
/// 
/// This is a singleton because we are not sure about what parts of the game could potentially
/// uses this, so making it accessible for everyone seems to be a good idea
/// and it does not holds any internal state.
/// </summary>
public class EventManager : MonoBehaviour
{
    private static EventManager instance;
    private Dictionary<EventType, Publisher> eventPublishers = new Dictionary<EventType, Publisher>();

    public static EventManager Instance => EventManager.instance;

    private Dictionary<Action,Action<object>> noParamWrappers = new Dictionary<Action, Action<object>>();

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;

            // Initialize publishers
            foreach (EventType eventType in Enum.GetValues(typeof(EventType)))
            {
                this.eventPublishers.Add(eventType, new Publisher());
            }
        }
    }

    /// <summary>
    /// Unsubscribes all methods for all events
    /// </summary>
    /// <remarks>Idlely the subscribers will unsubscribe, it is a fail-safe</remarks>
    public void ResetSystem()
    {
        foreach (Publisher publisher in this.eventPublishers.Values)
        {
            publisher.UnSubscribeAllMethods();
        }
    }

    /// <summary>
    /// Invoke an event.
    /// </summary>
    /// <param name="eventType">Type of the event that just happens</param>
    public void InvokeEvent(EventType eventType, object eventData)
    {
        this.eventPublishers[eventType].PublishTheEvent(eventData);
    }

    /// <summary>
    /// Suscribe to an event with a corresponding action.
    /// </summary>
    /// <param name="eventType">The event that suscripber is interested in</param>
    /// <param name="action">What action to be triggered when event happens</param>
    public void SubscribeToEvent(EventType eventType, Action<object> handler)
    {
        this.eventPublishers[eventType].Handlers += handler;
    }

    /// <summary>
    /// Wrapper delegate for event listeners that take no parameters
    /// Adds the wrapper to a dictionary for unsubscribing
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void SubscribeToEvent(EventType eventType, Action handler)
    {
        Action<object> handlerWrapper = (args) => handler();
        if(!noParamWrappers.ContainsKey(handler))
            noParamWrappers.Add(handler, handlerWrapper);
        this.eventPublishers[eventType].Handlers += handlerWrapper;
    }

    /// <summary>
    /// Unsubscribes to an event
    /// </summary>
    /// <remarks>Forget to unsubcribe would cause errors</remarks>
    public void UnsubscribeToEvent(EventType eventType, Action<object> handler)
    {
        this.eventPublishers[eventType].Handlers -= handler;
    }

    /// <summary>
    /// Unsubscribes to an event, taking in a no param action and looking up its wrapper
    /// </summary>
    /// <remarks>Forget to unsubcribe would cause errors</remarks>
    public void UnsubscribeToEvent(EventType eventType, Action handler)
    {
        if(noParamWrappers.ContainsKey(handler))
            this.eventPublishers[eventType].Handlers -= noParamWrappers[handler];
    }
}