using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtmosphereMachineHUD : MonoBehaviour
{
    [SerializeField] List<MachineValues> MachineValues = default;
    AtmosphereMachine CurrentMachine = default;
    private NeedSystemManager NeedSystemManager = default;

    public void SetupDependencies(NeedSystemManager needSystemManager)
    {
        this.NeedSystemManager = needSystemManager;
    }

    public void Initialize(AtmosphericComposition atmosphericComposition, AtmosphereMachine currentMachine)
    {
        this.CurrentMachine = currentMachine;
        foreach (MachineValues atm in this.MachineValues)
        {
            switch(atm.MachineValueType)
            {
                case(MachineValueType.X):
                    atm.StartingValue = atmosphericComposition.GasX;
                    break;
                case(MachineValueType.Y):
                    atm.StartingValue = atmosphericComposition.GasY;
                    break;
                case(MachineValueType.Z):
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
        foreach (MachineValues atm in this.MachineValues)
        {
            atm.UpdateStartingValue();
            switch(atm.MachineValueType)
            {
                case(MachineValueType.X):
                    atmosphericComposition.GasX = atm.StartingValue;
                    break;
                case(MachineValueType.Y):
                    atmosphericComposition.GasY = atm.StartingValue;
                    break;
                case(MachineValueType.Z):
                    atmosphericComposition.GasZ = atm.StartingValue;
                    break;
            }
        }
        this.CurrentMachine.UpdateAtmosphere(atmosphericComposition);
        this.NeedSystemManager.UpdateSystem(NeedType.Atmosphere);
    }

    public void ApplyStartingValues()
    {
        foreach (MachineValues atm in this.MachineValues)
        {
            atm.ApplyStartingValue();
        }
    }

    public void CloseHUD()
    {
        this.gameObject.SetActive(false);
    }
}
