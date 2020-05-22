using System.Collections.Generic;
using UnityEngine;

public class SpeciesPopupManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] GameObject SpeciesDiscoveredContent = default;
    [SerializeField] GameObject SpeciesPopupPrefab = default;
    [Header("For testing")]
    [SerializeField] List<Species> LevelSpeciesPlaceholder = default;
    [Header("RemoveSelfFromList and whatever else should happen")]
    public ItemSelectedEvent SpeciesSelected = new ItemSelectedEvent();

    public void Start()
    {
        this.AddDiscoveredSpecies();
    }

    public void AddDiscoveredSpecies()
    {
        foreach (var species in this.LevelSpeciesPlaceholder)
        {
            GameObject discoveredSpecies = Instantiate(this.SpeciesPopupPrefab, this.SpeciesDiscoveredContent.transform);
            discoveredSpecies.GetComponent<SpeciesEntryDisplayLogic>().Initialize(species);
            discoveredSpecies.GetComponent<SpeciesData>().JournalData.DiscoveredSpecies = species;
            this.SetupItemSelectedHandler(discoveredSpecies, this.SpeciesSelected);
        }
    }

    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent action)
    {
        item.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(action);
    }
}
