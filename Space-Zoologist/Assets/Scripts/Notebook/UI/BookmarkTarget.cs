using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Identifies a ui object in the Notebook
/// that can be the target of a bookmark. Most
/// LevelID pickers and ItemPicker components
/// have one of these attached
/// </summary>
public class BookmarkTarget : NotebookUIChild
{
    #region Private Fields
    private Func<object> targetComponentDataGetter = () => null;
    private Action<object> targetComponentDataSetter = x => { };
    #endregion

    #region Public Methods
    public void Setup(Func<object> targetComponentDataGetter, Action<object> targetComponentDataSetter)
    {
        base.Setup();
        this.targetComponentDataGetter = targetComponentDataGetter;
        this.targetComponentDataSetter = targetComponentDataSetter;
    }
    public object GetTargetComponentData() => targetComponentDataGetter.Invoke();
    public void SetTargetComponentData(object data) => targetComponentDataSetter.Invoke(data);
    #endregion
}
