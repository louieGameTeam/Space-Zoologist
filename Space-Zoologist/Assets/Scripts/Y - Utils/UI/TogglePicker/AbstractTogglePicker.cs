using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class AbstractTogglePicker : MonoBehaviour
{
    #region Public Typedefs
    [System.Serializable]
    public class ObjectEvent : UnityEvent<object> { }
    #endregion

    #region Public Properties
    public Toggle Toggle => toggle;
    public ObjectEvent OnObjectPickedChanged => onObjectPickedChanged;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the toggle that picks the object")]
    private Toggle toggle;
    [SerializeField]
    [Tooltip("Event invoked anytime the value of the object this toggle picked is changed")]
    private ObjectEvent onObjectPickedChanged;
    #endregion

    #region Public Methods
    public abstract object GetObjectPicked();
    public virtual void SetObjectPicked(object obj)
    {
        onObjectPickedChanged.Invoke(obj);
    }
    #endregion
}
