using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AtmosphereMachine : Machine
{
    private GameObject AtmosphereHUDGameObject = default;
    private EnclosureSystem EnclosureSystem = default;
    private AtmosphereMachineHUD AtmosphereMachineHUD = default;

    public void Start()
    {
        this.AtmosphereMachineHUD = this.AtmosphereHUDGameObject.GetComponent<AtmosphereMachineHUD>();
    }

    public void Initialize(EnclosureSystem enclosureSystem, GameObject atmosphereMachineHUD)
    {
        this.EnclosureSystem = enclosureSystem;
        this.AtmosphereHUDGameObject = atmosphereMachineHUD;
    }

    void OnMouseDown()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (!this.AtmosphereHUDGameObject.activeSelf && results.Count == 0) this.OpenHUD();
    }

    public override void OpenHUD()
    {
        this.AtmosphereHUDGameObject.SetActive(!this.AtmosphereHUDGameObject.activeSelf);
        if (this.AtmosphereHUDGameObject.activeSelf)
        {

            this.AtmosphereMachineHUD.Initialize(this);
            this.AtmosphereMachineHUD.SetAtmosphericComposition(this.EnclosureSystem.GetAtmosphericComposition(this.gameObject.transform.position));

        }
    }

    // TODO figure out how you can update the atmosphere of an enclosed area
    public void UpdateAtmosphere(AtmosphericComposition atmosphericComposition)
    {
        this.EnclosureSystem.UpdateAtmosphereComposition(this.transform.position, atmosphericComposition);
    }
}
