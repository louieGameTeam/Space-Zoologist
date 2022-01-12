using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizOption
{
    #region Public Properties
    public string Label => label;
    public int Weight => weight;
    public string DisplayName => ComputeDisplayName(label, weight);
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("String describing the option")]
    private string label;
    [SerializeField]
    [Tooltip("Amount that the option changes the score when picked")]
    private int weight;
    #endregion

    #region Public Methods
    public static string ComputeDisplayName(string label, int weight) => $"{label} ({(weight > 0 ? "+" : "")}{weight})";
    #endregion
}
