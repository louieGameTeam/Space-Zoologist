using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO figure out how to ensure proper placement
public class MachineStoreSection : StoreSection
{
    [SerializeField] public EnclosureSystem EnclosureSystem = default;
    [SerializeField] GameObject AtmosphereMachineHUD = default;
    [SerializeField] GameObject MachinePrefab = default;
    // Start is called before the first frame update
    void Start()
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
            CreateMachine(mousePosition);
            playerBalance.RuntimeValue -= selectedItem.Price;
        }
    }

    private void CreateMachine(Vector3 position)
    {
        GameObject newMachineGameObject = Instantiate(this.MachinePrefab, position, Quaternion.identity, EnclosureSystem.transform);
        newMachineGameObject.transform.position = new Vector3(newMachineGameObject.transform.position.x, newMachineGameObject.transform.position.y, 10);
        newMachineGameObject.name = base.selectedItem.name;
        newMachineGameObject.GetComponent<SpriteRenderer>().sprite = base.selectedItem.Icon;
        newMachineGameObject.GetComponent<AtmosphereMachine>().Initialize(this.EnclosureSystem, this.AtmosphereMachineHUD);
    }
}
