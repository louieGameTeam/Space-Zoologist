using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles all logic for the TopHUD
/// </summary>
public class TopHUD : MonoBehaviour
{
    [SerializeField] private Text playerBalanceText = default;
    private PlayerBalance PlayerBalance = default;

    public void SetupPlayerBalance(PlayerBalance playerBalance)
    {
        this.PlayerBalance = playerBalance;
    }

    private void Update()
    {
        playerBalanceText.text = $"${this.PlayerBalance.Balance}";
    }
}
