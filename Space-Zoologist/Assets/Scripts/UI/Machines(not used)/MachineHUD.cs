using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineHUD : MonoBehaviour
{
    protected Machine CurrentMachine;
    protected NeedSystemManager NeedSystemManager = default;
    public void SetupDependencies(NeedSystemManager needSystemManager)
    {
        this.NeedSystemManager = needSystemManager;
    }

    public virtual void Initialize(Machine machine)
    {
        this.CurrentMachine = machine;
    }
    public void CloseHUD()
    {
        this.gameObject.SetActive(false);
    }
}
