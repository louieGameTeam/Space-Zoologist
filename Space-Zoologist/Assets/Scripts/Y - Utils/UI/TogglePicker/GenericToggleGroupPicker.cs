using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericToggleGroupPicker<PickType> : AbstractToggleGroupPicker
{
    #region Public Properties
    public override List<object> ObjectsPicked => ValuesPicked.Select(element => (object)element).ToList();
    public List<PickType> ValuesPicked => pickers
        .Where(x => x.Toggle.isOn)
        .Select(x => (x as GenericTogglePicker<PickType>).ValuePicked)
        .ToList();
    public PickType FirstValuePicked
    {
        get
        {
            List<PickType> objects = ValuesPicked;
            if (objects.Count > 0) return objects[0];
            else return default;
        }
    }
    #endregion

    #region Public Methods
    public void SetValuesPicked(List<PickType> values)
    {
        foreach(GenericTogglePicker<PickType> picker in pickers)
        {
            picker.Toggle.isOn = values.Contains(picker.ValuePicked);
        }
    }
    public void SetValuePicked(PickType value) => SetValuesPicked(new List<PickType>() { value });
    #endregion
}
