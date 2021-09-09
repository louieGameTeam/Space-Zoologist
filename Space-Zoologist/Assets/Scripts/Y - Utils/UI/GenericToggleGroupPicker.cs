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
    #endregion
}
