using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OnClickViewer : MonoBehaviour
{
    [SerializeField] private Button button;

    public void PrintButtonListeners()
    {
        UnityEvent buttonEvent = button.onClick;
        int totalRegisteredEvents = buttonEvent.GetPersistentEventCount();

        for (int i = 0; i < totalRegisteredEvents; ++i)
        {
            Debug.Log("Component: " + buttonEvent.GetPersistentTarget(i));
            Debug.Log("Method Name: " + buttonEvent.GetPersistentMethodName(i));
        }
    }
}
