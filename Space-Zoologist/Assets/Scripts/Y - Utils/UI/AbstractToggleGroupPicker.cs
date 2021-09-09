using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class AbstractToggleGroupPicker : MonoBehaviour
{
    #region Public Properties
    public UnityEvent OnToggleStateChanged => onToggleStateChanged;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Event invoked when any toggle state in any child picker changes")]
    private UnityEvent onToggleStateChanged;
    #endregion

    #region Protected Fields
    protected List<AbstractTogglePicker> pickers = new List<AbstractTogglePicker>();
    #endregion

    #region Public Methods
    public void RegisterPicker(AbstractTogglePicker picker)
    {
        pickers.Add(picker);
    }
    public void UnregisterPicker(AbstractTogglePicker picker)
    {
        pickers.Remove(picker);
    }
    #endregion
}
