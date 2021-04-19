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
    [SerializeField] Machine[] Machines = default;
    private Dictionary<string, Machine> namesToMachines = new Dictionary<string, Machine>();

    public override void Initialize()
    {
        base.itemType = ItemType.Machine;
        this.namesToMachines = new Dictionary<string, Machine>();
        foreach (Machine machine in Machines)
        {
            this.namesToMachines.Add(machine.name, machine);
        }
        base.Initialize();
    }

    public override void OnCursorPointerUp(PointerEventData eventData)
    {
        base.OnCursorPointerUp(eventData);
        if (base.IsCursorOverUI(eventData) || !base.CanAfford(base.selectedItem))
        {
            base.OnItemSelectionCanceled();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            if (!base.GridSystem.PlacementValidation.IsItemPlacementValid(mousePosition, base.selectedItem))
            {
                Debug.Log("Cannot place item that location");
                return;
            }
            base.playerBalance.SubtractFromBalance(selectedItem.Price);
            CreateMachine(mousePosition);
        }
    }

    private void CreateMachine(Vector3 mousePosition)
    {
        GameObject newMachineGameObject = Instantiate(this.MachinePrefab, mousePosition, Quaternion.identity, EnclosureSystem.transform);
        newMachineGameObject.transform.position = new Vector3(newMachineGameObject.transform.position.x, newMachineGameObject.transform.position.y, 10);
        newMachineGameObject.name = base.selectedItem.name;
        newMachineGameObject.GetComponent<SpriteRenderer>().sprite = base.selectedItem.Icon;

        Vector3Int position = base.GridSystem.Grid.WorldToCell(mousePosition);
        base.GridSystem.CellGrid[position.x, position.y].ContainsMachine = true;
        base.GridSystem.CellGrid[position.x, position.y].Machine = newMachineGameObject;
        if (!this.namesToMachines.ContainsKey(base.selectedItem.ID)) { Debug.LogError("Machine named" + base.selectedItem.ID + "not found"); return; }
        newMachineGameObject.AddComponent(namesToMachines[base.selectedItem.ID].GetType());
        newMachineGameObject.GetComponent<Machine>().SetPosition(position);
    }
}
