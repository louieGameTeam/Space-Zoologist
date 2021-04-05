using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineManager : MonoBehaviour
{
    [SerializeField] LiquidMachineHUD LiquidMachineHUD = default;
    [SerializeField] AtmosphereMachineHUD AtmosphereMachineHUD = default;
    [SerializeField] NeedSystemManager NeedSystemManager = default;

    public void Start()
    {
        this.LiquidMachineHUD.SetupDependencies(this.NeedSystemManager);
        this.AtmosphereMachineHUD.SetupDependencies(this.NeedSystemManager);
    }
}
