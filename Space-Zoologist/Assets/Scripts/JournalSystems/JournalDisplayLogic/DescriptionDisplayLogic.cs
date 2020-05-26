using UnityEngine.UI;
using UnityEngine;

public class DescriptionDisplayLogic : MonoBehaviour
{
    [SerializeField] Text Description = default;

    public void InitializeSpeciesDescription(GameObject species)
    {
        this.Description.text = species.GetComponent<SpeciesJournalData>().JournalEntry.DiscoveredSpeciesEntryText;
    }

    public void InitializeNeedsDescription(GameObject need)
    {
        this.Description.text = need.GetComponent<NeedData>().Description;
    }
}
