using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizOption
{
    #region Public Properties
    public string Label => label;
    public int Weight => weight;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("String describing the option")]
    private string label;
    [SerializeField]
    [Tooltip("Amount that the option changes the score when picked")]
    private int weight;
    #endregion
}
