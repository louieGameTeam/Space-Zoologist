using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTogglePicker<PickType> : AbstractTogglePicker
{
    #region Public Properties
    public PickType ValuePicked
    {
        get => valuePicked;
        set => valuePicked = value;
    }
    public override object ObjectPicked { get => ValuePicked; set => ValuePicked = (PickType)value; }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Object that this toggle will pick")]
    private PickType valuePicked;
    #endregion
}
