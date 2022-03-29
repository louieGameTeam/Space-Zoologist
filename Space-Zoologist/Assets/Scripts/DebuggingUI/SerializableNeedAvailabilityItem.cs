using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableNeedAvailabilityItem
{
    #region Public Fields
    public string name;
    public ItemID id;
    public int itemCount;
    public float amountAvailable;
    public bool isDrinkingWater;
    public LiquidBodyContent waterContent;
    #endregion

    #region Constructors
    public SerializableNeedAvailabilityItem(NeedAvailabilityItem item)
    {
        name = item.ID.ToString();
        id = item.ID;
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
