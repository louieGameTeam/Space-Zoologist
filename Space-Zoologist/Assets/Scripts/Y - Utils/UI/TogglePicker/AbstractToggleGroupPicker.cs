using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class AbstractToggleGroupPicker : MonoBehaviour
{
    #region Public Properties
    public IReadOnlyList<AbstractTogglePicker> AbstractPickers => pickers;
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
    protected List<AbstractTogglePicker> pickers = null;
    [SerializeField]
    [Tooltip("Event invoked when any toggle state in any child picker changes")]
    private UnityEvent onToggleStateChanged = null;
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
    public void SetObjectsPicked(List<object> objects)
    {
        foreach (AbstractTogglePicker picker in pickers)
        {
            picker.Toggle.isOn = objects.Contains(picker.GetObjectPicked());
        }
    }
    public void SetObjectPicked(object obj) => SetObjectsPicked(new List<object>() { obj });
    // Get the toggle picker that picks the given object
    public AbstractTogglePicker GetTogglePicker(object objectPicked)
    {
        return pickers.Find(picker => picker.GetObjectPicked().Equals(objectPicked));
    }
    #endregion

    #region Private Methods
    private void SetupListeners()
    {
        foreach (AbstractTogglePicker picker in pickers)
        {
            //state check in order to prevent double invoke, one for previous toggle being deactivated and one for new toggle being activated
            UnityAction<bool> listener = (bool val) =>
            {
                if (val)
                    onToggleStateChanged.Invoke();
            };
            // Remove then re-add the listener so it is not added twice
            picker.Toggle.onValueChanged.RemoveListener(listener);
            picker.Toggle.onValueChanged.AddListener(listener);
        }
    }
    #endregion
}
