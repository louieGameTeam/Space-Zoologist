using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingScript : MonoBehaviour
{
    public struct ReflectionContainer
    {
        public string field;
    }

    private void Awake()
    {
        ReflectionContainer container = new ReflectionContainer
        {
            field = "Value 1"
        };
        Type containerType = container.GetType();
        FieldInfo fieldInfo = containerType.GetField("field");

        Debug.Log($"container.field = {container.field}");
        Debug.Log($"fieldInfo.GetValue(container) = {fieldInfo.GetValue(container)}");

        Debug.Log($"Calling fieldInfo.SetValue(container, \"Value 2\")");

        fieldInfo.SetValue(container, "Value 2");
        
        Debug.Log($"container.field = {container.field}");
        Debug.Log($"fieldInfo.GetValue(container) = {fieldInfo.GetValue(container)}");
    }
}
