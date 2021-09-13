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
    [Tooltip("List of toggles in this group")]
    protected List<AbstractTogglePicker> pickers;
    [SerializeField]
    [Tooltip("Event invoked when any toggle state in any child picker changes")]
    private UnityEvent onToggleStateChanged;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        Setup();
    }
    #endregion

    #region Public Methods
    public void Setup()
    {
        foreach(AbstractTogglePicker picker in pickers)
        {
            UnityAction<bool> listener = _ => onToggleStateChanged.Invoke();
            // Remove then re-add the listener so it is not added twice
            picker.Toggle.onValueChanged.RemoveListener(listener);
            picker.Toggle.onValueChanged.AddListener(listener);
        }
    }
    public void SetTogglePicked(int toggle)
    {
        for(int i = 0; i < pickers.Count; i++)
        {
            pickers[i].Toggle.isOn = i == toggle;
        }
    }
    #endregion
}
