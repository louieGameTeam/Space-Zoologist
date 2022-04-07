using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableFoodSource
{
    #region Public Fields
    public float foodOutput;
    public Vector2 position;
    #endregion

    #region Constructors
    public SerializableFoodSource(FoodSource food)
    {
        foodOutput = food.FoodOutput;
        position = food.Position;
    }
    #endregion
}
