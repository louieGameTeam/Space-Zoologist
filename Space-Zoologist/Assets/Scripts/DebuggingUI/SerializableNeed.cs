using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableNeed
{
    #region Public Fields
    public string needName;
    public string needType;
    public float needValue;
    public float severity;
    public bool isPreferred;
    #endregion

    #region Constructors
    public SerializableNeed(Need need)
    {
        needName = need.NeedName;
        needType = need.NeedType.ToString();
        needValue = need.NeedValue;
        severity = need.Severity;
        isPreferred = need.IsPreferred;
    }
    #endregion
}