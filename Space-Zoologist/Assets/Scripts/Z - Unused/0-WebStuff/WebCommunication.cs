using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

// TODO setup pausing
public class WebCommunication : MonoBehaviour
{
    public bool IsInputEnabled = true;

    [DllImport("__Internal")]
    private static extern void ToggleJournal();

    public void ToggleJ()
    {
        try
        {
            ToggleJournal();

            IsInputEnabled = !IsInputEnabled;
        }
        catch
        {
            Debug.Log("Journal not hooked up");
        }


    }

    [DllImport("__Internal")]
    private static extern void ToggleCatalog();

    public void ToggleC()
    {
        try
        {
            ToggleCatalog();

            IsInputEnabled = !IsInputEnabled;
        }
        catch
        {
            Debug.Log("Catalogue not hooked up");
        }

    }



    public void EnableInput()
    {
        IsInputEnabled = !IsInputEnabled;
    }


    void Update()
    {
        if (IsInputEnabled == true)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
                WebGLInput.captureAllKeyboardInput = true;
#endif
        }
        else if (IsInputEnabled == false)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
                WebGLInput.captureAllKeyboardInput = false;
#endif
        }
    }
}
