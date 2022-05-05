using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableNeedAvailabilityItem
{
    #region Public Fields
    public string id;
    public int itemCount;
    public float amountAvailable;
    public bool isDrinkingWater;
    public LiquidBodyContent waterContent;
    #endregion

    #region Constructors
    public SerializableNeedAvailabilityItem(NeedAvailabilityItem item)
    {
        id = item.ID.ToString();
        itemCount = item.ItemCount;
        amountAvailable = item.AmountAvailable;
        isDrinkingWater = item.IsDrinkingWater;

        if (isDrinkingWater)
        {
            waterContent = item.WaterContent;
        }
        else waterContent = null;
    }
    #endregion
}
