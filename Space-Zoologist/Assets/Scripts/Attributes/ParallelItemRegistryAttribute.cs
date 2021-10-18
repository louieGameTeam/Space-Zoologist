using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelItemRegistryAttribute : EditArrayWrapperOnEnumAttribute
{
    #region Public Properties
    public string InnerWrappedPropertyPath => innerWrappedPropertyPath;
    #endregion

    #region Private Fields
    private string innerWrappedPropertyPath;
    #endregion

    #region Constructors
    public ParallelItemRegistryAttribute(string wrappedPropertyPath, string innerWrappedPropertyPath) 
        : base(wrappedPropertyPath, typeof(ItemRegistry.Category))
    {
        this.innerWrappedPropertyPath = innerWrappedPropertyPath;
    }
    #endregion
}
