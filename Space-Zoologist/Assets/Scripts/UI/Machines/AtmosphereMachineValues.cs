using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtmosphereMachineValues : MonoBehaviour
{
    [SerializeField] Text MachineValue = default;
    [SerializeField] public AtmosphereComponent AtmosphereComponent = default;
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
        if (value < 100)
        {
            value++;
            this.MachineValue.text = value.ToString();
        }
    }

    public void DecrementValue()
    {
        float value = float.Parse(this.MachineValue.text);
        if (value > 0)
        {
            value--;
            this.MachineValue.text = value.ToString();
        }
    }
}
