using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField] List<EventResponseData> ResponsesData = default;
    int currentResponse = 0;
    public eventResponse response = default;

    public void triggerResponse()
    {
        if (currentResponse >= ResponsesData.Count)
        {
            return;
        }
        response(ResponsesData[currentResponse].resourceName, ResponsesData[currentResponse].amount);
        currentResponse++;
    }
}
