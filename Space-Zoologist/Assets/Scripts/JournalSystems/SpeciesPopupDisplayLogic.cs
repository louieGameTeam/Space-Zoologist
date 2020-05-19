using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pipeline script for setting up and displaying discovered species based off of level data and then passing them onto manager
/// </summary>
public class SpeciesPopupDisplayLogic : MonoBehaviour, ISetupSelectable
{
    [SerializeField] GameObject SpeciesDiscoveredContent = default;
    [SerializeField] GameObject SpeciesPopupPrefab = default;
    [Header("For testing")]
    [SerializeField] List<Species> LevelSpeciesPlaceholder = default;
    private GameObject SpeciesSelected = null;
    [Header("Species Manager and RemoveSelfFromList")]
    public ItemSelectedEvent speciesSelected = new ItemSelectedEvent();
    
    public void Start()
    {
        foreach (var species in this.LevelSpeciesPlaceholder)
        {
            GameObject discoveredSpecies = Instantiate(this.SpeciesPopupPrefab, this.SpeciesDiscoveredContent.transform);
            discoveredSpecies.GetComponent<SpeciesEntryDisplayLogic>().Initialize(species);
            discoveredSpecies.GetComponent<SpeciesData>().Data = species;
            this.SetupItemSelectedHandler(discoveredSpecies, this.speciesSelected);
        }
    }

    // Pooling not really needed if this is a one time process
    public void RemoveSelfFromList(GameObject discoveredSpecies)
    {
        Destroy(discoveredSpecies);
    }

    public void SetupItemSelectedHandler(GameObject discoveredSpecies, ItemSelectedEvent action)
    {
        discoveredSpecies.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(action);
    }

    public void UpdatePopupActiveSelf()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
