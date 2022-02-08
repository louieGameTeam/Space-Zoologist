using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static TComponent GetOrAddComponent<TComponent>(this Component component)
        where TComponent : Component
    {
        return component.gameObject.GetOrAddComponent<TComponent>();
    }
}
