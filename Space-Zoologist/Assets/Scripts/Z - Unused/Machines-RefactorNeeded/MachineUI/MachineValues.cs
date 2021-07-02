using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MachineValueType {X, Y, Z}
public class MachineValues : MonoBehaviour
{
    [SerializeField] Text MachineValue = default;
    [SerializeField] float ValueToIncrementBy = 1f;
    public MachineValueType MachineValueType = default;
    [HideInInspector]
    public float StartingValue = default;

    public void UpdateStartingValue()
    {
        this.StartingValue = float.Parse(this.MachineValue.text);
    }

    public void ApplyStartingValue()
    {
        this.MachineValue.text = this.StartingValue.ToString();
    }

    public void IncrementValue()
    {
        float value = float.Parse(this.MachineValue.text);
        if (value + ValueToIncrementBy <= 100)
        {
            value += ValueToIncrementBy;
            this.MachineValue.text = value.ToString();
        }
    }

    public void DecrementValue()
    {
        float value = float.Parse(this.MachineValue.text);
        if (value - ValueToIncrementBy >= 0)
        {
            value -= ValueToIncrementBy;
            this.MachineValue.text = value.ToString();
        }
    }
}
