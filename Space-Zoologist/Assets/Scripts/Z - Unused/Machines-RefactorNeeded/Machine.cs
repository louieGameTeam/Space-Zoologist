using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    public Vector3Int position;
    [SerializeField] protected GameObject machineHUDGO;
    protected MachineHUD machineHUD;
    public virtual void Initialize()
    {
        this.machineHUD = machineHUDGO.GetComponent<MachineHUD>();
    }
    public virtual void OpenHUD()
    {

    }
    public void SetPosition(Vector3Int pos)
    {
        this.position = pos;
    }
}
