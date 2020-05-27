using UnityEngine.UI;
using UnityEngine;

public class DescriptionDisplayLogic : MonoBehaviour
{
    [SerializeField] GameObject NeedsDescriptionDisplay = default;
    private InputField NeedsDescription = default;
    [SerializeField] GameObject SpeciesDescriptionDisplay = default;
    private InputField SpeciesDescription = default;

    void Start()
    {
        this.NeedsDescription = this.NeedsDescriptionDisplay.GetComponent<InputField>();
        this.SpeciesDescription = this.SpeciesDescriptionDisplay.GetComponent<InputField>();
    }

    public void InitializeSpeciesDescription(GameObject species)
    {
        this.SpeciesDescription.text = species.GetComponent<SpeciesJournalData>().JournalEntry.DiscoveredSpeciesEntryText;
        this.NeedsDescriptionDisplay.SetActive(false);
        this.SpeciesDescriptionDisplay.SetActive(true);
    }

    public void InitializeNeedsDescription(GameObject need)
    {
        this.NeedsDescription.text = need.GetComponent<NeedData>().Description;
        this.NeedsDescriptionDisplay.SetActive(true);
        this.SpeciesDescriptionDisplay.SetActive(false);
    }
}
