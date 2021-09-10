using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTogglePicker<PickType> : AbstractTogglePicker
{
    #region Public Properties
    public PickType ObjectPicked => objectPicked;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Object that this toggle will pick")]
    private PickType objectPicked;
    #endregion
}
