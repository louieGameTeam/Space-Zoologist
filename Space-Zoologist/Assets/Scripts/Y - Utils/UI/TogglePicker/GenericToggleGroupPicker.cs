using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericToggleGroupPicker<PickType> : AbstractToggleGroupPicker
{
    #region Public Properties
    public List<PickType> ObjectsPicked => pickers
        .Where(x => x.Toggle.isOn)
        .Select(x => (x as GenericTogglePicker<PickType>).ObjectPicked)
        .ToList();
    public PickType FirstObjectPicked
    {
        get
        {
            List<PickType> objects = ObjectsPicked;
            if (objects.Count > 0) return objects[0];
            else return default(PickType);
        }
    }
    #endregion

    #region Public Methods
    public void SetObjectsPicked(List<PickType> objects)
    {
        foreach(GenericTogglePicker<PickType> picker in pickers)
        {
            picker.Toggle.isOn = objects.Contains(picker.ObjectPicked);
        }
    }
    public void SetObjectPicked(PickType obj) => SetObjectsPicked(new List<PickType>() { obj });
    #endregion
}
