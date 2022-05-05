using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableNeedAvailability
{
    #region Public Fields
    public SerializableNeedAvailabilityItem[] items;
    #endregion

    #region Constructors
    public SerializableNeedAvailability(NeedAvailability availability)
    {
        items = availability
            .Items
            .Select(item => new SerializableNeedAvailabilityItem(item))
            .ToArray();
    }
    #endregion
}
