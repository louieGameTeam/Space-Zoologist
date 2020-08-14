using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereMachine : MonoBehaviour
{
    private GameObject AtmosphereHUDGameObject = default;
    private EnclosureSystem EnclosureSystem = default;
    AtmosphereMachineHUD AtmosphereMachineHUD = default;

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
        this.OpenHUD();
    }

    public void OpenHUD()
    {
        this.AtmosphereHUDGameObject.SetActive(!this.AtmosphereHUDGameObject.activeSelf);
        if (this.AtmosphereHUDGameObject.activeSelf)
        {
            this.AtmosphereMachineHUD.Initialize(this.EnclosureSystem.GetAtmosphericComposition(this.gameObject.transform.position), this);
        }
    }

    // TODO figure out how you can update the atmosphere of an enclosed area
    public void UpdateAtmosphere()
    {
        // this.EnclosureSystem.UpdateSurroundingAtmosphere()
    }
}
