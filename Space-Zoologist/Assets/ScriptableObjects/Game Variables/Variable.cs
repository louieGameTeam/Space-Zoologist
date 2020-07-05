using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Variable<T> : ScriptableObject
{

    public T DefaultValue;

    public T RuntimeValue { get; set; }

    protected void OnEnable()
    {
        RuntimeValue = DefaultValue;
    }
}

[CreateAssetMenu(fileName = "intVar", menuName = "Variables/Integer")]
public class IntVariable : Variable<int>
{

}

[CreateAssetMenu(fileName = "floatVar", menuName = "Variables/Float")]
public class FloatVariable : Variable<float>
{

}
