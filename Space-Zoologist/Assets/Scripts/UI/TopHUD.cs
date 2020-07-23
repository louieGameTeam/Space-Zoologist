using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles all logic for the TopHUD
/// </summary>
public class TopHUD : MonoBehaviour
{
    // Shared scriptable object variable holding the current player balance.
    [SerializeField] private IntVariable playerBalance = default;
    [SerializeField] private Text playerBalanceText = default;

    private void Update()
    {
        playerBalanceText.text = $"${playerBalance.RuntimeValue}";
    }
}
