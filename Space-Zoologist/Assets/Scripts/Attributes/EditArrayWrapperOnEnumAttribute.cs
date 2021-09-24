using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditArrayWrapperOnEnumAttribute : WrappedPropertyAttribute
{
    #region Public Properties
    public System.Type EnumType => enumType;
    #endregion

    #region Private Fields
    private System.Type enumType;
    #endregion

    #region Constructors
    public EditArrayWrapperOnEnumAttribute(System.Type enumType) : this("array", enumType) { }
    public EditArrayWrapperOnEnumAttribute(string wrappedPropertyPath, System.Type enumType) : base(wrappedPropertyPath)
    {
        this.enumType = enumType;
    }
    #endregion
}
