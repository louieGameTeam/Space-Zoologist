using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO ensure only one atmosphere machine placed per atmosphere and cleanup placement validation
public class MachineStoreSection : StoreSection
{
    [SerializeField] private EnclosureSystem EnclosureSystem = default;
    [SerializeField] TileSystem TileSystem = default;
    [SerializeField] GameObject AtmosphereMachineHUD = default;
    [SerializeField] GameObject LiquidMachineHUD = default;
    [SerializeField] GameObject MachinePrefab = default;

    public override void Initialize()
    {
        base.itemType = ItemType.Machine;
        base.Initialize();
    }

    public override void OnCursorPointerUp(PointerEventData eventData)
    {
        base.OnCursorPointerUp(eventData);
        if (base.IsCursorOverUI(eventData) || !base.HasSupply(base.selectedItem))
        {
            base.OnItemSelectionCanceled();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            //if (!base.GridSystem.PlacementValidation.IsItemPlacementValid(mousePosition, base.selectedItem))
            //{
            //    Debug.Log("Cannot place item that location");
            //    return;
            //}
            base.playerBalance.SubtractFromBalance(selectedItem.Price);
            base.ResourceManager.Placed(selectedItem, 1);
            CreateMachine(mousePosition);
        }
    }

    // Create and set up the machine just created
    private void CreateMachine(Vector3 mousePosition)
    {
        GameObject newMachineGameObject = Instantiate(this.MachinePrefab, mousePosition, Quaternion.identity, EnclosureSystem.transform);
        newMachineGameObject.transform.position = new Vector3(newMachineGameObject.transform.position.x, newMachineGameObject.transform.position.y, 10);
        newMachineGameObject.name = base.selectedItem.name;
        newMachineGameObject.GetComponent<SpriteRenderer>().sprite = base.selectedItem.Icon;

        Vector3Int position = base.GridSystem.Grid.WorldToCell(mousePosition);
        base.GridSystem.CellGrid[position.x, position.y].ContainsMachine = true;
        base.GridSystem.CellGrid[position.x, position.y].Machine = newMachineGameObject;
        if (base.selectedItem.ID.Equals("AtmosphereMachine"))
        {
            newMachineGameObject.AddComponent<AtmosphereMachine>().Initialize(this.EnclosureSystem, this.AtmosphereMachineHUD);
        }
        if (base.selectedItem.ID.Equals("LiquidMachine"))
        {
            newMachineGameObject.AddComponent<LiquidMachine>().Initialize(this.TileSystem, this.LiquidMachineHUD);
        }
    }
}
