using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiquidMachineHUD : MonoBehaviour
{
    [SerializeField] List<MachineValues> MachineValues = default;
    LiquidMachine CurrentMachine = default;

    public void Initialize(float[] liquidComposition, LiquidMachine currentMachine)
    {
        this.CurrentMachine = currentMachine;
        foreach (MachineValues atm in this.MachineValues)
        {
            switch(atm.MachineValueType)
            {
                case(MachineValueType.X):
                    atm.StartingValue = liquidComposition[0];
                    break;
                case(MachineValueType.Y):
                    atm.StartingValue = liquidComposition[1];
                    break;
                case(MachineValueType.Z):
                    atm.StartingValue = liquidComposition[2];
                    break;
            }
            atm.ApplyStartingValue();
        }
    }

    /// <summary>
    /// Update the Liquid with the current HUD values
    /// </summary>
    public void UpdateStartingValues()
    {
        float[] liquidComposition = new float[3];
        foreach (MachineValues atm in this.MachineValues)
        {
            atm.UpdateStartingValue();
            switch(atm.MachineValueType)
            {
                case(MachineValueType.X):
                    liquidComposition[0] = atm.StartingValue;
                    break;
                case(MachineValueType.Y):
                    liquidComposition[1] = atm.StartingValue;
                    break;
                case(MachineValueType.Z):
                    liquidComposition[2] = atm.StartingValue;
                    break;
            }
        }
        this.CurrentMachine.UpdateLiquid(liquidComposition);
        GameManager.Instance.UpdateNeedSystem(NeedType.Liquid);
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
