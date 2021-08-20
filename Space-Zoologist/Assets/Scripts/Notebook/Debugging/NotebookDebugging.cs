using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NotebookDebugging : MonoBehaviour, IScrollHandler
{
    public ScrollRect scroller;

    // Delegate the on scroll event back up to some parent
    public void OnScroll(PointerEventData data)
    {
        scroller.OnScroll(data);
    }
}
