using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is to attached to the inspector button and handles
/// entering/exiting inspect mode, getting 
/// </summary>
public class InspectMode : MonoBehaviour
{
    private bool isInInspectorMode = false;

    [SerializeField] private Text inspectorButtonText = null;
    [SerializeField] private NeedSystemUpdater needSystemUpdater = null;

    [SerializeField] private GameObject HUD = null;
    // The window to display
    [SerializeField] private GameObject inspectorWindow = null;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Player can only enter inspect mode when not in store</remarks>
    public void ToggleInspectMode()
    {
        // Cannot enter inspector mode while in istore
        if (this.needSystemUpdater.isInStore)
        {
            return;
        }

        // Toggle flag
        this.isInInspectorMode = !isInInspectorMode;

        // Toggle button text, displays and pause/free animals
        if (this.isInInspectorMode)
        {
            this.inspectorButtonText.text = "INSPECTOR:ON";
            this.needSystemUpdater.PauseAllAnimals();
            this.inspectorWindow.SetActive(true);
            this.HUD.SetActive(false);
        }
        else
        {
            this.inspectorButtonText.text = "INSPECTOR:OFF";
            this.needSystemUpdater.UnpauseAllAnimals();
            this.inspectorWindow.SetActive(false);
            this.HUD.SetActive(true);
        }

        //Debug.Log($"Inspector mode is {this.isInInspectorMode}");
    }
}
