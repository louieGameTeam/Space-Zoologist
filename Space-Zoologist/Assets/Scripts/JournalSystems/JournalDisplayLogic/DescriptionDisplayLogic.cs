using UnityEngine.UI;
using UnityEngine;

public class DescriptionDisplayLogic : MonoBehaviour
{
    // Two different descriptions used so both On End Edit event can be assigned two different actions
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

    public void ClearNeedDescription()
    {
        this.NeedsDescription.text = "";
    }
}
