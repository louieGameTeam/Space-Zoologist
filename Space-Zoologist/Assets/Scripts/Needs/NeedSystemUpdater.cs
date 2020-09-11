using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hanldes the update of NS
/// </summary>
public class NeedSystemUpdater : MonoBehaviour
{
    [SerializeField] NeedSystemManager NeedSystemManager = default;

    public bool IsPaused { get; set; }

    // Temp update
    private void Update()
    {
        if (this.IsPaused)
        {
            return;
        }
        NeedSystemManager.UpdateSystems();
    }
}
