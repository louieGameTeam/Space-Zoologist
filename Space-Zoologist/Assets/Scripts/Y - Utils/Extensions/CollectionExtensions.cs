using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionExtensions
{
    #region Public Methods
    public static bool IsNullOrEmpty(System.Array array) => array == null || array.Length <= 0;
    #endregion
}