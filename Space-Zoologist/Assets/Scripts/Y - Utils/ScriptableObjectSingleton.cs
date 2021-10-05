using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectSingleton<BaseType> : ScriptableObject where BaseType : ScriptableObjectSingleton<BaseType>
{
    #region Private Fields
    private static BaseType instance;
    #endregion

    #region Protected Properties
    protected static BaseType Instance
    {
        get
        {
            if (!instance)
            {
                BaseType[] baseTypes = Resources.LoadAll<BaseType>(string.Empty);

                // If some objects were found then set the instance to the first one
                if (baseTypes.Length > 0)
                {
                    instance = baseTypes[0];

                    if (baseTypes.Length > 1)
                    {
                        // Get a string listing the names of the objects found
                        string objectsFound = "\n\tObjects found:\n\t\t";
                        objectsFound += string.Join("\n\t\t", baseTypes.Select(obj => obj.name));

                        // Get a string displaying the object picked
                        string objectPicked = "\n\tObject picked:\n\t\t" + instance.name;

                        // Log a warning with the information
                        Debug.LogWarning("ScriptableObjectSingleton: found multiple scriptable objects where only ONE was expected" + objectsFound + objectPicked);
                    }
                }
                // If no instances found then throw exception
                else throw new MissingReferenceException(
                    "ScriptableObjectSinglet: no scriptable object with the desired type " +
                    "could be loaded from the resources folder. Make sure a scriptable object " +
                    "of the desired type exists somewhere in a directory with a parent file named 'Resources'");
            }
            // If instance is not null return it
            return instance;
        }
    }
    #endregion
}
