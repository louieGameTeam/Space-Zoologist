using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectSingleton<BaseType> : ScriptableObject where BaseType : ScriptableObject
{
    #region Private Fields
    private static BaseType instance = null;
    #endregion

    #region Protected Methods
    protected static BaseType GetOrCreateInstance(string typename, string path)
    {
        if (!instance)
        {
            // Load the resource
            instance = Resources.Load<BaseType>(path);

            // If the instance still could not be loaded then throw an exception
            if (!instance) throw new MissingReferenceException(
                typename + ": no instance of type '" + typename +
                "' could be loaded from the resources folder. Make sure an instance of type " +
                typename + " exists in the assets folder at the path '" + path + "'");
        }
        // If instance is not null return it
        return instance;
    }
    #endregion
}
