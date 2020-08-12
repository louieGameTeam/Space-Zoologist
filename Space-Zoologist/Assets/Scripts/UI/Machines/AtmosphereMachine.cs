using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereMachine : MonoBehaviour
{
    [SerializeField] List<AtmosphereMachineValues> AtmosphereMachineValues = default;

    public void Start()
    {
        this.UpdateStartingValues();
    }

    public void UpdateStartingValues()
    {
        foreach (AtmosphereMachineValues atm in this.AtmosphereMachineValues)
        {
            atm.UpdateStartingValue();
        }
    }

    public void RevertStartingValues()
    {
        foreach (AtmosphereMachineValues atm in this.AtmosphereMachineValues)
        {
            atm.RevertStartingValue();
        }
    }
}
