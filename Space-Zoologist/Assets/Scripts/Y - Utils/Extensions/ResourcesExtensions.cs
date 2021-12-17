using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourcesExtensions
{
    #region Public Methods
    public static GameObject InstantiateFromResources(string path, Transform parent)
    {
        // Load the prefab at the given path
        GameObject prefab = Resources.Load<GameObject>(path);

        // If the instance exists try to get the component and create it
        if (prefab)
        {
            return Object.Instantiate(prefab, parent);
        }
        // If no resource could be loaded then throw an exception
        else throw new MissingReferenceException("Attempted to instantiate prefab from resources at path '" +
            path + "' but no such prefab could be found found.");
    }
    public static TComponent InstantiateFromResources<TComponent>(string path, Transform parent)
        where TComponent : Component
    {
        // Load the prefab at the given path
        GameObject prefab = Resources.Load<GameObject>(path);

        // If the instance exists try to get the component and create it
        if (prefab)
        {
            TComponent component = prefab.GetComponent<TComponent>();

            // If component exists then instantiate it
            if (component)
            {
                return Object.Instantiate(component, parent);
            }
            // If the prefab does not have the right component then throw an exception
            else throw new MissingComponentException("Attempted to instantiate component on prefab at path '" +
                path + "' but no such component could be found");
        }
        // If no resource could be loaded then throw an exception
        else throw new MissingReferenceException("Attempted to load prefab at path '" +
            path + "' but no such prefab could found.");
    }
    #endregion
}
