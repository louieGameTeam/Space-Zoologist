using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO setup so can only be placed in liquid and only modifies the values in that liquid
public class LiquidMachine : MonoBehaviour
{
    private GameObject LiquidHUDGameObject = default;
    private TileSystem TileSystem = default;
    private LiquidMachineHUD LiquidMachineHUD = default;

    public void Start()
    {
        this.LiquidMachineHUD = this.LiquidHUDGameObject.GetComponent<LiquidMachineHUD>();
    }

    public void Initialize(TileSystem tileSystem, GameObject liquidMachineHUD)
    {
        this.TileSystem = tileSystem;
        this.LiquidHUDGameObject = liquidMachineHUD;
    }

    void OnMouseDown()
    {
        if (!this.LiquidHUDGameObject.activeSelf) this.OpenHUD();
    }

    public void OpenHUD()
    {
        this.LiquidHUDGameObject.SetActive(!this.LiquidHUDGameObject.activeSelf);
        if (this.LiquidHUDGameObject.activeSelf)
        {
            GameTile tile = this.TileSystem.GetGameTileAt(this.TileSystem.WorldToCell(this.gameObject.transform.position));
            this.LiquidMachineHUD.Initialize(this.TileSystem.GetTileContentsAt(this.TileSystem.WorldToCell(this.gameObject.transform.position), tile), this);
        }
    }

    public void UpdateLiquid(float[] liquidComposition)
    {
        this.TileSystem.ChangeLiquidBodyComposition(this.TileSystem.WorldToCell(this.gameObject.transform.position), liquidComposition);
    }
}
