using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AbstractTogglePicker : MonoBehaviour
{
    #region Public Properties
    public AbstractToggleGroupPicker Group => group;
    public Toggle Toggle => toggle;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Group that this toggle picker is in")]
    private AbstractToggleGroupPicker group;
    [SerializeField]
    [Tooltip("Reference to the toggle that picks the object")]
    private Toggle toggle;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        group.RegisterPicker(this);
        toggle.onValueChanged.AddListener(OnToggleStateChanged);
    }
    #endregion

    #region Private Methods
    private void OnToggleStateChanged(bool state)
    {
        group.OnToggleStateChanged.Invoke();
    }
    #endregion
}
