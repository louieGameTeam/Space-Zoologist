using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Each time a new AtmosphereMachine is placed, the store will make a new prefab of the UI and these scripts
// TODO setup machine section in the store
public class AtmosphereMachineHUD : MonoBehaviour
{
    [SerializeField] List<AtmosphereMachineValues> AtmosphereMachineValues = default;
    // The below fields will be injected from the store and the AtmosphericComposition will be calculated using the MachineLocation
    AtmosphericComposition AtmosphericComposition = default;
    AtmosphereMachine CurrentMachine = default;

    public void Initialize(AtmosphericComposition atmosphericComposition, AtmosphereMachine currentMachine)
    {
        this.CurrentMachine = currentMachine;
        foreach (AtmosphereMachineValues atm in this.AtmosphereMachineValues)
        {
            switch(atm.AtmosphereComponent)
            {
                case(AtmosphereComponent.GasX):
                    atm.StartingValue = atmosphericComposition.GasX;
                    break;
                case(AtmosphereComponent.GasY):
                    atm.StartingValue = atmosphericComposition.GasY;
                    break;
                case(AtmosphereComponent.GasZ):
                    atm.StartingValue = atmosphericComposition.GasZ;
                    break;
            }
            atm.ApplyStartingValue();
        }
    }

    /// <summary>
    /// Update the atmosphere with the current HUD values
    /// </summary>
    public void UpdateStartingValues()
    {
        AtmosphericComposition atmosphericComposition = new AtmosphericComposition();
        foreach (AtmosphereMachineValues atm in this.AtmosphereMachineValues)
        {
            atm.UpdateStartingValue();
            switch(atm.AtmosphereComponent)
            {
                case(AtmosphereComponent.GasX):
                    atmosphericComposition.GasX = atm.StartingValue;
                    break;
                case(AtmosphereComponent.GasY):
                    atmosphericComposition.GasY = atm.StartingValue;
                    break;
                case(AtmosphereComponent.GasZ):
                    atmosphericComposition.GasZ = atm.StartingValue;
                    break;
            }
        }
        this.CurrentMachine.UpdateAtmosphere();
    }

    public void ApplyStartingValues()
    {
        foreach (AtmosphereMachineValues atm in this.AtmosphereMachineValues)
        {
            atm.ApplyStartingValue();
        }
    }
}
