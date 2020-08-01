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
    private IntVariable playerBalance = default;
    [SerializeField] private Text playerBalanceText = default;
    [SerializeField] LevelDataReference LevelDataReference = default;

    private void Start()
    {
        this.playerBalance = LevelDataReference.LevelData.StartingBalance;
    }

    private void Update()
    {
        playerBalanceText.text = $"${playerBalance.RuntimeValue}";
    }
}
