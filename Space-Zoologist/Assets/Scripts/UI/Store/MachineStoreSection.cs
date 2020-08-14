using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO figure out how to ensure proper placement
public class MachineStoreSection : StoreSection
{
    [SerializeField] private EnclosureSystem EnclosureSystem = default;
    [SerializeField] private ReservePartitionManager RPM = default;
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
        if (base.IsCursorOverUI(eventData))
        {
            base.OnItemSelectionCanceled();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            if (!IsPlacementValid(mousePosition))
            {
                Debug.Log("Cannot place item that location");
                return;
            }
            CreateMachine(mousePosition);
            playerBalance.RuntimeValue -= selectedItem.Price;
        }
    }

    public override bool IsPlacementValid(Vector3 mousePosition)
    {
        return (mousePosition.x > 1 && mousePosition.y > 1
        && mousePosition.x < LevelDataReference.MapWidth - 1 && mousePosition.y < LevelDataReference.MapHeight - 1);
    }

    private void CreateMachine(Vector3 position)
    {
        GameObject newMachineGameObject = Instantiate(this.MachinePrefab, position, Quaternion.identity, EnclosureSystem.transform);
        newMachineGameObject.transform.position = new Vector3(newMachineGameObject.transform.position.x, newMachineGameObject.transform.position.y, 10);
        newMachineGameObject.name = base.selectedItem.name;
        newMachineGameObject.GetComponent<SpriteRenderer>().sprite = base.selectedItem.Icon;
        if (base.selectedItem.ID.Equals("Atmosphere"))
        {
            newMachineGameObject.AddComponent<AtmosphereMachine>().Initialize(this.EnclosureSystem, this.AtmosphereMachineHUD);
        }
        if (base.selectedItem.ID.Equals("Liquid"))
        {
            newMachineGameObject.AddComponent<LiquidMachine>().Initialize(this.RPM, this.LiquidMachineHUD);
        }
    }
}
