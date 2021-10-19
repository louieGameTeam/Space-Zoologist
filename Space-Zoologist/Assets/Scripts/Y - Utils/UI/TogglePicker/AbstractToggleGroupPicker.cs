using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class AbstractToggleGroupPicker : MonoBehaviour
{
    #region Public Properties
    public abstract List<object> ObjectsPicked { get; }
    public object FirstObjectPicked
    {
        get
        {
            List<object> picked = ObjectsPicked;
            if (picked.Count > 0) return picked[0];
            else return default;
        }
    }
    public UnityEvent OnToggleStateChanged => onToggleStateChanged;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of toggles in this group")]
    protected List<AbstractTogglePicker> pickers;
    [SerializeField]
    [Tooltip("Event invoked when any toggle state in any child picker changes")]
    private UnityEvent onToggleStateChanged;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        SetupListeners();
    }
    #endregion

    #region Public Methods
    public void SetTogglePickers(List<AbstractTogglePicker> pickers)
    {
        this.pickers = pickers;
        SetupListeners();
    }
    public void SetTogglePicked(int toggle)
    {
        for(int i = 0; i < pickers.Count; i++)
        {
            pickers[i].Toggle.isOn = i == toggle;
        }
    }
    #endregion

    #region Private Methods
    private void SetupListeners()
    {
        foreach (AbstractTogglePicker picker in pickers)
        {
            UnityAction<bool> listener = _ => onToggleStateChanged.Invoke();
            // Remove then re-add the listener so it is not added twice
            picker.Toggle.onValueChanged.RemoveListener(listener);
            picker.Toggle.onValueChanged.AddListener(listener);
        }
    }
    #endregion
}
