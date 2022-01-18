using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        Debug.Log("Whodunnit?!");
    }
}
