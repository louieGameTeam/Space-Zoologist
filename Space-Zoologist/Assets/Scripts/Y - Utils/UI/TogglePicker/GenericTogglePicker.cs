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
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Object that this toggle will pick")]
    private PickType valuePicked;
    #endregion

    #region Public Methods
    public override object GetObjectPicked() => valuePicked;
    public override void SetObjectPicked(object obj)
    {
        valuePicked = (PickType)obj;
        base.SetObjectPicked(obj);
    }
    #endregion
}
