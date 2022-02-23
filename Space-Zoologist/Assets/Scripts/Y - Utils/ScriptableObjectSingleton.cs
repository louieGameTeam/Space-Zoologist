using System.Linq;
using UnityEngine;

public class ScriptableObjectSingleton<BaseType> : ScriptableObject 
    where BaseType : ScriptableObjectSingleton<BaseType>
{
    #region Private Properties
    private static string FilePath => typeof(BaseType).Name;
    #endregion

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
                // Try to get a scriptable object at the file path
                BaseType scriptableObject = Resources.Load<BaseType>(FilePath);

                // If some objects were found then set the instance to the first one
                if (scriptableObject) instance = scriptableObject;
                // If no instances found then throw exception
                else throw new MissingReferenceException(
                    $"Expected to find a scriptable object of type '{typeof(BaseType).Name}' " +
                    $"at any resources path 'Resources/{FilePath}', but no such scriptable " +
                    $"object could be found. Please create an object of type " +
                    $"'{typeof(BaseType).Name}' at any resources path 'Resources/{FilePath}'");
            }
            // If instance is not null return it
            return instance;
        }
    }
    #endregion
}
