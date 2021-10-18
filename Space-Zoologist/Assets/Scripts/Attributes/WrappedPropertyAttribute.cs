using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappedPropertyAttribute : PropertyAttribute
{
    #region Public Properties
    public string WrappedPropertyPath => wrappedPropertyPath;
    #endregion

    #region Protected Fields
    protected string wrappedPropertyPath;
    #endregion

    #region Constructors
    public WrappedPropertyAttribute(string wrappedPropertyPath)
    {
        this.wrappedPropertyPath = wrappedPropertyPath;
    }
    #endregion
}
