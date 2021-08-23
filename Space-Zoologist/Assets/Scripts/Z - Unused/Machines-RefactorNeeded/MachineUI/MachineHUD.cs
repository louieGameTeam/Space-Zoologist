using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineHUD : MonoBehaviour
{
    protected Machine CurrentMachine;

    public virtual void Initialize(Machine machine)
    {
        this.CurrentMachine = machine;
    }
    public void CloseHUD()
    {
        this.gameObject.SetActive(false);
    }
}
