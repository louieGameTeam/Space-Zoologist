using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectMode : MonoBehaviour
{
    private bool isInInspectorMode = false;

    [SerializeField]
    private GameObject HUD = null;
    [SerializeField]
    private GameObject inspectorWindow = null;

    public void TuggleInspectMode()
    {
        this.isInInspectorMode = !isInInspectorMode;
        this.inspectorWindow.SetActive(isInInspectorMode);
        this.HUD.SetActive(!isInInspectorMode);
        //Debug.Log($"Inspector mode is {this.isInInspectorMode}");
    }
}
