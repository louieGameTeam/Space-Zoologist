using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO setup so can only be placed in liquid and only modifies the values in that liquid
public class LiquidMachine : MonoBehaviour
{
    private GameObject LiquidHUDGameObject = default;
    private ReservePartitionManager ReservePartitionManager = default;
    private LiquidMachineHUD LiquidMachineHUD = default;

    public void Start()
    {
        this.LiquidMachineHUD = this.LiquidHUDGameObject.GetComponent<LiquidMachineHUD>();
    }

    public void Initialize(ReservePartitionManager reservePartitionManager, GameObject liquidMachineHUD)
    {
        this.ReservePartitionManager = reservePartitionManager;
        this.LiquidHUDGameObject = liquidMachineHUD;
    }

    void OnMouseDown()
    {
        this.OpenHUD();
    }

    public void OpenHUD()
    {
        this.LiquidHUDGameObject.SetActive(!this.LiquidHUDGameObject.activeSelf);
        if (this.LiquidHUDGameObject.activeSelf)
        {
            //this.LiquidMachineHUD.Initialize(this.ReservePartitionManager.PopulationAccessibleLiquid(this.gameObject.transform.position), this);
        }
    }

    // TODO figure out how you can update the Liquid of an enclosed area
    public void UpdateLiquid(float[] liquidComposition)
    {
        //this.ReservePartitionManager.UpdateLiquidComposition(this.transform.position, atmosphericComposition);
    }
}
