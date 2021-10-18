using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives information about what knowledge a given quiz question is designed to test
/// </summary>
[System.Serializable]
public struct QuizCategory
{
    #region Public Properties
    public ItemID Item => item;
    public NeedType Need => need;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("The item that the quiz is testing on")]
    private ItemID item;
    [SerializeField]
    [Tooltip("The need that the quiz is testing on")]
    private NeedType need;
    #endregion

    #region Operators
    public static bool operator ==(QuizCategory a, QuizCategory b) => a.item == b.item && a.need == b.need;
    public static bool operator !=(QuizCategory a, QuizCategory b) => !(a == b);
    #endregion

    #region Object Overrides
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        else if (obj.GetType() == GetType()) return this == (QuizCategory)obj;
        else return false;
    }
    public override int GetHashCode()
    {
        return item.GetHashCode() + need.GetHashCode();
    }
    #endregion
}
