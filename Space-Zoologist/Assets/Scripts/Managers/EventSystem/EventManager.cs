using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EventType {
    // TODO: If LogToggled isn't going to be used in the future, remove it
    StoreToggled, InspectorToggled, LogToggled, // Pass bool (true if opened, false if closed)
    NewPopulation, NewFoodSource, NewEnclosedArea, // Pass the created object
    PopulationCountChange, // Pass a tuple (population, bool (true if increased, false if decreased))
    PopulationGrowthChange, PopulationExtinct, // Pass the population
    FoodSourceChange, // Pass the food source
    TerrainChange, // Pass a list of change tiles
    LiquidChange, // Pass the cell posistion
    NPCDialogue,
    NextDay,
    MainObjectivesCompleted, GameOver // Pass null is fine
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
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void SubscribeToEvent(EventType eventType, Action handler)
    {
        Action<object> handlerWrapper = (args) => handler();
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
}