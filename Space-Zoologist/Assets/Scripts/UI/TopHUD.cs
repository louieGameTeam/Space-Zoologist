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
    [SerializeField] private GameObject Warning = default;
    [SerializeField] private Text WarningText = default;

    public IEnumerator FlashWarning(string text)
    {
        if (!this.Warning.activeSelf)
        {
            this.Warning.SetActive(true);
        }
        Debug.Log("flashing warning");
        this.WarningText.text = text;
        yield return new WaitForSeconds(2f);
        this.WarningText.text = "";
        this.Warning.SetActive(false);
    }

    private void Update()
    {
        playerBalanceText.text = $"${GameManager.Instance.Balance}";
    }
}
