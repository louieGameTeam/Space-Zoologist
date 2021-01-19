using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// How long each in-game day converts to real life seconds at 1x speed
/// </summary>

/// <summary>
/// Hanldes the update of NS
/// </summary>
public class NeedSystemUpdater : MonoBehaviour
{
    [SerializeField] NeedSystemManager NeedSystemManager = default;
    public const float InGameDayPeriod = 24;
    float timer = 0;

    public bool IsPaused { get; set; }

    // Temp update
    private void Update()
    {
        if (this.IsPaused)
        {
            return;
        }
        NeedSystemManager.UpdateSystems();


        timer += Time.deltaTime;
        if (timer >= InGameDayPeriod) {
            // Do something
            timer -= InGameDayPeriod;
        }
    }
}
