using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTester : MonoBehaviour
{
    [SerializeField] EventResponseManager EventResponseManager = default;

    private void Start()
    {
        // EventResponseManager.InitializeResponseHandler(EventType.NPCDialogue, Test);
        // EventResponseManager.InitializeEventManagerResponse(EventType.PopulationCountIncreased, )
    }

    private void Test(string resourceName, int amount)
    {
        Debug.Log("ResourceName: " + resourceName + ", Amount: " + amount);
    }
}
