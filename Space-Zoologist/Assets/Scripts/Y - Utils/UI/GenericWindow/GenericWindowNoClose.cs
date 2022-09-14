using UnityEngine;
using UnityEngine.Events;

public class GenericWindowNoClose : GenericWindow {
    public override void Close (UnityAction action = null) {
        if (action != null) action.Invoke ();
    }
} 
