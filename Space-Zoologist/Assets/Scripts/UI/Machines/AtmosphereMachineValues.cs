using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtmosphereMachineValues : MonoBehaviour
{
    [SerializeField] Text MachineValue = default;
    int StartingValue = default;

    public void UpdateStartingValue()
    {
        this.StartingValue = Int32.Parse(this.MachineValue.text);
    }

    public void RevertStartingValue()
    {
        this.MachineValue.text = this.StartingValue.ToString();
    }

    public void IncrementValue()
    {
        int value = Int32.Parse(this.MachineValue.text);
        if (value < 100)
        {
            value++;
            this.MachineValue.text = value.ToString();
        }
    }

    public void DecrementValue()
    {
        int value = Int32.Parse(this.MachineValue.text);
        if (value > 0)
        {
            value--;
            this.MachineValue.text = value.ToString();
        }
    }
}
